using RabbitMQ.Client;
using Sprang.Core.Base;
using Sprang.Core.Features.Movimentacoes;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Sprang.Api.BackgroundServices;

public class MessageReceiver : DefaultBasicConsumer
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
            var message = body.ToArray();
            var mc2 = message.Deserialize<SerializableEvents>()!;
            var tasko = JsonSerializer.Deserialize<MovimentacaoCriadaEvent>(mc2.Message)!;
            Console.WriteLine($"Consuming Message");
            Console.WriteLine(string.Concat("Message received from the exchange ", exchange));
            Console.WriteLine(string.Concat("Consumer tag: ", consumerTag));
            Console.WriteLine(string.Concat("Delivery tag: ", deliveryTag));
            Console.WriteLine(string.Concat("Routing tag: ", routingKey));
            _deliveries.Add((body.ToArray(), properties, deliveryTag));
            _channel.BasicAck(deliveryTag, false);
        }
        catch (Exception ex)
        {
            //Model.BasicAck(deliveryTag, false);
        }
    }
}
