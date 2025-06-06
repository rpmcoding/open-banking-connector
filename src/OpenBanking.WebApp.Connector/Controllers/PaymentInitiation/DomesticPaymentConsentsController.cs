﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using Microsoft.AspNetCore.Mvc;

namespace FinnovationLabs.OpenBanking.WebApp.Connector.Controllers.PaymentInitiation;

[ApiController]
[ApiExplorerSettings(GroupName = "pisp")]
[Tags("Domestic Payment Consents")]
[Route("pisp/domestic-payment-consents")]
public class DomesticPaymentConsentsController : ControllerBase
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IRequestBuilder _requestBuilder;

    public DomesticPaymentConsentsController(IRequestBuilder requestBuilder, LinkGenerator linkGenerator)
    {
        _requestBuilder = requestBuilder;
        _linkGenerator = linkGenerator;
    }

    /// <summary>
    ///     Create domestic payment consent
    /// </summary>
    /// <param name="request"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<
        DomesticPaymentConsentCreateResponse>> PostAsync(
        [FromBody]
        DomesticPaymentConsentRequest request,
        [FromHeader(Name = "x-fapi-customer-ip-address")]
        string? xFapiCustomerIpAddress)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Determine extra headers
        IEnumerable<HttpHeader>? extraHeaders;
        if (xFapiCustomerIpAddress is not null)
        {
            extraHeaders = [new HttpHeader("x-fapi-customer-ip-address", xFapiCustomerIpAddress)];
        }
        else
        {
            extraHeaders = null;
        }

        DomesticPaymentConsentCreateResponse fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPaymentConsents
            .CreateAsync(request, requestUrlWithoutQuery, extraHeaders);

        return CreatedAtAction(
            nameof(GetAsync),
            new { domesticPaymentConsentId = fluentResponse.Id },
            fluentResponse);
    }

    /// <summary>
    ///     Read domestic payment consent
    /// </summary>
    /// <param name="domesticPaymentConsentId">ID of DomesticPaymentConsent</param>
    /// <param name="excludeExternalApiOperation"></param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpGet("{domesticPaymentConsentId:guid}")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<
        DomesticPaymentConsentCreateResponse>> GetAsync(
        Guid domesticPaymentConsentId,
        [FromHeader(Name = "x-obc-exclude-external-api-operation")]
        bool? excludeExternalApiOperation,
        [FromHeader(Name = "x-fapi-customer-ip-address")]
        string? xFapiCustomerIpAddress)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Determine extra headers
        IEnumerable<HttpHeader>? extraHeaders;
        if (xFapiCustomerIpAddress is not null)
        {
            extraHeaders = [new HttpHeader("x-fapi-customer-ip-address", xFapiCustomerIpAddress)];
        }
        else
        {
            extraHeaders = null;
        }

        // Operation
        DomesticPaymentConsentCreateResponse fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPaymentConsents
            .ReadAsync(
                new ConsentReadParams
                {
                    Id = domesticPaymentConsentId,
                    ModifiedBy = null,
                    ExtraHeaders = extraHeaders,
                    PublicRequestUrlWithoutQuery = requestUrlWithoutQuery,
                    ExcludeExternalApiOperation = excludeExternalApiOperation ?? false
                });

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Read domestic payment consent funds confirmation
    /// </summary>
    /// <param name="domesticPaymentConsentId">ID of DomesticPaymentConsent</param>
    /// <param name="xFapiCustomerIpAddress"></param>
    /// <returns></returns>
    [HttpGet("{domesticPaymentConsentId:guid}/funds-confirmation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<
        DomesticPaymentConsentFundsConfirmationResponse>> GetFundsConfirmationAsync(
        Guid domesticPaymentConsentId,
        [FromHeader(Name = "x-fapi-customer-ip-address")]
        string? xFapiCustomerIpAddress)
    {
        string requestUrlWithoutQuery =
            _linkGenerator.GetUriByAction(HttpContext) ??
            throw new InvalidOperationException("Can't generate calling URL.");

        // Determine extra headers
        IEnumerable<HttpHeader>? extraHeaders;
        if (xFapiCustomerIpAddress is not null)
        {
            extraHeaders = [new HttpHeader("x-fapi-customer-ip-address", xFapiCustomerIpAddress)];
        }
        else
        {
            extraHeaders = null;
        }

        // Operation
        DomesticPaymentConsentFundsConfirmationResponse fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPaymentConsents
            .ReadFundsConfirmationAsync(
                new ConsentBaseReadParams
                {
                    Id = domesticPaymentConsentId,
                    ModifiedBy = null,
                    ExtraHeaders = extraHeaders,
                    PublicRequestUrlWithoutQuery = requestUrlWithoutQuery
                });

        return Ok(fluentResponse);
    }

    /// <summary>
    ///     Delete domestic payment consent
    /// </summary>
    /// <param name="domesticPaymentConsentId">ID of DomesticPaymentConsent</param>
    /// <returns></returns>
    [HttpDelete("{domesticPaymentConsentId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse>> DeleteAsync(
        Guid domesticPaymentConsentId)
    {
        // Operation
        BaseResponse fluentResponse = await _requestBuilder
            .PaymentInitiation
            .DomesticPaymentConsents
            .DeleteLocalAsync(
                new LocalDeleteParams
                {
                    Id = domesticPaymentConsentId,
                    ModifiedBy = null
                });

        return Ok(fluentResponse);
    }
}
