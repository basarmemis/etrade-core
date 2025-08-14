namespace etrade_core.application.Common.Base;

/// <summary>
/// Marker interface for MediatR requests
/// </summary>
public interface IRequest
{
}

/// <summary>
/// Marker interface for MediatR requests that return a response
/// </summary>
/// <typeparam name="TResponse">The type of response</typeparam>
public interface IRequest<out TResponse> : IRequest
{
} 