// See https://aka.ms/new-console-template for more information

using Azure.Messaging.ServiceBus;

const string QueueName = "az-course-queue-1";

var ServiceBusConnectionString = Environment.GetEnvironmentVariable("AZURE_STORAGEBus_CONNECTION_STRING");

ServiceBusClient client;
ServiceBusProcessor processor = default!;

async Task MessageHandler(ProcessMessageEventArgs processMessageEventArgs)
{
    var body = processMessageEventArgs.Message.Body.ToString();
    Console.WriteLine(body);
    await processMessageEventArgs.CompleteMessageAsync(processMessageEventArgs.Message);
}

Task ErrorHandler(ProcessErrorEventArgs processErrorEventArgs)
{
    Console.WriteLine(processErrorEventArgs.Exception.ToString());
    return Task.CompletedTask;
}

client = new ServiceBusClient(ServiceBusConnectionString);
processor = client.CreateProcessor(QueueName, new ServiceBusProcessorOptions());

try
{
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;

    await processor.StartProcessingAsync();

    Console.WriteLine("Press any key to stop the processing");
    Console.ReadKey();

    Console.WriteLine("\nStopping the receiver...");
    await processor.StopProcessingAsync();
    Console.WriteLine("Stopped receiving messages");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    await processor.DisposeAsync();
    await client.DisposeAsync();
}