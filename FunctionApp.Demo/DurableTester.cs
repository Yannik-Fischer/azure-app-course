using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace FunctionApp.Demo
{
    public static class DurableTester
    {
        [Function(nameof(DurableTester))]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(DurableTester));
            logger.LogInformation("Saying hello.");
            var outputs = new List<string>();

            var input = context.GetInput<string>();

            // Replace name and input with values relevant for your Durable Functions Activity
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));

            outputs.Add(await context.CallActivityAsync<string>(nameof(AddToQueue), input));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        [Function(nameof(SayHello))]
        public static string SayHello([ActivityTrigger] string name, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("SayHello");
            logger.LogInformation("Saying hello to {name}.", name);

            return $"Hello {name}!";
        }

        [Function(nameof(AddToQueue))]
        [QueueOutput("outqueue-durable")]
        public static string AddToQueue([ActivityTrigger] string messageToAdd, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("AddToQueue");
            logger.LogInformation("Message added to Queue: {messageToAdd}", messageToAdd);

            var output = messageToAdd;

            return messageToAdd;
        }

        [Function("DurableTester_HttpStart")]
        public static async Task<HttpResponseData> HttpStart([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req, [DurableClient] DurableTaskClient client, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("DurableTester_HttpStart");

            var reader = new StreamReader(req.Body);
            var requestBody = await reader.ReadToEndAsync();

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(DurableTester), requestBody);

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            return await client.CreateCheckStatusResponseAsync(req, instanceId);
        }
    }
}
