using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace etrade_core.application.Common.Behaviors;

/// <summary>
/// Pipeline behavior for handling idempotency using distributed cache
/// </summary>
/// <typeparam name="TRequest">The type of request</typeparam>
/// <typeparam name="TResponse">The type of response</typeparam>
public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<IdempotencyBehavior<TRequest, TResponse>> _logger;

    public IdempotencyBehavior(IDistributedCache cache, ILogger<IdempotencyBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var isCommand = typeof(TRequest).GetInterfaces().Any(i => i.Name.Contains("ICommand"));

        if (!isCommand)
        {
            // Queries don't need idempotency
            return await next();
        }

        // Generate a unique key for the request
        var requestKey = GenerateRequestKey(request);
        var cacheKey = $"idempotency:{requestKey}";

        // Check if we've already processed this request
        var cachedResponse = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedResponse))
        {
            _logger.LogInformation("Idempotent request detected for {RequestName} with key {RequestKey}", requestName, requestKey);
            return JsonSerializer.Deserialize<TResponse>(cachedResponse)!;
        }

        // Process the request
        var response = await next();

        // Cache the response for future idempotent calls
        var responseJson = JsonSerializer.Serialize(response);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) // Cache for 24 hours
        };

        await _cache.SetStringAsync(cacheKey, responseJson, cacheOptions, cancellationToken);

        _logger.LogInformation("Cached response for idempotent request {RequestName} with key {RequestKey}", requestName, requestKey);

        return response;
    }

    private static string GenerateRequestKey(TRequest request)
    {
        // This is a simple implementation - in production, you might want to use a more sophisticated approach
        // such as hashing the request content or using a correlation ID from headers
        var requestHash = JsonSerializer.Serialize(request).GetHashCode();
        return $"{typeof(TRequest).Name}_{requestHash}";
    }
} 