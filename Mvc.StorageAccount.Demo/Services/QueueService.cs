using Azure.Storage.Queues;
using Mvc.StorageAccount.Demo.Data;
using Newtonsoft.Json;

namespace Mvc.StorageAccount.Demo.Services
{
    public class QueueService : IQueueService
    {
        private readonly IConfiguration _configuration;
        private readonly QueueClient _queueClient;

        public QueueService(IConfiguration configuration, QueueClient queueClient)
        {
            _configuration = configuration;
            _queueClient = queueClient;
            _queueClient.CreateIfNotExists();
        }

        public async Task SendMessage(EmailMessage emailMessage)
        {
            await _queueClient.SendMessageAsync(JsonConvert.SerializeObject(emailMessage));
        }

        [Obsolete]
        private async Task<QueueClient> GetQueueClient()
        {
            var queueClient = new QueueClient(_configuration["AzureStorage:ConnectionString"], _configuration["AzureStorage:QueueName"], new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });

            await queueClient.CreateIfNotExistsAsync();

            return queueClient;
        }
    }
}
