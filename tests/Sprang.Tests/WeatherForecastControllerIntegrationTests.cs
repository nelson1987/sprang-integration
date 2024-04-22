using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Security.Authentication;
using Testcontainers.MongoDb;

namespace Senat.Tests;

public class CreateProduct
{
    public record Command(string Name, string Category, decimal Price);
}

public class WeatherForecastControllerIntegrationTests : BaseIntegrationTest
{
    public WeatherForecastControllerIntegrationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public void Test1()
    {

        //var updateFinancialDataAsString = System.Text.Json.JsonSerializer.Serialize(_command);
        //using var stringContent = new StringContent(updateFinancialDataAsString, Encoding.UTF8, "application/json");
        //await _tigerApiFixture.Client.PostAsync("/WeatherForecast", stringContent);
    }

}

public class Product
{
    [BsonId]
    public ObjectId Id { get; set; }
    [BsonElement("SKU")]
    public int SKU { get; set; }
    [BsonElement("Description")]
    public string Description { get; set; }
    [BsonElement("Price")]
    public Double Price { get; set; }
}
public class MongoClientIntegrationTests : BaseIntegrationTest
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private CreateProduct.Command _command => _fixture.Build<CreateProduct.Command>().Create();

    public MongoClientIntegrationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task ReadFromMongoDbDatabase()
    {
        using var databases = await MongoClient.ListDatabasesAsync();
        Assert.True(await databases.AnyAsync());
    }


    [Fact]
    public async Task CreateFromMongoDbDatabase()
    {
        var tv = new Product
        {
            Description = "Television",
            SKU = 4001,
            Price = 2000
        };

        var database = MongoClient.GetDatabase("MongoDBStore");
        IMongoCollection<Product> products = database.GetCollection<Product>("products");
        await products.InsertOneAsync(tv);

        IMongoCollection<Product> productsList = database.GetCollection<Product>("products");
        var resultsBeforeUpdates = await productsList
            .Find<Product>(Builders<Product>.Filter.Empty)
            .FirstOrDefaultAsync();

        Assert.NotNull(tv.Id);
        Assert.Equal(tv.Description, resultsBeforeUpdates.Description);
        Assert.Equal(tv.SKU, resultsBeforeUpdates.SKU);
        Assert.Equal(tv.Price, resultsBeforeUpdates.Price);
    }
    /*
    [Fact]
    public async Task InsertFromMongoDbDatabase()
    {
        // Create some sample data
        var tv = new Product
        {
            Description = "Television",
            SKU = 4001,
            Price = 2000
        };
        var book = new Product
        {
            Description = "A funny book",
            SKU = 43221,
            Price = 19.99
        };
        var dogBowl = new Product
        {
            Description = "Bowl for Fido",
            SKU = 123,
            Price = 40.00
        };
        var database = MongoClient.GetDatabase("MongoDBStore");
        var collection = database.GetCollection<BsonDocument>("myCollection");

        using var session = MongoClient.StartSession();
        session.StartTransaction();
        try
        {
            await collection.InsertOneAsync(new BsonDocument("key", "value1"));
            await collection.InsertOneAsync(session, new BsonDocument("key", "value2"));
            await collection.InsertOneAsync(session, new BsonDocument("key", "value3"));
            await session.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }

        /*
        var session = await MongoClient.StartSessionAsync();
        session.StartTransaction();

        try
        {
            var database = session.Client.GetDatabase("MongoDBStore");

            IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>("test");
            // using code from earlier examples... // Create 
            await collection.InsertOneAsync(new BsonDocument() { ["Name"] = "Nick Cosentino", });
            // Read 
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = filterBuilder.Eq("Name", "Nick Cosentino");
            var results = collection.Find(filter);
            // Update 
            var updateBuilder = Builders<BsonDocument>.Update;
            var update = updateBuilder.Set("Name", "Dev Leader");
            collection.UpdateOne(filter, update);
            // Delete 
            filter = filterBuilder.Eq("Name", "Dev Leader");
            collection.DeleteOne(filter);


            //await database.CreateCollectionAsync("products");
            //IMongoCollection<Product> products = database.GetCollection<Product>("products");
            //await products.InsertOneAsync(session, tv);
            //await products.InsertOneAsync(session, book);
            //await products.InsertOneAsync(session, dogBowl);

            //var resultsBeforeUpdates = await products
            //    .Find<Product>(session, Builders<Product>.Filter.Empty)
            //    .ToListAsync();
            //Console.WriteLine("Original Prices:\n");
            //foreach (Product d in resultsBeforeUpdates)
            //{
            //    Console.WriteLine(
            //        String.Format("Product Name: {0}\tPrice: {1:0.00}",
            //            d.Description, d.Price)
            //    );
            //}

            // Increase all the prices by 10% for all products
            //var update = new UpdateDefinitionBuilder<Product>()
            //    .Mul<Double>(r => r.Price, 1.1);
            //await products.UpdateManyAsync(session,
            //    Builders<Product>.Filter.Empty,
            //    update); //,options);
            /*
             *

               MongoClientSettings settings = new MongoClientSettings();
               settings.Server = new MongoServerAddress(_host, _port);

               settings.UseTls = false;
               settings.SslSettings = new SslSettings();
               settings.SslSettings.EnabledSslProtocols = SslProtocols.Tls12;

               MongoIdentity identity = new MongoInternalIdentity(_authDbName, _userName);
               MongoIdentityEvidence evidence = new PasswordEvidence(_password);

               settings.Credential = new MongoCredential(_authMechanism, identity, evidence);

               MongoClient client = new MongoClient(settings);
               _mongoDb = client.GetDatabase(_dbName);
             *
             
             
        await session.CommitTransactionAsync();
        }
        catch (Exception e)
        {
            //Console.WriteLine("Error writing to MongoDB: " + e.Message);
            await session.AbortTransactionAsync();
            throw;
        }

        //Assert.True(await databases.AnyAsync());
        
    }
    */
}

