using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class RabbitMQService : IRabbitMQService
{
    private readonly string _hostName;
    private readonly string _queueName;
    private readonly ILogger<RabbitMQService> _logger;

    public RabbitMQService(IConfiguration configuration, ILogger<RabbitMQService> logger)
    {
        // Load configuration from appsettings or environment variables
        _hostName = configuration["RabbitMQ:Host"] ?? "localhost";
        _queueName = configuration["RabbitMQ:QueueName"] ?? "hl7_queue";
        _logger = logger;
    }

    public void Publish(Guid id, string json)
    {
        try
        {
            var factory = new ConnectionFactory() { HostName = _hostName };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declare the queue (durable, non-exclusive, autoDelete=false)
            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var properties = channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object> { { "unique_id", id.ToString() } };

            var body = Encoding.UTF8.GetBytes(json);

            // Publish the message
            channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: properties, body: body);

            _logger.LogInformation($"Message with ID {id} published to RabbitMQ queue {_queueName}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error publishing message to RabbitMQ: {ex.Message}");
            throw new ApplicationException("Failed to publish message to RabbitMQ.", ex);
        }
    }
}
