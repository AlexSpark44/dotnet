var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/ping", () => Results.Ok(new PingResponse("pong")))
    .WithName("Ping")
    .WithOpenApi();

app.Run();

public sealed record PingResponse(string Message);
public partial class Program;
