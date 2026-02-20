using Branch.Platform.Application;
using Branch.Platform.Infrastructure;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Host.UseSerilog((context, cfg) => cfg.ReadFrom.Configuration(context.Configuration).WriteTo.Console());

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, runOutboxDispatcher: true);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Branch.Platform.Worker"))
    .WithTracing(t => t
        .AddSource("Branch.Platform.Application")
        .AddSource("Branch.Platform.Orders")
        .AddOtlpExporter());

var host = builder.Build();
await host.RunAsync();
