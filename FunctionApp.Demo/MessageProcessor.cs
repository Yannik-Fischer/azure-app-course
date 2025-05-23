using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp.Demo
{
    public class MessageProcessor
    {
        private readonly ILogger<MessageProcessor> _logger;

        public MessageProcessor(ILogger<MessageProcessor> logger)
        {
            _logger = logger;
        }

        [Function(nameof(MessageProcessor))]
        public void Run([QueueTrigger("message-queue", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);
        }
    }
}
