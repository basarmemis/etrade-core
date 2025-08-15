// Sample.Messages/OrderCreatedRpcConsumer.cs
using System.Threading;
using System.Threading.Tasks;
using Messaging;

namespace Sample.Messages
{
    // SendAndWait (RPC)
    public sealed class OrderCreatedRpcConsumer
        : RequestConsumerBase<OrderCreatedMessageRequest, OrderCreatedMessageResponse>
    {
        protected override Task<OrderCreatedMessageResponse> Handle(OrderCreatedMessageRequest request, CancellationToken ct)
        {
            var accepted = request.TotalAmount >= 0;
            return Task.FromResult(new OrderCreatedMessageResponse(
                request.OrderId,
                accepted,
                accepted ? null : "Negative amount"
            ));
        }
    }
}