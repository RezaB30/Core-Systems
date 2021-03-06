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
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
        public int SubscriberNo { get { return (int)PDFItemIDs.SubscriberNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FirstName")]
        public int FirstName { get { return (int)PDFItemIDs.FirstName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LastName")]
        public int LastName { get { return (int)PDFItemIDs.LastName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "MothersName")]
        public int MothersName { get { return (int)PDFItemIDs.MothersName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FathersName")]
        public int FathersName { get { return (int)PDFItemIDs.FathersName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BirthDate")]
        public int BirthDate { get { return (int)PDFItemIDs.BirthDate; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BirthPlace")]
        public int BirthPlace { get { return (int)PDFItemIDs.BirthPlace; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TCKNo")]
        public int TCKNo { get { return (int)PDFItemIDs.TCKNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Nationality")]
        public int Nationality { get { return (int)PDFItemIDs.Nationality; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Profession")]
        public int Profession { get { return (int)PDFItemIDs.Profession; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "MothersMaidenName")]
        public int MothersMaidenName { get { return (int)PDFItemIDs.MothersMaidenName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Sex")]
        public Sex Sexes { get { return new Sex(); } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PhoneNo")]
        public int ContactPhoneNo { get { return (int)PDFItemIDs.ContactPhoneNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Email")]
        public int Email { get { return (int)PDFItemIDs.Email; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ResidenceAddress")]
        public Address ResidenceAddress { get { return new Address(Address.AddressTypes.Residency); } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriptionAddress")]
        public Address SubscriptionAddress { get { return new Address(Address.AddressTypes.Subscription); } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PSTN")]
        public int PSTN { get { return (int)PDFItemIDs.PSTN; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LandPhone")]
        public HasLandPhone LandPhone { get { return new HasLandPhone(); } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ServiceName")]
        public int TariffName { get { return (int)PDFItemIDs.TariffName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CompanyTitle")]
        public int CompanyTitle { get { return (int)PDFItemIDs.CompanyTitle; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaxOffice")]
        public int TaxOffice { get { return (int)PDFItemIDs.TaxOffice; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaxNo")]
        public int TaxNo { get { return (int)PDFItemIDs.TaxNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TCKAndTaxNo")]
        public int TCKAndTaxNo { get { return (int)PDFItemIDs.TCKAndTaxNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FullName")]
        public int FirstAndLastName { get { return (int)PDFItemIDs.FirstAndLastName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FullNameOrTitle")]
        public int DisplayName { get { return (int)PDFItemIDs.DisplayName; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CentralSystemNo")]
        public int CentralSystemNo { get { return (int)PDFItemIDs.CentralSystemNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TradeRegistrationNo")]
        public int TradeRegistrationNo { get { return (int)PDFItemIDs.TradeRegistrationNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillingAddress")]
        public int BillingAddress { get { return (int)PDFItemIDs.BillingAddress; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TelekomSubscriberNo")]
        public int XDSLNo { get { return (int)PDFItemIDs.XDSLNo; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TransitionSourceOperator")]
        public int TransitionSourceOperator { get { return (int)PDFItemIDs.TransitionSourceOperator; } }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Transfer")]
        public Transfer TransferAction { get { return new Transfer(); } }

        #region Subclasses

        public class Address
        {
            public AddressTypes AddressType { get; private set; }
            public Address(AddressTypes addressType) { AddressType = addressType; }
            public enum AddressTypes
            {
                Residency,
                Subscription
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Neighborhood")]
            public int NeighborhoodName
            {
                get
                {
                    switch (AddressType)
                    {
                        case AddressTypes.Residency:
                            return (int)PDFItemIDs.ResidenceAddress_NeighborhoodName;
                        case AddressTypes.Subscription:
                            return (int)PDFItemIDs.SubscriptionAddress_NeighborhoodName;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "StreetName")]
            public int StreetName
            {
                get
                {
                    switch (AddressType)
                    {
                        case AddressTypes.Residency:
                            return (int)PDFItemIDs.ResidenceAddress_StreetName;
                        case AddressTypes.Subscription:
                            return (int)PDFItemIDs.SubscriptionAddress_StreetName;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BuildingNo")]
            public int BuildingNo
            {
                get
                {
                    switch (AddressType)
                    {
                        case AddressTypes.Residency:
                            return (int)PDFItemIDs.ResidenceAddress_BuildingNo;
                        case AddressTypes.Subscription:
                            return (int)PDFItemIDs.SubscriptionAddress_BuildingNo;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DoorNo")]
            public int DoorNo
            {
                get
                {
                    switch (AddressType)
                    {
                        case AddressTypes.Residency:
                            return (int)PDFItemIDs.ResidenceAddress_DoorNo;
                        case AddressTypes.Subscription:
                            return (int)PDFItemIDs.SubscriptionAddress_DoorNo;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BuildingName")]
            public int BuildingName
            {
                get
                {
                    switch (AddressType)
                    {
                        case AddressTypes.Residency:
                            return (int)PDFItemIDs.ResidenceAddress_BuildingName;
                        case AddressTypes.Subscription:
                            return (int)PDFItemIDs.SubscriptionAddress_BuildingName;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Floor")]
            public int Floor
            {
                get
                {
                    switch (AddressType)
                    {
                        case AddressTypes.Residency:
                            return (int)PDFItemIDs.ResidenceAddress_Floor;
                        case AddressTypes.Subscription:
                            return (int)PDFItemIDs.SubscriptionAddress_Floor;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "District")]
            public int DistrictName
            {
                get
                {
                    switch (AddressType)
                    {
                        case AddressTypes.Residency:
                            return (int)PDFItemIDs.ResidenceAddress_DistrictName;
                        case AddressTypes.Subscription:
                            return (int)PDFItemIDs.SubscriptionAddress_DistrictName;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PostalCode")]
            public int PostalCode
            {
                get
                {
                    switch (AddressType)
                    {
                        case AddressTypes.Residency:
                            return (int)PDFItemIDs.ResidenceAddress_PostalCode;
                        case AddressTypes.Subscription:
                            return (int)PDFItemIDs.SubscriptionAddress_PostalCode;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Province")]
            public int ProvinceName
            {
                get
                {
                    switch (AddressType)
                    {
                        case AddressTypes.Residency:
                            return (int)PDFItemIDs.ResidenceAddress_ProvinceName;
                        case AddressTypes.Subscription:
                            return (int)PDFItemIDs.SubscriptionAddress_ProvinceName;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BBK")]
            public int BBK
            {
                get
                {
                    switch (AddressType)
                    {
                        case AddressTypes.Residency:
                            return (int)PDFItemIDs.ResidenceAddress_BBK;
                        case AddressTypes.Subscription:
                            return (int)PDFItemIDs.SubscriptionAddress_BBK;
                        default:
                            return 0;
                    }
                }
            }
        }

        public class TransferSubscription
        {
            public TransferSubscriptionTypes TransferSubscriptionType { get; private set; }
            public TransferSubscription(TransferSubscriptionTypes transferSubscriptionType) { TransferSubscriptionType = transferSubscriptionType; }
            public enum TransferSubscriptionTypes
            {
                Transferring,
                Transferred
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
            public int SubscriberNo
            {
                get
                {
                    switch (TransferSubscriptionType)
                    {
                        case TransferSubscriptionTypes.Transferring:
                            return (int)PDFItemIDs.Transferring_SubscriberNo;
                        case TransferSubscriptionTypes.Transferred:
                            return (int)PDFItemIDs.Transferred_SubscriberNo;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FirstName")]
            public int FirstName
            {
                get
                {
                    switch (TransferSubscriptionType)
                    {
                        case TransferSubscriptionTypes.Transferring:
                            return (int)PDFItemIDs.Transferring_FirstName;
                        case TransferSubscriptionTypes.Transferred:
                            return (int)PDFItemIDs.Transferred_FirstName;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LastName")]
            public int LastName
            {
                get
                {
                    switch (TransferSubscriptionType)
                    {
                        case TransferSubscriptionTypes.Transferring:
                            return (int)PDFItemIDs.Transferring_LastName;
                        case TransferSubscriptionTypes.Transferred:
                            return (int)PDFItemIDs.Transferred_LastName;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FullName")]
            public int FirstAndLastName
            {
                get
                {
                    switch (TransferSubscriptionType)
                    {
                        case TransferSubscriptionTypes.Transferring:
                            return (int)PDFItemIDs.Transferring_FirstAndLastName;
                        case TransferSubscriptionTypes.Transferred:
                            return (int)PDFItemIDs.Transferred_FirstAndLastName;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CompanyTitle")]
            public int CompanyTitle
            {
                get
                {
                    switch (TransferSubscriptionType)
                    {
                        case TransferSubscriptionTypes.Transferring:
                            return (int)PDFItemIDs.Transferring_CompanyTitle;
                        case TransferSubscriptionTypes.Transferred:
                            return (int)PDFItemIDs.Transferred_CompanyTitle;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FullNameOrTitle")]
            public int DisplayName
            {
                get
                {
                    switch (TransferSubscriptionType)
                    {
                        case TransferSubscriptionTypes.Transferring:
                            return (int)PDFItemIDs.Transferring_DisplayName;
                        case TransferSubscriptionTypes.Transferred:
                            return (int)PDFItemIDs.Transferred_DisplayName;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TCKNo")]
            public int TCKNo
            {
                get
                {
                    switch (TransferSubscriptionType)
                    {
                        case TransferSubscriptionTypes.Transferring:
                            return (int)PDFItemIDs.Transferring_TCKNo;
                        case TransferSubscriptionTypes.Transferred:
                            return (int)PDFItemIDs.Transferred_TCKNo;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaxNo")]
            public int TaxNo
            {
                get
                {
                    switch (TransferSubscriptionType)
                    {
                        case TransferSubscriptionTypes.Transferring:
                            return (int)PDFItemIDs.Transferring_TaxNo;
                        case TransferSubscriptionTypes.Transferred:
                            return (int)PDFItemIDs.Transferred_TaxNo;
                        default:
                            return 0;
                    }
                }
            }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TCKAndTaxNo")]
            public int TCKAndTaxNo
            {
                get
                {
                    switch (TransferSubscriptionType)
                    {
                        case TransferSubscriptionTypes.Transferring:
                            return (int)PDFItemIDs.Transferring_TCKAndTaxNo;
                        case TransferSubscriptionTypes.Transferred:
                            return (int)PDFItemIDs.Transferred_TCKAndTaxNo;
                        default:
                            return 0;
                    }
                }
            }
        }

        public class Transfer
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Transferring")]
            public TransferSubscription TransferringSubscription { get { return new TransferSubscription(TransferSubscription.TransferSubscriptionTypes.Transferring); } }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Transferred")]
            public TransferSubscription TransferredSubscription { get { return new TransferSubscription(TransferSubscription.TransferSubscriptionTypes.Transferred); } }
        }

        public class Sex
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Lists.Sexes), Name = "Male")]
            public int Male { get { return (int)PDFItemIDs.Sex_Male; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.Sexes), Name = "Female")]
            public int Female { get { return (int)PDFItemIDs.Sex_Female; } }
        }

        public class HasLandPhone
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Yes")]
            public int Yes { get { return (int)PDFItemIDs.LandPhone_Yes; } }

            [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "No")]
            public int No { get { return (int)PDFItemIDs.LandPhone_No; } }
        }
        #endregion
    }
}
