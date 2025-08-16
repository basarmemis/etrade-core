namespace etrade_core.infrastructure.CustomMessageQueue.Options
{
    public sealed class MessagingOptions
    {
        // Rabbit
        public string Host { get; set; } = "localhost";
        public string VHost { get; set; } = "/";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";

        // Endpoint tuning
        public ushort PrefetchCount { get; set; } = 32;
        public int? ConcurrentMessageLimit { get; set; } = Environment.ProcessorCount;

        // Retry & Redelivery
        public int RetryAttempts { get; set; } = 5;
        public TimeSpan RetryMin { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan RetryMax { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan[] RedeliveryIntervals { get; set; } = new[]
        {
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(30),
            TimeSpan.FromMinutes(2)
        };

        // DLQ (MassTransit default zaten aktif; ekstra ayar bırakıldı)
        public bool ConfigureDeadLetter { get; set; } = true;

        // Scheduler
        public bool UseDelayedMessageScheduler { get; set; } = false; // RabbitMQ Delayed Message Exchange plugin varsayımı

        // Queue features
        public bool UseQuorumQueues { get; set; } = false;
        public bool EnablePriority { get; set; } = false;
        public byte MaxPriority { get; set; } = 10;

        // Idempotency
        public bool EnableIdempotency { get; set; } = true;
        public TimeSpan IdempotencyWindow { get; set; } = TimeSpan.FromMinutes(10);
    }
}