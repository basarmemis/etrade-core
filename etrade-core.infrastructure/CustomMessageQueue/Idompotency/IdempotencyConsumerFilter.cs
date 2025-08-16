using etrade_core.infrastructure.CustomMessageQueue.Messages;
using etrade_core.infrastructure.CustomMessageQueue.Options;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace etrade_core.infrastructure.CustomMessageQueue.Idompotency
{
    /// <summary>
    /// Duplicate mesajları düşüren tüketim filtresi (CorrelationId|MessageId bazlı).
    /// Duplicate yakalanınca mesaj "işlenmiş" kabul edilir (ack), işlem atlanır.
    /// RPC için duplicate cevap cache’i istenirse ayrıca eklenebilir (şimdilik skip).
    /// </summary>
    public class IdempotencyConsumeFilter<T> : IFilter<ConsumeContext<T>> where T : class
    {
        private readonly IIdempotencyStore _store;
        private readonly MessagingOptions _options;
        private readonly ILogger<IdempotencyConsumeFilter<T>> _logger;

        public IdempotencyConsumeFilter(IIdempotencyStore store, MessagingOptions options, ILogger<IdempotencyConsumeFilter<T>> logger)
        {
            _store = store;
            _options = options;
            _logger = logger;
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            if (!_options.EnableIdempotency)
            {
                await next.Send(context);
                return;
            }

            var key = GetKey(context);
            if (!await _store.TryStartAsync(key, _options.IdempotencyWindow, context.CancellationToken))
            {
                _logger.LogWarning("Idempotent duplicate detected for {Key}, skipping.", key);
                // Skip: mesaj ack’lenir ama iş mantığı çalışmaz.
                return;
            }

            try
            {
                await next.Send(context);
            }
            finally
            {
                await _store.CompleteAsync(key, context.CancellationToken);
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("IdempotencyConsumeFilter");
        }

        private static string GetKey(ConsumeContext<T> ctx)
        {
            // Öncelik: MessageId, yoksa IMessage.CorrelationId, yoksa body hash
            var id = ctx.MessageId?.ToString();
            if (!string.IsNullOrEmpty(id)) return $"msg:{id}";

            if (ctx.Message is IMessage m) return $"corr:{m.CorrelationId:N}";

            return $"hash:{ctx.Message.GetHashCode()}";
        }
    }
}