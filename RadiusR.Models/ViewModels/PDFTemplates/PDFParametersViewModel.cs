using RadiusR.DB;
using RadiusR.DB.Enums;
using RezaB.Web.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.PDFTemplates
{
    public class PDFParametersViewModel
    {
        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
        public Coords SubscriberNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FirstName")]
        public Coords FirstName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LastName")]
        public Coords LastName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "MothersName")]
        public Coords MothersName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FathersName")]
        public Coords FathersName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BirthDate")]
        public Coords BirthDate { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BirthPlace")]
        public Coords BirthPlace { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TCKNo")]
        public Coords TCKNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Nationality")]
        public Coords Nationality { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Profession")]
        public Coords Profession { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "MothersMaidenName")]
        public Coords MothersMaidenName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Sex")]
        public Sex Sexes { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PhoneNo")]
        public Coords ContactPhoneNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Email")]
        public Coords Email { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ResidenceAddress")]
        public Address ResidenceAddress { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriptionAddress")]
        public Address SubscriptionAddress { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PSTN")]
        public Coords PSTN { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LandPhone")]
        public HasLandPhone LandPhone { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "ServiceName")]
        public Coords TariffName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CompanyTitle")]
        public Coords CompanyTitle { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaxOffice")]
        public Coords TaxOffice { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaxNo")]
        public Coords TaxNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TCKAndTaxNo")]
        public Coords TCKAndTaxNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FullName")]
        public Coords FirstAndLastName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FullNameOrTitle")]
        public Coords DisplayName { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CentralSystemNo")]
        public Coords CentralSystemNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TradeRegistrationNo")]
        public Coords TradeRegistrationNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BillingAddress")]
        public Coords BillingAddress { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TelekomSubscriberNo")]
        public Coords XDSLNo { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TransitionSourceOperator")]
        public Coords TransitionSourceOperator { get; set; }

        [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Transfer")]
        public Transfer TransferAction { get; set; }

        #region Subclasses

        public class Address
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Neighborhood")] //mah.
            public Coords NeighborhoodName { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "StreetName")]   //sok.
            public Coords StreetName { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BuildingNo")]   //binaDışKapı
            public Coords BuildingNo { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "DoorNo")]    //içkapı
            public Coords DoorNo { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BuildingName")]
            public Coords BuildingName { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Floor")]
            public Coords Floor { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "District")]
            public Coords DistrictName { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "PostalCode")]
            public Coords PostalCode { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Province")]
            public Coords ProvinceName { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "BBK")]
            public Coords BBK { get; set; }
        }

        public class TransferSubscription
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "SubscriberNo")]
            public Coords SubscriberNo { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FirstName")]
            public Coords FirstName { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "LastName")]
            public Coords LastName { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FullName")]
            public Coords FirstAndLastName { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "CompanyTitle")]
            public Coords CompanyTitle { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "FullNameOrTitle")]
            public Coords DisplayName { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TCKNo")]
            public Coords TCKNo { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TaxNo")]
            public Coords TaxNo { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "TCKAndTaxNo")]
            public Coords TCKAndTaxNo { get; set; }
        }

        public class Transfer
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Transferring")]
            public TransferSubscription TransferringSubscription { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Model.RadiusR), Name = "Transferred")]
            public TransferSubscription TransferredSubscription { get; set; }
        }

        public class Sex
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Lists.Sexes), Name = "Male")]
            public Coords Male { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Lists.Sexes), Name = "Female")]
            public Coords Female { get; set; }
        }

        public class HasLandPhone
        {
            [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "Yes")]
            public Coords Yes { get; set; }

            [Display(ResourceType = typeof(RadiusR.Localization.Pages.Common), Name = "No")]
            public Coords No { get; set; }
        }

        public class Coords
        {
            public InvariantDecimal X { get; set; }

            public InvariantDecimal Y { get; set; }
        }

        public class InvariantDecimal
        {
            public decimal Value { get; set; }

            public override string ToString()
            {
                return Value.ToString(CultureInfo.InvariantCulture);
            }
        }

        #endregion
        #region Converter
        public void UpdateDatabase(RadiusREntities db, int formType)
        {
            db.PDFFormItemPlacements.RemoveRange(db.PDFFormItemPlacements.Where(pdfItem => pdfItem.FormType == formType));
            if (SubscriberNo != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.SubscriberNo,
                    FormType = formType,
                    CoordsX = SubscriberNo.X.Value,
                    CoordsY = SubscriberNo.Y.Value
                });
            }
            if (FirstName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.FirstName,
                    FormType = formType,
                    CoordsX = FirstName.X.Value,
                    CoordsY = FirstName.Y.Value
                });
            }
            if (LastName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.LastName,
                    FormType = formType,
                    CoordsX = LastName.X.Value,
                    CoordsY = LastName.Y.Value
                });
            }
            if (MothersName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.MothersName,
                    FormType = formType,
                    CoordsX = MothersName.X.Value,
                    CoordsY = MothersName.Y.Value
                });
            }
            if (FathersName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.FathersName,
                    FormType = formType,
                    CoordsX = FathersName.X.Value,
                    CoordsY = FathersName.Y.Value
                });
            }
            if (BirthDate != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.BirthDate,
                    FormType = formType,
                    CoordsX = BirthDate.X.Value,
                    CoordsY = BirthDate.Y.Value
                });
            }
            if (BirthPlace != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.BirthPlace,
                    FormType = formType,
                    CoordsX = BirthPlace.X.Value,
                    CoordsY = BirthPlace.Y.Value
                });
            }
            if (TCKNo != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.TCKNo,
                    FormType = formType,
                    CoordsX = TCKNo.X.Value,
                    CoordsY = TCKNo.Y.Value
                });
            }
            if (Nationality != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.Nationality,
                    FormType = formType,
                    CoordsX = Nationality.X.Value,
                    CoordsY = Nationality.Y.Value
                });
            }
            if (Profession != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.Profession,
                    FormType = formType,
                    CoordsX = Profession.X.Value,
                    CoordsY = Profession.Y.Value
                });
            }
            if (MothersMaidenName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.MothersMaidenName,
                    FormType = formType,
                    CoordsX = MothersMaidenName.X.Value,
                    CoordsY = MothersMaidenName.Y.Value
                });
            }
            if (Sexes != null && Sexes.Male != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.Sex_Male,
                    FormType = formType,
                    CoordsX = Sexes.Male.X.Value,
                    CoordsY = Sexes.Male.Y.Value
                });
            }
            if (Sexes != null && Sexes.Female != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.Sex_Female,
                    FormType = formType,
                    CoordsX = Sexes.Female.X.Value,
                    CoordsY = Sexes.Female.Y.Value
                });
            }
            if (ContactPhoneNo != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.ContactPhoneNo,
                    FormType = formType,
                    CoordsX = ContactPhoneNo.X.Value,
                    CoordsY = ContactPhoneNo.Y.Value
                });
            }
            if (Email != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.Email,
                    FormType = formType,
                    CoordsX = Email.X.Value,
                    CoordsY = Email.Y.Value
                });
            }
            if (ResidenceAddress != null && ResidenceAddress.ProvinceName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.ResidenceAddress_ProvinceName,
                    FormType = formType,
                    CoordsX = ResidenceAddress.ProvinceName.X.Value,
                    CoordsY = ResidenceAddress.ProvinceName.Y.Value
                });
            }
            if (ResidenceAddress != null && ResidenceAddress.DistrictName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.ResidenceAddress_DistrictName,
                    FormType = formType,
                    CoordsX = ResidenceAddress.DistrictName.X.Value,
                    CoordsY = ResidenceAddress.DistrictName.Y.Value
                });
            }
            if (ResidenceAddress != null && ResidenceAddress.NeighborhoodName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.ResidenceAddress_NeighborhoodName,
                    FormType = formType,
                    CoordsX = ResidenceAddress.NeighborhoodName.X.Value,
                    CoordsY = ResidenceAddress.NeighborhoodName.Y.Value
                });
            }
            if (ResidenceAddress != null && ResidenceAddress.StreetName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.ResidenceAddress_StreetName,
                    FormType = formType,
                    CoordsX = ResidenceAddress.StreetName.X.Value,
                    CoordsY = ResidenceAddress.StreetName.Y.Value
                });
            }
            if (ResidenceAddress != null && ResidenceAddress.BuildingNo != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.ResidenceAddress_BuildingNo,
                    FormType = formType,
                    CoordsX = ResidenceAddress.BuildingNo.X.Value,
                    CoordsY = ResidenceAddress.BuildingNo.Y.Value
                });
            }
            if (ResidenceAddress != null && ResidenceAddress.DoorNo != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.ResidenceAddress_DoorNo,
                    FormType = formType,
                    CoordsX = ResidenceAddress.DoorNo.X.Value,
                    CoordsY = ResidenceAddress.DoorNo.Y.Value
                });
            }
            if (ResidenceAddress != null && ResidenceAddress.BuildingName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.ResidenceAddress_BuildingName,
                    FormType = formType,
                    CoordsX = ResidenceAddress.BuildingName.X.Value,
                    CoordsY = ResidenceAddress.BuildingName.Y.Value
                });
            }
            if (ResidenceAddress != null && ResidenceAddress.Floor != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.ResidenceAddress_Floor,
                    FormType = formType,
                    CoordsX = ResidenceAddress.Floor.X.Value,
                    CoordsY = ResidenceAddress.Floor.Y.Value
                });
            }
            if (ResidenceAddress != null && ResidenceAddress.PostalCode != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.ResidenceAddress_PostalCode,
                    FormType = formType,
                    CoordsX = ResidenceAddress.PostalCode.X.Value,
                    CoordsY = ResidenceAddress.PostalCode.Y.Value
                });
            }
            if (ResidenceAddress != null && ResidenceAddress.BBK != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.ResidenceAddress_BBK,
                    FormType = formType,
                    CoordsX = ResidenceAddress.BBK.X.Value,
                    CoordsY = ResidenceAddress.BBK.Y.Value
                });
            }
            if (SubscriptionAddress != null && SubscriptionAddress.ProvinceName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.SubscriptionAddress_ProvinceName,
                    FormType = formType,
                    CoordsX = SubscriptionAddress.ProvinceName.X.Value,
                    CoordsY = SubscriptionAddress.ProvinceName.Y.Value
                });
            }
            if (SubscriptionAddress != null && SubscriptionAddress.DistrictName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.SubscriptionAddress_DistrictName,
                    FormType = formType,
                    CoordsX = SubscriptionAddress.DistrictName.X.Value,
                    CoordsY = SubscriptionAddress.DistrictName.Y.Value
                });
            }
            if (SubscriptionAddress != null && SubscriptionAddress.NeighborhoodName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.SubscriptionAddress_NeighborhoodName,
                    FormType = formType,
                    CoordsX = SubscriptionAddress.NeighborhoodName.X.Value,
                    CoordsY = SubscriptionAddress.NeighborhoodName.Y.Value
                });
            }
            if (SubscriptionAddress != null && SubscriptionAddress.StreetName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.SubscriptionAddress_StreetName,
                    FormType = formType,
                    CoordsX = SubscriptionAddress.StreetName.X.Value,
                    CoordsY = SubscriptionAddress.StreetName.Y.Value
                });
            }
            if (SubscriptionAddress != null && SubscriptionAddress.BuildingNo != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.SubscriptionAddress_BuildingNo,
                    FormType = formType,
                    CoordsX = SubscriptionAddress.BuildingNo.X.Value,
                    CoordsY = SubscriptionAddress.BuildingNo.Y.Value
                });
            }
            if (SubscriptionAddress != null && SubscriptionAddress.DoorNo != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.SubscriptionAddress_DoorNo,
                    FormType = formType,
                    CoordsX = SubscriptionAddress.DoorNo.X.Value,
                    CoordsY = SubscriptionAddress.DoorNo.Y.Value
                });
            }
            if (SubscriptionAddress != null && SubscriptionAddress.BuildingName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.SubscriptionAddress_BuildingName,
                    FormType = formType,
                    CoordsX = SubscriptionAddress.BuildingName.X.Value,
                    CoordsY = SubscriptionAddress.BuildingName.Y.Value
                });
            }
            if (SubscriptionAddress != null && SubscriptionAddress.Floor != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.SubscriptionAddress_Floor,
                    FormType = formType,
                    CoordsX = SubscriptionAddress.Floor.X.Value,
                    CoordsY = SubscriptionAddress.Floor.Y.Value
                });
            }
            if (SubscriptionAddress != null && SubscriptionAddress.PostalCode != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.SubscriptionAddress_PostalCode,
                    FormType = formType,
                    CoordsX = SubscriptionAddress.PostalCode.X.Value,
                    CoordsY = SubscriptionAddress.PostalCode.Y.Value
                });
            }
            if (SubscriptionAddress != null && SubscriptionAddress.BBK != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.SubscriptionAddress_BBK,
                    FormType = formType,
                    CoordsX = SubscriptionAddress.BBK.X.Value,
                    CoordsY = SubscriptionAddress.BBK.Y.Value
                });
            }
            if (PSTN != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.PSTN,
                    FormType = formType,
                    CoordsX = PSTN.X.Value,
                    CoordsY = PSTN.Y.Value
                });
            }
            if (LandPhone != null && LandPhone.Yes != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.LandPhone_Yes,
                    FormType = formType,
                    CoordsX = LandPhone.Yes.X.Value,
                    CoordsY = LandPhone.Yes.Y.Value
                });
            }
            if (LandPhone != null && LandPhone.No != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.LandPhone_No,
                    FormType = formType,
                    CoordsX = LandPhone.No.X.Value,
                    CoordsY = LandPhone.No.Y.Value
                });
            }
            if (TariffName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.TariffName,
                    FormType = formType,
                    CoordsX = TariffName.X.Value,
                    CoordsY = TariffName.Y.Value
                });
            }
            if (CompanyTitle != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.CompanyTitle,
                    FormType = formType,
                    CoordsX = CompanyTitle.X.Value,
                    CoordsY = CompanyTitle.Y.Value
                });
            }
            if (TaxOffice != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.TaxOffice,
                    FormType = formType,
                    CoordsX = TaxOffice.X.Value,
                    CoordsY = TaxOffice.Y.Value
                });
            }
            if (TaxNo != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.TaxNo,
                    FormType = formType,
                    CoordsX = TaxNo.X.Value,
                    CoordsY = TaxNo.Y.Value
                });
            }
            if (TCKAndTaxNo != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.TCKAndTaxNo,
                    FormType = formType,
                    CoordsX = TCKAndTaxNo.X.Value,
                    CoordsY = TCKAndTaxNo.Y.Value
                });
            }
            if (FirstAndLastName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.FirstAndLastName,
                    FormType = formType,
                    CoordsX = FirstAndLastName.X.Value,
                    CoordsY = FirstAndLastName.Y.Value
                });
            }
            if (DisplayName != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.DisplayName,
                    FormType = formType,
                    CoordsX = DisplayName.X.Value,
                    CoordsY = DisplayName.Y.Value
                });
            }
            if (CentralSystemNo != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.CentralSystemNo,
                    FormType = formType,
                    CoordsX = CentralSystemNo.X.Value,
                    CoordsY = CentralSystemNo.Y.Value
                });
            }
            if (TradeRegistrationNo != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.TradeRegistrationNo,
                    FormType = formType,
                    CoordsX = TradeRegistrationNo.X.Value,
                    CoordsY = TradeRegistrationNo.Y.Value
                });
            }
            if (BillingAddress != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.BillingAddress,
                    FormType = formType,
                    CoordsX = BillingAddress.X.Value,
                    CoordsY = BillingAddress.Y.Value
                });
            }
            if (XDSLNo != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.XDSLNo,
                    FormType = formType,
                    CoordsX = XDSLNo.X.Value,
                    CoordsY = XDSLNo.Y.Value
                });
            }
            if (TransitionSourceOperator != null)
            {
                db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                {
                    ItemID = (int)PDFItemIDs.TransitionSourceOperator,
                    FormType = formType,
                    CoordsX = TransitionSourceOperator.X.Value,
                    CoordsY = TransitionSourceOperator.Y.Value
                });
            }
            if (TransferAction != null)
            {
                if (TransferAction.TransferredSubscription != null)
                {
                    if (TransferAction.TransferredSubscription.SubscriberNo != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferred_SubscriberNo,
                            FormType = formType,
                            CoordsX = TransferAction.TransferredSubscription.SubscriberNo.X.Value,
                            CoordsY = TransferAction.TransferredSubscription.SubscriberNo.Y.Value
                        });
                    }
                    if (TransferAction.TransferredSubscription.FirstName != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferred_FirstName,
                            FormType = formType,
                            CoordsX = TransferAction.TransferredSubscription.FirstName.X.Value,
                            CoordsY = TransferAction.TransferredSubscription.FirstName.Y.Value
                        });
                    }
                    if (TransferAction.TransferredSubscription.LastName != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferred_LastName,
                            FormType = formType,
                            CoordsX = TransferAction.TransferredSubscription.LastName.X.Value,
                            CoordsY = TransferAction.TransferredSubscription.LastName.Y.Value
                        });
                    }
                    if (TransferAction.TransferredSubscription.FirstAndLastName != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferred_FirstAndLastName,
                            FormType = formType,
                            CoordsX = TransferAction.TransferredSubscription.FirstAndLastName.X.Value,
                            CoordsY = TransferAction.TransferredSubscription.FirstAndLastName.Y.Value
                        });
                    }
                    if (TransferAction.TransferredSubscription.CompanyTitle != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferred_CompanyTitle,
                            FormType = formType,
                            CoordsX = TransferAction.TransferredSubscription.CompanyTitle.X.Value,
                            CoordsY = TransferAction.TransferredSubscription.CompanyTitle.Y.Value
                        });
                    }
                    if (TransferAction.TransferredSubscription.DisplayName != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferred_DisplayName,
                            FormType = formType,
                            CoordsX = TransferAction.TransferredSubscription.DisplayName.X.Value,
                            CoordsY = TransferAction.TransferredSubscription.DisplayName.Y.Value
                        });
                    }
                    if (TransferAction.TransferredSubscription.TCKNo != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferred_TCKNo,
                            FormType = formType,
                            CoordsX = TransferAction.TransferredSubscription.TCKNo.X.Value,
                            CoordsY = TransferAction.TransferredSubscription.TCKNo.Y.Value
                        });
                    }
                    if (TransferAction.TransferredSubscription.TaxNo != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferred_TaxNo,
                            FormType = formType,
                            CoordsX = TransferAction.TransferredSubscription.TaxNo.X.Value,
                            CoordsY = TransferAction.TransferredSubscription.TaxNo.Y.Value
                        });
                    }
                    if (TransferAction.TransferredSubscription.TCKAndTaxNo != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferred_TCKAndTaxNo,
                            FormType = formType,
                            CoordsX = TransferAction.TransferredSubscription.TCKAndTaxNo.X.Value,
                            CoordsY = TransferAction.TransferredSubscription.TCKAndTaxNo.Y.Value
                        });
                    }
                }
                if (TransferAction.TransferringSubscription != null)
                {
                    if (TransferAction.TransferringSubscription.SubscriberNo != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferring_SubscriberNo,
                            FormType = formType,
                            CoordsX = TransferAction.TransferringSubscription.SubscriberNo.X.Value,
                            CoordsY = TransferAction.TransferringSubscription.SubscriberNo.Y.Value
                        });
                    }
                    if (TransferAction.TransferringSubscription.FirstName != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferring_FirstName,
                            FormType = formType,
                            CoordsX = TransferAction.TransferringSubscription.FirstName.X.Value,
                            CoordsY = TransferAction.TransferringSubscription.FirstName.Y.Value
                        });
                    }
                    if (TransferAction.TransferringSubscription.LastName != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferring_LastName,
                            FormType = formType,
                            CoordsX = TransferAction.TransferringSubscription.LastName.X.Value,
                            CoordsY = TransferAction.TransferringSubscription.LastName.Y.Value
                        });
                    }
                    if (TransferAction.TransferringSubscription.FirstAndLastName != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferring_FirstAndLastName,
                            FormType = formType,
                            CoordsX = TransferAction.TransferringSubscription.FirstAndLastName.X.Value,
                            CoordsY = TransferAction.TransferringSubscription.FirstAndLastName.Y.Value
                        });
                    }
                    if (TransferAction.TransferringSubscription.CompanyTitle != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferring_CompanyTitle,
                            FormType = formType,
                            CoordsX = TransferAction.TransferringSubscription.CompanyTitle.X.Value,
                            CoordsY = TransferAction.TransferringSubscription.CompanyTitle.Y.Value
                        });
                    }
                    if (TransferAction.TransferringSubscription.DisplayName != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferring_DisplayName,
                            FormType = formType,
                            CoordsX = TransferAction.TransferringSubscription.DisplayName.X.Value,
                            CoordsY = TransferAction.TransferringSubscription.DisplayName.Y.Value
                        });
                    }
                    if (TransferAction.TransferringSubscription.TCKNo != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferring_TCKNo,
                            FormType = formType,
                            CoordsX = TransferAction.TransferringSubscription.TCKNo.X.Value,
                            CoordsY = TransferAction.TransferringSubscription.TCKNo.Y.Value
                        });
                    }
                    if (TransferAction.TransferringSubscription.TaxNo != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferring_TaxNo,
                            FormType = formType,
                            CoordsX = TransferAction.TransferringSubscription.TaxNo.X.Value,
                            CoordsY = TransferAction.TransferringSubscription.TaxNo.Y.Value
                        });
                    }
                    if (TransferAction.TransferringSubscription.TCKAndTaxNo != null)
                    {
                        db.PDFFormItemPlacements.Add(new PDFFormItemPlacement()
                        {
                            ItemID = (int)PDFItemIDs.Transferring_TCKAndTaxNo,
                            FormType = formType,
                            CoordsX = TransferAction.TransferringSubscription.TCKAndTaxNo.X.Value,
                            CoordsY = TransferAction.TransferringSubscription.TCKAndTaxNo.Y.Value
                        });
                    }
                }
            }
        }

        public PDFParametersViewModel() { }

        public PDFParametersViewModel(RadiusREntities db, int formType)
        {
            var parameters = db.PDFFormItemPlacements.Where(pdfItem => pdfItem.FormType == formType).ToArray();
            foreach (var item in parameters)
            {
                switch ((PDFItemIDs)item.ItemID)
                {
                    case PDFItemIDs.SubscriberNo:
                        SubscriberNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.FirstName:
                        FirstName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.LastName:
                        LastName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.MothersName:
                        MothersName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.FathersName:
                        FathersName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.BirthDate:
                        BirthDate = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.BirthPlace:
                        BirthPlace = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.TCKNo:
                        TCKNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Nationality:
                        Nationality = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Profession:
                        Profession = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.MothersMaidenName:
                        MothersMaidenName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Sex_Male:
                        if (Sexes == null)
                        {
                            Sexes = new Sex();
                        }
                        Sexes.Male = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Sex_Female:
                        if (Sexes == null)
                        {
                            Sexes = new Sex();
                        }
                        Sexes.Female = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.ContactPhoneNo:
                        ContactPhoneNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Email:
                        Email = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.ResidenceAddress_ProvinceName:
                        if (ResidenceAddress == null)
                        {
                            ResidenceAddress = new Address();
                        }
                        ResidenceAddress.ProvinceName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.ResidenceAddress_DistrictName:
                        if (ResidenceAddress == null)
                        {
                            ResidenceAddress = new Address();
                        }
                        ResidenceAddress.DistrictName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.ResidenceAddress_NeighborhoodName:
                        if (ResidenceAddress == null)
                        {
                            ResidenceAddress = new Address();
                        }
                        ResidenceAddress.NeighborhoodName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.ResidenceAddress_StreetName:
                        if (ResidenceAddress == null)
                        {
                            ResidenceAddress = new Address();
                        }
                        ResidenceAddress.StreetName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.ResidenceAddress_BuildingNo:
                        if (ResidenceAddress == null)
                        {
                            ResidenceAddress = new Address();
                        }
                        ResidenceAddress.BuildingNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.ResidenceAddress_DoorNo:
                        if (ResidenceAddress == null)
                        {
                            ResidenceAddress = new Address();
                        }
                        ResidenceAddress.DoorNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.ResidenceAddress_BuildingName:
                        if (ResidenceAddress == null)
                        {
                            ResidenceAddress = new Address();
                        }
                        ResidenceAddress.BuildingName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.ResidenceAddress_Floor:
                        if (ResidenceAddress == null)
                        {
                            ResidenceAddress = new Address();
                        }
                        ResidenceAddress.Floor = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.ResidenceAddress_PostalCode:
                        if (ResidenceAddress == null)
                        {
                            ResidenceAddress = new Address();
                        }
                        ResidenceAddress.PostalCode = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.ResidenceAddress_BBK:
                        if (ResidenceAddress == null)
                        {
                            ResidenceAddress = new Address();
                        }
                        ResidenceAddress.BBK = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.SubscriptionAddress_ProvinceName:
                        if (SubscriptionAddress == null)
                        {
                            SubscriptionAddress = new Address();
                        }
                        SubscriptionAddress.ProvinceName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.SubscriptionAddress_DistrictName:
                        if (SubscriptionAddress == null)
                        {
                            SubscriptionAddress = new Address();
                        }
                        SubscriptionAddress.DistrictName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.SubscriptionAddress_NeighborhoodName:
                        if (SubscriptionAddress == null)
                        {
                            SubscriptionAddress = new Address();
                        }
                        SubscriptionAddress.NeighborhoodName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.SubscriptionAddress_StreetName:
                        if (SubscriptionAddress == null)
                        {
                            SubscriptionAddress = new Address();
                        }
                        SubscriptionAddress.StreetName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.SubscriptionAddress_BuildingNo:
                        if (SubscriptionAddress == null)
                        {
                            SubscriptionAddress = new Address();
                        }
                        SubscriptionAddress.BuildingNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.SubscriptionAddress_DoorNo:
                        if (SubscriptionAddress == null)
                        {
                            SubscriptionAddress = new Address();
                        }
                        SubscriptionAddress.DoorNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.SubscriptionAddress_BuildingName:
                        if (SubscriptionAddress == null)
                        {
                            SubscriptionAddress = new Address();
                        }
                        SubscriptionAddress.BuildingName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.SubscriptionAddress_Floor:
                        if (SubscriptionAddress == null)
                        {
                            SubscriptionAddress = new Address();
                        }
                        SubscriptionAddress.Floor = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.SubscriptionAddress_PostalCode:
                        if (SubscriptionAddress == null)
                        {
                            SubscriptionAddress = new Address();
                        }
                        SubscriptionAddress.PostalCode = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.SubscriptionAddress_BBK:
                        if (SubscriptionAddress == null)
                        {
                            SubscriptionAddress = new Address();
                        }
                        SubscriptionAddress.BBK = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.PSTN:
                        PSTN = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.LandPhone_Yes:
                        if (LandPhone == null)
                        {
                            LandPhone = new HasLandPhone();
                        }
                        LandPhone.Yes = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.LandPhone_No:
                        if (LandPhone == null)
                        {
                            LandPhone = new HasLandPhone();
                        }
                        LandPhone.No = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.TariffName:
                        TariffName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.CompanyTitle:
                        CompanyTitle = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.TaxOffice:
                        TaxOffice = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.TaxNo:
                        TaxNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.TCKAndTaxNo:
                        TCKAndTaxNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.FirstAndLastName:
                        FirstAndLastName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.DisplayName:
                        DisplayName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.CentralSystemNo:
                        CentralSystemNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.TradeRegistrationNo:
                        TradeRegistrationNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.BillingAddress:
                        BillingAddress = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.XDSLNo:
                        XDSLNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.TransitionSourceOperator:
                        TransitionSourceOperator = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferred_SubscriberNo:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferredSubscription = TransferAction.TransferredSubscription ?? new TransferSubscription();
                        TransferAction.TransferredSubscription.SubscriberNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferred_FirstName:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferredSubscription = TransferAction.TransferredSubscription ?? new TransferSubscription();
                        TransferAction.TransferredSubscription.FirstName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferred_LastName:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferredSubscription = TransferAction.TransferredSubscription ?? new TransferSubscription();
                        TransferAction.TransferredSubscription.LastName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferred_FirstAndLastName:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferredSubscription = TransferAction.TransferredSubscription ?? new TransferSubscription();
                        TransferAction.TransferredSubscription.FirstAndLastName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferred_CompanyTitle:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferredSubscription = TransferAction.TransferredSubscription ?? new TransferSubscription();
                        TransferAction.TransferredSubscription.CompanyTitle = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferred_DisplayName:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferredSubscription = TransferAction.TransferredSubscription ?? new TransferSubscription();
                        TransferAction.TransferredSubscription.DisplayName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferred_TCKNo:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferredSubscription = TransferAction.TransferredSubscription ?? new TransferSubscription();
                        TransferAction.TransferredSubscription.TCKNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferred_TaxNo:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferredSubscription = TransferAction.TransferredSubscription ?? new TransferSubscription();
                        TransferAction.TransferredSubscription.TaxNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferred_TCKAndTaxNo:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferredSubscription = TransferAction.TransferredSubscription ?? new TransferSubscription();
                        TransferAction.TransferredSubscription.TCKAndTaxNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferring_SubscriberNo:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferringSubscription = TransferAction.TransferringSubscription ?? new TransferSubscription();
                        TransferAction.TransferringSubscription.SubscriberNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferring_FirstName:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferringSubscription = TransferAction.TransferringSubscription ?? new TransferSubscription();
                        TransferAction.TransferringSubscription.FirstName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferring_LastName:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferringSubscription = TransferAction.TransferringSubscription ?? new TransferSubscription();
                        TransferAction.TransferringSubscription.LastName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferring_FirstAndLastName:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferringSubscription = TransferAction.TransferringSubscription ?? new TransferSubscription();
                        TransferAction.TransferringSubscription.FirstAndLastName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferring_CompanyTitle:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferringSubscription = TransferAction.TransferringSubscription ?? new TransferSubscription();
                        TransferAction.TransferringSubscription.CompanyTitle = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferring_DisplayName:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferringSubscription = TransferAction.TransferringSubscription ?? new TransferSubscription();
                        TransferAction.TransferringSubscription.DisplayName = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferring_TCKNo:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferringSubscription = TransferAction.TransferringSubscription ?? new TransferSubscription();
                        TransferAction.TransferringSubscription.TCKNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferring_TaxNo:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferringSubscription = TransferAction.TransferringSubscription ?? new TransferSubscription();
                        TransferAction.TransferringSubscription.TaxNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    case PDFItemIDs.Transferring_TCKAndTaxNo:
                        TransferAction = TransferAction ?? new Transfer();
                        TransferAction.TransferringSubscription = TransferAction.TransferringSubscription ?? new TransferSubscription();
                        TransferAction.TransferringSubscription.TCKAndTaxNo = new Coords()
                        {
                            X = new InvariantDecimal() { Value = item.CoordsX },
                            Y = new InvariantDecimal() { Value = item.CoordsY }
                        };
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion
    }
}
