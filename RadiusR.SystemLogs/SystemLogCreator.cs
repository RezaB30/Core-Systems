using RadiusR.DB;
using RadiusR.DB.Enums;
using RadiusR.DB.Enums.RecurringDiscount;
using RadiusR.SystemLogs.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RadiusR.SystemLogs
{
    public partial class SystemLogProcessor
    {
        public static SystemLog AddAdditionalFees(IEnumerable<long> feeIDs, int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.AddAdditionalFee,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = string.Join(",", feeIDs.Select(f => f.ToString()))
            };
        }

        public static SystemLog RemoveAdditionalFee(long feeID, int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.RemoveAdditionalFee,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = feeID.ToString()
            };
        }

        public static SystemLog CancelAdditionalFee(long feeID, int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.CancelAdditionalFee,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = feeID.ToString()
            };
        }

        public static SystemLog BillPayment(IEnumerable<long> billIDs, int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, PaymentType paymentType, string paymentUser = null)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.BillPayment,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] { string.Join(",", billIDs.Select(b => "<a href='" + InsertLink("Details", "Bill", new { id = b.ToString() }) + "'>" + b.ToString() + "</a>")), "<span class='important'>" + InsertResource(typeof(RadiusR.Localization.Lists.PaymentType), paymentType.ToString()) + "</span>", paymentUser != null ? "<span class='important'>" + ClearString(paymentUser) + "</span>" : null }))
            };
        }

        public static SystemLog CancelPayment(IEnumerable<long> billIDs, int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, string paymentUser = null)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.CancelPayment,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] { string.Join(",", billIDs.Select(b => "<a href='" + InsertLink("Details", "Bill", new { id = b.ToString() }) + "'>" + b.ToString() + "</a>")), paymentUser != null ? "<span class='important'>" + ClearString(paymentUser) + "</span>" : null }))
                //Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, billIDs.Select(b => "<a href='" + InsertLink("Details", "Bill", new { id = b.ToString() }) + "'>" + b.ToString() + "</a>")))
            };
        }

        public static SystemLog CancelBill(IEnumerable<long> billIDs, int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.CancelBill,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] { string.Join(",", billIDs.Select(b => "<a href='" + InsertLink("Details", "Bill", new { id = b.ToString() }) + "'>" + b.ToString() + "</a>")) }))
            };
        }

        public static SystemLog LineQualityCheck(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, object lineQualityObject)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.LineQualityCheck,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = lineQualityObject == null ? null : HttpUtility.HtmlEncode("<span class='important'>" + string.Join(", ", lineQualityObject.GetType().GetProperties().Select(p => "[" + p.Name + ":" + (p.GetValue(lineQualityObject) != null ? p.GetValue(lineQualityObject).ToString() : null) + "]")) + "</span>")
            };
        }

        public static SystemLog ChangeCustomer(int? userID, long customerID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ChangeCustomer,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                CustomerID = customerID
            };
        }

        public static SystemLog ChangeSubscriber(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ChangeSubscriber,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID
            };
        }

        public static SystemLog ExtendExpirationDate(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, string oldDate, string newDate)
        {
            oldDate = ClearString(oldDate);
            newDate = ClearString(newDate);
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ChangeLastAllowedDate,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] {
                    "<span class='important'>" + oldDate + "</span>",
                    "<span class='important'>" + newDate + "</span>"
                }))
            };
        }

        public static SystemLog AddWorkOrder(long customerSetupTaskID, int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.AddWorkOrder,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode("<a href='" + InsertLink("Details", "CustomerSetupService", new { id = customerSetupTaskID.ToString() }) + "'>" + customerSetupTaskID.ToString() + "</a>")
            };
        }

        public static SystemLog RemoveWorkOrder(long customerSetupTaskID, int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.RemoveWorkOrder,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(customerSetupTaskID.ToString())
            };
        }

        public static SystemLog AddFile(string fileName, int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            fileName = fileName.Trim().Replace('\t', ' ');
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.AddFile,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode("<a href='" + InsertLink("Files", "Client", new { id = subscriptionID.ToString() }) + "'>" + fileName + "</a>")
            };
        }

        public static SystemLog RemoveFile(string fileName, int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.RemoveFile,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode("<a href='" + InsertLink("Files", "Client", new { id = subscriptionID.ToString() }) + "'>" + fileName + "</a>")
            };
        }

        public static SystemLog AddSubscription(int? userID, long subscriptionID, long customerID, SubscriptionRegistrationType registrationType, SystemLogInterface interfaceType, string interfaceUsername, string subscriptionNo)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.AddSubscription,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                CustomerID = customerID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new string[]
                {
                    "<a href='" + InsertLink("Details", "Client", new { id = subscriptionID.ToString() }) + "'>" + subscriptionNo + "</a>",
                    "<span class='important'>" + InsertResource(typeof(RadiusR.Localization.Lists.SubscriptionRegistrationType), registrationType.ToString()) + "</span>"
                }))
            };
        }

        public static SystemLog ChangeDiscount(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, long billID)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.EditDiscount,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode("<a href='" + InsertLink("Details", "Bill", new { id = billID.ToString() }) + "'>" + billID.ToString() + "</a>")
            };
        }

        public static SystemLog AddressChange(int? userID, long? subscriptionID, long? customerID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.AddressChange,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                CustomerID = customerID
            };
        }

        public static SystemLog ChangeService(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, string oldServiceName, string newServiceName)
        {
            oldServiceName = ClearString(oldServiceName);
            newServiceName = ClearString(newServiceName);
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ChangeService,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] {
                    "<a href='" + InsertLinkWithDictionary("Index", "Service", new Dictionary<string,string>() { { "search.Name", oldServiceName } }) + "'>" + oldServiceName + "</a>",
                    "<a href='" + InsertLinkWithDictionary("Index", "Service", new Dictionary<string, string>() { { "search.Name", newServiceName } }) + "'>" + newServiceName + "</a>"
                }))
            };
        }

        public static SystemLog ChangeServiceSchedule(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, string oldServiceName, string newServiceName)
        {
            oldServiceName = ClearString(oldServiceName);
            newServiceName = ClearString(newServiceName);
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ChangeServiceSchedule,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] {
                    "<a href='" + InsertLinkWithDictionary("Index", "Service", new Dictionary<string,string>() { { "search.Name", oldServiceName } }) + "'>" + oldServiceName + "</a>",
                    "<a href='" + InsertLinkWithDictionary("Index", "Service", new Dictionary<string, string>() { { "search.Name", newServiceName } }) + "'>" + newServiceName + "</a>"
                }))
            };
        }

        public static SystemLog CancelScheduledChangeService(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.CancelScheduledChangeService,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID
            };
        }

        public static SystemLog ForceChangeService(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ForceChangeService,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID
            };
        }

        public static SystemLog ChangeClientState(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, CustomerState oldState, CustomerState newState)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ChangeClientState,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] {
                    "<span class='important'>" + InsertResource(typeof(RadiusR.Localization.Lists.CustomerState), oldState.ToString()) + "</span>",
                    "<span class='important'>" + InsertResource(typeof(RadiusR.Localization.Lists.CustomerState), newState.ToString()) + "</span>"
                }))
            };
        }

        public static SystemLog StepUpSubscriptionSpeedProfile(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.StepUpSubscriptionSpeedProfile,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID
            };
        }

        public static SystemLog StepDownSubscriptionSpeedProfile(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.StepDownSubscriptionSpeedProfile,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID
            };
        }

        public static SystemLog ChangeSubscriptionSpeedProfile(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, object speedProfileObject)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ChangeSubscriptionSpeedProfile,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = speedProfileObject == null ? null : HttpUtility.HtmlEncode("<span class='important'>" + string.Join(", ", speedProfileObject.GetType().GetProperties().Select(p => "[" + p.Name + ":" + (p.GetValue(speedProfileObject) != null ? p.GetValue(speedProfileObject).ToString() : null) + "]")) + "</span>")
            };
        }

        public static SystemLog EditClientIdentityDocument(int? userID, long customerID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.EditClientIdentityDocument,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                CustomerID = customerID
            };
        }

        public static SystemLog AddSubscriptionQuota(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, string formattedQuotaString)
        {
            formattedQuotaString = ClearString(formattedQuotaString);
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.SellQuota,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode("<span class='important'>" + formattedQuotaString + "</span>")
            };
        }

        public static SystemLog AddCreditCard(long customerID, SystemLogInterface interfaceType, string interfaceUsername, string maskedCardNo)
        {
            maskedCardNo = ClearString(maskedCardNo);
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.AddCreditCard,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                CustomerID = customerID,
                Parameters = HttpUtility.HtmlEncode("<span class='important'>" + maskedCardNo + "</span>")
            };
        }

        public static SystemLog RemoveCreditCard(long customerID, SystemLogInterface interfaceType, string interfaceUsername, string maskedCardNo)
        {
            maskedCardNo = ClearString(maskedCardNo);
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.RemoveCreditCard,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                CustomerID = customerID,
                Parameters = HttpUtility.HtmlEncode("<span class='important'>" + maskedCardNo + "</span>")
            };
        }

        public static SystemLog ActivateAutomaticPayment(long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, string gatewayName)
        {
            gatewayName = ClearString(gatewayName);
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ActivateAutomaticPayment,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode("<span class='important'>" + gatewayName + "</span>")
            };
        }

        public static SystemLog DeactivateAutomaticPayment(long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, string gatewayName)
        {
            gatewayName = ClearString(gatewayName);
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.DeactivateAutomaticPayment,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode("<span class='important'>" + gatewayName + "</span>")
            };
        }

        public static SystemLog ChangeSubscriberUsername(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, string oldUsername, string newUsername)
        {
            oldUsername = ClearString(oldUsername);
            newUsername = ClearString(newUsername);
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ChangeSubscriberUsername,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] {
                    "<span class='important'>" + oldUsername + "</span>",
                    "<span class='important'>" + newUsername + "</span>"
                }))
            };
        }

        public static SystemLog ChangeSubscriberPassword(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, string oldPassword, string newPassword)
        {
            oldPassword = ClearString(oldPassword);
            newPassword = ClearString(newPassword);
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ChangeSubscriberPassword,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] {
                    "<span class='important'>" + oldPassword + "</span>",
                    "<span class='important'>" + newPassword + "</span>"
                }))
            };
        }

        public static SystemLog CreateTelekomWorkOrder(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, long telekomWorkOrderID, TelekomWorkOrderDetails details)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.CreateTelekomWorkOrder,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] 
                {
                    "<span class='important'>" + InsertResource(typeof(RadiusR.Localization.Lists.TelekomOperations.TelekomOperationType), details.OperationType.ToString()) + "("+ InsertResource(typeof(RadiusR.Localization.Lists.TelekomOperations.TelekomOperationSubType), details.OperationSubType.ToString()) +")" + "</span>",
                    "<a href='" + InsertLinkWithDictionary("Details", "TelekomWorkOrder", new Dictionary<string, string>() { { "id", telekomWorkOrderID.ToString() } }) + "'>" + telekomWorkOrderID + "</a>",
                    "<span class='important'>" +
                    (
                        // transition
                        details.OperationType == DB.Enums.TelekomOperations.TelekomOperationType.Transition ?
                        string.Format("{0}: ({1})",
                        InsertResource(typeof(RadiusR.Localization.Model.RadiusR), "TransactionID"), details.TransactionID)
                        // other types
                        : string.Format("{0}: ({1}), {2}: ({3}), {4}: ({5}), {6}: ({7})",
                        InsertResource(typeof(RadiusR.Localization.Model.RadiusR), "ManagementCode"), details.ManagementCode,
                        InsertResource(typeof(RadiusR.Localization.Model.RadiusR), "ProvinceCode"), details.ProvinceCode,
                        InsertResource(typeof(RadiusR.Localization.Model.RadiusR), "QueueNo"), details.QueueNo,
                        InsertResource(typeof(RadiusR.Localization.Model.RadiusR), "TelekomSubscriberNo"), details.TelekomSubscriberNo)
                    )
                    + "</span>"
                }))
            };
        }

        public static SystemLog CloseTelekomWorkOrder(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, long telekomWorkOrderID, RadiusR.DB.Enums.TelekomOperations.TelekomOperationType workOrderType, RadiusR.DB.Enums.TelekomOperations.TelekomOperationSubType workOrderSubType)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.CloseTelekomWorkOrder,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] {
                    "<span class='important'>" + InsertResource(typeof(RadiusR.Localization.Lists.TelekomOperations.TelekomOperationType), workOrderType.ToString()) + "("+ InsertResource(typeof(RadiusR.Localization.Lists.TelekomOperations.TelekomOperationSubType), workOrderSubType.ToString()) +")" + "</span>",
                    "<a href='" + InsertLinkWithDictionary("Details", "TelekomWorkOrder", new Dictionary<string, string>() { { "id", telekomWorkOrderID.ToString() } }) + "'>" + telekomWorkOrderID + "</a>"
                }))
            };
        }

        public static SystemLog RetryTelekomWorkOrder(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, long telekomWorkOrderID, TelekomWorkOrderDetails details)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.RetryTelekomWorkOrder,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] 
                {
                    "<span class='important'>" + InsertResource(typeof(RadiusR.Localization.Lists.TelekomOperations.TelekomOperationType), details.OperationType.ToString()) + "("+ InsertResource(typeof(RadiusR.Localization.Lists.TelekomOperations.TelekomOperationSubType), details.OperationSubType.ToString()) +")" + "</span>",
                    "<a href='" + InsertLinkWithDictionary("Details", "TelekomWorkOrder", new Dictionary<string, string>() { { "id", telekomWorkOrderID.ToString() } }) + "'>" + telekomWorkOrderID + "</a>",
                    "<span class='important'>" + string.Format("{0}: ({1}), {2}: ({3}), {4}: ({5}), {6}: ({7})",
                    InsertResource(typeof(RadiusR.Localization.Model.RadiusR), "ManagementCode"), details.ManagementCode,
                    InsertResource(typeof(RadiusR.Localization.Model.RadiusR), "ProvinceCode"), details.ProvinceCode,
                    InsertResource(typeof(RadiusR.Localization.Model.RadiusR), "QueueNo"), details.QueueNo,
                    InsertResource(typeof(RadiusR.Localization.Model.RadiusR), "TelekomSubscriberNo"), details.TelekomSubscriberNo ) + "</span>"
                }))
            };
        }

        public static SystemLog ChangeInstallationAddress(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, string oldAddress, string newAddress)
        {
            oldAddress = ClearString(oldAddress);
            newAddress = ClearString(newAddress);
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ChangeInstallationAddress,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] {
                    "<span class='important'>" + oldAddress + "</span>",
                    "<span class='important'>" + newAddress + "</span>"
                }))
            };
        }

        public static SystemLog EditSubscriberCommitment(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.EditSubscriberCommitment,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID
            };
        }

        public static SystemLog ChangeSubscriberGroup(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ChangeSubscriberGroup,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID
            };
        }

        public static SystemLog ChangeSubscriberDSLNo(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, string oldDSLNo, string newDSLNo)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ChangeSubscriberDSLNo,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] {
                    "<span class='important'>" + oldDSLNo + "</span>",
                    "<span class='important'>" + newDSLNo + "</span>"
                }))
            };
        }

        public static SystemLog ChangeSubscriberStaticIP(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, string oldIP, string newIP)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ChangeSubscriberStaticIP,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] {
                    "<span class='important'>" + oldIP + "</span>",
                    "<span class='important'>" + newIP + "</span>"
                }))
            };
        }

        public static SystemLog ExtendPackage(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername, int extendedMonths)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.ExtendPackage,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode("<span class='important'>" + extendedMonths + "</span>")
            };
        }

        public static SystemLog TelekomSync(int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.TelekomSync,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID
            };
        }

        public static SystemLog AddRecurringDiscount(long recurringDiscountId, int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.AddRecurringDiscount,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = recurringDiscountId.ToString()
            };
        }

        public static SystemLog RemoveRecurringDiscount(long recurringDiscountId, RecurringDiscountCancellationCause cancellationCause, int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.RemoveRecurringDiscount,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] {
                    "<span class='important'>" + InsertResource(typeof(RadiusR.Localization.Lists.RecurringDiscount.RecurringDiscountCancellationCause), cancellationCause.ToString()) + "</span>",
                    recurringDiscountId.ToString()
                }))
            };
        }

        public static SystemLog DisableRecurringDiscount(long recurringDiscountId, RecurringDiscountCancellationCause cancellationCause, int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.DisableRecurringDiscount,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] {
                    "<span class='important'>" + InsertResource(typeof(RadiusR.Localization.Lists.RecurringDiscount.RecurringDiscountCancellationCause), cancellationCause.ToString()) + "</span>",
                    recurringDiscountId.ToString()
                }))
            };
        }

        public static SystemLog CloseWorkOrder(long customerSetupTaskID, int? userID, long subscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.CloseWorkOrder,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode("<a href='" + InsertLink("Details", "CustomerSetupService", new { id = customerSetupTaskID.ToString() }) + "'>" + customerSetupTaskID.ToString() + "</a>")
            };
        }

        public static SystemLog SubscriptionTransferApplied(int? userID, long subscriptionID, long fromSubscriptionID, long toSubscriptionID, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.SubscriptionTransferApplied,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(ParameterSeparator, new[] {
                    "<a href='" + InsertLinkWithDictionary("Details", "Client", new Dictionary<string,string>() { { "id", fromSubscriptionID.ToString() } }) + "'>" + fromSubscriptionID + "</a>",
                    "<a href='" + InsertLinkWithDictionary("Details", "Client", new Dictionary<string,string>() { { "id", toSubscriptionID.ToString() } }) + "'>" + toSubscriptionID + "</a>",
                }))
            };
        }

        public static SystemLog SentFormViaEmail(int? userID, long subscriptionID, GeneralPDFFormTypes[] forms, SystemLogInterface interfaceType, string interfaceUsername)
        {
            return new SystemLog()
            {
                Date = DateTime.Now,
                LogType = (int)SystemLogTypes.SentFormViaEmail,
                Interface = (short)interfaceType,
                InterfaceUsername = interfaceUsername,
                AppUserID = userID,
                SubscriptionID = subscriptionID,
                Parameters = HttpUtility.HtmlEncode(string.Join(", ", forms.Select(ft => "<span class='important'>" + InsertResource(typeof(RadiusR.Localization.Lists.GeneralPDFFormTypes), ft.ToString()) + "</span>").ToArray()))
            };
        }

        private static string ClearString(string input)
        {
            return input.Replace(ParameterSeparator, " ").Trim();
        }
    }
}
