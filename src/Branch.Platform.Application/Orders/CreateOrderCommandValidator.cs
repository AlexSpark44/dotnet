using FluentValidation;

namespace Branch.Platform.Application.Orders;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Payload.CustomerId).NotEmpty();
        RuleFor(x => x.Payload.Currency).Length(3);
        RuleFor(x => x.Payload.Items).NotEmpty();
        RuleForEach(x => x.Payload.Items).ChildRules(items =>
        {
            items.RuleFor(i => i.ProductId).NotEmpty();
            items.RuleFor(i => i.Quantity).GreaterThan(0);
            items.RuleFor(i => i.UnitPrice).GreaterThan(0);
        });
    }
}
