using RezaB.TurkTelekom.WebServices.TTApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.DomainsCache
{
    public static class UsernameFactory
    {
        private static readonly int MaxRetries = 10;
        private static readonly int UsernameNumericLength = 10;
        private static readonly int SubscriberNoNumericLength = 7;
        private static readonly int ReferenceNoNumericLength = 6;

        public static bool IsUsernameValid(CachedDomain domain, string username)
        {
            // check db
            using (RadiusREntities db = new RadiusREntities())
            {
                if (db.Subscriptions.Any(client => client.Username.StartsWith(username + "@")))
                {
                    return false;
                }
            }
            // check telekom
            if (domain.TelekomCredential != null)
            {
                var serviceClient = new UsernameValidationClient(domain.TelekomCredential.XDSLWebServiceUsernameInt, domain.TelekomCredential.XDSLWebServicePassword);
                var response = serviceClient.ValidateUsername(username);
                if (!response.Data)
                    return false;
            }

            return true;
        }

        public static string GenerateUsername(CachedDomain domain)
        {
            var rnd = new Random();
            var characterPalette = "0123456789";
            var generatedUsername = string.Empty;

            var currentIteration = 0;
            while (currentIteration < MaxRetries)
            {
                for (int i = 0; i < UsernameNumericLength; i++)
                {
                    generatedUsername += characterPalette[rnd.Next(characterPalette.Length)];
                }
                generatedUsername = domain.UsernamePrefix + generatedUsername;
                if(IsUsernameValid(domain, generatedUsername))
                {
                    return generatedUsername + "@" + domain.Name;
                }

                generatedUsername = string.Empty;
                currentIteration++;
            }

            return null;
        }

        public static string GenerateUniqueSubscriberNo(CachedDomain domain)
        {
            var rnd = new Random();
            var characterPalette = "0123456789";
            var generatedSubscriberNo = string.Empty;

            var currentIteration = 0;
            while (currentIteration < MaxRetries)
            {
                for (int i = 0; i < SubscriberNoNumericLength; i++)
                {
                    generatedSubscriberNo += characterPalette[rnd.Next(characterPalette.Length)];
                }
                generatedSubscriberNo = domain.SubscriberNoPrefix + generatedSubscriberNo;

                using (RadiusREntities db = new RadiusREntities())
                {
                    if(!db.Subscriptions.Any(client => client.SubscriberNo == generatedSubscriberNo))
                    {
                        return generatedSubscriberNo;
                    }
                }

                generatedSubscriberNo = string.Empty;
                currentIteration++;
            }

            return null;
        }

        public static string GenerateUniqueReferenceNo()
        {
            var rnd = new Random();
            var characterPalette = @"0123456789ABCDEFGHIJKLMNPQRSTUVWXYZ";
            var generatedReferenceNo = string.Empty;

            var currentIteration = 0;
            while (currentIteration < MaxRetries)
            {
                for (int i = 0; i < ReferenceNoNumericLength; i++)
                {
                    generatedReferenceNo += characterPalette[rnd.Next(characterPalette.Length)];
                }

                using (RadiusREntities db = new RadiusREntities())
                {
                    if (!db.Subscriptions.Any(client => client.ReferenceNo == generatedReferenceNo))
                    {
                        return generatedReferenceNo;
                    }
                }

                generatedReferenceNo = string.Empty;
                currentIteration++;
            }

            return null;
        }
    }
}
