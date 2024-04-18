namespace Sprang.Tests;

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
