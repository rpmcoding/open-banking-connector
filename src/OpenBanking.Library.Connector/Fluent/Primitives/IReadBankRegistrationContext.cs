// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for Read.
/// </summary>
/// <typeparam name="TPublicResponse"></typeparam>
public interface IReadBankRegistrationContext<TPublicResponse>
    where TPublicResponse : class
{
    private protected IObjectRead<TPublicResponse, BankRegistrationReadParams> ReadObject { get; }

    /// <summary>
    ///     READ object by ID (includes GETing object from bank API).
    ///     Object will be read from bank and also from local database if it is a Bank Registration or Consent.
    /// </summary>
    /// <param name="readParams"></param>
    /// <returns></returns>
    async Task<TPublicResponse> ReadAsync(BankRegistrationReadParams readParams)
    {
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await ReadObject.ReadAsync(readParams);

        return response;
    }
}
