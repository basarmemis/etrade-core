using Microsoft.Extensions.Caching.Memory;

namespace etrade_core.infrastructure.CustomMessageQueue.Idompotency
{
    /// <summary>
    /// Basit in-memory store. Üretimde Redis gibi bir store ile değiştirebilirsin.
    /// </summary>
    public sealed class MemoryIdempotencyStore : IIdempotencyStore
    {
        private readonly IMemoryCache _cache;
        public MemoryIdempotencyStore(IMemoryCache cache) => _cache = cache;

        public Task<bool> TryStartAsync(string key, TimeSpan ttl, CancellationToken ct = default)
        {
            if (_cache.TryGetValue(key, out _)) return Task.FromResult(false);
            _cache.Set(key, true, ttl);
            return Task.FromResult(true);
        }
        public Task CompleteAsync(string key, CancellationToken ct = default)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}