using Azure.Storage.Queues;

namespace ABC_Retailers_CLDV.Models
{
    public class QueueService
    {
        private readonly QueueServiceClient _client;

        public QueueService(IConfiguration config)
        {
            _client = new QueueServiceClient(config["AzureStorage:ConnectionString"]);
        }

        public QueueClient GetQueue(string queueName)
        {
            var queue = _client.GetQueueClient(queueName);
            queue.CreateIfNotExists();
            return queue;
        }

        public void SendMessage(string queueName, string message)
        {
            var queue = GetQueue(queueName);
            queue.SendMessage(message);
        }
    }
}
