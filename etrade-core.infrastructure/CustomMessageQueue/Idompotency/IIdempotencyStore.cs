namespace etrade_core.infrastructure.CustomMessageQueue.Idompotency
{
    public interface IIdempotencyStore
    {
        Task<bool> TryStartAsync(string key, TimeSpan ttl, CancellationToken ct = default);
        Task CompleteAsync(string key, CancellationToken ct = default);
    }
}