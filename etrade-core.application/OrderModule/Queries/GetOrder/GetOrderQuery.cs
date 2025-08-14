using MediatR;

namespace etrade_core.application.OrderModule.Queries.GetOrder;

public class GetOrderQuery : IRequest<GetOrderResponse>
{
    public int OrderId { get; set; }
}

public class GetOrderResponse
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public List<GetOrderItemResponse> OrderItems { get; set; } = new();
}

public class GetOrderItemResponse
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public List<GetOrderItemAttributeResponse> Attributes { get; set; } = new();
}

public class GetOrderItemAttributeResponse
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
} 