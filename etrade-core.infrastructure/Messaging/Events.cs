// Messaging.Events.cs
namespace Messaging
{
    /// <summary>
    /// PublishToAll ve SchedulePublish’te, TPayload mesajını event olarak taşır.
    /// </summary>
    public sealed record EventEnvelope<TPayload>(TPayload Payload) : EventBase where TPayload : class;
}
