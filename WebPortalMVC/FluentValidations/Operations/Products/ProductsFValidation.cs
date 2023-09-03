using Domain.DTO.Operations.Distributors.Input;
using Domain.DTO.Operations.Products.Input;
using FluentValidation;

namespace WebPortalMVC.FluentValidations.Operations.Products;

public class ProductsFValidation : AbstractValidator<CreateProductsInputDto>
{
	public ProductsFValidation()
	{
		RuleFor(x => x.name).NotEmpty().WithMessage("Product Name is required").NotNull().WithMessage("Product Name is required")
            .MaximumLength(100).WithMessage("Maximum of 100 characters only");
		
        RuleFor(x => x.price).NotNull().WithMessage("Product price is required");

        RuleFor(x => x.profit).NotNull().WithMessage("Product Profit is required");
        RuleFor(x => x.total_payout).NotNull().WithMessage("Product Calculated Payout is required");

        RuleFor(x => x.p_category).NotEmpty().WithMessage("Product category is required").NotNull().WithMessage("Product category is required")
            .MaximumLength(15).WithMessage("Maximum of 16 characters only");
        RuleFor(x => x.mini_desc).MaximumLength(200).WithMessage("Maximum of 200 characters only");
    }
}
