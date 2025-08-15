// Messaging.SenderBase.cs
using MassTransit;

namespace Messaging
{
    /// <summary>
    /// TRequest için 6 kalıbı destekler: Send, RPC, PublishToQueue, PublishToAll, ScheduleSend, SchedulePublish.
    /// </summary>
    public abstract class MessageSenderBase<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        private readonly ISendEndpointProvider _send;
        private readonly IPublishEndpoint _publish;
        private readonly IRequestClient<TRequest> _client;
        private readonly IMessageScheduler _scheduler;

        protected MessageSenderBase(
            ISendEndpointProvider send,
            IPublishEndpoint publish,
            IRequestClient<TRequest> client,
            IMessageScheduler scheduler)
        {
            _send = send;
            _publish = publish;
            _client = client;
            _scheduler = scheduler;
        }

        protected virtual string CmdQueueName => Naming.QueueName(typeof(TRequest), MessagePattern.SendAndForget);
        protected virtual string RpcQueueName => Naming.QueueName(typeof(TRequest), MessagePattern.SendAndWait);
        protected virtual string PubQueueName => Naming.QueueName(typeof(TRequest), MessagePattern.PublishToQueue);
        protected virtual string EvtQueueName => Naming.QueueName(typeof(TRequest), MessagePattern.PublishToAll);

        // 1) SendAndForget (command)
        public async Task SendAndForgetAsync(TRequest message, CancellationToken ct = default)
        {
            var endpoint = await _send.GetSendEndpoint(Naming.ExchangeUri(CmdQueueName));
            await endpoint.Send(message, ct);
        }

        // 2) RPC
        public async Task<TResponse> SendAndWaitAsync(TRequest message, CancellationToken ct = default)
        {
            var response = await _client.GetResponse<TResponse>(message, ct);
            return response.Message;
        }

        // 3) PublishToQueue (tek kuyruk hedefle)
        //public async Task PublishToQueueAsync(TRequest message, CancellationToken ct = default)
        //{
        //    var endpoint = await _send.GetSendEndpoint(Naming.QueueUri(PubQueueName));
        //    await endpoint.Send(message, ct);
        //}
        public async Task PublishToQueueAsync(TRequest message, CancellationToken ct = default)
        {
            // exchange: <pubq endpoint exchange>  -> zaten pubq kuyruğuna bağlı
            var endpoint = await _send.GetSendEndpoint(Naming.ExchangeUri(PubQueueName));
            // İstek token’ı erken iptal ederse RabbitMQ kanalı kapanabiliyor;
            // publish/send tarafında topology yok, yine de güvenli olmak için token'ı zor iptal ettirmeyelim:
            await endpoint.Send(message, CancellationToken.None);
        }

        // 4) PublishToAll (yalnızca event consumer’ları alır)
        public Task PublishToAllAsync(TRequest message, CancellationToken ct = default)
            => _publish.Publish(new EventEnvelope<TRequest>(message), ct);

        // 5) ScheduleSend (ileriye tarihli send)
        public Task ScheduleSendAsync(TRequest message, DateTime scheduledUtc, CancellationToken ct = default)
            => _scheduler.ScheduleSend(Naming.ExchangeUri(CmdQueueName), scheduledUtc, message, ct);

        // 6) SchedulePublish (ileriye tarihli publish – event)
        public Task SchedulePublishAsync(TRequest message, DateTime scheduledUtc, CancellationToken ct = default)
            => _scheduler.SchedulePublish(scheduledUtc, new EventEnvelope<TRequest>(message), ct);
    }
}