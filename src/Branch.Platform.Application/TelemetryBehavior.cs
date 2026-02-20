using System.Diagnostics;
using MediatR;

namespace Branch.Platform.Application;

public sealed class TelemetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly ActivitySource Source = new("Branch.Platform.Application");

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using var activity = Source.StartActivity($"application.{typeof(TRequest).Name}");
        activity?.SetTag("request.type", typeof(TRequest).FullName);
        return await next();
    }
}
