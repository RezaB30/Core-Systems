using RadiusR.DB;
using RadiusR_Manager.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models
{
    public static class PaymentTokenManager
    {
        private static Random codeGenerator = new Random();
        private static MemoryCache cache = new MemoryCache("PaymentTokens");

        public static PaymentToken GetToken(int tokenCode)
        {
            return cache[tokenCode.ToString()] as PaymentToken;
        }

        public static int AddToken(PaymentToken token)
        {
            var generatedCode = 0;
            do
            {
                generatedCode = codeGenerator.Next();
            }
            while (cache.Contains(generatedCode.ToString()));

            cache.Add(generatedCode.ToString(), token, new CacheItemPolicy()
            {
                Priority = CacheItemPriority.Default,
                SlidingExpiration = TimeSpan.FromMinutes(15)
            });

            return generatedCode;
        }

        public static void RemoveToken(int id)
        {
            cache.Remove(id.ToString());
        }

        public abstract class PaymentToken
        {
            public string ReturnUrl { get; set; }

            public decimal Amount { get; set; }

            public string ClientAddress { get; set; }

            public string ClientName { get; set; }

            public string ClientTel { get; set; }

            public string Language { get; set; }

            public string SubscriberNo { get; set; }

            public string ServiceName { get; set; }
        }

        public class BillPaymentToken: PaymentToken
        {
            public IEnumerable<long> BillIds { get; set; }
        }

        public class PacketExtentionToken: PaymentToken
        {
            public long ClientId { get; set; }

            public int ExtentionPeriodCount { get; set; }
        }

        public class AdditionalFeePaymentToken: PaymentToken
        {
            public IEnumerable<Fee> AdditionalFees { get; set; }

            public long ClientID { get; set; }
        }
    }
}
