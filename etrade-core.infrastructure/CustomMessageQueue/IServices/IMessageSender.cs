using etrade_core.infrastructure.CustomMessageQueue.Messages;

namespace etrade_core.infrastructure.CustomMessageQueue.IServices
{
    public interface IMessageSenderService
    {
        Task SendAndForgetAsync<TRequest>(TRequest message, CancellationToken ct = default)
            where TRequest : MessageRequestBase;

        Task<TResponse> SendAndWaitAsync<TRequest, TResponse>(TRequest message, CancellationToken ct = default)
            where TRequest : MessageRequestBase
            where TResponse : MessageResponseBase;

        Task PublishToQueueAsync<TRequest>(TRequest message, CancellationToken ct = default)
            where TRequest : MessageRequestBase;

        Task PublishToAllAsync<TRequest>(TRequest message, CancellationToken ct = default)
            where TRequest : MessageRequestBase;

        Task ScheduleSendAsync<TRequest>(TRequest message, DateTime scheduledUtc, CancellationToken ct = default)
            where TRequest : MessageRequestBase;

        Task SchedulePublishAsync<TRequest>(TRequest message, DateTime scheduledUtc, CancellationToken ct = default)
            where TRequest : MessageRequestBase;
    }
}