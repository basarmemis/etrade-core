// Sample.Messages/OrderCreatedEventConsumer.cs
using System.Threading;
using System.Threading.Tasks;
using Messaging;

namespace Sample.Messages
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