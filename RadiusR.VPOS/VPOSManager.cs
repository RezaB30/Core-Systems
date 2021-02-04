using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Web.VPOS;
using RadiusR.DB;

namespace RadiusR.VPOS
{
    public static class VPOSManager
    {
        public static VPOS3DHostModel GetVPOSModel(string okUrl, string failUrl, decimal purchaseAmount, string language, string customerName = null, long? orderId = null, int? installmentCount = null)
        {
            var defaultCurrencyCode = (int)CurrencyCodes.TRY;
            var rand = new Random();
            var orderIdPrefix = rand.Next().ToString("00000000");

            switch (VPOSSettings.VPOSType)
            {
                case DB.Enums.VPOSTypes.QNBFinans:
                    return new RezaB.Web.VPOS.QNBFinans.QNBFinansVPOS3DHostModel()
                    {
                        CurrencyCode = defaultCurrencyCode,
                        MerchantId = VPOSSettings.MerchantID,
                        Storekey = VPOSSettings.StoreKey,
                        UserCode = VPOSSettings.UserID,
                        UserPass = VPOSSettings.UserPassword,
                        OkUrl = okUrl,
                        FailUrl = failUrl,
                        PurchaseAmount = purchaseAmount,
                        Language = language,
                        OrderId = orderId.HasValue ? orderIdPrefix + "-" + orderId : null,
                        InstallmentCount = installmentCount,
                        BillingCustomerName = customerName,
                        FormMethod = "POST"
                    };
                case DB.Enums.VPOSTypes.Ziraat:
                    return new RezaB.Web.VPOS.Ziraat.Ziraat3DHostModel()
                    {
                        CurrencyCode = defaultCurrencyCode,
                        MerchantId = VPOSSettings.MerchantID,
                        Storekey = VPOSSettings.StoreKey,
                        OkUrl = okUrl,
                        FailUrl = failUrl,
                        PurchaseAmount = purchaseAmount,
                        Language = language,
                        OrderId = orderId.HasValue ? orderIdPrefix + "-" + orderId : null,
                        InstallmentCount = installmentCount,
                        BillingCustomerName = customerName,
                        FormMethod = "POST"
                    };
                case DB.Enums.VPOSTypes.Halk:
                    return new RezaB.Web.VPOS.Halk.Halk3DHostModel()
                    {
                        CurrencyCode = defaultCurrencyCode,
                        MerchantId = VPOSSettings.MerchantID,
                        Storekey = VPOSSettings.StoreKey,
                        OkUrl = okUrl,
                        FailUrl = failUrl,
                        PurchaseAmount = purchaseAmount,
                        Language = language,
                        OrderId = orderId.HasValue ? orderIdPrefix + "-" + orderId : null,
                        InstallmentCount = installmentCount,
                        BillingCustomerName = customerName,
                        FormMethod = "POST"
                    };
                case DB.Enums.VPOSTypes.PayTR:
                    return new RezaB.Web.VPOS.PayTR.PayTRVPOS3DHostModel()
                    {
                        CurrencyCode = defaultCurrencyCode,
                        MerchantId = VPOSSettings.MerchantID,
                        MerchantSalt = VPOSSettings.MerchantSalt,
                        Storekey = VPOSSettings.StoreKey,
                        OkUrl = okUrl,
                        FailUrl = failUrl,
                        PurchaseAmount = purchaseAmount,
                        Language = language,
                        OrderId = orderId.HasValue ? orderIdPrefix + "-" + orderId : null,
                        InstallmentCount = installmentCount,
                        BillingCustomerName = customerName,
                        FormMethod = "POST"
                    };
                case DB.Enums.VPOSTypes.Vakif:
                    return new RezaB.Web.VPOS.Vakif.Vakifbank3DHostModel()
                    {
                        CurrencyCode = defaultCurrencyCode,
                        MerchantId = VPOSSettings.MerchantID,
                        OkUrl = okUrl,
                        FailUrl = failUrl,
                        PurchaseAmount = purchaseAmount,
                        Language = language,
                        OrderId = orderId.HasValue ? orderIdPrefix + "-" + orderId : null,
                        InstallmentCount = installmentCount,
                        HostTerminalId = VPOSSettings.MerchantSalt,
                        Storekey = VPOSSettings.StoreKey,
                        BillingCustomerName = customerName,
                        FormMethod = "GET"
                    };
                default:
                    return null;
            }
        }

        public static string GetErrorMessageParameterName()
        {
            switch (VPOSSettings.VPOSType)
            {
                case DB.Enums.VPOSTypes.QNBFinans:
                case DB.Enums.VPOSTypes.Ziraat:
                case DB.Enums.VPOSTypes.Halk:
                    return "ErrMsg";
                case DB.Enums.VPOSTypes.Vakif:
                    return "Message";
                default:
                    return string.Empty;
            }
        }
    }
}
