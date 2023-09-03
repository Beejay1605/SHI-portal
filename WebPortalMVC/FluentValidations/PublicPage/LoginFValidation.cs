using Domain.DTO.PublicPage.Login.Input;
using FluentValidation;

namespace WebPortalMVC.FluentValidations.PublicPage;
 
public class LoginFValidation : AbstractValidator<LoginInputDto>
{
    public LoginFValidation()
    {
        RuleFor(x => x.username).NotEmpty().WithMessage("Username is required");
        RuleFor(x => x.password).NotEmpty().WithMessage("Password is required");
    }
} 