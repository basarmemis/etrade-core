using etrade_core.infrastructure.CustomMessageQueue.Messages;
using MassTransit;

namespace etrade_core.infrastructure.CustomMessageQueue.ConsumerBases
{
    // RPC tüketici (SendAndWait)
    public abstract class RequestConsumerBase<TRequest, TResponse> : IConsumer<TRequest>
        where TRequest : MessageRequestBase
        where TResponse : MessageResponseBase
    {
        public async Task Consume(ConsumeContext<TRequest> context)
        {
            var response = await Handle(context.Message, context.CancellationToken);
            if (context.ResponseAddress != null)
                await context.RespondAsync(response);
        }

        protected abstract Task<TResponse> Handle(TRequest request, CancellationToken ct);
    }
}