using PkiptSubstitutionBot.client.Application.Consts;
using PkiptSubstitutionBot.client.Application.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PkiptSubstitutionBot.client.Application.Services;

public class RabbitMqService
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    private RabbitMqService(
        IConnection connection, 
        IChannel channel)
    {
        _connection = connection;
        _channel = channel;
    }

    public static async Task<RabbitMqService> CreateAsync(RabbitMqOptions rabbitMqOptions)
    {
        var factory = new ConnectionFactory 
        { 
            HostName = rabbitMqOptions.HostName, 
            Port = rabbitMqOptions.Port, 
            UserName = rabbitMqOptions.UserName, 
            Password = rabbitMqOptions.Password
        };
        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: RabbitMqQueueNameConsts.Subst,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        await channel.QueueDeclareAsync(
            queue: RabbitMqQueueNameConsts.Messages,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        return new RabbitMqService(connection, channel);
    }

    public async Task SendMessageAsync<T>(T message, string queueName)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        await _channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            mandatory: true,
            basicProperties: new BasicProperties
            {
                Persistent = true
            },
            body: body);
    }
}
