﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class BankTestingFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            builder.UseContentRoot("");
        }
    }
}
