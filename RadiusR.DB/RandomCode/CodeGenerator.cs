using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.RandomCode
{
    public static class CodeGenerator
    {
        private const string supportRequestPINHeadCharacterPallete = @"ABCDEFGHJKLMNPQRSTUVWXYZ";

        public static string GenerateSupportRequestPIN()
        {
            var rnd = new Random();
            var retries = 0;
            do
            {
                var result = string.Empty;
                for (int i = 0; i < 2; i++)
                {
                    result += supportRequestPINHeadCharacterPallete[rnd.Next(supportRequestPINHeadCharacterPallete.Length)];
                }
                result += (rnd.NextDouble() * 10000000000).ToString("0000000000");
                using (RadiusREntities db = new RadiusREntities())
                {
                    if (!db.SupportRequests.Any(sr => sr.SupportPin == result))
                        return result;
                }
                retries++;
            } while (retries < 3);

            return null;
        }
    }
}
