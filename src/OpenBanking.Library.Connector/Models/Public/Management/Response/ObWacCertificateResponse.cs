﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;

public interface IObWacCertificatePublicQuery : IEntityBaseQuery
{
    /// <summary>
    ///     Associated key (PKCS #8) as "stringified" PEM file with escaped newline characters ("\n") and "PRIVATE KEY" label.
    ///     Example: "-----BEGIN PRIVATE KEY-----\nABC\n-----END PRIVATE KEY-----\n"
    /// </summary>
    public SecretDescription AssociatedKey { get; }

    /// <summary>
    ///     OB WAC (transport) certificate (X.509) as "stringified" PEM file with escaped newline characters ("\n") and
    ///     "CERTIFICATE"
    ///     label.
    ///     Example: "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
    /// </summary>
    public string Certificate { get; }
}

/// <summary>
///     Response to ObWacCertificate read and create requests.
/// </summary>
public class ObWacCertificateResponse : EntityBaseResponse, IObWacCertificatePublicQuery
{
    /// <summary>
    ///     Associated key (PKCS #8) as "stringified" PEM file with escaped newline characters ("\n") and "PRIVATE KEY" label.
    ///     Example: "-----BEGIN PRIVATE KEY-----\nABC\n-----END PRIVATE KEY-----\n"
    /// </summary>
    public required SecretDescription AssociatedKey { get; init; }

    /// <summary>
    ///     OB WAC (transport) certificate (X.509) as "stringified" PEM file with escaped newline characters ("\n") and
    ///     "CERTIFICATE"
    ///     label.
    ///     Example: "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
    /// </summary>
    public required string Certificate { get; init; }
}
