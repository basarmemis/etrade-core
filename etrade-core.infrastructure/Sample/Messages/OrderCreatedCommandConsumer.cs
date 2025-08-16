// Sample.Messages/OrderCreatedCommandConsumer.cs
using etrade_core.infrastructure.CustomMessageQueue.ConsumerBases;

namespace etrade_core.infrastructure.Sample.Messages
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