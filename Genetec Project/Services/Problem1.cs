using Azure.Messaging.ServiceBus;
using Genetec_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string matched = "";

        static Queue<WaitingPayload> queue = new Queue<WaitingPayload>();


        static async Task MessageHandler(ProcessMessageEventArgs args) {
            string body = args.Message.Body.ToString();
            var receivingPayload = JsonConvert.DeserializeObject<ReceptionPayload>(body);
            string rec_load = JsonConvert.SerializeObject(receivingPayload);

            var sendingPayload = JsonConvert.DeserializeObject<SendingPayload>(rec_load);
            File.WriteAllBytes(@"current.json", Convert.FromBase64String(receivingPayload.ContextImageJpg));

            await CheckPreviousMatch();

            await using (FileStream my_stream = new FileStream("current.json", FileMode.Open, FileAccess.Read)) {
                sendingPayload.ContextImageReference = await Problem3.UploadImage(@"current.json", my_stream);
            }

            Console.WriteLine("Current time: {0:HH:mm:ss.fff}", DateTime.Now);
            Console.WriteLine(receivingPayload.LicensePlate);
            Console.WriteLine("[{0}]", string.Join(", ", Table.table));


            if (checkMatch(receivingPayload.LicensePlate)) {
                sendingPayload.LicensePlate = matched;

                string sen_load = JsonConvert.SerializeObject(sendingPayload);
                Console.WriteLine(sen_load);
                var response = await client.PostAsync(uri, new StringContent(sen_load, Encoding.UTF8, "application/json"));
                Console.WriteLine("Sent data to server, response : " + response);
                Console.WriteLine("Found match !");
            } else {
                var result = await Problem5.MakeRequest(sendingPayload.ContextImageReference);
                string sendingPayloadString = JsonConvert.SerializeObject(sendingPayload);
                var waitingPayload = JsonConvert.DeserializeObject<WaitingPayload>(sendingPayloadString);
                waitingPayload.Id = result;
                queue.Enqueue(waitingPayload);
                Console.WriteLine("No match, sending to Azure recognition"); 
            }
            Console.WriteLine();        
            Console.WriteLine();        
            Console.WriteLine();        
            Console.WriteLine();        
            // complete the message. messages is deleted from the queue. 
            await args.CompleteMessageAsync(args.Message);
        }



       
        static async Task CheckPreviousMatch() {
            if (queue.Count == 0)
                return;

            WaitingPayload current = queue.Peek();
            string result = await Problem5.MakeGetRequest(current.Id);
            Console.WriteLine("Result:" + result);
            var receivingPayload = JsonConvert.DeserializeObject<CognitivePayload>(result);
            if(receivingPayload.Status == "Succeeded") {
                Console.WriteLine();
                Console.WriteLine("Checking plate from previous call");
                Console.WriteLine("Old plate was: " + current.LicensePlate);
                for (int i = 0; i < receivingPayload.RecognitionResult.Lines.Length; i++) {
                    receivingPayload.RecognitionResult.Lines[i].Text = Regex.Replace(receivingPayload.RecognitionResult.Lines[i].Text, @"[^A-Z0-9]+", String.Empty);
                    if (current.LicensePlate.Equals(receivingPayload.RecognitionResult.Lines[i].Text)) {
                        Console.WriteLine("Same plate reading as before");
                        queue.Dequeue();
                        return;
                    }
                    
                    int length = receivingPayload.RecognitionResult.Lines[i].Text.Length;
                    
                    if (length ==6 || length==7) {
                        Console.WriteLine(receivingPayload.RecognitionResult.Lines[i].Text);
                        if (checkMatch(receivingPayload.RecognitionResult.Lines[i].Text)) {
                            string rec_load = JsonConvert.SerializeObject(current);

                            var sendingPayload = JsonConvert.DeserializeObject<SendingPayload>(rec_load);
                            sendingPayload.LicensePlate = matched;
                            string sen_load = JsonConvert.SerializeObject(sendingPayload);
                            Console.WriteLine(sen_load);
                            var response = await client.PostAsync(uri, new StringContent(sen_load, Encoding.UTF8, "application/json"));
                            Console.WriteLine("Sent data to server, response : " + response);
                            Console.WriteLine("Found match !");
                        } else {
                            Console.WriteLine("Still No match");
                        }
                    } else {
                        int j = i + 1;
                        while (length < 7) {
                            if (j >= receivingPayload.RecognitionResult.Lines.Length) {
                                break;
                            }

                            string combined = "";
                            for (int x = i; x <= j; x++) {
                                combined += Regex.Replace(receivingPayload.RecognitionResult.Lines[x].Text, @"[^A-Z0-9]+", String.Empty);
                            }
                            length = combined.Length;
                            if (length == 6 || length == 7) {
                                Console.WriteLine(combined);
                            } else {
                                Console.WriteLine("Not checking: " + combined);
                                j++;
                                continue;
                            }
                            if ((length == 6 || length == 7) && checkMatch(combined)) {
                                string rec_load = JsonConvert.SerializeObject(current);

                                var sendingPayload = JsonConvert.DeserializeObject<SendingPayload>(rec_load);
                                sendingPayload.LicensePlate = matched;
                                string sen_load = JsonConvert.SerializeObject(sendingPayload);
                                Console.WriteLine(sen_load);
                                var response = await client.PostAsync(uri, new StringContent(sen_load, Encoding.UTF8, "application/json"));
                                Console.WriteLine("Sent data to server, response : " + response);
                                Console.WriteLine("Found match !");
                            } else {
                                Console.WriteLine("Still No match");
                            }
                            j++;
                        }
                    }
                    
                    
                }
                queue.Dequeue();
            } else {
                Console.WriteLine("Still no result from Azure, Status : " +receivingPayload.Status);
            }
            Console.WriteLine();
        }


        static bool checkMatch(string plate) {
            for (int i = 0; i < Table.table.Length; i++) {
                if (Problem4.FuzzyEquals(plate,Table.table[i])) {
                    matched = Table.table[i];
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
