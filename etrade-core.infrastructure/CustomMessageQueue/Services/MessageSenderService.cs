using etrade_core.infrastructure.CustomMessageQueue.Enums;
using etrade_core.infrastructure.CustomMessageQueue.Helpers;
using etrade_core.infrastructure.CustomMessageQueue.IServices;
using etrade_core.infrastructure.CustomMessageQueue.Messages;
using MassTransit;

namespace etrade_core.infrastructure.CustomMessageQueue.Services
{
    /// <summary>
    /// Tüm mesaj tipleri için tek sender servisi.
    /// 6 pattern: Send, RPC, PublishToQueue, PublishToAll, ScheduleSend, SchedulePublish
    /// </summary>
    public sealed class MessageSenderService : IMessageSenderService
    {
        private readonly ISendEndpointProvider _send;
        private readonly IPublishEndpoint _publish;
        private readonly IClientFactory _clientFactory;
        private readonly IMessageScheduler _scheduler;

        public MessageSenderService(
            ISendEndpointProvider send,
            IPublishEndpoint publish,
            IClientFactory clientFactory,
            IMessageScheduler scheduler)
        {
            _send = send;
            _publish = publish;
            _clientFactory = clientFactory;
            _scheduler = scheduler;
        }

        public async Task SendAndForgetAsync<TRequest>(TRequest message, CancellationToken ct = default)
            where TRequest : MessageRequestBase
        {
            var cmdName = MessageNaming.QueueName(typeof(TRequest), MessagePattern.SendAndForget);
            // declare yarışı yaşamamak için exchange’e gönder
            var endpoint = await _send.GetSendEndpoint(MessageNaming.ExchangeUri(cmdName));
            await endpoint.Send(message, ct);
        }

        public Task PublishToAllAsync<TRequest>(TRequest message, CancellationToken ct = default)
            where TRequest : MessageRequestBase
        {
            // Yalnızca EventConsumerBase<TRequest> alanlar tüketsin diye envelope
            return _publish.Publish(new EventEnvelope<TRequest>(message), ct);
        }

        public async Task<TResponse> SendAndWaitAsync<TRequest, TResponse>(TRequest message, CancellationToken ct = default)
            where TRequest : MessageRequestBase
            where TResponse : MessageResponseBase
        {
            var rpcName = MessageNaming.QueueName(typeof(TRequest), MessagePattern.SendAndWait);
            // RequestClient'i adrese göre oluştur (exchange:)
            var client = _clientFactory.CreateRequestClient<TRequest>(MessageNaming.ExchangeUri(rpcName));
            var response = await client.GetResponse<TResponse>(message, ct);
            return response.Message;
        }

        public async Task PublishToQueueAsync<TRequest>(TRequest message, CancellationToken ct = default)
            where TRequest : MessageRequestBase
        {
            var pubqName = MessageNaming.QueueName(typeof(TRequest), MessagePattern.PublishToQueue);
            // publish-to-queue hattında da exchange’e gönder
            var endpoint = await _send.GetSendEndpoint(MessageNaming.ExchangeUri(pubqName));
            await endpoint.Send(message, ct);
        }

        public Task ScheduleSendAsync<TRequest>(TRequest message, DateTime scheduledUtc, CancellationToken ct = default)
            where TRequest : MessageRequestBase
        {
            var cmdName = MessageNaming.QueueName(typeof(TRequest), MessagePattern.SendAndForget);
            // v8: ScheduleSend(Uri, ...) — exchange adresi veriyoruz
            return _scheduler.ScheduleSend(MessageNaming.ExchangeUri(cmdName), scheduledUtc, message, ct);
        }

        public Task SchedulePublishAsync<TRequest>(TRequest message, DateTime scheduledUtc, CancellationToken ct = default)
            where TRequest : MessageRequestBase
        {
            return _scheduler.SchedulePublish(scheduledUtc, new EventEnvelope<TRequest>(message), ct);
        }
    }
}