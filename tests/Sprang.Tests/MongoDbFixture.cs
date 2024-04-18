using MongoDB.Driver;
using Sprang.Core.Features.ContasBancarias;

namespace Sprang.Tests;

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
