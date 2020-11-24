using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class SupportGroupRequestListViewModel
    {
        public int GroupID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SupportGroup")]
        public string GroupName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "GroupInbox")]
        public int GroupInbox { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "GroupRedirectedInbox")]
        public int GroupRedirectedInbox { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "GroupInProgress")]
        public int GroupInProgress { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PersonalInbox")]
        public int PersonalInbox { get; set; }

        public bool IsLeader { get; set; }

        //public int Outbox { get; set; }
    }
}
