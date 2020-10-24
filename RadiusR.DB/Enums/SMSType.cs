using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Enums
{
    public enum SMSType
    {
        NewBill = 1,
        PaymentReminder = 2,
        SoftQuota100 = 3,
        HardQuota100 = 4,
        Activation = 5,
        PaymentDone = 6,
        UserCredentials = 7,
        CreditAck = 8,
        DebtDisconnection = 9,
        PrePaidExpiration = 10,
        ExtendPackage = 11,
        WebsiteCredentials = 12,
        ForgotPassword = 13,
        Quota80 = 14,
        SmartQuota100 = 15,
        SmartQuotaMax = 16,
        SellQuota = 17,
        CancelPayment = 18,
        MobilExpressAddRemoveCard = 19,
        MobilExpressActivation = 20,
        MobilExpressDeactivation = 21,
        FailedAutomaticPayment = 22,
        OperationCode = 23
    }
}
