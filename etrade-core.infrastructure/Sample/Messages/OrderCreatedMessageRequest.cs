// Sample.Messages/OrderCreatedMessageRequest.cs
using System;
using Messaging;

namespace Sample.Messages
{
    [MessageName("sales")] // prefix opsiyonel: "sales.order-created-message-request.<suffix>"
    public sealed record OrderCreatedMessageRequest(
        Guid OrderId,
        decimal TotalAmount,
        string Currency
    ) : RequestBase<OrderCreatedMessageResponse>;
}