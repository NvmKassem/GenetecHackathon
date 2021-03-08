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
    class Problem2
    {
        static string connectionString = "Endpoint=sb://licenseplatepublisher.servicebus.windows.net/;SharedAccessKeyName=listeneronly;SharedAccessKey=w+ifeMSBq1AQkedLCpMa8ut5c6bJzJxqHuX9Jx2XGOk=";
        static string topicName = "wantedplatelistupdate";
        static string subscriptionName = "tooiogqzgjradlyx";

        static string baseUrl = "https://licenseplatevalidator.azurewebsites.net/api/lpr/wantedplates";

        static HttpClient client = new HttpClient();


        static async Task MessageHandler(ProcessMessageEventArgs args) {
            string body = args.Message.Body.ToString();
            var payload = JsonConvert.DeserializeObject<Notification>(body);

            Console.WriteLine("Received notification");

            //Update table
            await getTable(payload.Url);

            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args) {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        static async Task getTable(string url) {
            //Update table
            var response = await client.GetAsync(url);
            var response_data = await response.Content.ReadAsStringAsync();
            Console.WriteLine(response_data);
            string[] table = JsonConvert.DeserializeObject<string[]>(response_data);
            Table.updateFile(table);
            Console.WriteLine(table);
            
        }

        public static async Task ReceiveMessagesFromSubscriptionAsync() {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "dGVhbTAyOi1BTU1wc25oW251T3IxcFM=");
            //await getTable(baseUrl);
            Table.readFile();
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
