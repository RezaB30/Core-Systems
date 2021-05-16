using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.RandomCode
{
    public static class RandomUsernameGenerator
    {
        public static string GenerateUniqueSetupServiceUsername(int length)
        {
            var palette = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rand = new Random();

            string results = string.Empty;
            using (RadiusREntities db = new RadiusREntities())
            {
                do
                {
                    var generatedText = new StringBuilder();

                    for (int i = 0; i < length; i++)
                    {
                        generatedText.Append(palette[rand.Next(palette.Length)]);
                    }
                    results = generatedText.ToString();
                } while (db.CustomerSetupUsers.Any(csu => csu.Username == results));
            }

            return results;
        }
    }
}
