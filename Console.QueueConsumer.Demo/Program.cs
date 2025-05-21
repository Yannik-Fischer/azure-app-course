// See https://aka.ms/new-console-template for more information

using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text;

var connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
var queueClient = new QueueClient(connectionString, "attendee-emails");

if (await queueClient.ExistsAsync())
{
    QueueProperties properties = await queueClient.GetPropertiesAsync();

    for (var i = 0; i < properties.ApproximateMessagesCount; i++)
    {
        var message = await RetrieveNextMessage();
        Console.WriteLine($"Received: {message}");
    }
}

async Task<string> RetrieveNextMessage()
{
    QueueMessage[] retrievedMessages = await queueClient.ReceiveMessagesAsync(1);
    var data = Convert.FromBase64String(retrievedMessages[0].Body.ToString());
    var message = Encoding.UTF8.GetString(data);

    await queueClient.DeleteMessageAsync(retrievedMessages[0].MessageId, retrievedMessages[0].PopReceipt);

    return message;
}