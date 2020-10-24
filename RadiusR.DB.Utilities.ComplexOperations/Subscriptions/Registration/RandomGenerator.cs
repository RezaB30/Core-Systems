using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.Registration
{
    public static class RandomGenerator
    {
        public static string GenerateRadiusPassword()
        {
            Random rnd = new Random();
            string result = "";
            for (int i = 0; i < 6; i++)
            {
                result += rnd.Next(0, 10).ToString();
            }
            return result;
        }
    }
}
