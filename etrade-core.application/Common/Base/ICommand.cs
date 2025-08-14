namespace etrade_core.application.Common.Base;

/// <summary>
/// Marker interface for commands
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Marker interface for commands that return a response
/// </summary>
/// <typeparam name="TResponse">The type of response</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>, ICommand
{
} 