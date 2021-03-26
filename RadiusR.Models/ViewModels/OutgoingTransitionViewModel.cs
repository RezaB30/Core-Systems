using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels
{
    public class OutgoingTransitionViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DomainName")]
        public int DomainID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DomainName")]
        public string DomainName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CreationDate")]
        [UIHint("ExactTime")]
        public DateTime? CreationDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ElapsedTime")]
        [UIHint("Hours")]
        public TimeSpan? ElapsedTime
        {
            get
            {
                return CreationDate.HasValue ? (DateTime.Now - CreationDate.Value) : (TimeSpan?)null;
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TransitionTransactionID")]
        public long TransactionID { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TelekomSubscriberNo")]
        public string XDSLNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CounterpartOperator")]
        public string CounterpartOperator { get; set; }

        public IndividualCustomerInfo IndividualInfo { get; set; }

        public CorporateCustomerInfo CorporateInfo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ClientType")]
        public string CustomerType
        {
            get
            {
                if (IndividualInfo != null)
                {
                    return RadiusR.Localization.Pages.Common.IndividualCustomer;
                }
                else
                {
                    return RadiusR.Localization.Pages.Common.CorporateCustomer;
                }
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CustomerName")]
        public string CustomerName
        {
            get
            {
                if (IndividualInfo != null)
                {
                    return $"{IndividualInfo.FirstName} {IndividualInfo.LastName}";
                }
                else
                {
                    return CorporateInfo.CompanyTitle;
                }
            }
        }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TCKNo")]
        public string TCKNo
        {
            get
            {
                if (IndividualInfo != null)
                {
                    return IndividualInfo.TCKNo;
                }
                else
                {
                    return CorporateInfo.ExecutiveTCKNo;
                }
            }
        }

        public bool XDSLNoIsValid { get; set; }

        public class IndividualCustomerInfo
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FirstName")]
            public string FirstName { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LastName")]
            public string LastName { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TCKNo")]
            public string TCKNo { get; set; }

            public string DBFirstName { get; set; }

            public string DBLastName { get; set; }

            public string DBTCKNo { get; set; }

            public bool FirstNameIsValid
            {
                get
                {
                    return FirstName.ToLower().Trim() == DBFirstName.ToLower().Trim();
                }
            }

            public bool LastNameIsValid
            {
                get
                {
                    return LastName.ToLower().Trim() == DBLastName.ToLower().Trim();
                }
            }

            public bool TCKNoIsValid
            {
                get
                {
                    return TCKNo.Trim() == DBTCKNo;
                }
            }
        }

        public class CorporateCustomerInfo
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CompanyTitle")]
            public string CompanyTitle { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TCKNo")]
            public string ExecutiveTCKNo { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaxNo")]
            public string TaxNo { get; set; }

            public string DBCompanyTitle { get; set; }

            public string DBExecutiveTCKNo { get; set; }

            public string DBTaxNo { get; set; }

            public bool CompanyTitleIsValid
            {
                get
                {
                    return CompanyTitle.ToLower().Trim() == DBCompanyTitle.ToLower().Trim();
                }
            }

            public bool ExecutiveTCKNoIsValid
            {
                get
                {
                    return ExecutiveTCKNo.Trim() == DBExecutiveTCKNo;
                }
            }

            public bool TaxNoIsValid
            {
                get
                {
                    return TaxNo.Trim() == DBTaxNo;
                }
            }
        }
    }
}
