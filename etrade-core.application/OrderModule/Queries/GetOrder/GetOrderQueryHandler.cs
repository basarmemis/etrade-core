using MediatR;
using etrade_core.application.IRepositories;
using etrade_core.domain.OrderModule.Entities;

namespace etrade_core.application.OrderModule.Queries.GetOrder;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, GetOrderResponse>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<GetOrderResponse> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        
        if (order == null)
        {
            throw new InvalidOperationException($"Order with ID {request.OrderId} not found");
        }

        return new GetOrderResponse
        {
            OrderId = (int)order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            CustomerEmail = order.CustomerEmail,
            OrderItems = order.OrderItems?.Select(oi => new GetOrderItemResponse
            {
                ProductId = (int)oi.ProductId,
                ProductName = oi.Product?.Name ?? "Unknown Product",
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                TotalPrice = oi.TotalPrice,
                Attributes = oi.Attributes?.Select(attr => new GetOrderItemAttributeResponse
                {
                    Name = attr.AttributeKey,
                    Value = attr.AttributeValue
                }).ToList() ?? new List<GetOrderItemAttributeResponse>()
            }).ToList() ?? new List<GetOrderItemResponse>()
        };
    }
} 