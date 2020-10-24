using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR.DB.Utilities.Billing;
using RadiusR.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Scheduler_Test_Suit
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Subscriber Nos (comma seperated):");
            var line = Console.ReadLine();
            // test batch query
            if (string.IsNullOrEmpty(line))
            {
                var minClientID = (long)0;
                while (true)
                {
                    using (RadiusREntities db = new RadiusREntities())
                    {
                        db.Database.CommandTimeout = 120;
                        db.Database.Log = Console.WriteLine;
                        var baseQuery = db.Subscriptions
                                    .OrderBy(subscription => subscription.ID)
                                    .GetValidClientsForBilling()
                                    .Where(client => client.ID >= minClientID);
                        var maxID = baseQuery.Take(1000).Select(s => s.ID).DefaultIfEmpty(0).Max();
                        var currentBatch = db.PrepareForBilling(baseQuery.Where(client => client.ID < maxID)).ToArray();

                        if (!currentBatch.Any())
                            break;


                        minClientID = currentBatch.Max(client => client.Subscription.ID) + 1;
                    }
                }

                Console.WriteLine("Done.");
                Console.ReadKey();
                return;
            }
            var subNos = line.Split(',').Select(no => no.Trim()).Where(no => !string.IsNullOrWhiteSpace(no)).ToArray();
            Console.WriteLine("Sub Nos:");
            Console.WriteLine(string.Join(Environment.NewLine, subNos));
            DateTime? toDate = null;
            while (true)
            {
                Console.WriteLine("Issue Bills Untill: (leave empty for today)");
                var inputDate = Console.ReadLine();
                if (string.IsNullOrEmpty(inputDate))
                {
                    //toDate = DateTime.Today;
                    break;
                }
                DateTime current;
                if (DateTime.TryParseExact(inputDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out current))
                {
                    toDate = current;
                    break;
                }
            }

            using (RadiusREntities db = new RadiusREntities())
            {
                db.Database.Log = Console.WriteLine;
                var subs = db.PrepareForBilling(db.Subscriptions.Where(sub => subNos.Contains(sub.SubscriberNo))
                    .OrderBy(subscription => subscription.ID).AsQueryable()
                    .GetValidClientsForBilling());


                foreach (var sub in subs)
                {
                    sub.IssueBill(toDate);
                }

                db.SaveChanges();
            }

            Console.WriteLine("Done.");
            Console.ReadKey();
        }


    }

    public static class Extentions
    {
        public static IQueryable<Subscription> GetValidClientsForBilling(this IQueryable<Subscription> clients)
        {
            return clients.Where(client => (client.State == (short)CustomerState.Active || client.State == (short)CustomerState.Reserved) && client.ActivationDate.HasValue && client.Service.BillingType != (short)ServiceBillingType.PrePaid);
        }
    }
}
