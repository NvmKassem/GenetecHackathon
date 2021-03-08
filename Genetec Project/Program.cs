using System.Threading.Tasks;
using Genetec_Project.Services;

namespace Genetec_Project
{

    class Program
    {

        static async Task Main() {
            // receive messages from the subscription
            //await Problem1.ReceiveMessagesFromSubscriptionAsync();
            var task1 = Problem2.ReceiveMessagesFromSubscriptionAsync();
            var task2 = Problem1.ReceiveMessagesFromSubscriptionAsync();

            await Task.WhenAll(task1, task2);
        }


    }
}
