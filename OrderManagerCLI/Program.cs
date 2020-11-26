using Microsoft.EntityFrameworkCore;
using OrderManagerCLI.Contexts;
using System;
using System.Collections.Generic;
using System.Threading;

namespace OrderManagerCLI
{
    class Program
    {
        private static readonly AutoResetEvent _closingEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            InitDB();
            OrderResponseManager om = new OrderResponseManager();
            Console.WriteLine("Waiting for Program Cycle");
            Console.CancelKeyPress += ((s, a) =>
            {
                Console.WriteLine("Bye!");
                _closingEvent.Set();
            });
            _closingEvent.WaitOne();
        }

        private static void InitDB()
        {
            using (ECommerce2Context db = new ECommerce2Context()) {
                IEnumerable<string> pendings = db.Database.GetPendingMigrations();
                if (pendings != null) { 
                    db.Database.Migrate();
                    Console.WriteLine("DB Migrated");
                }
                else
                {
                    Console.WriteLine("DB Migration Not Needed");
                }
            }
        }
    }
}
