using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR_Manager.Models.RadiusViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.Extentions
{
    public static class IQueryableSubscriptions
    {
        public static IQueryable<Subscription> FilterBySearchViewModel(this IQueryable<Subscription> query, ViewModels.CustomerSearchViewModel searchModel, RadiusREntities db, IPrincipal user)
        {
            // apply searchModel to sql rows
            if (searchModel.DisabledForDebt)
            {
                var disconnectionTimeOfDay = TimeSpan.ParseExact(db.RadiusDefaults.FirstOrDefault(def => def.Attribute == "DailyDisconnectionTime").Value, "hh\\:mm\\:ss", CultureInfo.InvariantCulture);
                query = query.Where(client => client.State == (short)CustomerState.Active && DbFunctions.AddSeconds(client.LastAllowedDate, (int)disconnectionTimeOfDay.TotalSeconds) < DateTime.Now);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Name))
            {
                query = query.Where(c => c.Customer.CustomerType == (short)CustomerType.Individual && (c.Customer.FirstName.ToLower() + " " + c.Customer.LastName.ToLower()).Contains(searchModel.Name.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Phone))
            {
                query = query.Where(c => c.Customer.ContactPhoneNo.Contains(searchModel.Phone));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.TCKNo))
            {
                query = query.Where(c => c.Customer.CustomerIDCard.TCKNo.Contains(searchModel.TCKNo));
            }
            if (searchModel.State != 0)
            {
                query = query.Where(c => c.State == searchModel.State);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.Username))
            {
                query = query.Where(c => c.Username.Contains(searchModel.Username.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.SubscriberNo))
            {
                query = query.Where(c => c.SubscriberNo.Contains(searchModel.SubscriberNo.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.TelekomSubscriberNo))
            {
                query = query.Where(c => c.SubscriptionTelekomInfo != null && c.SubscriptionTelekomInfo.SubscriptionNo.Contains(searchModel.TelekomSubscriberNo.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.PSTNNo))
            {
                query = query.Where(c => c.SubscriptionTelekomInfo != null && c.SubscriptionTelekomInfo.PSTN.Contains(searchModel.PSTNNo));
            }
            if (searchModel.HasStaticIP)
            {
                query = query.Where(c => !string.IsNullOrEmpty(c.StaticIP));
            }
            if (!string.IsNullOrWhiteSpace(searchModel.ServiceName))
            {
                query = query.Where(c => c.Service.Name == searchModel.ServiceName);
            }
            if (searchModel.Address != null && searchModel.Address.ProvinceID != 0)
            {
                query = query.FilterBySetupAddress(searchModel.Address);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.CompanyTitle))
            {
                query = query.Where(c => c.Customer.CustomerType != (short)CustomerType.Individual && c.Customer.CorporateCustomerInfo.Title.ToLower().Contains(searchModel.CompanyTitle.ToLower()));
            }
            if (searchModel.CustomerType > 0)
            {
                query = query.Where(c => c.Customer.CustomerType == searchModel.CustomerType);
            }
            if (!string.IsNullOrWhiteSpace(searchModel.ValidDisplayName))
            {
                query = query.Where(c => c.Customer.CustomerType != (short)CustomerType.Individual && c.Customer.CorporateCustomerInfo.Title.ToLower().Contains(searchModel.ValidDisplayName.ToLower()) || c.Customer.CustomerType == (short)CustomerType.Individual && (c.Customer.FirstName.ToLower() + " " + c.Customer.LastName.ToLower()).Contains(searchModel.ValidDisplayName.ToLower()));
            }
            if (searchModel.ActivationDateStart.HasValue)
            {
                query = query.Where(c => DbFunctions.TruncateTime(c.ActivationDate) >= searchModel.ActivationDateStart);
            }
            if (searchModel.ActivationDateEnd.HasValue)
            {
                query = query.Where(c => DbFunctions.TruncateTime(c.ActivationDate) <= searchModel.ActivationDateEnd);
            }
            if (searchModel.RegistrationDateStart.HasValue)
            {
                query = query.Where(c => DbFunctions.TruncateTime(c.MembershipDate) >= searchModel.RegistrationDateStart);
            }
            if (searchModel.RegistrationDateEnd.HasValue)
            {
                query = query.Where(c => DbFunctions.TruncateTime(c.MembershipDate) <= searchModel.RegistrationDateEnd);
            }
            if (searchModel.GroupID.HasValue)
            {
                query = query.Where(c => c.Groups.Any(g => g.ID == searchModel.GroupID));
            }
            if (searchModel.BillingPeriod.HasValue)
            {
                query = query.Where(c => c.PaymentDay == searchModel.BillingPeriod);
            }

            return query;
        }

        public static IQueryable<Subscription> FilterBySetupAddress(this IQueryable<Subscription> query, AddressViewModel address)
        {
            // parallel parameters
            if (!string.IsNullOrWhiteSpace(address.Floor))
            {
                query = query.Where(c => c.Address.Floor == address.Floor);
            }
            if (address.PostalCode.HasValue)
            {
                query = query.Where(c => c.Address.PostalCode == address.PostalCode);
            }
            // serial parameters
            if (address.ApartmentID != 0)
            {
                return query.Where(c => c.Address.ApartmentID == address.ApartmentID);
            }
            if (address.DoorID != 0)
            {
                return query.Where(c => c.Address.DoorID == address.DoorID);
            }
            if (address.StreetID != 0)
            {
                return query.Where(c => c.Address.StreetID == address.StreetID);
            }
            if (address.NeighbourhoodID != 0)
            {
                return query.Where(c => c.Address.NeighborhoodID == address.NeighbourhoodID);
            }
            if (address.RuralCode != 0)
            {
                return query.Where(c => c.Address.RuralCode == address.RuralCode);
            }
            if (address.DistrictID != 0)
            {
                return query.Where(c => c.Address.DistrictID == address.DistrictID);
            }
            if (address.ProvinceID != 0)
            {
                return query.Where(c => c.Address.ProvinceID == address.ProvinceID);
            }

            return query;
        }

        public static IQueryable<TelekomWorkOrder> FilterBySetupAddress(this IQueryable<TelekomWorkOrder> query, AddressViewModel address)
        {
            // parallel parameters
            if (!string.IsNullOrWhiteSpace(address.Floor))
            {
                query = query.Where(c => c.Subscription.Address.Floor == address.Floor);
            }
            if (address.PostalCode.HasValue)
            {
                query = query.Where(c => c.Subscription.Address.PostalCode == address.PostalCode);
            }
            // serial parameters
            if (address.ApartmentID != 0)
            {
                return query.Where(c => c.Subscription.Address.ApartmentID == address.ApartmentID);
            }
            if (address.DoorID != 0)
            {
                return query.Where(c => c.Subscription.Address.DoorID == address.DoorID);
            }
            if (address.StreetID != 0)
            {
                return query.Where(c => c.Subscription.Address.StreetID == address.StreetID);
            }
            if (address.NeighbourhoodID != 0)
            {
                return query.Where(c => c.Subscription.Address.NeighborhoodID == address.NeighbourhoodID);
            }
            if (address.RuralCode != 0)
            {
                return query.Where(c => c.Subscription.Address.RuralCode == address.RuralCode);
            }
            if (address.DistrictID != 0)
            {
                return query.Where(c => c.Subscription.Address.DistrictID == address.DistrictID);
            }
            if (address.ProvinceID != 0)
            {
                return query.Where(c => c.Subscription.Address.ProvinceID == address.ProvinceID);
            }

            return query;
        }
    }
}
