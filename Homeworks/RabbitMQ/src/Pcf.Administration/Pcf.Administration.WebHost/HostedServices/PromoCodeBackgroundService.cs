using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Pcf.Administration.Core.Messages;
using Pcf.Administration.Core.Services;

namespace Pcf.Administration.WebHost.HostedServices
{
    public class PromoCodeBackgroundService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IModel _channel;

        public PromoCodeBackgroundService(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var host = _configuration["RabbitMq:Host"] ?? "localhost";
            var factory = new ConnectionFactory { HostName = host };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("promocode_exchange", ExchangeType.Fanout);
            _channel.QueueDeclare("administration_promocode_queue", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind("administration_promocode_queue", "promocode_exchange", "");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<PromoCodeIssuedMessage>(json);

                if (message?.PartnerManagerId.HasValue == true)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var employeeService = scope.ServiceProvider.GetRequiredService<IEmployeeService>();
                    await employeeService.UpdateAppliedPromocodesAsync(message.PartnerManagerId.Value);
                }
            };

            _channel.BasicConsume(queue: "administration_promocode_queue", autoAck: true, consumer: consumer);

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();
            return base.StopAsync(cancellationToken);
        }
    }
}