public class IntegrationTestWebAppFactory
    : WebApplicationFactory<Program>,
        IAsyncLifetime
{
    protected IConfiguration _connection { get; set; }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder.ConfigureAppConfiguration(configure =>
        {
            configure.Add(new RabbitMqConfigurationSource());
            _connection = configure.Build();
        }).ConfigureTestServices(Services =>
        {
            Services.AddScoped<IMongoClient, MongoClient>(x =>
                new MongoClient(_connection.GetSection("MongoDb:ConnectionStrings").Value));
        });

    public async Task InitializeAsync()
    {
        // return _dbContainer.StartAsync();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        //return _dbContainer.StopAsync();
        await Task.CompletedTask;
    }
}

public abstract class BaseIntegrationTest
    : IClassFixture<IntegrationTestWebAppFactory>,
        IDisposable
{
    private readonly IServiceScope _scope;

    protected readonly IMongoClient MongoClient;
    //protected readonly ISender Sender;
    //protected readonly ApplicationDbContext DbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        MongoClient = _scope.ServiceProvider.GetRequiredService<IMongoClient>();
        //Sender = _scope.ServiceProvider.GetRequiredService<ISender>();

        //DbContext = _scope.ServiceProvider
        //    .GetRequiredService<ApplicationDbContext>();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        //DbContext?.Dispose();
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
        TaskFactory.StartNew(LoadAsync)
            .Unwrap()
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();
    }

    public async Task LoadAsync()
    {
        var MongoDbContainer = new MongoDbBuilder()
            //.WithEnvironment("MONGO_INITDB_ROOT_USERNAME", "root")
            //.WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", "password")
            //.WithEnvironment("MONGO_INITDB_DATABASE", "MongoDBStore")
            //.WithVolumeMount("./scripts/mongo-init.js", "/docker-entrypoint-initdb.d/mongo-init.js:ro")
            //.WithCommand("mongosh", "--retryWrites")
            //.WithCommand("--replSet", "repro")
            // 
            //.WithCommand("mongod", "--replSet", "rs0")
            //.WithCommand("mongosh", "--retryWrites", "false")
            //.WithPortBinding("27017")
            .Build();

        await MongoDbContainer.StartAsync()
                .ConfigureAwait(false);

        Set("MongoDb:ConnectionStrings", MongoDbContainer.GetConnectionString());
        //Set("MongoDb:Hostname", MongoDbContainer.Hostname);
        //Set("MongoDb:Database", "MongoDBStore");
    }
}
