
using System;

using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue; 

namespace QueueApp
{
    class Program
    {
        private const string ConnectionString = "DefaultEndpointsProtocol=https;EndpointSuffix=core.windows.net;AccountName=<name>;AccountKey=vyw6aKz2PtSAgQ4ljJQgJFgxbCETdXt39ZyYQ5fLqoBJj/gT+43TbrhoVco7Rqj/AAJVlvFORRfnYqGHiX9QcQ==";

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                string value = String.Join(" ", args);
                SendArticleAsync(value).Wait();
                Console.WriteLine($"Sent: {value}");
            }
        }

        static async Task SendArticleAsync(string newsMessage)
        {
            // 1 - Cuenta de azure storage
            CloudStorageAccount account = CloudStorageAccount.Parse(ConnectionString);
            // 2 - obtener cliente de azure queue storage
            CloudQueueClient client = account.CreateCloudQueueClient();
            // 3 - obtener referencia de la cola
            CloudQueue queue = client.GetQueueReference("newsqueue");
            // 4 - comprobar si la cola está lista o hay que crearla
            bool createdQueue = await queue.CreateIfNotExistsAsync();
            if (createdQueue)
            {
                Console.WriteLine("The queue of news articles was created.");
            }
            // 5 - crear un mensaje y añadirlo a la cola
            CloudQueueMessage articleMessage= new CloudQueueMessage(newsMessage);
            await queue.AddMessageAsync(articleMessage);
        }
    }
}
