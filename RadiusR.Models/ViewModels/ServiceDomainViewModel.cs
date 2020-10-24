using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class ServiceDomainViewModel
    {
        public int DomainID { get; set; }

        public string DomainName { get; set; }

        public bool CanBeChanged { get; set; }
    }
}
