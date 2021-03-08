using Azure.Messaging.ServiceBus;
using Genetec_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Genetec_Project.Services
{
    class Problem1
    {
        static string connectionString = "Endpoint=sb://licenseplatepublisher.servicebus.windows.net/;SharedAccessKeyName=ConsumeReads;SharedAccessKey=VNcJZVQAVMazTAfrssP6Irzlg/pKwbwfnOqMXqROtCQ=";
        static string topicName = "licenseplateread";
        static string subscriptionName = "tooiogqzgjradlyx";
        static string uri = "https://licenseplatevalidator.azurewebsites.net/api/lpr/platelocation";

        static HttpClient client = new HttpClient();


        static async Task MessageHandler(ProcessMessageEventArgs args) {
            string body = args.Message.Body.ToString();
            var payload = JsonConvert.DeserializeObject<Payload>(body);
            string load = JsonConvert.SerializeObject(payload);

            var response = await client.PostAsync(uri, new StringContent(load, Encoding.UTF8, "application/json"));
            Console.WriteLine("Send data to server, response : " + response);
            // complete the message. messages is deleted from the queue. 
            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args) {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        public static async Task ReceiveMessagesFromSubscriptionAsync() {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "dGVhbTAyOi1BTU1wc25oW251T3IxcFM=");

            await using (ServiceBusClient client = new ServiceBusClient(connectionString)) {
                // create a processor that we can use to process the messages
                ServiceBusProcessor processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

                // add handler to process messages
                processor.ProcessMessageAsync += MessageHandler;

                // add handler to process any errors
                processor.ProcessErrorAsync += ErrorHandler;

                // start processing 
                await processor.StartProcessingAsync();

                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();

                // stop processing 
                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
        }
    }
}
