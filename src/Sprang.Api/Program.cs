using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Sprang.Api.BackgroundServices;
using Sprang.Api.Features;
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

//builder.Host.UseSerilog((context, loggerConfig) =>
//    loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddRateLimiter(x =>
{
    x.AddFixedWindowLimiter("Fixed", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 5;
    });
});

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

app.UseExceptionHandler(x =>
{
    x.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature is not null)
        {
            await context.Response.WriteAsJsonAsync(new
            {
                StatusCodes = context.Response.StatusCode,
                Message = "Internal Server Error"
            });
        }
    });
});
//app.UseSerilogRequestLogging();
//app.UseRequestContextLogging();
app.Run();

public partial class Program
{
}