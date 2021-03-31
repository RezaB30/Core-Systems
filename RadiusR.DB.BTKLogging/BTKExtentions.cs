using RadiusR.DB.BTKLogging.Data;
using RadiusR.DB.BTKLogging.Enums;
using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.BTKLogging
{
    public static class BTKExtentions
    {
        public static IEnumerable<string> GetIPDRLog(this IQueryable<RadiusAccounting> query, DateTime lastOperationTime, DateTime nextOperationTime)
        {
            var finalQuery = query
                .Include(ra => ra.Subscription)
                .Include(ra => ra.RadiusAccountingIPInfo)
                .Where(ra=> ra.Subscription.StaticIP == null);
            var newThisPeriod = finalQuery.Where(accountingRecord => accountingRecord.StartTime >= lastOperationTime);
            // session starts
            var sessionStarts = newThisPeriod.AsEnumerable()
                .Select(accountingRecord => string.Join("|", new[]
                {
                    accountingRecord.Username,
                    accountingRecord.RadiusAccountingIPInfo != null? accountingRecord.RadiusAccountingIPInfo.LocalIP : accountingRecord.FramedIPAddress,
                    "1",
                    "65535",
                    accountingRecord.RadiusAccountingIPInfo != null ? accountingRecord.RadiusAccountingIPInfo.RealIP : null,
                    accountingRecord.RadiusAccountingIPInfo != null ? accountingRecord.RadiusAccountingIPInfo.PortRange != null ? accountingRecord.RadiusAccountingIPInfo.PortRange.Split('-')[0] : "0" : null,
                    accountingRecord.RadiusAccountingIPInfo != null ? accountingRecord.RadiusAccountingIPInfo.PortRange != null ? accountingRecord.RadiusAccountingIPInfo.PortRange.Split('-')[1] : "65535" : null,
                    BTKLoggingUtilities.TranslateDateTime(accountingRecord.StartTime),
                    BTKLoggingUtilities.TranslateDateTime(accountingRecord.StartTime),
                    "0",
                    "0",
                    null,
                    "session_start",
                    accountingRecord.NASPort,
                    accountingRecord.Subscription.SubscriberNo,
                    //accountingRecord.SessionID
                    $"{accountingRecord.SessionID}{accountingRecord.UniqueID}" //19.03.2021 15:07 BTK: SERDAR TANRIVERDI TEL: 03125865216
                }));
            // this period stops
            var thisPeriodStops = newThisPeriod.Where(accountingRecord => accountingRecord.StopTime < nextOperationTime).AsEnumerable()
                .Select(accountingRecord => string.Join("|", new[]
                {
                    accountingRecord.Username,
                    accountingRecord.RadiusAccountingIPInfo != null? accountingRecord.RadiusAccountingIPInfo.LocalIP : accountingRecord.FramedIPAddress,
                    "1",
                    "65535",
                    accountingRecord.RadiusAccountingIPInfo != null ? accountingRecord.RadiusAccountingIPInfo.RealIP : null,
                    accountingRecord.RadiusAccountingIPInfo != null ? accountingRecord.RadiusAccountingIPInfo.PortRange != null ? accountingRecord.RadiusAccountingIPInfo.PortRange.Split('-')[0] : "0" : null,
                    accountingRecord.RadiusAccountingIPInfo != null ? accountingRecord.RadiusAccountingIPInfo.PortRange != null ? accountingRecord.RadiusAccountingIPInfo.PortRange.Split('-')[1] : "65535" : null,
                    BTKLoggingUtilities.TranslateDateTime(accountingRecord.StartTime),
                    BTKLoggingUtilities.TranslateDateTime(accountingRecord.StopTime.Value),
                    accountingRecord.UploadBytes.ToString(),
                    accountingRecord.DownloadBytes.ToString(),
                    accountingRecord.TerminateCause.HasValue? ((AcctTerminateCause)accountingRecord.TerminateCause.Value).ToString():null,
                    "session_stop",
                    accountingRecord.NASPort,
                    accountingRecord.Subscription.SubscriberNo,
                    //accountingRecord.SessionID
                    $"{accountingRecord.SessionID}{accountingRecord.UniqueID}" //19.03.2021 15:07 BTK: SERDAR TANRIVERDI TEL: 03125865216
                }));
            // previous periods and send final
            return sessionStarts.Concat(thisPeriodStops).Concat(finalQuery.Where(accountingRecord => accountingRecord.StartTime < lastOperationTime).AsEnumerable()
                .Select(accountingRecord => string.Join("|", new[]
                {
                    accountingRecord.Username,
                    accountingRecord.RadiusAccountingIPInfo != null? accountingRecord.RadiusAccountingIPInfo.LocalIP : accountingRecord.FramedIPAddress,
                    "1",
                    "65535",
                    accountingRecord.RadiusAccountingIPInfo != null ? accountingRecord.RadiusAccountingIPInfo.RealIP : null,
                    accountingRecord.RadiusAccountingIPInfo != null ? accountingRecord.RadiusAccountingIPInfo.PortRange != null ? accountingRecord.RadiusAccountingIPInfo.PortRange.Split('-')[0] : "0" : null,
                    accountingRecord.RadiusAccountingIPInfo != null ? accountingRecord.RadiusAccountingIPInfo.PortRange != null ? accountingRecord.RadiusAccountingIPInfo.PortRange.Split('-')[1] : "65535" : null,
                    BTKLoggingUtilities.TranslateDateTime(accountingRecord.StartTime),
                    accountingRecord.StopTime.HasValue ? BTKLoggingUtilities.TranslateDateTime(accountingRecord.StopTime.Value) : accountingRecord.StartTime > lastOperationTime ? BTKLoggingUtilities.TranslateDateTime(accountingRecord.StartTime) : BTKLoggingUtilities.TranslateDateTime(new DateTime(nextOperationTime.Year, nextOperationTime.Month, nextOperationTime.Day, nextOperationTime.Hour, accountingRecord.StartTime.Minute, accountingRecord.StartTime.Second)),
                    accountingRecord.UploadBytes.ToString(),
                    accountingRecord.DownloadBytes.ToString(),
                    accountingRecord.TerminateCause.HasValue? ((AcctTerminateCause)accountingRecord.TerminateCause.Value).ToString():null,
                    accountingRecord.StopTime.HasValue? "session_stop": "interim_update",
                    accountingRecord.NASPort,
                    accountingRecord.Subscription.SubscriberNo,
                    //accountingRecord.SessionID
                    $"{accountingRecord.SessionID}{accountingRecord.UniqueID}" //19.03.2021 15:07 BTK: SERDAR TANRIVERDI TEL: 03125865216
                })));
        }

        public static IEnumerable<string> GetSessionsLog(this IQueryable<RadiusAccounting> query)
        {
            var finalQuery = query
                .Include(ra => ra.Subscription.SubscriptionTelekomInfo);
            return finalQuery.AsEnumerable()
                .Select(accountingRecord => string.Join("|", new[]
                {
                    accountingRecord.Username,
                    accountingRecord.Subscription.StaticIP,
                    BTKLoggingUtilities.TranslateDateTime(accountingRecord.StartTime),
                    BTKLoggingUtilities.TranslateDateTime(DateTime.Now),
                    accountingRecord.UploadBytes.ToString(),
                    accountingRecord.DownloadBytes.ToString(),
                    accountingRecord.TerminateCause.HasValue? ((AcctTerminateCause)accountingRecord.TerminateCause.Value).ToString():null,
                    accountingRecord.StopTime.HasValue? "session_stop": accountingRecord.UpdateTime.HasValue? "interim_update": "session_start",
                    accountingRecord.Subscription.SubscriptionTelekomInfo != null ? accountingRecord.Subscription.SubscriptionTelekomInfo.RedbackName ?? string.Empty : string.Empty,
                    accountingRecord.SessionID
                }));
        }

        public static IEnumerable<string> GetIPBlockLog(this IEnumerable<BTKIPBlock> query)
        {
            return query.Select(IPBlock => string.Join("|", new string[]
            {
                IPBlock.OperatorName,
                IPBlock.StartingIP,
                IPBlock.EndingIP,
                IPBlock.ServiceType.ToString(),
                Convert.ToInt32(IPBlock.UsesNAT).ToString(),
                BTKLoggingUtilities.TranslateDateTime(IPBlock.AllocationDate),
                BTKLoggingUtilities.TranslateDateTime(IPBlock.DeallocationDate),
                IPBlock.BlockType == (short)IPBlockType.Dynamic ? "D" : IPBlock.BlockType == (short)IPBlockType.Static ? "S" : string.Empty,
                IPBlock.UseLocation
            }));
        }

        public static IEnumerable<string> GetClientOldLog(this IQueryable<Subscription> query)
        {
            var finalQuery = query
                .Include(subscription => subscription.Customer.CustomerIDCard)
                .Include(subscription => subscription.Customer.CorporateCustomerInfo)
                .Include(subscription => subscription.Address)
                .Include(subscription => subscription.SubscriptionTelekomInfo)
                .Include(subscription => subscription.Service);

            return finalQuery.AsEnumerable()
                .Select(subscription => string.Join("|;|", new string[]
                {
                    subscription.Customer.FirstName,
                    subscription.Customer.LastName,
                    ClearAddressText(subscription.Address.AddressText),
                    subscription.Address.PostalCode.ToString("00000"),
                    subscription.Address.DistrictName,
                    subscription.Address.ProvinceName,
                    "TÜRKIYE",
                    //subscription.SubscriptionTelekomInfo != null ? subscription.SubscriptionTelekomInfo.SubscriptionNo : string.Empty,
                    subscription.SubscriberNo,
                    subscription.ID.ToString(),
                    AppSettings.CountryPhoneCode.Substring(1) + subscription.Customer.ContactPhoneNo,
                    subscription.Username,
                    subscription.Service.RateLimit,
                    BTKLoggingUtilities.GetServiceType(subscription.Service.InfrastructureType),
                    subscription.SubscriptionTelekomInfo != null ? subscription.SubscriptionTelekomInfo.RedbackName ?? string.Empty : string.Empty,
                    subscription.StaticIP,
                    subscription.Customer.Email,
                    ClearAddressText(subscription.Customer.BillingAddress.AddressText),
                    subscription.Customer.CustomerIDCard.TCKNo,
                    string.Empty,
                    string.Empty,
                    BTKLoggingUtilities.TranslateDateTime(subscription.MembershipDate),
                    BTKLoggingUtilities.TranslateDateTime(subscription.EndDate),
                    BTKLoggingUtilities.GetLineStateOld(subscription.State)
                }));
        }

        private static IEnumerable<string> GetClientLog(this IQueryable<Subscription> query, ClientLogTypes logType, DateTime? from = null)
        {
            var finalQuery = query
                .Include(subscription => subscription.Service)
                .Include(subscription => subscription.Customer.CustomerIDCard)
                .Include(subscription => subscription.Customer.CorporateCustomerInfo.Address)
                .Include(subscription => subscription.Customer.Address)
                .Include(subscription => subscription.Address)
                .Include(subscription => subscription.SubscriptionTelekomInfo)
                .Include(subscription => subscription.SubscriptionCancellation)
                .Include(subscription => subscription.SystemLogs.Select(log => log.AppUser));
            if (logType == ClientLogTypes.Changes)
                finalQuery = finalQuery.Include(subscription => subscription.SubscriptionStateHistories);

            var queryResults = finalQuery.ToArray();

            if (logType == ClientLogTypes.Changes)
            {
                var multiLineLogs = queryResults.Select(subscription => new
                {
                    Subscription = subscription,
                    LogDescriptions = BTKLoggingUtilities.GetChangeCodes(subscription, from.Value)
                });

                return multiLineLogs.SelectMany(line => line.LogDescriptions.Select(description => line.Subscription.CreateClientLogLine(description)));
            }

            return queryResults
                .Select(subscription => subscription.CreateClientLogLine()).ToArray();
        }

        public static IEnumerable<string> GetClientsCatalogLog(this IQueryable<Subscription> query)
        {
            return GetClientLog(query, ClientLogTypes.Catalog);
        }

        public static IEnumerable<string> GetClientsChangeLogs(this IQueryable<Subscription> query, DateTime from)
        {
            return GetClientLog(query, ClientLogTypes.Changes, from);
        }

        private static string CreateClientLogLine(this Subscription subscription, ClientChangeDescription changeDescription = null)
        {
            return string.Join("|;|", new string[] {
                BTKSettings.BTKOperatorCode,
                subscription.SubscriberNo,
                subscription.SubscriberNo,
                BTKLoggingUtilities.GetLineState(subscription.State),
                BTKLoggingUtilities.GetLineStateCode(subscription),
                BTKLoggingUtilities.GetLineStateDetails(subscription),
                changeDescription == null ? string.Empty : changeDescription.Code.Value.ToString(),
                changeDescription == null ? string.Empty : changeDescription.Description,
                changeDescription == null ? string.Empty : BTKLoggingUtilities.TranslateDateTime(changeDescription.Time),
                BTKLoggingUtilities.GetServiceType(subscription.Service.InfrastructureType),
                BTKLoggingUtilities.GetCustomerType(subscription.Customer.CustomerType),
                BTKLoggingUtilities.TranslateDateTime(subscription.MembershipDate),
                BTKLoggingUtilities.TranslateDateTime(subscription.EndDate),
                subscription.Customer.FirstName,
                subscription.Customer.LastName,
                subscription.Customer.CustomerIDCard.TCKNo,
                subscription.Customer.CustomerIDCard.PassportNo ?? string.Empty,
                subscription.Customer.CorporateCustomerInfo != null ? subscription.Customer.CorporateCustomerInfo.Title : string.Empty,
                subscription.Customer.CorporateCustomerInfo != null ? subscription.Customer.CorporateCustomerInfo.TaxNo : string.Empty,
                subscription.Customer.CorporateCustomerInfo != null ? subscription.Customer.CorporateCustomerInfo.CentralSystemNo : string.Empty,
                BTKLoggingUtilities.GetSex(subscription.Customer.Sex),
                BTKLoggingUtilities.GetNationality(subscription.Customer.Nationality),
                subscription.Customer.FathersName,
                subscription.Customer.MothersName,
                subscription.Customer.MothersMaidenName,
                subscription.Customer.BirthPlace,
                BTKLoggingUtilities.TranslateDate(subscription.Customer.BirthDate),
                BTKLoggingUtilities.GetProfessionCode(subscription.Customer.Profession),
                subscription.Service.Name.ToUpper(),
                subscription.Customer.CustomerIDCard != null ? subscription.Customer.CustomerIDCard.VolumeNo : string.Empty,
                subscription.Customer.CustomerIDCard != null ? subscription.Customer.CustomerIDCard.RowNo : string.Empty,
                subscription.Customer.CustomerIDCard != null ? subscription.Customer.CustomerIDCard.PageNo : string.Empty,
                subscription.Customer.CustomerIDCard != null ? subscription.Customer.CustomerIDCard.Province : string.Empty,
                subscription.Customer.CustomerIDCard != null ? subscription.Customer.CustomerIDCard.District : string.Empty,
                subscription.Customer.CustomerIDCard != null ? subscription.Customer.CustomerIDCard.Neighbourhood : string.Empty,
                subscription.Customer.CustomerIDCard != null ? BTKLoggingUtilities.GetIdentityCardType(subscription.Customer.CustomerIDCard.TypeID) : string.Empty,
                subscription.Customer.CustomerIDCard != null ? subscription.Customer.CustomerIDCard.SerialNo : string.Empty,
                subscription.Customer.CustomerIDCard != null ? subscription.Customer.CustomerIDCard.PlaceOfIssue : string.Empty,
                subscription.Customer.CustomerIDCard != null ? BTKLoggingUtilities.TranslateDate(subscription.Customer.CustomerIDCard.DateOfIssue) : string.Empty,
                subscription.Customer.CustomerType == (short)CustomerType.Individual ? "B" : subscription.Customer.CorporateCustomerInfo != null ? "Y" : string.Empty,
                subscription.Address.ProvinceName,
                subscription.Address.DistrictName,
                subscription.Address.NeighborhoodName,
                subscription.Address.StreetName,
                subscription.Address.DoorNo,
                subscription.Address.ApartmentNo,
                subscription.Address.PostalCode.ToString("00000"),
                subscription.Address.AddressNo.ToString(),
                subscription.Customer.ContactPhoneNo,
                string.Empty,
                subscription.Customer.Email,
                subscription.Customer.Address.ProvinceName,
                subscription.Customer.Address.DistrictName,
                subscription.Customer.Address.NeighborhoodName,
                subscription.Customer.Address.StreetName,
                subscription.Customer.Address.DoorNo,
                subscription.Customer.Address.ApartmentNo,
                subscription.Customer.Address.AddressNo.ToString(),
                subscription.Customer.CorporateCustomerInfo != null ? subscription.Customer.FirstName : string.Empty,
                subscription.Customer.CorporateCustomerInfo != null ? subscription.Customer.LastName : string.Empty,
                subscription.Customer.CorporateCustomerInfo != null ? subscription.Customer.CustomerIDCard.TCKNo : string.Empty,
                subscription.Customer.CorporateCustomerInfo != null ? subscription.Customer.ContactPhoneNo : string.Empty,
                subscription.Customer.CorporateCustomerInfo != null ? subscription.Customer.CorporateCustomerInfo.Address.AddressText.Replace(Environment.NewLine, " ") :string.Empty,
                string.Empty,
                string.Empty,
                BTKSettings.BTKActivationUser,//BTKLoggingUtilities.GetActivationUser(subscription),
                string.Empty,
                string.Empty,
                string.Empty,//BTKLoggingUtilities.GetLastUpdateUser(subscription),
                subscription.StaticIP,
                subscription.Service.RateLimit,
                subscription.Username,
                subscription.SubscriptionTelekomInfo != null ? subscription.SubscriptionTelekomInfo.RedbackName ?? string.Empty : string.Empty,
            });
        }

        public static IQueryable<RadiusAccounting> GetActiveInTimeSpan(this IQueryable<RadiusAccounting> query, DateTime from, DateTime to)
        {
            return query.Where(acc => acc.StartTime < to && (acc.StopTime >= from || acc.StopTime == null) && acc.RadiusAccountingIPInfo != null);
        }

        private static string ClearAddressText(string addressText)
        {
            return addressText.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
        }
    }
}
