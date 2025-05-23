﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;

/// <summary>
///     Token endpoint auth methods supported by Open Banking Connector
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum TokenEndpointAuthMethodSupportedValues
{
    [EnumMember(Value = "ClientSecretBasic")]
    ClientSecretBasic,

    [EnumMember(Value = "ClientSecretPost")]
    ClientSecretPost,

    [EnumMember(Value = "PrivateKeyJwt")]
    PrivateKeyJwt,

    [EnumMember(Value = "TlsClientAuth")]
    TlsClientAuth
}
