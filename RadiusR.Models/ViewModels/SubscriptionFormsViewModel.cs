using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class SubscriptionFormsViewModel
    {
        public long SubscriptionID { get; set; }

        public bool HasEmailAddress { get; set; }

        public bool HasDSLInfo { get; set; }
    }
}
