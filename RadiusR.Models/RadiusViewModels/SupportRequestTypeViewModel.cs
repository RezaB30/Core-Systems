using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class SupportRequestTypeViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public bool? IsStaffOnly { get; set; }

        public bool? IsActive { get; set; }

        public IEnumerable<SupportRequestSubTypeViewModel> SubTypes { get; set; }
    }
}
