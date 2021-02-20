using RadiusR.DB.DomainsCache;
using RadiusR.DB.Enums.RecurringDiscount;
using RadiusR_Manager.Models.RadiusViewModels;
using RadiusR_Manager.Models.ViewModels;
using RadiusR_Manager.Models.ViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RadiusR_Manager.Controllers
{
    public partial class ClientController
    {
        private void FixSubscriptionAddedFeesModelState(string prefix, IEnumerable<SubscriberFeesAddViewModel> model, IEnumerable<SubscriberFeesAddViewModel> available)
        {
            prefix = prefix ?? string.Empty;

            for (int i = 0; i < model.Count(); i++)
            {
                var currentValue = model.ToArray()[i];
                if (currentValue.FeeTypeID.HasValue)
                {
                    var currentSample = available.FirstOrDefault(af => af.FeeTypeID == currentValue.FeeTypeID);
                    if (currentSample != null)
                    {
                        if (currentSample.Variants == null)
                        {
                            ModelState.Remove(prefix + "[" + i + "].SelectedVariantID");
                        }
                        if (currentSample.IsAllTime || currentSample.CustomFees != null)
                        {
                            ModelState.Remove(prefix + "[" + i + "].InstallmentCount");
                        }
                        if (currentSample.IsAllTime && currentSample.CustomFees != null && currentValue.CustomFees != null)
                        {
                            for (int j = 0; j < currentValue.CustomFees.Count(); j++)
                            {
                                ModelState.Remove(prefix + "[" + i + "].CustomFees[" + j + "].Installment");
                            }
                        }
                    }
                }
            }
        }

        private void FixCustomerAddressesModelState(CustomerRegistrationViewModel model)
        {
            if (model.GeneralInfo.BillingSameAsSetupAddress == true)
            {
                var toRemoveKeys = ModelState.Keys.Where(k => k.StartsWith("GeneralInfo.BillingAddress")).ToArray();
                foreach (var key in toRemoveKeys)
                {
                    ModelState.Remove(key);
                }
            }
            if (model.IndividualInfo.ResidencySameAsSetupAddress == true)
            {
                var toRemoveKeys = ModelState.Keys.Where(k => k.StartsWith("IndividualInfo.ResidencyAddress")).ToArray();
                foreach (var key in toRemoveKeys)
                {
                    ModelState.Remove(key);
                }
            }
            if (model.CorporateInfo.ExecutiveResidencySameAsSetupAddress == true)
            {
                var toRemoveKeys = ModelState.Keys.Where(k => k.StartsWith("CorporateInfo.ExecutiveResidencyAddress")).ToArray();
                foreach (var key in toRemoveKeys)
                {
                    ModelState.Remove(key);
                }
            }
            if (model.CorporateInfo.CompanySameAsSetupAddress == true)
            {
                var toRemoveKeys = ModelState.Keys.Where(k => k.StartsWith("CorporateInfo.CompanyAddress")).ToArray();
                foreach (var key in toRemoveKeys)
                {
                    ModelState.Remove(key);
                }
            }
        }

        private void FixCustomerTypeModelState(CustomerRegistrationViewModel model)
        {
            if (model.GeneralInfo.CustomerType == (short)RadiusR.DB.Enums.CustomerType.Individual)
            {
                var toRemoveKeys = ModelState.Keys.Where(k => k.StartsWith("CorporateInfo.")).ToArray();
                foreach (var key in toRemoveKeys)
                {
                    ModelState.Remove(key);
                }
            }
            else if (model.GeneralInfo.CustomerType == (short)RadiusR.DB.Enums.CustomerType.LegalCompany || model.GeneralInfo.CustomerType == (short)RadiusR.DB.Enums.CustomerType.PrivateCompany || model.GeneralInfo.CustomerType == (short)RadiusR.DB.Enums.CustomerType.PublicCompany)
            {
                var toRemoveKeys = ModelState.Keys.Where(k => k.StartsWith("IndividualInfo.")).ToArray();
                foreach (var key in toRemoveKeys)
                {
                    ModelState.Remove(key);
                }
            }
        }

        private void FixCustomerSubscriptionModelState(string prefix, CustomerSubscriptionViewModel model)
        {
            prefix = string.IsNullOrEmpty(prefix) ? string.Empty : prefix + ".";

            // non-transition registration type
            if (model.RegistrationType != (short)RadiusR.DB.Enums.SubscriptionRegistrationType.Transition)
            {
                model.TransitionXDSLNo = null;
                ModelState.Remove(prefix + "TransitionXDSLNo");
            }
            // non-transfer registration type
            if (model.RegistrationType != (short)RadiusR.DB.Enums.SubscriptionRegistrationType.Transfer)
            {
                model.TransferringSubscriptionID = null;
                model.TransferringSubscriptionNo = null;
                ModelState.Remove(prefix + "TransferringSubscriptionNo");
            }
            // non-new registeration registration type
            if (model.RegistrationType != (short)RadiusR.DB.Enums.SubscriptionRegistrationType.NewRegistration)
            {
                model.TelekomDetailedInfo = null;
                var toRemoveKeys = ModelState.Keys.Where(k => k.StartsWith(prefix + "TelekomDetailedInfo")).ToArray();
                foreach (var key in toRemoveKeys)
                {
                    ModelState.Remove(key);
                }
            }

            if (model.DomainID > 0)
            {
                // TT Packet
                var cachedDomain = DomainsCache.GetDomainByID(model.DomainID);
                if (cachedDomain != null && (cachedDomain.TelekomCredential == null || (model.TelekomDetailedInfo != null && !string.IsNullOrEmpty(model.TelekomDetailedInfo.SubscriberNo))))
                {
                    var toRemoveKeys = ModelState.Keys.Where(k => k.StartsWith(prefix + "TelekomDetailedInfo.TelekomTariffInfo")).ToArray();
                    foreach (var key in toRemoveKeys)
                    {
                        ModelState.Remove(key);
                    }
                }
                // Billing Period
                if (model.ServiceID > 0)
                {
                    var dbService = db.Services.Find(model.ServiceID);
                    if (dbService != null && dbService.BillingType == (short)RadiusR.DB.Enums.ServiceBillingType.PrePaid)
                    {
                        model.BillingPeriod = 1;
                        ModelState.Remove(prefix + "BillingPeriod");
                    }
                }
            }
            if (model.CommitmentInfo == null || (!model.CommitmentInfo.CommitmentLength.HasValue && !model.CommitmentInfo.CommitmentExpirationDate.HasValue))
            {
                var toRemoveKeys = ModelState.Keys.Where(k => k.StartsWith(prefix + "Commitment")).ToArray();
                foreach (var key in toRemoveKeys)
                {
                    ModelState.Remove(key);
                }
            }
            if (model.ReferralDiscount == null || string.IsNullOrEmpty(model.ReferralDiscount.ReferenceNo))
            {
                var toRemoveKeys = ModelState.Keys.Where(k => k.StartsWith(prefix + "ReferralDiscount")).ToArray();
                foreach (var key in toRemoveKeys)
                {
                    ModelState.Remove(key);
                }
            }
        }

        private void FixRecurringDiscountModelState(string prefix, RecurringDiscountViewModel model)
        {
            FixRecurringDiscountModelState(ModelState, prefix, model);
        }

        public static void FixRecurringDiscountModelState(System.Web.Mvc.ModelStateDictionary modelState , string prefix, RecurringDiscountViewModel model)
        {
            if (model.DiscountType == (short)RecurringDiscountType.Static)
                modelState.Remove(prefix + "PercentageAmount");
            else if (model.DiscountType == (short)RecurringDiscountType.Percentage)
                modelState.Remove(prefix + "Amount");
            if (model.ApplicationType == (short)RecurringDiscountApplicationType.BillBased)
            {
                model.FeeTypeID = null;
                modelState.Remove(prefix + "FeeTypeID");
            }
            // bill base discounts can only be of percentage type
            if (model.ApplicationType == (short)RecurringDiscountApplicationType.BillBased && model.DiscountType == (short)RecurringDiscountType.Static)
                modelState.AddModelError(prefix + "DiscountType", RadiusR.Localization.Validation.ModelSpecific.StaticDiscountNotValidForBillBaseDiscounts);
        }
    }
}