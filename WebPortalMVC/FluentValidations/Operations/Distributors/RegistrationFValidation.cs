
using Domain.DTO.Operations.Distributors.Input;
using FluentValidation;
using Repository.Contexts;
using Repository.Repositories.EF.Interfaces;

namespace WebPortalMVC.FluentValidations.Operations.Distributors
{
    public class RegistrationFValidation : AbstractValidator<RegistrationInputDto>
    {
        

        public RegistrationFValidation() 
        { 
            RuleFor(x => x.firstname).NotEmpty().WithMessage("First Name is Required")
                .MaximumLength(80).WithMessage("FirstName must be 80 Characters below");
                
            RuleFor(x => x.middlename).MaximumLength(30).WithMessage("Middle Name must be 30 Characters below");
            RuleFor(x => x.lastname).NotEmpty().WithMessage("Last Name is Required")
                .MaximumLength(80).WithMessage("LastName must be 80 Characters below");
            RuleFor(x => x.sex).NotNull().WithMessage("Gender is Required").NotEmpty().WithMessage("Gender is Required");
            RuleFor(x => x).NotEmpty().WithMessage("Gender is Required");
            RuleFor(x => x.completeAddress).NotEmpty().WithMessage("Address is Required")
                .MaximumLength(250).WithMessage("Address must be 250 Characters below");
            RuleFor(x => x.contact).NotEmpty().WithMessage("Contact No. is Required")
                .Must((obj) =>
                {
                    if (string.IsNullOrEmpty(obj))
                    {
                        return false;
                    }
                    else
                    {
                        if (Int64.Parse(obj) >= 1000000000 && Int64.Parse(obj) <= 99999999999)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }).WithMessage("Contact must be 11 Digits only");
            RuleFor(x => x.dateOfBirth).NotEmpty().WithMessage("Date of Birth is Required")
                .LessThan(p => DateTime.Now).WithMessage("Invalid Date of birth"); 
            RuleFor(x => x.email).NotEmpty().WithMessage("Email is Required")
                .EmailAddress().WithMessage("Invalid email address ");
            RuleFor(x => x.msessenger).NotEmpty().WithMessage("Messenger Account is Required")
                .MaximumLength(200).WithMessage("Messenger Account must be 200 characters below");
            RuleFor(x => x.tin).NotEmpty().WithMessage("TIN # is Required")
                 .MaximumLength(30).WithMessage("TIN # must be 200 characters below");
            //RuleFor(x => x.noOfAccount).NotEmpty().WithMessage("No. of Account is Required");
            //RuleFor(x => x.directupLineCode).NotNull().WithMessage("Direct Upline Code is Required");
            //RuleFor(x => x.placementCode).NotEmpty().WithMessage("Placement Code is Required");
            //RuleFor(x => x.purchaseDate).NotEmpty().WithMessage("Purchase Date is Required");

        }
         
    }
}
