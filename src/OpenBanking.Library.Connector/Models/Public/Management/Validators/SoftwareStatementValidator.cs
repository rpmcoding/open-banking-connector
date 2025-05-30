﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FluentValidation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Validators;

public class SoftwareStatementValidator : AbstractValidator<SoftwareStatement>
{
    public SoftwareStatementValidator()
    {
        RuleFor(x => x.OrganisationId)
            .Must(ValidationRules.IsNonWhitespace);
        RuleFor(x => x.SoftwareId)
            .Must(ValidationRules.IsNonWhitespace);
        RuleFor(x => x.DefaultObWacCertificateId)
            .NotEmpty();
        RuleFor(x => x.DefaultObSealCertificateId)
            .NotEmpty();
        RuleFor(x => x.DefaultQueryRedirectUrl)
            .Must(ValidationRules.IsUrl);
        RuleFor(x => x.DefaultFragmentRedirectUrl)
            .Must(ValidationRules.IsUrl);
    }
}
