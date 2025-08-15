// Messaging.Naming.cs
using System;
using System.Text.RegularExpressions;

namespace Messaging
{
    internal static class Naming
    {
        // "OrderCreatedMessageRequest" -> "order-created-message-request"
        public static string ClassKebab(Type t)
        {
            var name = t.Name;
            name = Regex.Replace(name, "([a-z0-9])([A-Z])", "$1-$2");
            return name.ToLowerInvariant();
        }

        // [prefix.]<class-kebab>.<suffix>
        public static string QueueName(Type messageType, MessagePattern pattern)
        {
            var attr = (MessageNameAttribute?)Attribute.GetCustomAttribute(messageType, typeof(MessageNameAttribute));
            var prefix = string.IsNullOrWhiteSpace(attr?.Prefix) ? null : attr!.Prefix!.Trim();
            var baseName = ClassKebab(messageType);
            var head = string.IsNullOrWhiteSpace(prefix) ? baseName : $"{prefix}.{baseName}";
            var suffix = pattern switch
            {
                MessagePattern.SendAndForget => "cmd",
                MessagePattern.SendAndWait => "rpc",
                MessagePattern.PublishToQueue => "pubq",
                MessagePattern.PublishToAll => "evtq",
                MessagePattern.ScheduleSend => "cmd",
                MessagePattern.SchedulePublish => "evtq",
                _ => "q"
            };
            return $"{head}.{suffix}";
        }

        public static Uri QueueUri(string queueName) => new($"queue:{queueName}");
        public static Uri ExchangeUri(string exchangeName) => new($"exchange:{exchangeName}");
    }
}