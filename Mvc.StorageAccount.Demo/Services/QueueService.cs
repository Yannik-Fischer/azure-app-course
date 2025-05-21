using Azure.Storage.Queues;
using Mvc.StorageAccount.Demo.Data;
using Newtonsoft.Json;

namespace Mvc.StorageAccount.Demo.Services
{
    public class QueueService : IQueueService
    {
        private const string queueName = "attendee-emails";
        private readonly IConfiguration _configuration;

        public QueueService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMessage(EmailMessage emailMessage)
        {
            var queueClient = await GetQueueClient();
            await queueClient.SendMessageAsync(JsonConvert.SerializeObject(emailMessage));
        }

        private async Task<QueueClient> GetQueueClient()
        {
            var queueClient = new QueueClient(_configuration["StorageConnectionString"], queueName, new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });

            await queueClient.CreateIfNotExistsAsync();

            return queueClient;
        }
    }
}
