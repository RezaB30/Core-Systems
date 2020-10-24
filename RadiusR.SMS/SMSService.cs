using NLog;
using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR.SMS.NetGsm;
using RadiusR.SMS.Verimor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RadiusR.SMS
{
    public class SMSService
    {
        private static Logger logger = LogManager.GetLogger("SMSInternal");

        /// <summary>
        /// Send SMS with parameters.
        /// </summary>
        /// <param name="subscriber">The subscriber that would recieve the SMS.</param>
        /// <param name="type">Type of SMS. (null for plain text SMS)</param>
        /// <param name="parameters">Parameters used in SMS.</param>
        /// <param name="billId">Bill id if it is a bill related SMS.</param>
        /// <param name="rawText">SMS text if SMS type is null to be used instead.</param>
        /// <returns></returns>
        public SMSArchive SendSubscriberSMS(Subscription subscriber, SMSType? type = null, IDictionary<string, object> parameters = null, long? billId = null, string rawText = null)
        {
            var parameteredString = type.HasValue ? SMSClipBoard.GetSMSText(type.Value, subscriber.Customer.Culture) : rawText;
            var createdMessage = ParameterManager.FillParameters(subscriber, parameteredString, parameters);
            return Send(subscriber, createdMessage, billId, type);
        }
        /// <summary>
        /// Send SMS with parameters.
        /// </summary>
        /// <param name="phoneNo">Sent phone no.</param>
        /// <param name="culture">Language of the SMS.</param>
        /// <param name="type">Type of SMS. (null for plain text SMS)</param>
        /// <param name="parameters">Parameters used in SMS.</param>
        /// <param name="billId">Bill id if it is a bill related SMS.</param>
        /// <param name="rawText">SMS text if SMS type is null to be used instead.</param>
        public void SendGenericSMS(string phoneNo, string culture, SMSType? type = null, IDictionary<string, object> parameters = null, long? billId = null, string rawText = null)
        {
            var parameteredString = type.HasValue ? SMSClipBoard.GetSMSText(type.Value, culture) : rawText;
            var createdMessage = ParameterManager.FillParameters(null, parameteredString, parameters, culture);
            Execute(phoneNo, createdMessage);
        }

        protected virtual SMSArchive Send(Subscription subscription, string message, long? billId = null, SMSType? type = null)
        {
            var isSent = Execute(subscription.Customer.ContactPhoneNo, message, billId, type);
            if (!isSent)
                return null;
            return new SMSArchive()
            {
                SubscriptionID = subscription.ID,
                BillID = billId,
                SMSTypeID = (short?)type,
                Date = DateTime.Now,
                Text = message
            };
        }

        protected static bool Execute(string phoneNo, string message, long? billId = null, SMSType? type = null)
        {
            try
            {
                if (SettingsCache.IsActive && (!type.HasValue || SettingsCache.ActiveTypes.ContainsKey((short)type.Value)))
                {
                    switch (SettingsCache.APIType)
                    {
                        case (short)SMSAPITypes.NetGSM:
                            smsnnClient netgsmClient = new smsnnClient();
                            netgsmClient.sms_gonder_1n(SettingsCache.ServiceUsername, SettingsCache.ServicePassword, SettingsCache.ServiceTitle, SettingsCache.ServiceTitle, message, new[] { phoneNo }, "TR", "", "", "");
                            return true;
                        case (short)SMSAPITypes.Verimor:
                            VerimorSMSClient verimorClient = new VerimorSMSClient();
                            verimorClient.SendSMS(SettingsCache.ServiceUsername, SettingsCache.ServicePassword, SettingsCache.ServiceTitle, message, new[] { phoneNo });
                            return true;
                        default:
                            throw new Exception("Invalid APIType " + SettingsCache.APIType);
                    }
                }
            }
            catch (Exception ex)
            {
                // log error
                logger.Error(ex, string.Format("Error sending \"{0}\" to ({1})", message ?? "", phoneNo == null ? "" : string.Join(",", new[] { phoneNo })));
            }

            return false;
        }

        public class Settings
        {
            public TimeSpan CachingLength { get; internal set; }

            public string ServiceUsername { get; internal set; }

            public string ServicePassword { get; internal set; }

            public string ServiceTitle { get; internal set; }

            public bool IsActive { get; internal set; }

            public short APIType { get; internal set; }
        }

        public static Settings GetSettings()
        {
            var results = new Settings()
            {
                CachingLength = SettingsCache.CachingLength,
                IsActive = SettingsCache.IsActive,
                ServicePassword = SettingsCache.ServicePassword,
                ServiceUsername = SettingsCache.ServiceUsername,
                ServiceTitle = SettingsCache.ServiceTitle,
                APIType = SettingsCache.APIType
            };

            return results;
        }

        public static void ReloadSettings()
        {
            SettingsCache.Reload();
        }
    }
}
