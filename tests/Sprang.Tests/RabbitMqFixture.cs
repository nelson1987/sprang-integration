using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Sprang.Tests;

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
