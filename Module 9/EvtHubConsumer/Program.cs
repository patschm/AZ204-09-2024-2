using System;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Consumer;
using System.Text;
using Azure.Storage.Blobs;
using System.Collections.Concurrent;

namespace EvtHubConsumer;

class Program
{
    private const string conStr = "Endpoint=sb://tessas.servicebus.windows.net/;SharedAccessKeyName=lezert;SharedAccessKey=MCqglbdC9DelvRbr7eGkiKehhnYMhz0MA+AEhA38jMQ=;EntityPath=sjoerd";
    private const string hubName = "sjoerd";

    private const string checkpointStorage = "DefaultEndpointsProtocol=https;AccountName=psstoor;AccountKey=9GtNt8vHxuJpT5wziWYQc0ECrkTU2GsakfoVqbQnh134HCDtFX2xrMKLCOoT6dabWy4x7rOn6qP3+ASt+mGqJg==;EndpointSuffix=core.windows.net";

    private static string? group = "ikke";

    static async Task Main(string[] args)
    {
        if (args.Length > 0) { 
            group = args[0].Trim();
        }
        //await LeanAndMean();
        await UsingProcessors();
        Console.ReadLine();
        Console.WriteLine("Started...");
    }

    private static async Task UsingProcessors()
    {
        // For checkpoints
        // Checkpoints keep track of what you read (stored in blob)
        // Otherwise you'll see the same events over and over again
        var partitionEventCount = new ConcurrentDictionary<string, int>();
        BlobContainerClient blobContainerClient = new BlobContainerClient(checkpointStorage, "eventhub");
        blobContainerClient.CreateIfNotExists();

        var processor = new EventProcessorClient(blobContainerClient, group, conStr, hubName);

        processor.ProcessEventAsync += async partitionEvent =>
        {
            var partID = partitionEvent.Partition.PartitionId;
            Console.WriteLine($"Event Read ({partID}): {Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray())}");

            // Set new checkpoint
            int eventsSince = partitionEventCount.AddOrUpdate(partID, 1, (str, cnt) => cnt + 1);

            if (eventsSince >= 25)
            {
                await partitionEvent.UpdateCheckpointAsync();
                partitionEventCount[partID] = 0;
            }
        };
        processor.ProcessErrorAsync += errorEvent =>
        {
            Console.WriteLine($"Ooops (Partition: {errorEvent.PartitionId}): {errorEvent.Exception.Message}");
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync();
        Console.WriteLine("Processor is running. Press Enter to stop");
        Console.ReadLine();
        await processor.StopProcessingAsync();

    }

    private static async Task LeanAndMean()
    {
        await using (var consumerClient = new EventHubConsumerClient(
            EventHubConsumerClient.DefaultConsumerGroupName,
            //"ikke",
            conStr,
            hubName))
        {
            int eventsRead = 0;

            //ReadEventOptions opts = new ReadEventOptions();
            await foreach (PartitionEvent partitionEvent in consumerClient.ReadEventsAsync())
            {
                Console.WriteLine($"Event Read ({partitionEvent.Partition.PartitionId}): {Encoding.UTF8.GetString(partitionEvent.Data.Body.ToArray())}");
                eventsRead++;
            }
            Console.WriteLine($"Events read: {eventsRead}");
        }

    }

}
