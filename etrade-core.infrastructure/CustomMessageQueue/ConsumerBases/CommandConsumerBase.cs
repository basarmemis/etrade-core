using MassTransit;

namespace etrade_core.infrastructure.CustomMessageQueue.ConsumerBases
{
    // Komut/tek yönlü tüketici (SendAndForget / PublishToQueue hedefleri)
    public abstract class CommandConsumerBase<TCommand> : IConsumer<TCommand> where TCommand : class
    {
        public Task Consume(ConsumeContext<TCommand> context)
            => Handle(context.Message, context.CancellationToken);

        protected abstract Task Handle(TCommand message, CancellationToken ct);
    }
}