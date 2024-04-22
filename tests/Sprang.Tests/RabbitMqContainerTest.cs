using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Sprang.Core.Features.Movimentacoes;
using Testcontainers.RabbitMq;

namespace Sprang.Tests;

public sealed class RabbitMqContainerTest : IAsyncLifetime
{
    private readonly RabbitMqContainer _rabbitMqContainer
        = new RabbitMqBuilder().Build();

    public Task InitializeAsync()
        => _rabbitMqContainer.StartAsync();

    public Task DisposeAsync()
        => _rabbitMqContainer.DisposeAsync().AsTask();

    [Fact]
    public void ConsumeMessageFromQueue()
    {
        const string queue = "hello";

        const string message = "Hello World!";

        string actualMessage = null;

        // Signal the completion of message reception.
        EventWaitHandle waitHandle = new ManualResetEvent(false);

        // Create and establish a connection.
        var connectionFactory = new ConnectionFactory();
        connectionFactory.Uri = new Uri(_rabbitMqContainer.GetConnectionString());
        using var connection = connectionFactory.CreateConnection();

        // Send a message to the channel.
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue, false, false, false, null);
        channel.BasicPublish(string.Empty, queue, null, Encoding.Default.GetBytes(message));

        // Consume a message from the channel.
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, eventArgs) =>
        {
            actualMessage = Encoding.Default.GetString(eventArgs.Body.ToArray());
            waitHandle.Set();
        };

        channel.BasicConsume(queue, true, consumer);
        waitHandle.WaitOne(TimeSpan.FromSeconds(1));

        Assert.Equal(message, actualMessage);
    }
}

public sealed class RabbitMqConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new RabbitMqConfigurationProvider();
    }
}

public sealed class RabbitMqConfigurationProvider : ConfigurationProvider
{
    private static readonly TaskFactory TaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

    public override void Load()
    {
        // Until the asynchronous configuration provider is available,
        // we use the TaskFactory to spin up a new task that handles the work:
        // https://github.com/dotnet/runtime/issues/79193
        // https://github.com/dotnet/runtime/issues/36018
        TaskFactory.StartNew(LoadAsync)
            .Unwrap()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    public async Task LoadAsync()
    {
        var rabbitMqContainer = new RabbitMqBuilder().Build();

        await rabbitMqContainer.StartAsync()
                .ConfigureAwait(false);

        Set("ConnectionStrings:RabbitMq", rabbitMqContainer.GetConnectionString());
    }
}

public class TesteControllerIntegrationTests : IntegrationTests, IClassFixture<TigerApiFixture>
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly TigerApiFixture _tigerApiFixture;
    private MovimentacaoCommand _command => _fixture.Build<MovimentacaoCommand>().Create();

    public TesteControllerIntegrationTests(TigerApiFixture tigerApiFixture)
    {
        _tigerApiFixture = tigerApiFixture;
    }

    [Fact]
    public async Task Dado_Requisicao_Valida_Retorna_Created()
    {
        // Act
        var updateFinancialDataAsString = System.Text.Json.JsonSerializer.Serialize(_command);
        using var stringContent = new StringContent(updateFinancialDataAsString, Encoding.UTF8, "application/json");
        await _tigerApiFixture.Client.PostAsync("/movimentacoes", stringContent);
    }
}
