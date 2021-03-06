using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.PDFTemplates
{
    public class PDFEditContextMenuViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "SubscriberNo")]
        public int SubscriberNo { get { return (int)PDFItemIDs.SubscriberNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "FirstName")]
        public int FirstName { get { return (int)PDFItemIDs.FirstName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "LastName")]
        public int LastName { get { return (int)PDFItemIDs.LastName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "MothersName")]
        public int MothersName { get { return (int)PDFItemIDs.MothersName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "FathersName")]
        public int FathersName { get { return (int)PDFItemIDs.FathersName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "BirthDate")]
        public int BirthDate { get { return (int)PDFItemIDs.BirthDate; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "BirthPlace")]
        public int BirthPlace { get { return (int)PDFItemIDs.BirthPlace; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "TCKNo")]
        public int TCKNo { get { return (int)PDFItemIDs.TCKNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Nationality")]
        public int Nationality { get { return (int)PDFItemIDs.Nationality; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Profession")]
        public int Profession { get { return (int)PDFItemIDs.Profession; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "MothersMaidenName")]
        public int MothersMaidenName { get { return (int)PDFItemIDs.MothersMaidenName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Sex")]
        public Sex Sexes { get { return new Sex(); } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "ContactPhoneNo")]
        public int ContactPhoneNo { get { return (int)PDFItemIDs.ContactPhoneNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Email")]
        public int Email { get { return (int)PDFItemIDs.Email; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ResidenceAddress")]
        public ResidencyAddress ResidenceAddress { get { return new ResidencyAddress(); } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriptionAddress")]
        public InstallationAddress SubscriptionAddress { get { return new InstallationAddress(); } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "PSTN")]
        public int PSTN { get { return (int)PDFItemIDs.PSTN; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LandPhone")]
        public HasLandPhone LandPhone { get { return new HasLandPhone(); } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "TariffName")]
        public int TariffName { get { return (int)PDFItemIDs.TariffName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "CompanyTitle")]
        public int CompanyTitle { get { return (int)PDFItemIDs.CompanyTitle; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "TaxOffice")]
        public int TaxOffice { get { return (int)PDFItemIDs.TaxOffice; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "TaxNo")]
        public int TaxNo { get { return (int)PDFItemIDs.TaxNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "TCKAndTaxNo")]
        public int TCKAndTaxNo { get { return (int)PDFItemIDs.TCKAndTaxNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "FirstAndLastName")]
        public int FirstAndLastName { get { return (int)PDFItemIDs.FirstAndLastName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "DisplayName")]
        public int DisplayName { get { return (int)PDFItemIDs.DisplayName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "CentralSystemNo")]
        public int CentralSystemNo { get { return (int)PDFItemIDs.CentralSystemNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "TradeRegistrationNo")]
        public int TradeRegistrationNo { get { return (int)PDFItemIDs.TradeRegistrationNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "BillingAddress")]
        public int BillingAddress { get { return (int)PDFItemIDs.BillingAddress; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "XDSLNo")]
        public int XDSLNo { get { return (int)PDFItemIDs.XDSLNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "TransitionSourceOperator")]
        public int TransitionSourceOperator { get { return (int)PDFItemIDs.TransitionSourceOperator; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Transfer")]
        public Transfer TransferAction { get { return new Transfer(); } }

        #region Subclasses
        public class ResidencyAddress
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "ResidenceAddress_NeighborhoodName")]
            public int NeighborhoodName { get { return (int)PDFItemIDs.ResidenceAddress_NeighborhoodName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "ResidenceAddress_StreetName")]
            public int StreetName { get { return (int)PDFItemIDs.ResidenceAddress_StreetName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "ResidenceAddress_BuildingNo")]
            public int BuildingNo { get { return (int)PDFItemIDs.ResidenceAddress_BuildingNo; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "ResidenceAddress_DoorNo")]
            public int DoorNo { get { return (int)PDFItemIDs.ResidenceAddress_DoorNo; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "ResidenceAddress_BuildingName")]
            public int BuildingName { get { return (int)PDFItemIDs.ResidenceAddress_BuildingName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "ResidenceAddress_Floor")]
            public int Floor { get { return (int)PDFItemIDs.ResidenceAddress_Floor; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "ResidenceAddress_DistrictName")]
            public int DistrictName { get { return (int)PDFItemIDs.ResidenceAddress_DistrictName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "ResidenceAddress_PostalCode")]
            public int PostalCode { get { return (int)PDFItemIDs.ResidenceAddress_PostalCode; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "ResidenceAddress_ProvinceName")]
            public int ProvinceName { get { return (int)PDFItemIDs.ResidenceAddress_ProvinceName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "ResidenceAddress_BBK")]
            public int BBK { get { return (int)PDFItemIDs.ResidenceAddress_BBK; } }
        }

        public class InstallationAddress
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "SubscriptionAddress_NeighborhoodName")]
            public int NeighborhoodName { get { return (int)PDFItemIDs.SubscriptionAddress_NeighborhoodName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "SubscriptionAddress_StreetName")]
            public int StreetName { get { return (int)PDFItemIDs.SubscriptionAddress_StreetName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "SubscriptionAddress_BuildingNo")]
            public int BuildingNo { get { return (int)PDFItemIDs.SubscriptionAddress_BuildingNo; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "SubscriptionAddress_DoorNo")]
            public int DoorNo { get { return (int)PDFItemIDs.SubscriptionAddress_DoorNo; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "SubscriptionAddress_BuildingName")]
            public int BuildingName { get { return (int)PDFItemIDs.SubscriptionAddress_BuildingName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "SubscriptionAddress_Floor")]
            public int Floor { get { return (int)PDFItemIDs.SubscriptionAddress_Floor; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "SubscriptionAddress_DistrictName")]
            public int DistrictName { get { return (int)PDFItemIDs.SubscriptionAddress_DistrictName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "SubscriptionAddress_PostalCode")]
            public int PostalCode { get { return (int)PDFItemIDs.SubscriptionAddress_PostalCode; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "SubscriptionAddress_ProvinceName")]
            public int ProvinceName { get { return (int)PDFItemIDs.SubscriptionAddress_ProvinceName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "SubscriptionAddress_BBK")]
            public int BBK { get { return (int)PDFItemIDs.SubscriptionAddress_BBK; } }
        }

        public class TransferringSub
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferring_SubscriberNo")]
            public int SubscriberNo { get { return (int)PDFItemIDs.Transferring_SubscriberNo; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferring_FirstName")]
            public int FirstName { get { return (int)PDFItemIDs.Transferring_FirstName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferring_LastName")]
            public int LastName { get { return (int)PDFItemIDs.Transferring_LastName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferring_FirstAndLastName")]
            public int FirstAndLastName { get { return (int)PDFItemIDs.Transferring_FirstAndLastName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferring_CompanyTitle")]
            public int CompanyTitle { get { return (int)PDFItemIDs.Transferring_CompanyTitle; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferring_DisplayName")]
            public int DisplayName { get { return (int)PDFItemIDs.Transferring_DisplayName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferring_TCKNo")]
            public int TCKNo { get { return (int)PDFItemIDs.Transferring_TCKNo; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferring_TaxNo")]
            public int TaxNo { get { return (int)PDFItemIDs.Transferring_TaxNo; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferring_TCKAndTaxNo")]
            public int TCKAndTaxNo { get { return (int)PDFItemIDs.Transferring_TCKAndTaxNo; } }
        }

        public class TransferredSub
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferred_SubscriberNo")]
            public int SubscriberNo { get { return (int)PDFItemIDs.Transferred_SubscriberNo; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferred_FirstName")]
            public int FirstName { get { return (int)PDFItemIDs.Transferred_FirstName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferred_LastName")]
            public int LastName { get { return (int)PDFItemIDs.Transferred_LastName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferred_FirstAndLastName")]
            public int FirstAndLastName { get { return (int)PDFItemIDs.Transferred_FirstAndLastName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferred_CompanyTitle")]
            public int CompanyTitle { get { return (int)PDFItemIDs.Transferred_CompanyTitle; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferred_DisplayName")]
            public int DisplayName { get { return (int)PDFItemIDs.Transferred_DisplayName; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferred_TCKNo")]
            public int TCKNo { get { return (int)PDFItemIDs.Transferred_TCKNo; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferred_TaxNo")]
            public int TaxNo { get { return (int)PDFItemIDs.Transferred_TaxNo; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Transferred_TCKAndTaxNo")]
            public int TCKAndTaxNo { get { return (int)PDFItemIDs.Transferred_TCKAndTaxNo; } }
        }

        

        public class Transfer
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Transferring")]
            public TransferringSub TransferringSubscription { get { return new TransferringSub(); } }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Transferred")]
            public TransferredSub TransferredSubscription { get { return new TransferredSub(); } }
        }

        public class Sex
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Sex_Male")]
            public int Male { get { return (int)PDFItemIDs.Sex_Male; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "Sex_Female")]
            public int Female { get { return (int)PDFItemIDs.Sex_Female; } }
        }

        public class HasLandPhone
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "LandPhone_Yes")]
            public int Yes { get { return (int)PDFItemIDs.LandPhone_Yes; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.PDFItemIDs), Name = "LandPhone_No")]
            public int No { get { return (int)PDFItemIDs.LandPhone_No; } }
        }
        #endregion
    }
}
