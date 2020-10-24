using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB
{
    public partial class Customer
    {
        public string ValidDisplayName
        {
            get
            {
                if (CorporateCustomerInfo != null)
                {
                    return CorporateCustomerInfo.Title;
                }
                else
                {
                    return FirstName + " " + LastName;
                }
            }
        }
    }
}
