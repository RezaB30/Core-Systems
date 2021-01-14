using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Enums
{
    public enum SystemLogTypes
    {
        ChangeCustomer = 1,
        ChangeSubscriber = 2,
        ChangeLastAllowedDate = 3,
        LineQualityCheck = 4,
        AddAdditionalFee = 5,
        RemoveAdditionalFee = 6,
        BillPayment = 7,
        CancelPayment = 8,
        AddWorkOrder = 9,
        RemoveWorkOrder = 10,
        AddFile = 11,
        RemoveFile = 12,
        AddSubscription = 13,
        EditDiscount = 14,
        ChangeSubscriptionState = 15,
        AddressChange = 16,
        ChangeService = 17,
        ForceChangeService = 18,
        ChangeClientState = 19,
        StepUpSubscriptionSpeedProfile = 20,
        StepDownSubscriptionSpeedProfile = 21,
        ChangeSubscriptionSpeedProfile = 22,
        EditClientIdentityDocument = 23,
        SellQuota = 24,
        AddCreditCard = 25,
        RemoveCreditCard = 26,
        ActivateAutomaticPayment = 27,
        DeactivateAutomaticPayment = 28,
        ChangeSubscriberUsername = 29,
        CreateTelekomWorkOrder = 30,
        CloseTelekomWorkOrder = 31,
        RetryTelekomWorkOrder = 32,
        ChangeInstallationAddress = 33,
        EditSubscriberCommitment = 34,
        ChangeSubscriberGroup = 35,
        ChangeSubscriberDSLNo = 36,
        ChangeSubscriberStaticIP = 37,
        ChangeServiceSchedule = 38,
        CancelScheduledChangeService = 39,
        CancelBill = 40,
        ChangeSubscriberPassword = 41,
        CancelAdditionalFee = 42,
        ExtendPackage = 43,
        TelekomSync = 44,
        AddRecurringDiscount = 45,
        RemoveRecurringDiscount = 46,
        DisableRecurringDiscount = 47,
        CloseWorkOrder = 48,
    }
}
