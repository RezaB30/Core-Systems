using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RezaB.Data.Formating;

namespace RadiusR.SMS
{
    public static class SMSParamaterRepository
    {
        private static IEnumerable<SMSparameter> Parameters
        {
            get
            {
                return new SMSparameter[]
                {
                    new SMSStringParameter(SMSParameterNameCollection.ReferenceNo),
                    new SMSStringParameter(SMSParameterNameCollection.SubscriberNo),
                    new SMSStringParameter(SMSParameterNameCollection.Username),
                    new SMSStringParameter(SMSParameterNameCollection.Password),
                    new SMSStringParameter(SMSParameterNameCollection.RemainingDays),
                    new SMSStringParameter(SMSParameterNameCollection.OnlinePassword),
                    new SMSStringParameter(SMSParameterNameCollection.SubscriberName),
                    new SMSStringParameter(SMSParameterNameCollection.SubscriberServiceName),
                    new SMSAddressParameter(SMSParameterNameCollection.InstallationAddress),
                    new SMSAddressParameter(SMSParameterNameCollection.ResidenceAddress),
                    new SMSDateTimeparameter(SMSParameterNameCollection.ExpirationDate),
                    new SMSStringParameter(SMSParameterNameCollection.ContactPhoneNo),
                    new SMSUsageBytesParameter(SMSParameterNameCollection.RemainingQuota, new SMSType[] { }),
                    new SMSExactBytesParameter(SMSParameterNameCollection.LastQuotaTotal, new SMSType[] { SMSType.Quota80, SMSType.SmartQuota100, SMSType.SellQuota }),
                    new SMSPriceParameter(SMSParameterNameCollection.LastQuotaPrice, new SMSType[] { SMSType.SellQuota }),
                    new SMSExactBytesParameter(SMSParameterNameCollection.SmartQuotaUnit, new SMSType[] { SMSType.SmartQuota100 }),
                    new SMSPriceParameter(SMSParameterNameCollection.SmartQuotaUnitPrice, new SMSType[] { SMSType.SmartQuota100 }),
                    new SMSPriceParameter(SMSParameterNameCollection.SmartQuotaMaxPrice, new SMSType[] { SMSType.SmartQuotaMax }),
                    new SMSStringParameter(SMSParameterNameCollection.RateLimit, new SMSType[] { SMSType.SoftQuota100 }),
                    new SMSDateTimeparameter(SMSParameterNameCollection.BillIssueDate, new SMSType[] { SMSType.NewBill, SMSType.PaymentDone, SMSType.PaymentReminder, SMSType.CancelPayment, SMSType.FailedAutomaticPayment }),
                    new SMSPriceParameter(SMSParameterNameCollection.BillTotal, new SMSType[] { SMSType.NewBill, SMSType.PaymentDone, SMSType.PaymentReminder, SMSType.CancelPayment, SMSType.FailedAutomaticPayment }),
                    new SMSDateTimeparameter(SMSParameterNameCollection.LastPaymentDay, new SMSType[] { SMSType.NewBill, SMSType.PaymentDone, SMSType.PaymentReminder, SMSType.CancelPayment }),
                    new SMSPriceParameter(SMSParameterNameCollection.TotalCredit, new SMSType[] { SMSType.CreditAck }),
                    new SMSStringParameter(SMSParameterNameCollection.ExtendedMonths, new SMSType[] { SMSType.ExtendPackage }),
                    new SMSStringParameter(SMSParameterNameCollection.SMSCode, new SMSType[] { SMSType.ForgotPassword, SMSType.MobilExpressAddRemoveCard, SMSType.OperationCode}),
                    new SMSStringParameter(SMSParameterNameCollection.CardNo, new SMSType[] { SMSType.MobilExpressActivation }),
                    new SMSStringParameter(SMSParameterNameCollection.ErrorMessage, new SMSType[] { SMSType.FailedAutomaticPayment }),
                    new SMSStringParameter(SMSParameterNameCollection.SupportPIN, new SMSType[] { SMSType.SupportRequestInProgress, SMSType.SupportRequestResolved })
                };
            }
        }

        public static IEnumerable<SMSparameter> GetValidSMSParameters(SMSType? smsType)
        {
            return smsType.HasValue ? Parameters.Where(p => p.ValidIn == null || p.ValidIn.Contains(smsType.Value)) : Parameters.Where(p => p.ValidIn == null);
        }

        internal static string GetParameterFormattedValue(string parameterName, object value, string culture)
        {
            var parameter = Parameters.FirstOrDefault(p => p.Name == parameterName);
            if (parameter == null)
                return string.Empty;
            return parameter.formatValue(value, culture);
        }

        public abstract class SMSparameter
        {
            public string Name { get; private set; }

            public IEnumerable<SMSType> ValidIn { get; private set; }

            public abstract string formatValue(object value, string culture);

            public SMSparameter(string name, IEnumerable<SMSType> validIn = null)
            {
                Name = name;
                ValidIn = validIn;
            }
        }

