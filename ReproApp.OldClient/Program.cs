namespace ReproApp.OldClient
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    class Program
    {
        static async Task Main(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus_ConnectionString");

            var management = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!await management.QueueExistsAsync("queue"))
            {
                await management.CreateQueueAsync("queue");
            }

            var sender = QueueClient.CreateFromConnectionString(connectionString, "queue");

            while (true)
            {
                using (var tx = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
                    Debugger.Break();
                    await sender.SendAsync(new BrokeredMessage(Encoding.Default.GetBytes(DateTime.Now.ToString("s"))));

                    Console.WriteLine("message sent");

                    tx.Complete();

                    Console.WriteLine("tx completed");
                }
            }

            await sender.CloseAsync();
        }
    }
}
