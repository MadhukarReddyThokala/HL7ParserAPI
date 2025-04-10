public interface IRabbitMQService
{
    void Publish(Guid recordId, String parsedMessage);
}
