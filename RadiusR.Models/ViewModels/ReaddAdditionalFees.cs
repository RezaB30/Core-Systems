using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Models.ViewModels
{
    public class ReaddAdditionalFees
    {
        public string id { get; set; }

        public short state { get; set; }

        public string redirectUrl { get; set; }

        public IEnumerable<ClientFeeViewModel> AddingFees { get; set; }
    }
}