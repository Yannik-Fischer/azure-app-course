// See https://aka.ms/new-console-template for more information

using Azure.Messaging.ServiceBus;

const string QueueName = "az-course-queue-1";
const int MaxMessageCount = 5;

var ServiceBusConnectionString = Environment.GetEnvironmentVariable("AZURE_STORAGEBus_CONNECTION_STRING");

ServiceBusClient client;
ServiceBusSender sender;

client = new ServiceBusClient(ServiceBusConnectionString);
sender = client.CreateSender(QueueName);

try
{
    using (var batch = await sender.CreateMessageBatchAsync())
    {
        for (var i = 1; i <= MaxMessageCount; i++)
        {
            if (!batch.TryAddMessage(new ServiceBusMessage($"This is message {i}")))
            {
                Console.WriteLine($"Message {i} was not added to the batch");
            }
        }

        await sender.SendMessagesAsync(batch);
        Console.WriteLine("Messages sent");
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    await sender.DisposeAsync();
    await client.DisposeAsync();
}