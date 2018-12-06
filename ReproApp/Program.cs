using System;

namespace ReproApp
{
    using System.Diagnostics;
    using System.Text;
    using System.Threading.Tasks;
    using System.Transactions;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Microsoft.Azure.ServiceBus.Management;

    class Program
    {
        static async Task Main(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus_ConnectionString");

            var management = new ManagementClient(connectionString);

            if (!await management.QueueExistsAsync("queue"))
            {
                await management.CreateQueueAsync("queue");
            }

            var sender = new MessageSender(connectionString, "queue");

            while (true)
            {
                using (var tx = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
                {
                    Debugger.Break();
                    await sender.SendAsync(new Message(Encoding.Default.GetBytes(DateTime.Now.ToString("s"))));

                    Console.WriteLine("message sent");

                    tx.Complete();

                    Console.WriteLine("tx completed");
                }
            }

            await management.CloseAsync();
            await sender.CloseAsync();
        }
    }
}