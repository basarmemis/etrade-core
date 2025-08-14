using MediatR;
using Microsoft.Extensions.Logging;
using etrade_core.application.IRepositories;

namespace etrade_core.application.Common.Behaviors;

/// <summary>
/// Pipeline behavior for handling database transactions
/// </summary>
/// <typeparam name="TRequest">The type of request</typeparam>
/// <typeparam name="TResponse">The type of response</typeparam>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var isCommand = typeof(TRequest).GetInterfaces().Any(i => i.Name.Contains("ICommand"));

        if (!isCommand)
        {
            // Queries don't need transactions
            return await next();
        }

        _logger.LogInformation("Starting transaction for {RequestName}", requestName);

        try
        {
            var response = await next();
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Transaction committed for {RequestName}", requestName);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed for {RequestName}", requestName);
            throw;
        }
    }
} 