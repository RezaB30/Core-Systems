using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.Customer
{
    public class CustomerRegistrationViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CustomerIdentity")]
        public IDCardViewModel IDCard { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CustomerGeneralInfo")]
        public CustomerGeneralInfoViewModel GeneralInfo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "IndividualCustomerInfo")]
        public IndividualCustomerInfoViewModel IndividualInfo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CorporateCustomerInfo")]
        public CorporateCustomerInfoViewModel CorporateInfo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriptionInfo")]
        public CustomerSubscriptionViewModel SubscriptionInfo { get; set; }

        public CustomerRegistrationViewModel()
        {
            IDCard = new IDCardViewModel();
            GeneralInfo = new CustomerGeneralInfoViewModel()
            {
                Culture = "tr-tr"
            };
            IndividualInfo = new IndividualCustomerInfoViewModel()
            {
                Profession = (int)RadiusR.DB.Enums.Profession.Code_962,
                Nationality = (int)RadiusR.DB.Enums.CountryCodes.TUR
            };
            CorporateInfo = new CorporateCustomerInfoViewModel()
            {
                ExecutiveProfession = (int)RadiusR.DB.Enums.Profession.Code_962,
                ExecutiveNationality = (int)RadiusR.DB.Enums.CountryCodes.TUR
            };
            SubscriptionInfo = new CustomerSubscriptionViewModel();
        }
    }
}
