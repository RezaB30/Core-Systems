using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class AgentCollectionViewModel
    {
        public long ID { get; set; }

        public string AgentName { get; set; }

        public DateTime CreationDate { get; set; }

        public string CreatorName { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string PayerName { get; set; }

        public string AllowanceAmount { get; private set; }

        public decimal _allowanceAmount
        {
            set
            {
                AllowanceAmount = value.ToString("###,##0.00");
            }
        }
    }
}
