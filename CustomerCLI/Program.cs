using System;
using System.Threading;

namespace CustomerCLI
{
    class Program
    {
        private static readonly AutoResetEvent _closingEvent = new AutoResetEvent(false);
        static void Main(string[] args)
        {
            string[] productIds = new string[2];
            productIds[0] = "f5ed9460-f063-4b0d-92fb-d0e605c65457";
            productIds[1] = "b4b5a9c9-70c5-4e91-b0c6-e38c94475659";
            string customerGuid = Guid.NewGuid().ToString();

            OrderRequestManager or = new OrderRequestManager(customerGuid, productIds);
            Timer t = new Timer(or.SendRequest, null, 0, 5000);

            Console.WriteLine("Waiting for Program Cycle");
            Console.CancelKeyPress += ((s, a) =>
            {
                Console.WriteLine("Bye!");
                _closingEvent.Set();
            });
            _closingEvent.WaitOne();
        }
        
    }
}
