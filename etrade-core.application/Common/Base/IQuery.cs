namespace etrade_core.application.Common.Base;

/// <summary>
/// Marker interface for queries
/// </summary>
/// <typeparam name="TResponse">The type of response</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
} 