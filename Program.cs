using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
namespace queueReciverApp
{
    class Program
    {
        static string connectionstring = "Endpoint=sb://sarikaservicebus1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=un1eEih0ZbBruy/dPe6jiJyWSv2RQzKxSgl9zoLacDA=";
        //Create queue
        static string queuename = "myqueuesarika";
        //the client owns the coonection and can be used to ceate Sender
        static ServiceBusClient client;
        //the processor that eads and process the message from the queue
        static ServiceBusProcessor processor;
        //Method 1: fo Handling Recived Message
        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Recieved: {body}");
            await args.CompleteMessageAsync(args.Message);
        }
        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        static async Task Main(string[] args)
        {
            Console.WriteLine("Demo for eciving Message From service Bus...");

            client = new ServiceBusClient(connectionstring);

            processor = client.CreateProcessor(queuename, new ServiceBusProcessorOptions());

            try
            {
                //Add Handler to pocess message
                processor.ProcessMessageAsync += MessageHandler;
                //Add handler to process any error
                processor.ProcessErrorAsync += ErrorHandler;
                //start processing
                await processor.StartProcessingAsync();
               Console.WriteLine("Wait for a minute and Then pess any key to End");
                Console.ReadKey();
                //stop pocessing
                Console.WriteLine("\n Stopping the Receiver.....");
                await processor.StartProcessingAsync();
                Console.WriteLine("Stopped recieving message....!!!");
            }
            finally
            {
                //calling disposeSsync on client types is required to ensure that objects ae propely cleanned up.
                await processor.DisposeAsync();
                await client.DisposeAsync();

            }
        }
    }
}
