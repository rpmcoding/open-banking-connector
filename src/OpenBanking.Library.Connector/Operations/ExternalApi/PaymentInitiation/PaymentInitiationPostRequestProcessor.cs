﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation;

internal class PaymentInitiationPostRequestProcessor<TVariantApiRequest> : IPostRequestProcessor<TVariantApiRequest>
    where TVariantApiRequest : class
{
    private readonly string _accessToken;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly OBSealKey _obSealKey;
    private readonly string _orgId;
    private readonly SoftwareStatementEntity _softwareStatementEntity;
    private readonly bool _useB64;

    public PaymentInitiationPostRequestProcessor(
        string orgId,
        string accessToken,
        IInstrumentationClient instrumentationClient,
        SoftwareStatementEntity softwareStatement,
        OBSealKey obSealKey)
    {
        _instrumentationClient = instrumentationClient;
        _orgId = orgId;
        _useB64 = false; // was true before PISP v3.1.4 which is no longer supported
        _softwareStatementEntity = softwareStatement;
        _obSealKey = obSealKey;
        _accessToken = accessToken;
    }

    public async Task<(TResponse response, string? xFapiInteractionId)> PostAsync<TResponse>(
        Uri uri,
        IEnumerable<HttpHeader>? extraHeaders,
        TVariantApiRequest request,
        TppReportingRequestInfo? tppReportingRequestInfo,
        JsonSerializerSettings? requestJsonSerializerSettings,
        JsonSerializerSettings? responseJsonSerializerSettings,
        IApiClient apiClient)
        where TResponse : class
    {
        // Process request
        var requestDescription = $"POST {uri})";

        // Create JWT and log
        var jsonSerializerSettings =
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        string payloadJson = JsonConvert.SerializeObject(
            request,
            jsonSerializerSettings);
        string jwt = JwtFactory.CreateJwt(
            GetJoseHeaders(
                _softwareStatementEntity.OrganisationId,
                _softwareStatementEntity.SoftwareId,
                _obSealKey.KeyId,
                _useB64),
            payloadJson,
            _obSealKey.Key,
            null);
        StringBuilder requestTraceSb = new StringBuilder()
            .AppendLine($"#### JWT ({requestDescription})")
            .Append(jwt);
        _instrumentationClient.Trace(requestTraceSb.ToString());

        // Assemble headers and body
        var headers = new List<HttpHeader>
        {
            new("x-fapi-financial-id", _orgId),
            new("Authorization", "Bearer " + _accessToken),
            new("x-idempotency-key", Guid.NewGuid().ToString()),
            CreateJwsSignatureHeader(jwt)
        };
        if (extraHeaders is not null)
        {
            foreach (HttpHeader header in extraHeaders)
            {
                headers.Add(header);
            }
        }

        // Send request
        (TResponse response, string? xFapiInteractionId) = await new HttpRequestBuilder()
            .SetMethod(HttpMethod.Post)
            .SetUri(uri)
            .SetHeaders(headers)
            .SetJsonContent(request, requestJsonSerializerSettings)
            .SendExpectingJsonResponseAsync<TResponse>(
                apiClient,
                tppReportingRequestInfo,
                responseJsonSerializerSettings);

        return (response, xFapiInteractionId);
    }

    private static HttpHeader CreateJwsSignatureHeader(string jwt)
    {
        // Create headers
        string[] jwsComponents = jwt.Split('.');
        var jwsSignature = $"{jwsComponents[0]}..{jwsComponents[2]}";
        return new HttpHeader("x-jws-signature", jwsSignature);
    }

    private static Dictionary<string, object> GetJoseHeaders(
        string orgId,
        string softwareId,
        string signingId,
        bool useB64)
    {
        signingId.ArgNotNull(nameof(signingId));
        orgId.ArgNotNull(nameof(orgId));
        softwareId.ArgNotNull(nameof(softwareId));

        // b64 header was removed from 3.1.4 onwards
        string[] crit;
        bool? b64;
        if (useB64)
        {
            crit = new[]
            {
                "http://openbanking.org.uk/iat", "http://openbanking.org.uk/iss", "http://openbanking.org.uk/tan",
                "b64"
            };
            b64 = false;
        }
        else
        {
            crit = new[]
            {
                "http://openbanking.org.uk/iat", "http://openbanking.org.uk/iss", "http://openbanking.org.uk/tan"
            };
            b64 = null;
        }

        Dictionary<string, object> dict = JwtFactory.DefaultJwtHeadersExcludingTyp(signingId);
        dict.Add("cty", "application/json");
        dict.Add("crit", crit);
        dict.Add("http://openbanking.org.uk/iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        dict.Add(
            "http://openbanking.org.uk/iss",
            $"{orgId}/{softwareId}"); // TODO: adjust. See HSBC implementation guide
        dict.Add("http://openbanking.org.uk/tan", "openbanking.org.uk");
        if (!(b64 is null))
        {
            dict.Add("b64", b64.Value);
        }

        return dict;
    }
}
