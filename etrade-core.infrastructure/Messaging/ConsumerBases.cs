// Messaging.ConsumerBases.cs
using System.Threading;
using System.Threading.Tasks;
using MassTransit;

namespace Messaging
{
    // Komut/tek yönlü tüketici (SendAndForget / PublishToQueue hedefleri)
    public abstract class CommandConsumerBase<TCommand> : IConsumer<TCommand> where TCommand : class
    {
        public Task Consume(ConsumeContext<TCommand> context)
            => Handle(context.Message, context.CancellationToken);

        protected abstract Task Handle(TCommand message, CancellationToken ct);
    }

    // RPC tüketici (SendAndWait)
    public abstract class RequestConsumerBase<TRequest, TResponse> : IConsumer<TRequest>
        where TRequest : class
        where TResponse : class
    {
        public async Task Consume(ConsumeContext<TRequest> context)
        {
            var response = await Handle(context.Message, context.CancellationToken);
            if (context.ResponseAddress != null)
                await context.RespondAsync(response);
        }

        protected abstract Task<TResponse> Handle(TRequest request, CancellationToken ct);
    }

    // Event tüketici: yayınlanan EventEnvelope<TPayload> alır, direkt payload verilir
    public abstract class EventConsumerBase<TPayload> : IConsumer<EventEnvelope<TPayload>> where TPayload : class
    {
        public Task Consume(ConsumeContext<EventEnvelope<TPayload>> context)
            => Handle(context.Message.Payload, context.CancellationToken);

        protected abstract Task Handle(TPayload payload, CancellationToken ct);
    }
}