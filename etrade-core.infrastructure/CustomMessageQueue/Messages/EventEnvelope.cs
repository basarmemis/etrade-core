namespace etrade_core.infrastructure.CustomMessageQueue.Messages
{
    public class EventEnvelope<TPayload>(TPayload Payload) : EventBase where TPayload : class
    {
        public TPayload Payload { get; init; } = Payload;
    }
}