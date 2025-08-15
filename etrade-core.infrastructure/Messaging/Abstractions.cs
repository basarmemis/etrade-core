// Messaging.Abstractions.cs
using System;

namespace Messaging
{
    public interface IMessage
    {
        Guid CorrelationId { get; }
        DateTime TimestampUtc { get; }
    }

    public abstract record MessageBase : IMessage
    {
        public Guid CorrelationId { get; init; } = Guid.NewGuid();
        public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;
    }

    public abstract record RequestBase<TResponse> : MessageBase where TResponse : class;
    public abstract record ResponseBase : MessageBase;

    // Event’ler (PublishToAll / SchedulePublish hedefi)
    public abstract record EventBase : MessageBase;

    /// <summary>
    /// Kuyruk/Exchange adlandırma için isteğe bağlı prefix.
    /// Boş olabilir; boş ise sadece sınıf ismi baz alınır.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class MessageNameAttribute : Attribute
    {
        public string? Prefix { get; }
        public MessageNameAttribute(string? prefix = null) => Prefix = prefix;
    }

    public enum MessagePattern
    {
        SendAndForget,
        SendAndWait,
        PublishToQueue,
        PublishToAll,
        ScheduleSend,
        SchedulePublish
    }
}