using RabbitMQ.Client;

namespace Sprang.Core.Base;

public interface IGenericProducer<T> where T : AuditableEvent
{
    Task Send(T @event, CancellationToken cancellationToken = default);
}

public class GenericProducer<T> : IGenericProducer<T> where T : AuditableEvent
{
    public Task Send(T @event, CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        factory.ClientProvidedName = "app:audit component:event-consumer";
        using var connection = factory.CreateConnection();
        using var model = connection.CreateModel();

        var message = SerializableMessageBuilder.Init(@event);
        var messagebuffer = message.Serialize();
        var properties = model.CreateBasicProperties();
        properties.ContentType = "text/plain";
        properties.DeliveryMode = 2;
        properties.Headers = new Dictionary<string, object>()
        {
            { "latitude", 51.5252949 },
            { "longitude", -0.0905493 }
        };
        properties.Persistent = false;
        properties.Expiration = TimeSpan.FromSeconds(30).Milliseconds.ToString();// "36000";
        properties.CorrelationId = Guid.NewGuid().ToString();

        model.BasicPublish("demoExchange", "directexchange_key", properties, messagebuffer);
        return Task.CompletedTask;
    }
}
