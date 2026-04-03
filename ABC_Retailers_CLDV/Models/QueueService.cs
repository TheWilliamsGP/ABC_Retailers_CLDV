using Azure.Storage.Queues;

public class QueueService
{
    private readonly QueueServiceClient _queueServiceClient;

    public QueueService(IConfiguration configuration)
    {
        _queueServiceClient = new QueueServiceClient(configuration["AzureStorage:ConnectionString"]);
    }

    public QueueClient GetQueue(string queueName)
    {
        var queue = _queueServiceClient.GetQueueClient(queueName);
        queue.CreateIfNotExists();
        return queue;
    }

    public void SendMessage(string queueName, string message)
    {
        var queue = GetQueue(queueName);
        queue.SendMessage(message);
    }
}