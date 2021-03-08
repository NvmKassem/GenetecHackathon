using System.Threading.Tasks;
using Genetec_Project.Services;

namespace Genetec_Project
{

    class Program
    {

        static async Task Main() {
            // receive messages from the subscription
            await Problem1.ReceiveMessagesFromSubscriptionAsync();
        }


    }
}
