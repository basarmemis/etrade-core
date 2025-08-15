namespace etrade_core.infrastructure.CustomMessageQueue.Messages
{
    public abstract class MessageBase : IMessage
    {
        public Guid CorrelationId { get; init; } = Guid.NewGuid();
        public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;
    }
}