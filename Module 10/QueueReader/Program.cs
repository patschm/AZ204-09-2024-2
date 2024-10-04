using Azure;
using Azure.Messaging.ServiceBus;

namespace QueueReader;

class Program
{
    static string EndPoint = "ps-namespace.servicebus.windows.net";
    static (string Name, string KeY) SasKeyReader = ("tessa", "HG8LJKJjOQpvOyONmYFH/CDpyfSsS2C3t+ASbPpmpv8=");
    static string QueueName = "sessions";

    static async Task Main(string[] args)
    {
        //await ReadQueueAsync();
        await ReadQueueProcessorAsync();
        Console.WriteLine("Press Enter to Quit");
        Console.ReadLine();
    }

    private static async Task ReadQueueAsync()
    {
        var cred = new AzureNamedKeyCredential(SasKeyReader.Name, SasKeyReader.KeY);
        var client = new ServiceBusClient(EndPoint, cred);
        var receiver = client.CreateReceiver(QueueName);
        do
        {
            var msg = await receiver.ReceiveMessageAsync();
            Console.WriteLine($"Lock Duration: {msg.LockedUntil} Lock Token: {msg.LockToken}");
            var data = msg.Body.ToString();
            Console.WriteLine(data);
            await receiver.CompleteMessageAsync(msg);
            //await receiver.AbandonMessageAsync(msg);
            //await receiver.RenewMessageLockAsync(msg);
            await Task.Delay(1000);
        }
        while (true);
    }
    private static async Task ReadQueueProcessorAsync()
    {
        var cred = new AzureNamedKeyCredential(SasKeyReader.Name, SasKeyReader.KeY);
        var client = new ServiceBusClient(EndPoint, cred);
        //var receiver = client.CreateProcessor(QueueName);
        var receiver = client.CreateSessionProcessor(QueueName);
        int i = 0;
        receiver.ProcessMessageAsync += async evtArg => {
            var msg = evtArg.Message;
            Console.WriteLine($"Lock Duration: {msg.LockedUntil} Lock Token: {msg.LockToken} (Session: {msg.SessionId})");
            var data = msg.Body.ToString();
            if (++i % 5 == 0)             
                throw new Exception("Ooops");
            Console.WriteLine(data);
            await evtArg.CompleteMessageAsync(msg);
        };

        receiver.ProcessErrorAsync += evtArg => {
            Console.WriteLine("Ook Ooops");
            Console.WriteLine(evtArg.Exception.Message);
            return Task.CompletedTask;
        };

        await receiver.StartProcessingAsync();
        Console.WriteLine("Press Enter to quit processing");
        Console.ReadLine();
        await receiver.StopProcessingAsync();

    }
}
