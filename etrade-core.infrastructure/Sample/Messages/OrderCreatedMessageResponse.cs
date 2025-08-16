// Sample.Messages/OrderCreatedMessageResponse.cs
using etrade_core.infrastructure.CustomMessageQueue.Messages;

namespace etrade_core.infrastructure.Sample.Messages
{
    public sealed class OrderCreatedMessageResponse : MessageResponseBase
    {
        public Guid OrderId { get; set; }
        public bool Accepted { get; set; }
        public string? Reason { get; set; }
    }
}
