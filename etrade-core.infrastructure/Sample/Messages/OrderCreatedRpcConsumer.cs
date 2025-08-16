// Sample.Messages/OrderCreatedRpcConsumer.cs
using etrade_core.infrastructure.CustomMessageQueue.ConsumerBases;

namespace etrade_core.infrastructure.Sample.Messages
{
    // SendAndWait (RPC)
    public sealed class OrderCreatedRpcConsumer
        : RequestConsumerBase<OrderCreatedMessageRequest, OrderCreatedMessageResponse>
    {
        protected override Task<OrderCreatedMessageResponse> Handle(OrderCreatedMessageRequest request, CancellationToken ct)
        {
            var accepted = request.TotalAmount >= 0;
            return Task.FromResult(new OrderCreatedMessageResponse
            {
                OrderId = request.OrderId,
                Accepted = accepted,
                Reason = accepted ? null : "Negative amount"
            });
        }
    }
}