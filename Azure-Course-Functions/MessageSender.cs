using System;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzCourse.Function;

public class MessageSender
{
    private readonly ILogger _logger;

    public MessageSender(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<MessageSender>();
    }

    [Function("MessageSender")]
    public void Run([TimerTrigger("*/5 * * * * *")] TimerInfo myTimer)
    {
        var message = $"C# Timer trigger function executed at: {DateTime.Now}";

        HttpRequestMessage requestMessage = new(HttpMethod.Post, "http://localhost:7071/api/MessageReceiver");
        requestMessage.Content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");

        new HttpClient().Send(requestMessage);
        _logger.LogInformation("Timer function executed");
    }
}