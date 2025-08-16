using etrade_core.infrastructure.CustomMessageQueue.Messages;
using MassTransit;

namespace etrade_core.infrastructure.CustomMessageQueue.ConsumerBases
{
    // Event tüketici: yayınlanan EventEnvelope<TPayload> alır, direkt payload verilir
    public abstract class EventConsumerBase<TPayload> : IConsumer<EventEnvelope<TPayload>> where TPayload : class
    {
        public Task Consume(ConsumeContext<EventEnvelope<TPayload>> context)
            => Handle(context.Message.Payload, context.CancellationToken);

        protected abstract Task Handle(TPayload payload, CancellationToken ct);
    }
}