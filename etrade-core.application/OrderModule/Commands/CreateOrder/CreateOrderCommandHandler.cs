using MediatR;
using etrade_core.application.Common.Base;
using etrade_core.application.IRepositories;
using etrade_core.domain.OrderModule.Entities;
using etrade_core.domain.OrderModule.Enums;
using etrade_core.domain.ProductModule.Enums;

namespace etrade_core.application.OrderModule.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Validate products exist and get current prices
        var productIds = request.OrderItems.Select(oi => oi.ProductId).ToList();
        var products = new List<etrade_core.domain.ProductModule.Entities.Product>();
        
        foreach (var productId in productIds)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product != null)
            {
                products.Add(product);
            }
        }

        if (products.Count != productIds.Count)
        {
            throw new InvalidOperationException("One or more products not found");
        }

        // Create order
        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            OrderDate = DateTime.UtcNow,
            TotalAmount = 0, // Will be calculated below
            Status = OrderStatus.Pending,
            CustomerEmail = request.UserId, // Using UserId as email for now
            OfferingTypes = ProductOfferingTypes.Purchsase, // Default to purchase
            UserProfileId = 1, // TODO: Get actual UserProfileId from UserId
            OrderItems = new List<OrderItem>()
        };

        // Create order items
        foreach (var itemDto in request.OrderItems)
        {
            var product = products.First(p => p.Id == itemDto.ProductId);
            var orderItem = new OrderItem
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
                TotalPrice = itemDto.Quantity * itemDto.UnitPrice,
                OfferingType = ProductOfferingTypes.Purchsase,
                Attributes = itemDto.Attributes?.Select(attr => new OrderItemAttribute
                {
                    AttributeKey = attr.Name,
                    AttributeValue = attr.Value,
                    AttributeType = "Text"
                }).ToList() ?? new List<OrderItemAttribute>()
            };

            order.OrderItems.Add(orderItem);
        }

        // Calculate total
        order.TotalAmount = order.OrderItems.Sum(oi => oi.TotalPrice);

        // Save order
        await _orderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateOrderResponse
        {
            OrderId = (int)order.Id,
            OrderNumber = order.OrderNumber,
            CreatedAt = order.OrderDate,
            TotalAmount = order.TotalAmount
        };
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
} 