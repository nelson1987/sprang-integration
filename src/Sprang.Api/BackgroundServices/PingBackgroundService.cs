using System.Text;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Threading;
using System.Reflection;
using System.Threading.Channels;
using System;
using System.Collections.Concurrent;

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
        Consumer consumer = new Consumer();
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("{BackgroundService} running at '{Date}', pinging '{URL}'",
                nameof(PingBackgroundService), DateTime.Now, _configuration.Url);
            try
            {
                var response = await consumer.Consume(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during ping");
            }
        }
    }
}

public class MessageReceiver : DefaultBasicConsumer, IDisposable
{
    private readonly IModel _channel;
    private readonly BlockingCollection<(byte[] Body, IBasicProperties Properties, ulong deliveryTag)> _deliveries = new();
    private ulong _latestDeliveryTag;
    public MessageReceiver(IModel channel) : base(channel)
    {
        _channel = channel;
    }

    public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey,
        IBasicProperties properties, ReadOnlyMemory<byte> body)
    {
        try
        {

            Console.WriteLine($"Consuming Message");
            Console.WriteLine(string.Concat("Message received from the exchange ", exchange));
            Console.WriteLine(string.Concat("Consumer tag: ", consumerTag));
            Console.WriteLine(string.Concat("Delivery tag: ", deliveryTag));
            Console.WriteLine(string.Concat("Routing tag: ", routingKey));
            var message = body.ToArray();
            Console.WriteLine(string.Concat("Message: ", Encoding.UTF8.GetString(message)));
            _deliveries.Add((body.ToArray(), properties, deliveryTag));
        }
        catch (Exception ex)
        {
            //Model.BasicAck(deliveryTag, false);
        }
    }

    public void Dispose()
    {
        //
        //_channel.BasicNack(_latestDeliveryTag, false, true);
        //_channel.BasicAck(_latestDeliveryTag, false);
        _channel.Dispose();
    }
}

public class Consumer
{
    public async Task<int> Consume(CancellationToken cancanCancellationToken)
    {
        try
        {
            await Task.Run(() =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest",
                };
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.ExchangeDeclare("demoExchange", ExchangeType.Direct);
                channel.QueueDeclare("demoqueue", false, false, false, null);
                channel.QueueBind("demoqueue", "demoExchange", "directexchange_key");
                channel.BasicQos(0, 1, true);
                MessageReceiver messageReceiver = new MessageReceiver(channel);
                channel.BasicConsume("demoqueue", false, messageReceiver);
            }, cancanCancellationToken);
        }
        catch (Exception ex)
        {
            //_logger.LogWarning(ex, "Error during ping");
        }

        return await Task.FromResult(0);
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