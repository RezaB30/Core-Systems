using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using RadiusR.DB;
using RadiusR.DB.Enums;

namespace RadiusR.SMS
{
    /// <summary>
    /// Provides repository for sms texts.
    /// </summary>
    internal static class SMSClipBoard
    {
        /// <summary>
        /// Retrieves SMS text.
        /// </summary>
        /// <param name="type">Type of SMS.</param>
        /// <param name="culture">Culture of text.</param>
        /// <returns></returns>
        internal static string GetSMSText(SMSType type, string culture)
        {
            return Retrieve(type, culture);
        }
        ///// <summary>
        ///// Gets new bill SMS text.
        ///// </summary>
        ///// <param name="culture">SMS language</param>
        ///// <returns>SMS text</returns>
        //internal static string NewBillText(string culture)
        //{
        //    return Retrieve(SMSType.NewBill, culture);
        //}
        ///// <summary>
        ///// Gets bill reminder SMS text.
        ///// </summary>
        ///// <param name="culture">SMS language</param>
        ///// <returns>SMS text</returns>
        //internal static string BillReminderText(string culture)
        //{
        //    return Retrieve(SMSType.PaymentReminder, culture);
        //}
        ///// <summary>
        ///// Gets SMS text for FUP reaching 80%.
        ///// </summary>
        ///// <param name="culture">SMS language</param>
        ///// <returns>SMS text</returns>
        //internal static string FUP80Text(string culture)
        //{
        //    //return Retrieve(SMSType.FUP80Percent, culture);
        //    return null;
        //}
        ///// <summary>
        ///// Gets SMS text for FUP reaching 100%.
        ///// </summary>
        ///// <param name="culture">SMS language</param>
        ///// <returns>SMS text</returns>
        //internal static string FUP100Text(string culture)
        //{
        //    //return Retrieve(SMSType.FUP100Percent, culture);
        //    return null;
        //}
        ///// <summary>
        ///// Gets SMS text for client credentials.
        ///// </summary>
        ///// <param name="culture">SMS language</param>
        ///// <returns>SMS text</returns>
        //internal static string ActivationText(string culture)
        //{
        //    return Retrieve(SMSType.Activation, culture);
        //}
        ///// <summary>
        ///// Gets payment acknowledgement SMS text.
        ///// </summary>
        ///// <param name="culture">SMS language</param>
        ///// <returns>SMS text</returns>
        //internal static string PaymentAckText(string culture)
        //{
        //    return Retrieve(SMSType.PaymentDone, culture);
        //}
        ///// <summary>
        ///// Gets client internet service credentials SMS text.
        ///// </summary>
        ///// <param name="culture">SMS language</param>
        ///// <returns>SMS text</returns>
        //internal static string ClientCredentialsText(string culture)
        //{
        //    return Retrieve(SMSType.UserCredentials, culture);
        //}
        ///// <summary>
        ///// Gets client payment website credentials.
        ///// </summary>
        ///// <param name="culture">SMS language</param>
        ///// <returns>SMS text</returns>
        //internal static string WebsiteCredentialsText(string culture)
        //{
        //    return Retrieve(SMSType.WebsiteCredentials, culture);
        //}
        ///// <summary>
        ///// Gets credit acknowledgement SMS text.
        ///// </summary>
        ///// <param name="culture">SMS language</param>
        ///// <returns>SMS text</returns>
        //internal static string CreditAckText(string culture)
        //{
        //    return Retrieve(SMSType.CreditAck, culture);
        //}
        ///// <summary>
        ///// Gets SMS text for disconnection because of debt.
        ///// </summary>
        ///// <param name="culture">SMS language</param>
        ///// <returns>SMS text</returns>
        //internal static string DebtDisconnectionText(string culture)
        //{
        //    return Retrieve(SMSType.DebtDisconnection, culture);
        //}
        ///// <summary>
        ///// Gets SMS text for prepaid expiration reminder.
        ///// </summary>
        ///// <param name="culture">SMS language</param>
        ///// <returns>SMS text</returns>
        //internal static string PrePaidExpirationText(string culture)
        //{
        //    return Retrieve(SMSType.PrePaidExpiration, culture);
        //}
        ///// <summary>
        ///// Gets SMS text for extending prepaid package.
        ///// </summary>
        ///// <param name="culture">SMS language</param>
        ///// <returns>SMS text</returns>
        //internal static string ExtendPackageText(string culture)
        //{
        //    return Retrieve(SMSType.ExtendPackage, culture);
        //}
        ///// <summary>
        ///// Gets SMS text for forgot password.
        ///// </summary>
        ///// <param name="culture"></param>
        ///// <returns></returns>
        //internal static string ForgotPasswordText(string culture)
        //{
        //    return Retrieve(SMSType.ForgotPassword, culture);
        //}

        private static MemoryCache cache = new MemoryCache("SMSClipboardCash");

        private static CacheItemPolicy policy = new CacheItemPolicy()
        {
            Priority = CacheItemPriority.Default,
            SlidingExpiration = SettingsCache.CachingLength
        };

        private static string Retrieve(SMSType type, string culture)
        {
            var result = cache.Get(type.ToString() + "-" + culture) as string;
            if (!string.IsNullOrEmpty(result))
                return result;
            using (RadiusREntities entities = new RadiusREntities())
            {
                result = entities.SMSTexts.FirstOrDefault(text => text.Culture == culture && text.TypeID == (short)type).Text;
                cache.Set(type.ToString() + "-" + culture, result, policy);
                return result;
            }
        }
    }
}
