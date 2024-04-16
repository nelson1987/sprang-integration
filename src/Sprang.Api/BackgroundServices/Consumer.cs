using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace Sprang.ApiTests.HostedServices
{
    public class PingConsumer : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                //_logger.LogInformation("{BackgroundService} running at '{Date}', pinging '{URL}'",
                //    nameof(PingBackgroundService), DateTime.Now, _configuration.Url);
                try
                {
                    //var factory = new ConnectionFactory { HostName = "localhost" };
                    //using var connection = factory.CreateConnection();
                    //using var channel = connection.CreateModel();

                    //channel.QueueDeclare(queue: "hello",
                    //    durable: false,
                    //    exclusive: false,
                    //    autoDelete: false,
                    //    arguments: null);

                    //var consumer = new EventingBasicConsumer(channel);
                    //consumer.Received += (model, ea) =>
                    //{
                    //    var body = ea.Body.ToArray();
                    //    var message = Encoding.UTF8.GetString(body);
                    //    Console.WriteLine($" [x] Received {message}");
                    //};
                    //channel.BasicConsume(queue: "hello",
                    //    autoAck: true,
                    //    consumer: consumer);
                    /*
                    //using var response = await _client.GetAsync(_configuration.Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                    //_logger.LogInformation("Is '{Host}' responding: {Status}",
                    //    _configuration.Value.Url.Authority, response.IsSuccessStatusCode);


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
                    //Thread.Sleep(TimeSpan.FromSeconds(5));
                    channel.BasicConsume(queue: "orders", autoAck: true, consumer: consumer);
                    */
                }
                catch (Exception ex)
                {
                    //_logger.LogWarning(ex, "Error during ping");
                }
            }
        }
    }
}
