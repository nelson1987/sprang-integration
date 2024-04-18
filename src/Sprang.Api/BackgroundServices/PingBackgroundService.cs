using Microsoft.Extensions.Options;

namespace Sprang.Api.BackgroundServices;

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
        Consumer consumer = new Consumer();
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("{BackgroundService} running at '{Date}', pinging '{URL}'",
                nameof(PingBackgroundService), DateTime.Now, _configuration.Url);
            try
            {
                await consumer.Consume(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during ping");
            }
        }
    }
}
