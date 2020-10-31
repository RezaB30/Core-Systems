using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class SupportRequestSubTypeViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int? SupportRequestTypeID { get; set; }

        public string SupportRequestTypeName { get; set; }

        public bool? IsActive { get; set; }
    }
}
