using System;
using System.Threading.Tasks;
using Genetec_Project.Services;

namespace Genetec_Project
{

    class Program
    {

        static async Task Main() {
            // receive messages from the subscription
            var task1 = Problem2.ReceiveMessagesFromSubscriptionAsync();
            var task2 = Problem1.ReceiveMessagesFromSubscriptionAsync();

            Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
            await Task.WhenAll(task1, task2);

        }


    }
}
