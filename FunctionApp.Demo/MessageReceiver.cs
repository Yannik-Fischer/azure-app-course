using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp.Demo
{
    public class MessageReceiver
    {
        private readonly ILogger<MessageReceiver> _logger;

        public MessageReceiver(ILogger<MessageReceiver> logger)
        {
            _logger = logger;
        }

        [QueueOutput("message-queue")]
        [Function("MessageReceiver")]
        public async Task<string> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            return requestBody;
        }
    }
}
