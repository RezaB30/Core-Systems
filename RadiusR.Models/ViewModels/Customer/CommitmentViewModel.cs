using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.Customer
{
    public class CommitmentViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CommitmentLength")]
        [EnumType(typeof(RadiusR.DB.Enums.CommitmentLength), typeof(RadiusR.Localization.Lists.CommitmentLength))]
        [UIHint("LocalizedList")]
        public short? CommitmentLength { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CommitmentExpirationDate")]
        public DateTime? CommitmentExpirationDate { get; set; }
    }
}
