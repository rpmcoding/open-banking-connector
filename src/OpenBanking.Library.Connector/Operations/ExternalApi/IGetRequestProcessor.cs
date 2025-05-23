﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

internal interface IGetRequestProcessor
{
    public Task<(TResponse response, string? xFapiInteractionId)> GetAsync<TResponse>(
        Uri uri,
        TppReportingRequestInfo? tppReportingRequestInfo,
        JsonSerializerSettings? jsonSerializerSettings,
        IApiClient apiClient,
        IEnumerable<HttpHeader>? extraHeaders)
        where TResponse : class;
}
