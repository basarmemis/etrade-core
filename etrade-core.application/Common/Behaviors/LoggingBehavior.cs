using MediatR;
using Microsoft.Extensions.Logging;

namespace etrade_core.application.Common.Behaviors;

/// <summary>
/// Pipeline behavior for logging requests and responses
/// </summary>
/// <typeparam name="TRequest">The type of request</typeparam>
/// <typeparam name="TResponse">The type of response</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid();

        _logger.LogInformation("Handling {RequestName} with ID {RequestId}", requestName, requestId);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            var response = await next();
            sw.Stop();

            _logger.LogInformation("Handled {RequestName} with ID {RequestId} in {ElapsedMilliseconds}ms", 
                requestName, requestId, sw.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Error handling {RequestName} with ID {RequestId} after {ElapsedMilliseconds}ms", 
                requestName, requestId, sw.ElapsedMilliseconds);
            throw;
        }
    }
} 