using etrade_core.domain.OrderModule.Entities;
using MediatR;

namespace etrade_core.application.OrderModule.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<CreateOrderResponse>
{
    public string UserId { get; set; } = string.Empty;
    public List<CreateOrderItemDto> OrderItems { get; set; } = new();
    public string? Notes { get; set; }
}

public class CreateOrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public List<CreateOrderItemAttributeDto>? Attributes { get; set; }
}

public class CreateOrderItemAttributeDto
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class CreateOrderResponse
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }
} 