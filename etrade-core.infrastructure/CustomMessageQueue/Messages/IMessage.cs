namespace etrade_core.infrastructure.CustomMessageQueue.Messages
{
    public interface IMessage
    {
        Guid CorrelationId { get; }
        DateTime TimestampUtc { get; }
    }
}