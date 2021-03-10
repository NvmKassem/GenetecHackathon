using Azure.Messaging.ServiceBus;
using Genetec_Project.Models;
using Newtonsoft.Json;
using System;
using System.IO;
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
            var receivingPayload = JsonConvert.DeserializeObject<ReceptionPayload>(body);
            string rec_load = JsonConvert.SerializeObject(receivingPayload);

           
            if (checkMatch(receivingPayload.LicensePlate)) {
                var sendingPayload = JsonConvert.DeserializeObject<SendingPayload>(rec_load);
                File.WriteAllBytes(@"current.json", Convert.FromBase64String(receivingPayload.ContextImageJpg));

                await using (FileStream my_stream = new FileStream("current.json", FileMode.Open, FileAccess.Read)) {
                    sendingPayload.ContextImageReference = await Problem3.UploadImage(@"current.json", my_stream);
                }

                string sen_load = JsonConvert.SerializeObject(sendingPayload);
                Console.WriteLine(sen_load);
                var response = await client.PostAsync(uri, new StringContent(sen_load, Encoding.UTF8, "application/json"));
                Console.WriteLine("Send data to server, response : " + response);
                Console.WriteLine("Found match !");
            } else {
                Console.WriteLine("No match"); 
            }
            Console.WriteLine();        
            // complete the message. messages is deleted from the queue. 
            await args.CompleteMessageAsync(args.Message);
        }

        static bool checkMatch(string plate) {
            Console.WriteLine("Current time: {0:HH:mm:ss.fff}", DateTime.Now);
            Console.WriteLine(plate);
            Console.WriteLine("[{0}]", string.Join(", ", Table.table));
            for (int i = 0; i < Table.table.Length; i++) {
                if (Problem4.FuzzyEquals(plate,Table.table[i])) {
                    return true;
                }
            }
            return false;
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
