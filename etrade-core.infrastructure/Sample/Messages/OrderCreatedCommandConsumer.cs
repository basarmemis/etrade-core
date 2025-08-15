// Sample.Messages/OrderCreatedCommandConsumer.cs
using System.Threading;
using System.Threading.Tasks;
using Messaging;

namespace Sample.Messages
{
    // SendAndForget & PublishToQueue
    public sealed class OrderCreatedCommandConsumer : CommandConsumerBase<OrderCreatedMessageRequest>
    {
        protected override Task Handle(OrderCreatedMessageRequest msg, CancellationToken ct)
        {
            // TODO: siparişi işaretle, logla vs.
            return Task.CompletedTask;
        }
    }
}