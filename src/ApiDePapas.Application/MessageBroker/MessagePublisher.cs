using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ApiDePapas.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace ApiDePapas.Infrastructure.MessageBroker
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IConfiguration _config;

        public MessagePublisher(IConfiguration config)
        {
            _config = config;
        }

        public Task PublishAsync<T>(T message, string exchangeName)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config["MessageBroker:Host"],
                UserName = _config["MessageBroker:Username"],
                Password = _config["MessageBroker:Password"],
                VirtualHost = _config["MessageBroker:VirtualHost"]
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: null, body: body);

            return Task.CompletedTask;
        }
    }
}
