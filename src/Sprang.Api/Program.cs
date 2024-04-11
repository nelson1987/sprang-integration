using Microsoft.Extensions.Diagnostics.HealthChecks;
using Sprang.Api.BackgroundServices;
using Sprang.Core;

var builder = WebApplication.CreateBuilder(args);
IConfiguration _configuration = builder.Configuration;

builder.Services.AddCore();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddHealthChecks()
    .Add(new HealthCheckRegistration(
        name: "SampleHealthCheck1",
        instance: new SampleHealthCheck(),
        failureStatus: null,
        tags: null,
        timeout: default)
    {
        Delay = TimeSpan.FromSeconds(40),
        Period = TimeSpan.FromSeconds(30)
    });


builder.Services.Configure<PingWebsiteSettings>(_configuration.GetSection("PingWebsite"));
builder.Services.AddHostedService<PingBackgroundService>();
builder.Services.AddHttpClient(nameof(PingBackgroundService));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapHealthChecks("/healthz");

app.Run();

public partial class Program
{
}