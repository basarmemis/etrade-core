namespace etrade_core.application.Common.Base;

/// <summary>
/// Interface for sending requests through MediatR
/// </summary>
public interface ISender
{
    /// <summary>
    /// Sends a request to a single handler
    /// </summary>
    /// <typeparam name="TResponse">The type of response</typeparam>
    /// <param name="request">The request to send</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>The response from the handler</returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request to a single handler that doesn't return a response
    /// </summary>
    /// <param name="request">The request to send</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task Send(IRequest request, CancellationToken cancellationToken = default);
} 