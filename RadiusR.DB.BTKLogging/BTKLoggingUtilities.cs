using RadiusR.DB.BTKLogging.Data;
using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.BTKLogging
{
    public static class BTKLoggingUtilities
    {
        public static string GetLineState(short subscriptionState)
        {
            switch ((CustomerState)subscriptionState)
            {
                case CustomerState.Registered:
                    return string.Empty;
                case CustomerState.Reserved:
                case CustomerState.Active:
                    return "A";
                case CustomerState.Disabled:
                    return "D";
                case CustomerState.Cancelled:
                    return "I";
                default:
                    break;
            }

            return string.Empty;
        }

        public static string GetLineStateOld(short subscriptionState)
        {
            switch ((CustomerState)subscriptionState)
            {
                case CustomerState.Registered:
                    return string.Empty;
                case CustomerState.Reserved:
                case CustomerState.Disabled:
                case CustomerState.Active:
                    return "AKTIF";
                case CustomerState.Cancelled:
                    return "PASIF";
                default:
                    break;
            }

            return string.Empty;
        }

        public static string GetLineStateCode(Subscription subscription)
        {
            if ((CustomerState)subscription.State == CustomerState.Active || (CustomerState)subscription.State == CustomerState.Reserved)
            {
                return "1";
            }
            if ((CustomerState)subscription.State == CustomerState.Disabled)
            {
                return "15";
            }
            if ((CustomerState)subscription.State == CustomerState.Cancelled)
            {
                if (subscription.SubscriptionCancellation == null)
                    return "3";
                switch ((CancellationReason)subscription.SubscriptionCancellation.ReasonID)
                {
                    case CancellationReason.ChangedNumber:
                        return "2";
                    case CancellationReason.Others:
                        return "3";
                    case CancellationReason.FakeDocuments:
                        return "4";
                    case CancellationReason.CustomerRequest:
                        return "5";
                    case CancellationReason.Transfer:
                        return "6";
                    case CancellationReason.WrongOwner:
                        return "7";
                    case CancellationReason.BlackList:
                        return "8";
                    case CancellationReason.OutOfUse:
                        return "9";
                    case CancellationReason.MissingDocuments:
                        return "10";
                    case CancellationReason.AccidentalInput:
                        return "11";
                    case CancellationReason.RelatedProductCancelled:
                        return "12";
                    default:
                        break;
                }
            }

            return string.Empty;
        }

        public static string GetLineStateDetails(Subscription subscription)
        {
            var lineStateCode = GetLineStateCode(subscription);
            return !string.IsNullOrEmpty(lineStateCode) ? _LineStateCodeDictionary[lineStateCode] : string.Empty;
        }

        public static string GetServiceType(short infrastructureType)
        {
            return Enum.GetName(typeof(ServiceInfrastructureTypes), infrastructureType).Replace("_", " ");
        }

        public static string GetCustomerType(short customerType)
        {
            switch ((CustomerType)customerType)
            {
                case CustomerType.Individual:
                    return "G-SAHIS";
                case CustomerType.PrivateCompany:
                    return "G-SIRKET";
                case CustomerType.LegalCompany:
                    return "T-SIRKET";
                case CustomerType.PublicCompany:
                    return "T-KAMU";
                default:
                    break;
            }

            return string.Empty;
        }

        public static string TranslateDateTime(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("yyyyMMddHHmmss") : string.Empty;
        }

        public static string TranslateDate(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("yyyyMMdd") : string.Empty;
        }

        public static string GetSex(short? sex)
        {
            if (sex.HasValue)
            {
                switch ((Sexes)sex)
                {
                    case Sexes.Male:
                        return "E";
                    case Sexes.Female:
                        return "K";
                    default:
                        break;
                }
            }

            return string.Empty;
        }

        public static string GetNationality(int nationality)
        {
            return Enum.GetName(typeof(CountryCodes), nationality);
        }

        public static string GetProfessionCode(int profession)
        {
            return profession.ToString("000");
        }

        public static string GetIdentityCardType(short typeId)
        {
            return _IDCardTypeDictionary[typeId];
        }

        public static string GetActivationUser(Subscription subscription)
        {
            var validLog = subscription.SystemLogs.OrderBy(log => log.Date).FirstOrDefault(log => (SystemLogTypes)log.LogType == SystemLogTypes.AddSubscription);

            return validLog != null ? validLog.AppUser.Email : string.Empty;
        }

        public static string GetLastUpdateUser(Subscription subscription)
        {
            var validLog = subscription.SystemLogs.OrderByDescending(log => log.Date).FirstOrDefault(log => (SystemLogTypes)log.LogType == SystemLogTypes.ChangeCustomer || (SystemLogTypes)log.LogType == SystemLogTypes.ChangeSubscriber || (SystemLogTypes)log.LogType == SystemLogTypes.ChangeService || (SystemLogTypes)log.LogType == SystemLogTypes.AddressChange || (SystemLogTypes)log.LogType == SystemLogTypes.EditClientIdentityDocument);

            return validLog != null ? validLog.AppUser.Email : string.Empty;
        }

        internal static IEnumerable<ClientChangeDescription> GetChangeCodes(Subscription subscription, DateTime from)
        {
            var systemLogs = subscription.SystemLogs.Where(log => log.Date > from && RelevantSystemLogTypes.Contains(log.LogType));
            var stateChanges = subscription.SubscriptionStateHistories.Where(history => history.ChangeDate > from).Where(history => (history.NewState == (short)CustomerState.Active && history.OldState == (short)CustomerState.Disabled) || (history.OldState == (short)CustomerState.Disabled && history.NewState == (short)CustomerState.Active) || history.NewState == (short)CustomerState.Cancelled);
            return systemLogs.Select(log => new ClientChangeDescription()
            {
                Code = GetClientChangeCodeFromSystemLogType(log.LogType),
                Description = GetClientChangeCodeFromSystemLogType(log.LogType).HasValue ? _ClientChangeDictionary[GetClientChangeCodeFromSystemLogType(log.LogType).Value] : null,
                Time = log.Date
            }).Where(description => description.Code.HasValue)
            .Concat(stateChanges.Select(change => new ClientChangeDescription()
            {
                Code = change.NewState == (short)CustomerState.Cancelled ? 10 : 2,
                Description = change.NewState == (short)CustomerState.Cancelled ? _ClientChangeDictionary[10] : _ClientChangeDictionary[2],
                Time = change.ChangeDate
            }));
        }

        private static int? GetClientChangeCodeFromSystemLogType(int systemlogType)
        {
            switch ((SystemLogTypes)systemlogType)
            {
                case SystemLogTypes.ChangeCustomer:
                case SystemLogTypes.EditClientIdentityDocument:
                case SystemLogTypes.ChangeSubscriber:
                    return 11;
                case SystemLogTypes.AddSubscription:
                    return 1;
                case SystemLogTypes.AddressChange:
                    return 5;
                case SystemLogTypes.ChangeService:
                    return 7;
                default:
                    return null;
            }
        }

        private static Dictionary<string, string> _LineStateCodeDictionary = new Dictionary<string, string>()
        {
            { "1", "AKTIF" },
            { "2", "IPTAL_NUMARA DEGISTIRME" },
            {"3", "IPTAL" },
            {"4", "IPTAL_SAHTE EVRAK" },
            {"5", "IPTAL_MUSTERI TALEBI" },
            {"6", "IPTAL_DEVIR" },
            {"7", "IPTAL_HAT BENIM DEGIL" },
            {"8", "IPTAL_KARA LISTE" },
            {"9", "IPTAL_KULLANIM DISI" },
            {"10", "IPTAL_EKSIK EVRAK" },
            {"11", "IPTAL_SEHVEN GIRIS" },
            {"12", "IPTAL_BAGLI URUN IPTALI" },
            {"13", "KISITLI_KONTUR BITTI" },
            {"14", "KISITLI_ARAMAYA KAPALI" },
            {"15", "DONDURULMUS_MUSTERI TALEBI" },
            {"16", "DONDURULMUS_ISLETME" }
        };

        private static Dictionary<int, string> _IDCardTypeDictionary = new Dictionary<int, string>()
        {
            {1, "TCKK" },
            {2, "TCNC" },
            {3, "TCYK" },
            {4, "TCPC" },
            {5, "TCPL" },
            {6, "TCPY" },
            {7, "TCPG" },
            {8, "TCPK" },
            {9, "TCGP" },
            {10, "YP" },
            {11, "AC" },
            {12, "GC" },
            {13, "NE" },
            {14, "SB" },
            {15, "HB" },
            {16, "GK" },
            {17, "TCSC" },
            {18, "TCHS" },
            {19, "TCSV" },
            {20, "TCGK" },
            {21, "TCMA" },
            {22, "TCEV" }
        };

        private static Dictionary<int, string> _ClientChangeDictionary = new Dictionary<int, string>()
        {
            {1, "YENI ABONELIK KAYDI" },
            {2, "HAT DURUM DEGISIKLIGI" },
            {3, "SIM KART DEGISIKLIGI" },
            {4, "ODEME TIPI DEGISIKLIGI" },
            {5, "ADRES DEGISIKLIGI" },
            {6, "IMSI DEGISIKLIGI" },
            {7, "TARIFE DEGISIKLIGI" },
            {8, "DEVIR (MUSTERI) DEGISIKLIGI" },
            {9, "NUMARA DEGISIKLIGI" },
            {10, "HAT IPTAL" },
            {11, "MUSTERI BILGI DEGISIKLIGI" },
            {12, "NUMARA TASIMA" },
            {13, "NUMARA DEGISMEDEN NAKIL" },
            {14, "NUMARA DEGISTIREREK NAKIL" },
            {15, "IP DEGISIKLIGI" }
        };

        internal static int[] RelevantSystemLogTypes = new[]
        {
            (int)SystemLogTypes.AddSubscription,
            (int)SystemLogTypes.AddressChange,
            (int)SystemLogTypes.ChangeService,
            (int)SystemLogTypes.ChangeCustomer,
            (int)SystemLogTypes.ChangeSubscriber,
            (int)SystemLogTypes.CancelScheduledChangeService
        };
    }
}
