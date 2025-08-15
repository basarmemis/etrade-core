// Sample.Messages/OrderCreatedMessageResponse.cs
using System;
using Messaging;

namespace Sample.Messages
{
    public sealed record OrderCreatedMessageResponse(
        Guid OrderId,
        bool Accepted,
        string? Reason = null
    ) : ResponseBase;
}
