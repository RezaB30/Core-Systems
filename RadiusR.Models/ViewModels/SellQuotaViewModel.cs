using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class SellQuotaViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name ="Package")]
        public int PacketID { get; set; }
    }
}
