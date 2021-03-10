using RadiusR.DB.DomainsCache;
using RadiusR.DB.Enums;
using RadiusR.SystemLogs;
using RezaB.TurkTelekom.WebServices.Exceptions;
using RezaB.TurkTelekom.WebServices.InfrastructureInfo;
using RezaB.TurkTelekom.WebServices.SubscriberInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TelekomSynchronization
{
    public static class TelekomSynchronizationUtilities
    {
        /// <summary>
        /// Synchronizes subscription's Telekom info, username and password with Telekom system.
        /// </summary>
        /// <param name="db">Database context.</param>
        /// <param name="options">Synchronization options.</param>
        /// <returns></returns>
        public static TelekomSynchronizationResults UpdateSubscriberTelekomInfoFromWebService(this RadiusREntities db, TelekomSynchronizationOptions options)
        {
            // check options
            if (options.DBSubscription == null || !options.AppUserID.HasValue || string.IsNullOrWhiteSpace(options.DSLNo) || !options.LogInterface.HasValue)
            {
                return new TelekomSynchronizationResults()
                {
                    ResultCode = TelekomSynchronizationResultCodes.InvalidOptions
                };
            }
            // check subscription
            if (options.DBSubscription.SubscriptionTelekomInfo == null)
            {
                return new TelekomSynchronizationResults()
                {
                    ResultCode = TelekomSynchronizationResultCodes.SubscriptionHasNoTelekomInfo
                };
            }
            // get domain from cache
            var domain = RadiusR.DB.DomainsCache.DomainsCache.GetDomainByID(options.DBSubscription.DomainID);
            if (domain == null || domain.TelekomCredential == null)
            {
                return new TelekomSynchronizationResults()
                {
                    ResultCode = TelekomSynchronizationResultCodes.InvalidDomain
                };
            }

            // initialize
            var result = new TelekomSynchronizationResults()
            {
                ResultCode = TelekomSynchronizationResultCodes.Success,
                TelekomExceptions = new List<TTWebServiceException>()
            };

            // Tariff Update
            {
                var serviceClient = new TelekomSubscriberInfoServiceClient(domain.TelekomCredential.XDSLWebServiceUsernameInt, domain.TelekomCredential.XDSLWebServicePassword);
                var response = serviceClient.GetSubscriberDetailedInfo(options.DSLNo);
                if (response.InternalException != null)
                {
                    result.ResultCode = TelekomSynchronizationResultCodes.TelekomError;
                    result.TelekomExceptions.Add(response.InternalException);
                    //return new TelekomSynchronizationResults(){
                    //    ResultCode = TelekomSynchronizationResultCodes.TelekomError,
                    //    TelekomExceptions = response.InternalException 
                    //};
                }
                else
                {
                    if (response.Data.PacketCode.HasValue)
                        options.DBSubscription.SubscriptionTelekomInfo.PacketCode = response.Data.PacketCode.Value;
                    if (response.Data.TariffCode.HasValue)
                        options.DBSubscription.SubscriptionTelekomInfo.TariffCode = response.Data.TariffCode.Value;
                    if (response.Data.DSLType.HasValue)
                        options.DBSubscription.SubscriptionTelekomInfo.XDSLType = (short)response.Data.DSLType.Value;
                    if (response.Data.CustomerCode.HasValue)
                        options.DBSubscription.SubscriptionTelekomInfo.TTCustomerCode = response.Data.CustomerCode.Value;
                    if (!string.IsNullOrEmpty(response.Data.PSTNNo) && response.Data.PSTNNo != options.DSLNo)
                        options.DBSubscription.SubscriptionTelekomInfo.PSTN = response.Data.PSTNNo;
                    else
                        options.DBSubscription.SubscriptionTelekomInfo.PSTN = null;
                    if (!string.IsNullOrWhiteSpace(response.Data.SubscriberUsername))
                    {
                        var newUsername = response.Data.SubscriberUsername + "@" + domain.Name;
                        if (db.Subscriptions.Any(s => s.Username.ToLower() == newUsername && s.ID != options.DBSubscription.ID))
                            return new TelekomSynchronizationResults()
                            {
                                ResultCode = TelekomSynchronizationResultCodes.SynchronizedUsernameExists,
                                SynchronizedUsername = newUsername
                            };
                        options.DBSubscription.Username = newUsername;
                    }
                }
                // Password Update
                {
                    var secondaryResponse = serviceClient.GetFTTXAAAInfo(options.DSLNo);
                    if (secondaryResponse.InternalException != null)
                    {
                        result.ResultCode = TelekomSynchronizationResultCodes.TelekomError;
                        result.TelekomExceptions.Add(secondaryResponse.InternalException);
                        //return new TelekomSynchronizationResults()
                        //{
                        //    ResultCode = TelekomSynchronizationResultCodes.TelekomError,
                        //    TelekomExceptions = secondaryResponse.InternalException
                        //};
                    }
                    else
                    {
                        options.DBSubscription.RadiusPassword = secondaryResponse.Data.Password;
                    }
                }
            }
            // redback update
            {
                var serviceClient = new InfrastructurInfoServiceClient(domain.TelekomCredential.XDSLWebServiceUsernameInt, domain.TelekomCredential.XDSLWebServicePassword);
                var response = serviceClient.GetCLIDInfo(options.DSLNo);
                if (response.InternalException == null)
                {
                    options.DBSubscription.SubscriptionTelekomInfo.RedbackName = response.Data.RedbackName;
                }
                else
                {
                    result.ResultCode = TelekomSynchronizationResultCodes.TelekomError;
                    result.TelekomExceptions.Add(response.InternalException);
                }
            }
            // operator name update
            {
                var serviceClient = new RezaB.TurkTelekom.WebServices.TTChurnApplication.TTChurnApplicationClient(domain.TelekomCredential.XDSLWebServiceUsernameInt, domain.TelekomCredential.XDSLWebServicePassword);
                var response = serviceClient.GetOperatorByXDSLNo(options.DSLNo);
                if (response.InternalException == null)
                {
                    var cachedOperator = TransitionOperatorsCache.GetAllOperators().FirstOrDefault(to => to.Username == response.Data);
                    if (cachedOperator == null)
                    {
                        result.ResultCode = TelekomSynchronizationResultCodes.TelekomError;
                        result.TelekomExceptions.Add(new TTWebServiceException($"Operator user [{response.Data}] does not exist."));
                        //return new TelekomSynchronizationResults()
                        //{
                        //    ResultCode = TelekomSynchronizationResultCodes.TelekomError,
                        //    TelekomExceptions = new TTWebServiceException($"Operator user [{response.Data}] does not exist.")
                        //};
                    }
                    else
                    {
                        options.DBSubscription.SubscriptionTelekomInfo.OperatorID = cachedOperator.ID;
                    }
                }
                else
                {
                    result.ResultCode = TelekomSynchronizationResultCodes.TelekomError;
                    result.TelekomExceptions.Add(response.InternalException);
                }
            }
            // system log
            db.SystemLogs.Add(SystemLogProcessor.TelekomSync(options.AppUserID, options.DBSubscription.ID, options.LogInterface.Value, options.LogInterfaceUsername));

            return result;

            // success
            //return new TelekomSynchronizationResults()
            //{
            //    ResultCode = TelekomSynchronizationResultCodes.Success
            //};
        }
    }
}
