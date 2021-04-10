using RezaB.TurkTelekom.WebServices;
using RezaB.TurkTelekom.WebServices.TTApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.TelekomOperations
{
    public static class TelekomRegistrationTicketFactory
    {
        public static TelekomRegistrationTicket CreateRegistrationTicket(Customer dbCustomer, Subscription dbSubsciption)
        {
            var selectedDomain = DomainsCache.DomainsCache.GetDomainByID(dbSubsciption.DomainID);
            if (selectedDomain == null)
            {
                return null;
            }
            var selectedTelekomTariff = DomainsCache.TelekomTariffsCache.GetSpecificTariff(selectedDomain, dbSubsciption.SubscriptionTelekomInfo.PacketCode.Value, dbSubsciption.SubscriptionTelekomInfo.TariffCode.Value);
            if (selectedTelekomTariff == null)
            {
                return null;
            }

            return new TelekomRegistrationTicket()
            {
                AddressInfo = new TelekomRegistrationTicket.RegistrationAddressInfo()
                {
                    AddressText = dbSubsciption.Address.AddressText,
                    ApartmentID = dbSubsciption.Address.ApartmentID,
                    BuildingID = dbSubsciption.Address.DoorID,
                    BuildingNo = dbSubsciption.Address.DoorNo,
                    DistrictID = (int)dbSubsciption.Address.DistrictID,
                    DoorNo = dbSubsciption.Address.ApartmentNo,
                    FloorNo = dbSubsciption.Address.Floor,
                    NeighbourhoodName = dbSubsciption.Address.NeighborhoodName,
                    PostalCode = dbSubsciption.Address.PostalCode.ToString("00000"),
                    ProvinceID = (int)dbSubsciption.Address.ProvinceID,
                    StreetName = dbSubsciption.Address.StreetName
                },
                ConnectionInfo = new TelekomRegistrationTicket.RegistrationConnectionInfo()
                {
                    DomainName = selectedDomain.Name,
                    ISPCode = selectedDomain.TelekomCredential.OLOPortalCustomerCodeInt,
                    Password = dbSubsciption.RadiusAuthorization.Password,
                    Username = dbSubsciption.RadiusAuthorization.Username.Split('@').FirstOrDefault()
                },
                TariffInfo = new TelekomRegistrationTicket.RegistrationTariffInfo()
                {
                    ApplicationType = (ApplicationType)selectedDomain.AccessMethod.Value,
                    PacketCode = dbSubsciption.SubscriptionTelekomInfo.PacketCode.Value,
                    TariffCode = dbSubsciption.SubscriptionTelekomInfo.TariffCode.Value,
                    SpeedCode = selectedTelekomTariff.SpeedCode,
                    XDSLType = (XDSLType)dbSubsciption.SubscriptionTelekomInfo.XDSLType
                },
                PersonalInfo = new TelekomRegistrationTicket.RegistrationPersonalInfo()
                {
                    FirstName = dbCustomer.FirstName,
                    LastName = dbCustomer.LastName,
                    PhoneNo = dbCustomer.ContactPhoneNo
                },
                PSTNNo = dbSubsciption.SubscriptionTelekomInfo.PSTN,
                HandicapTCK = dbSubsciption.SubscriptionTelekomInfo.IsPaperWorkNeeded == true ? dbCustomer.CustomerIDCard.TCKNo : null
            };
        }
    }
}
