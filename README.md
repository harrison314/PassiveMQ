# Passive MQ

**Passive message qeueu** is _push-pull_ message qeueu like [Azure Queue storage](https://docs.microsoft.com/en-us/azure/storage/queues/storage-dotnet-how-to-use-queues).
It is a simple implementation based on _ASP.Net Core 3.1_ and _MS SQL Server_.
For optimized performance and safety (race conditions)
uses hints available in T-SQL to make each operation atomic.

_PassiveMQ_ is designed to integrate intranet systems in enviroments where non-HTTP protocols is not allowed on the network.

API Example:

Send message to queueu:
```cs
PassiveMqAccount account = PassiveMqAccount.Parse("Endpoint=http://localhost:5586");
PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");

await client.CrateIfNotExists();

await client.AddMessage(new MqCreateMessage("any message", "any label")); // label is menaning as type or kind of message content
await client.AddMessage(new MqCreateMessage(new byte[]{ 0, 1 , 2 , 3 }));
```

Processing messages in loop:
```cs
PassiveMqAccount account = PassiveMqAccount.Parse("Endpoint=http://localhost:5586");
PassiveMqQueuClient client = this.Account.CreateQueueClient("myTestQueue");

for (; ;)
{
     MqMessage message = await client.GetMessage(TimeSpan.FromSeconds(5)); // set inactiove time for message
     if (message == null)
     {
         await Task.Delay(1000);
     }
     else
     {
         ProcessMessage(message); // processing with duration less than 5 seconds
         await client.DeleteMessage(message); // delete message from qeueu
     }
}
```