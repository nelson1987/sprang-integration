using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Sprang.Api.BackgroundServices;
public class PingWebsiteSettings
{
    public Uri Url { get; set; }
    public int TimeIntervalInMinutes { get; set; }
}

public class PingBackgroundService : BackgroundService
{
    private readonly HttpClient _client;
    private readonly ILogger<PingBackgroundService> _logger;
    private readonly PingWebsiteSettings _configuration;

    public PingBackgroundService(
        IHttpClientFactory httpClientFactory,
        ILogger<PingBackgroundService> logger,
        IOptions<PingWebsiteSettings> configuration)
    {
        _client = httpClientFactory.CreateClient(nameof(PingBackgroundService));
        _logger = logger;
        _configuration = configuration.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("{BackgroundService} running at '{Date}', pinging '{URL}'",
                nameof(PingBackgroundService), DateTime.Now, _configuration.Url);
            try
            {
                using var response = await _client.GetAsync(_configuration.Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                //_logger.LogInformation("Is '{Host}' responding: {Status}",
                //    _configuration.Value.Url.Authority, response.IsSuccessStatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during ping");
            }
        }
    }
}

public class SampleHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = true;

        if (isHealthy)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("A healthy result."));
        }

        return Task.FromResult(
            new HealthCheckResult(
                context.Registration.FailureStatus, "An unhealthy result."));
    }
}