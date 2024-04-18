using RabbitMQ.Client;

namespace Sprang.Api.BackgroundServices;

public class Consumer
{
    private async Task WaitAndRunChannel(IModel channel, string queueName)
    {
        MessageReceiver messageReceiver = new MessageReceiver(channel);
        channel.BasicConsume("demoqueue", false, messageReceiver);
    }

    public async Task Consume(CancellationToken cancanCancellationToken)
    {
        try
        {
            await Task.Run(() =>
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri("amqp://guest:guest@localhost:5672/")
                };
                using var connection = factory.CreateConnection();
                var channel = connection.CreateModel();
                channel.ExchangeDeclare("demoExchange", ExchangeType.Direct);
                channel.QueueDeclare("demoqueue", false, false, false, null);
                channel.QueueBind("demoqueue", "demoExchange", "directexchange_key");
                channel.BasicQos(0, 1, true);
                WaitAndRunChannel(channel, "demoqueue");

            }, cancanCancellationToken);
        }
        catch (Exception ex)
        {
            //_logger.LogWarning(ex, "Error during ping");
        }

        await Task.CompletedTask;
    }
}
