// Sample.Messages/OrderCreatedMessageSender.cs
using Messaging;
using MassTransit;

namespace Sample.Messages
{
    public sealed class OrderCreatedMessageSender
        : MessageSenderBase<OrderCreatedMessageRequest, OrderCreatedMessageResponse>
    {
        public OrderCreatedMessageSender(
            ISendEndpointProvider send,
            IPublishEndpoint publish,
            IRequestClient<OrderCreatedMessageRequest> client,
            IMessageScheduler scheduler)
            : base(send, publish, client, scheduler) { }
    }
}