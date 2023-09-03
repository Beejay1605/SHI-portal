
using Domain.DTO.Operations.Team.input;
using FluentValidation;

namespace WebPortalMVC.FluentValidations.Operations.Payincode;

public class EncodePayinFValidation : AbstractValidator<EncodePayinsInputDto>
{
    public EncodePayinFValidation()
    {
        RuleFor(x => x.distributor_id)
            .NotNull().WithMessage("Distributor is Required")
            .NotEqual(0).WithMessage("Distributor is Required");

        RuleFor(x => x.binary_data)
            .NotNull().NotEmpty();

        RuleFor(x => x.key_code)
            .NotNull().OverridePropertyName("key-code").WithMessage("Pay-in Code is Required")
            .NotEmpty().OverridePropertyName("key-code").WithMessage("Pay-in Code is Required")
            .MaximumLength(16).OverridePropertyName("key-code").WithMessage("Invalid Code")
            .MinimumLength(16).OverridePropertyName("key-code").WithMessage("Invalid Code");
    }
}