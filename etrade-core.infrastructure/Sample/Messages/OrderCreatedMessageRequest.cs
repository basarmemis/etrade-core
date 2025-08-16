// Sample.Messages/OrderCreatedMessageRequest.cs
using etrade_core.infrastructure.CustomMessageQueue.Attributes;
using etrade_core.infrastructure.CustomMessageQueue.Messages;

namespace etrade_core.infrastructure.Sample.Messages
{
    [MessageNamePrefix(null)] // prefix opsiyonel: "sales.order-created-message-request.<suffix>"
    public sealed class OrderCreatedMessageRequest : MessageRequestBase
    {
        public Guid OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "TRY";
    }
}