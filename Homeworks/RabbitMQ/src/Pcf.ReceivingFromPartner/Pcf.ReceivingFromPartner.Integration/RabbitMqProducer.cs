using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Pcf.ReceivingFromPartner.Core.Abstractions;
using Pcf.ReceivingFromPartner.Core.Messages;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class RabbitMqProducer : IRabbitMqProducer, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqProducer(IConnection connection)
        {
            _connection = connection;
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("promocode_exchange", ExchangeType.Fanout);
        }

        public void PublishPromoCodeIssued(PromoCodeIssuedMessage message)
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(
                exchange: "promocode_exchange",
                routingKey: "",
                basicProperties: null,
                body: body);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
