using Azure;
using Azure.Storage.Queues;

namespace StorageQueueWriter;

class Program
{
    static string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=psstoor;AccountKey=Uc/DEdGN5QjdMLmInQF/Hvv7b3He7HLfnM29nfEUw+uRQCa/ps0OK3ZO7/kTNJs/iJMp8qGNv6Hv+AStxB0pIg==;EndpointSuffix=core.windows.net";
    static string QueueName = "brakkeding";
    static async Task Main(string[] args)
    {
        await WriteToQueueAsync();
        Console.WriteLine("Press Enter to Quit");
        Console.ReadLine();
    }

    private static async Task WriteToQueueAsync()
    {
        //var token = new AzureSasCredential("?sv=2022-11-02&ss=q&srt=so&sp=rwdacp&se=2024-09-27T15:45:25Z&st=2024-09-27T07:45:25Z&spr=https&sig=9Dk9zVpfpCilobg60VtcdojtcVIXRP%2BGm6t8q7qV0D0%3D");     
        //var client = new QueueClient(new Uri("https://psstatehup.queue.core.windows.net/kueueue"), token);
       var client = new QueueClient(ConnectionString, QueueName);
        for (int i = 0; i < 100; i++)
        {
            await client.SendMessageAsync($"Hello Number {i}");
        }
    }

}
