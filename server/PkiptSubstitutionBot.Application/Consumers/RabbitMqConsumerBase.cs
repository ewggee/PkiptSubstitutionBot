using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PkiptSubstitutionBot.Application.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PkiptSubstitutionBot.Application.Consumers;

public abstract class RabbitMqConsumerBase : BackgroundService
{
    protected readonly ILogger<RabbitMqConsumerBase> _logger;
    private readonly RabbitMqOptions _rabbitMqOptions;
    protected IConnection _connection;
    protected IChannel _channel;
    protected readonly string _queueName;

    public RabbitMqConsumerBase(
        ILogger<RabbitMqConsumerBase> logger, 
        IOptions<RabbitMqOptions> rabbitMqOptions,
        string queueName)
    {
        _logger = logger;
        _rabbitMqOptions = rabbitMqOptions.Value;
        _queueName = queueName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.HostName,
            Port = _rabbitMqOptions.Port,
            UserName = _rabbitMqOptions.UserName,
            Password = _rabbitMqOptions.Password
        };
        _connection = await factory.CreateConnectionAsync(
            cancellationToken: stoppingToken);
        _channel = await _connection.CreateChannelAsync(
            cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += ReceiveAsync;

        await _channel.BasicConsumeAsync(
            queue: _queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task ReceiveAsync(object obj, BasicDeliverEventArgs eventArgs)
    {
        try
        {
            var body = eventArgs.Body.ToArray();
            var encodedMessage = Encoding.UTF8.GetString(body);

            await ProccessMessageAsync(encodedMessage, default);

            await _channel.BasicAckAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка обработки сообщения");
            await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, true);
        }
    }

    protected abstract Task ProccessMessageAsync(string encodedMessage, CancellationToken ct);
}
