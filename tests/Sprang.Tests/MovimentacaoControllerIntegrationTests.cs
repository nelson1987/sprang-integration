using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sprang.Core;
using System.Text;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http.Headers;

namespace Sprang.Tests;

public class MovimentacaoControllerIntegrationTests : IntegrationTests, IClassFixture<TigerApiFixture>
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly TigerApiFixture _tigerApiFixture;
    private MovimentacaoCommand _command => _fixture.Build<MovimentacaoCommand>().Create();

    public MovimentacaoControllerIntegrationTests(TigerApiFixture tigerApiFixture)
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

        //var products = MongoDbFixture.Produtos();
        //Assert.Equal(products.Count, 1);

        //var consumed = RabbitMqFixture.Produtos();
        //Assert.Equal(consumed.Count, 1);
    }
}

public static class MongoDbFixture
{
    public static List<ContaBancaria> Produtos()
    {

        string MongoDBConnectionString = "mongodb://root:password@localhost:27017/";
        var client = new MongoClient(MongoDBConnectionString);
        var session = client.StartSession();
        var products = session.Client.GetDatabase("MongoDBStore")
            .GetCollection<ContaBancaria>("products");
        return products.Find(x => x.Id != Guid.Empty).ToList();
    }
}

public static class RabbitMqFixture
{
    public static List<string> Produtos()
    {
        List<string> consumeList = new List<string>();

        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "orders",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        //Set Event object which listen message from chanel which is sent by producer
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            consumeList.Add(message);
        };
        Thread.Sleep(TimeSpan.FromSeconds(5));
        channel.BasicConsume(queue: "orders", autoAck: true, consumer: consumer);
        Thread.Sleep(TimeSpan.FromSeconds(5));
        return consumeList;
    }

}

public class TriggonApi : WebApplicationFactory<Program>
{
    static TriggonApi()
        => Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Test");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder.UseEnvironment("Test")
            .ConfigureTestServices(services =>
            {
            });
}

public class TigerApiFixture
{
    private static readonly TriggonApi _server;
    private static readonly HttpClient _client;

    public TriggonApi Server => _server;
    public HttpClient Client => _client;
    static TigerApiFixture()
    {
        _server = new();
        _client = _server.CreateDefaultClient();
    }
    public TigerApiFixture()
    {
        _client.DefaultRequestHeaders.Clear();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(scheme: "TestScheme");
    }
}

public abstract class IntegrationTests : IAsyncLifetime
{
    protected IntegrationTests()
    {

    }

    public async Task InitializeAsync()
    {
        //await KafkaFixture.ClearTopicsMessages();
        //await MongoDbFixture.CreateDatabase("warehouseTests", "Solicitacao");
    }

    public async Task DisposeAsync()
    {
        //await MongoDbFixture.ClearDatabase("warehouseTests", "Solicitacao");
    }
}

public static class KafkaFixture
{
    public static async Task ClearTopicsMessages()
    {
        await Task.CompletedTask;
    }
    public static async Task ConsumeTopicsMessages()
    {
        await Task.CompletedTask;
    }
}