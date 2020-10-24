using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.RadiusViewModels
{
    public class EBillViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillNo")]
        public string BillCode { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ReferenceNo")]
        public string ReferenceNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IssueDate")]
        public DateTime Date { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "EBillType")]
        [EnumType(typeof(RadiusR.DB.Enums.EBillType), typeof(RadiusR.Localization.Lists.EBillType))]
        [UIHint("LocalizedList")]
        public short EBillType { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "EBillIssueDate")]
        public DateTime EBillIssueDate { get; set; }
    }
}