        public class SMSStringParameter : SMSparameter
        {
            public override string formatValue(object value, string culture)
            {
                return value as string ?? string.Empty;
            }

            public SMSStringParameter(string name, IEnumerable<SMSType> validIn = null) : base(name, validIn) { }
        }

        public class SMSRateLimitParameter : SMSparameter
        {
            public override string formatValue(object value, string culture)
            {
                var castedValue = value as string;
                if (castedValue == null)
                    return string.Empty;

                var parsedRate = RateLimitParser.ParseString(castedValue);
                var suffix = "bps";
                switch (parsedRate.DownloadRateSuffix)
                {
                    case "M":
                        suffix = "Mbps";
                        break;
                    case "k":
                        suffix = "Kbps";
                        break;
                    default:
                        break;
                }
                return parsedRate.DownloadRate + suffix;
            }

            public SMSRateLimitParameter(string name, IEnumerable<SMSType> validIn = null) : base(name, validIn) { }
        }

        public class SMSAddressParameter : SMSparameter
        {
            private static Regex repeatetiveSpace = new Regex(@"(\s{2,}|\t|\n|\r)");

            public override string formatValue(object value, string culture)
            {
                var castedValue = value as string;
                if (castedValue == null)
                    return string.Empty;

                return repeatetiveSpace.Replace(castedValue, (Match) => " ");
            }

            public SMSAddressParameter(string name, IEnumerable<SMSType> validIn = null) : base(name, validIn) { }
        }

        public class SMSDateTimeparameter : SMSparameter
        {
            public override string formatValue(object value, string culture)
            {
                if (value == null || !(value is DateTime || value is DateTime?))
                    return string.Empty;
                var castedValue = (DateTime)value;
                return castedValue.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture(culture));
            }

            public SMSDateTimeparameter(string name, IEnumerable<SMSType> validIn = null) : base(name, validIn) { }
        }

        public class SMSPriceParameter : SMSparameter
        {
            public override string formatValue(object value, string culture)
            {
                if (value == null || !(value is decimal))
                    return string.Empty;
                var castedValue = (decimal)value;
                return castedValue.ToString("###,##0.00", CultureInfo.CreateSpecificCulture(culture));
            }

            public SMSPriceParameter(string name, IEnumerable<SMSType> validIn = null) : base(name, validIn) { }
        }

        public class SMSUsageBytesParameter : SMSparameter
        {
            public override string formatValue(object value, string culture)
            {
                if (value == null || !(value is long))
                    return string.Empty;
                var castedValue = (long)value;
                return RateLimitFormatter.ToDecimalTrafficStandard(castedValue, culture);
            }

            public SMSUsageBytesParameter(string name, IEnumerable<SMSType> validIn = null) : base(name, validIn) { }
        }

        public class SMSExactBytesParameter : SMSparameter
        {
            public override string formatValue(object value, string culture)
            {
                if (value == null || !(value is long))
                    return string.Empty;
                var castedValue = (long)value;
                return RateLimitFormatter.ToTrafficStandard(castedValue, false);
            }

            public SMSExactBytesParameter(string name, IEnumerable<SMSType> validIn = null) : base(name, validIn) { }
        }

        public static class SMSParameterNameCollection
        {
            public const string SubscriberNo = "([subscriberNo])";
            public const string ReferenceNo = "([referenceNo])";
            public const string Username = "([username])";
            public const string Password = "([password])";
            public const string RemainingDays = "([remainingDays])";
            public const string OnlinePassword = "([onlinePassword])";
            public const string SubscriberName = "([subscriberName])";
            public const string SubscriberServiceName = "([subscriberServiceName])";
            public const string InstallationAddress = "([installationAddress])";
            public const string ResidenceAddress = "([residenceAddress])";
            public const string ExpirationDate = "([expirationDate])";
            public const string ContactPhoneNo = "([contactPhoneNo])";
            public const string RemainingQuota = "([remainingQuota])";
            public const string LastQuotaTotal = "([lastQuotaTotal])";
            public const string LastQuotaPrice = "([lastQuotaPrice])";
            public const string SmartQuotaUnit = "([smartQuotaUnit])";
            public const string SmartQuotaUnitPrice = "([smartQuotaUnitPrice])";
            public const string SmartQuotaMaxPrice = "([smartQuotaMaxPrice])";
            public const string RateLimit = "([rateLimit])";
            public const string BillIssueDate = "([billIssueDate])";
            public const string BillTotal = "([billTotal])";
            public const string LastPaymentDay = "([lastPaymentDay])";
            public const string TotalCredit = "([totalCredit])";
            public const string ExtendedMonths = "([extendedMonths])";
            public const string SMSCode = "([smsCode])";
            public const string CardNo = "([cardNo])";
            public const string ErrorMessage = "([errorMessage])";
            public const string SupportPIN = "([supportPIN])";
        }
    }
}
