
using System;

using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue; 

namespace QueueApp
{
    class Program
    {
        private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=articulos;AccountKey=JJkyJA4+egOkuUHzKiDyGIH18SSVIgVXdW5fVOMYnY+p++VLLHBARc7a108em/aBQHlagbtmJ+MGgLeucuQdHg==;EndpointSuffix=core.windows.net";

        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                string value = String.Join(" ", args);
                await SendArticleAsync(value);
                Console.WriteLine($"Sent: {value}");
            }
            else
            {
                string value = await ReceiveArticleAsync();
                Console.WriteLine($"Received {value}");
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

        static CloudQueue GetQueue()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            return queueClient.GetQueueReference("newsqueue");
        }

        static async Task<string> ReceiveArticleAsync()
        {
            CloudQueue queue = GetQueue();
            bool exists = await queue.ExistsAsync();
            if (exists)
            {
                CloudQueueMessage retrievedArticle = await queue.GetMessageAsync();
                if (retrievedArticle != null)
                {
                    string newsMessage = retrievedArticle.AsString;
                    await queue.DeleteMessageAsync(retrievedArticle);
                    return newsMessage;
                }
            }

            return "<queue empty or not created>";
        }
    }
}
