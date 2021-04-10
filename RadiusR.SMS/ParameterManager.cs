using RadiusR.DB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RadiusR.SMS
{
    static class ParameterManager
    {
        private static Regex parameterRegex = new Regex(@"(\(\[.+?\]\))");

        public static string FillParameters(Subscription subscription, string parameteredString, IDictionary<string, object> parameters, string culture = null)
        {
            var fullParameters = MergeCommonParameters(subscription, parameters, subscription == null ? culture : null);
            var results = parameteredString;
            var collection = parameterRegex.Matches(parameteredString);
            foreach (Match item in collection)
            {
                if (fullParameters.ContainsKey(item.Value))
                    results = results.Replace(item.Value, fullParameters[item.Value]);
                else
                    results = results.Replace(item.Value, string.Empty);
            }
            return results;
        }

        private static IDictionary<string, string> MergeCommonParameters(Subscription subscription, IDictionary<string, object> extendedParameters, string culture = null)
        {
            var results = new Dictionary<string, string>();
            if (subscription != null)
                results = new Dictionary<string, string>()
                {
                    { SMSParamaterRepository.SMSParameterNameCollection.SubscriberNo, SMSParamaterRepository.GetParameterFormattedValue(SMSParamaterRepository.SMSParameterNameCollection.SubscriberNo, subscription.SubscriberNo, subscription.Customer.Culture) },
                    { SMSParamaterRepository.SMSParameterNameCollection.ReferenceNo, SMSParamaterRepository.GetParameterFormattedValue(SMSParamaterRepository.SMSParameterNameCollection.ReferenceNo, subscription.ReferenceNo, subscription.Customer.Culture) },
                    { SMSParamaterRepository.SMSParameterNameCollection.Username, SMSParamaterRepository.GetParameterFormattedValue(SMSParamaterRepository.SMSParameterNameCollection.Username, subscription.RadiusAuthorization.Username, subscription.Customer.Culture) },
                    { SMSParamaterRepository.SMSParameterNameCollection.Password, SMSParamaterRepository.GetParameterFormattedValue(SMSParamaterRepository.SMSParameterNameCollection.Password, subscription.RadiusAuthorization.Password, subscription.Customer.Culture) },
                    { SMSParamaterRepository.SMSParameterNameCollection.RemainingDays, SMSParamaterRepository.GetParameterFormattedValue(SMSParamaterRepository.SMSParameterNameCollection.RemainingDays, subscription.DaysRemaining, subscription.Customer.Culture) },
                    { SMSParamaterRepository.SMSParameterNameCollection.OnlinePassword, SMSParamaterRepository.GetParameterFormattedValue(SMSParamaterRepository.SMSParameterNameCollection.OnlinePassword, subscription.OnlinePassword, subscription.Customer.Culture) },
                    { SMSParamaterRepository.SMSParameterNameCollection.SubscriberName, SMSParamaterRepository.GetParameterFormattedValue(SMSParamaterRepository.SMSParameterNameCollection.SubscriberName, subscription.ValidDisplayName, subscription.Customer.Culture) },
                    { SMSParamaterRepository.SMSParameterNameCollection.SubscriberServiceName, SMSParamaterRepository.GetParameterFormattedValue(SMSParamaterRepository.SMSParameterNameCollection.SubscriberServiceName, subscription.Service.Name, subscription.Customer.Culture) },
                    { SMSParamaterRepository.SMSParameterNameCollection.InstallationAddress, SMSParamaterRepository.GetParameterFormattedValue(SMSParamaterRepository.SMSParameterNameCollection.InstallationAddress, subscription.Address.AddressText, subscription.Customer.Culture) },
                    { SMSParamaterRepository.SMSParameterNameCollection.ResidenceAddress, SMSParamaterRepository.GetParameterFormattedValue(SMSParamaterRepository.SMSParameterNameCollection.ResidenceAddress, subscription.Customer.Address.AddressText, subscription.Customer.Culture) },
                    { SMSParamaterRepository.SMSParameterNameCollection.ContactPhoneNo, SMSParamaterRepository.GetParameterFormattedValue(SMSParamaterRepository.SMSParameterNameCollection.ContactPhoneNo, subscription.Customer.ContactPhoneNo, subscription.Customer.Culture) },
                    { SMSParamaterRepository.SMSParameterNameCollection.ExpirationDate, SMSParamaterRepository.GetParameterFormattedValue(SMSParamaterRepository.SMSParameterNameCollection.ExpirationDate, subscription.RadiusAuthorization.ExpirationDate, subscription.Customer.Culture) }
                };

            if (extendedParameters != null)
            {
                foreach (var item in extendedParameters)
                {
                    if (results.ContainsKey(item.Key))
                        results[item.Key] = SMSParamaterRepository.GetParameterFormattedValue(item.Key, item.Value, culture != null ? culture : subscription.Customer.Culture);
                    else
                        results.Add(item.Key, SMSParamaterRepository.GetParameterFormattedValue(item.Key, item.Value, culture != null ? culture : subscription.Customer.Culture));
                }
            }
            return results;
        }
    }
}
