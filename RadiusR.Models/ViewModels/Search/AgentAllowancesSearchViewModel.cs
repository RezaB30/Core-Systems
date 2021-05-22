using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.Search
{
    public class AgentAllowancesSearchViewModel
    {
        public DateTime? CreationDateStart { get; set; }

        public DateTime? CreationDateEnd { get; set; }

        public DateTime? PaymentDateStart { get; set; }

        public DateTime? PaymentDateEnd { get; set; }

        public int? AgentID { get; set; }
    }
}
