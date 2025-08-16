// Sample.Messages/OrderCreatedEventConsumer.cs
using etrade_core.infrastructure.CustomMessageQueue.ConsumerBases;

namespace etrade_core.infrastructure.Sample.Messages
{
    // PublishToAll & SchedulePublish -> yalnızca event consumer’ları alır
    public sealed class OrderCreatedEventConsumer : EventConsumerBase<OrderCreatedMessageRequest>
    {
        protected override Task Handle(OrderCreatedMessageRequest payload, CancellationToken ct)
        {
            // Event akışı: analytics, bildirim vb.
            return Task.CompletedTask;
        }
    }
}