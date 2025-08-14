using FluentValidation;

namespace etrade_core.application.OrderModule.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.OrderItems)
            .NotEmpty().WithMessage("Order must contain at least one item");

        RuleForEach(x => x.OrderItems)
            .SetValidator(new CreateOrderItemDtoValidator());
    }
}

public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Product ID must be greater than 0");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0");

        RuleForEach(x => x.Attributes)
            .SetValidator(new CreateOrderItemAttributeDtoValidator());
    }
}

public class CreateOrderItemAttributeDtoValidator : AbstractValidator<CreateOrderItemAttributeDto>
{
    public CreateOrderItemAttributeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Attribute name is required");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Attribute value is required");
    }
} 