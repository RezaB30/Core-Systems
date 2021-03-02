using RadiusR.DB.DomainsCache;
using RadiusR.DB.Enums;
using RadiusR.DB.ModelExtentions;
using RadiusR.DB.TelekomOperations;
using RadiusR.DB.Utilities.Billing;
using RadiusR.DB.Utilities.Extentions;
using RadiusR.DB.Utilities.Scheduler;
using RadiusR.SMS;
using RadiusR.SystemLogs;
using RezaB.TurkTelekom.WebServices;
using RezaB.TurkTelekom.WebServices.Exceptions;
using RezaB.TurkTelekom.WebServices.SubscriberInfo;
using RezaB.TurkTelekom.WebServices.TTApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.StateChanges
{
    public static class StateChangeUtilities
    {
        /// <summary>
        /// <para>Changes subscription state to <b>Registered</b>.</para>
        /// <para>Usage: Pre-Registered -&gt; Registered.</para>
        /// </summary>
        /// <param name="subscriptionId"></param>
        /// <param name="registerOptions"></param>
        public static void ChangeSubscriptionState(long subscriptionId, RegisterSubscriptionOptions registerOptions)
        {
            using (RadiusREntities db = new RadiusREntities())
            {
                var subscription = db.Subscriptions.Find(subscriptionId);
                // check if it is a valid change.
                ValidateStateChange(registerOptions, (CustomerState)subscription.State);
                // set subscription state
                var oldState = subscription.State;
                subscription.State = (short)registerOptions.NewState;
                // check transition has xdsl no
                if (subscription.RegistrationType == (short)Enums.SubscriptionRegistrationType.Transition && string.IsNullOrWhiteSpace(subscription.SubscriptionTelekomInfo?.SubscriptionNo))
                {
                    throw new Exception("Transition subscription does not have XDSL no!");
                }
                // if needs Telekom registration
                var domain = DomainsCache.DomainsCache.GetDomainByID(subscription.DomainID);
                TelekomWorkOrder addedTelekomWorkOrder = null;
                if (domain.TelekomCredential != null && subscription.SubscriptionTelekomInfo != null && string.IsNullOrWhiteSpace(subscription.SubscriptionTelekomInfo.SubscriptionNo))
                {
                    // check for transfer
                    if (subscription.RegistrationType == (short)Enums.SubscriptionRegistrationType.Transfer)
                    {
                        var pendingTransfer = subscription.SubscriptionTransferredToHistories.FirstOrDefault(sth => !sth.Date.HasValue);
                        if (pendingTransfer == null)
                        {
                            throw new Exception("No transferring subscription found!");
                        }
                        var transferringSubscription = pendingTransfer.TransferrerSubscription;
                        if (!transferringSubscription.IsActive)
                        {
                            throw new Exception("Transferring subscription is not active!");
                        }
                        if (transferringSubscription.SubscriptionTelekomInfo == null || string.IsNullOrWhiteSpace(transferringSubscription.SubscriptionTelekomInfo.SubscriptionNo))
                        {
                            throw new Exception("Transferring subscription has no valid telekom info!");
                        }
                        if (transferringSubscription.DomainID != subscription.DomainID)
                        {
                            throw new Exception("Transferring subscription domain does not match!");
                        }
                        // transfer subscription
                        CopyTelekomInfo(transferringSubscription.SubscriptionTelekomInfo, subscription.SubscriptionTelekomInfo);
                        // swap login info
                        subscription.Username = transferringSubscription.Username;
                        subscription.RadiusPassword = transferringSubscription.RadiusPassword;
                        // cancel transferring subscription
                        ChangeSubscriptionState(transferringSubscription.ID, new CancelSubscriptionOptions()
                        {
                            AppUserID = registerOptions.AppUserID,
                            LogInterface = registerOptions.LogInterface,
                            LogInterfaceUsername = registerOptions.LogInterfaceUsername,
                            ScheduleSMSes = registerOptions.ScheduleSMSes,
                            CancellationReason = CancellationReason.Transfer,
                            CancellationReasonDescription = string.Format(Resources.StateChanges.TransferCancellationReason, subscription.SubscriberNo),
                            DoNotCancelTelekomService = true,
                            ForceCancellation = true
                        });
                        // set transfer history date
                        pendingTransfer.Date = DateTime.Now;
                        // transfer system log
                        db.SystemLogs.Add(SystemLogProcessor.SubscriptionTransferApplied(registerOptions.AppUserID, subscriptionId, transferringSubscription.ID, subscriptionId, registerOptions.LogInterface, registerOptions.LogInterfaceUsername));
                        db.SystemLogs.Add(SystemLogProcessor.SubscriptionTransferApplied(registerOptions.AppUserID, transferringSubscription.ID, transferringSubscription.ID, subscriptionId, registerOptions.LogInterface, registerOptions.LogInterfaceUsername));
                    }
                    // transition
                    else if (subscription.RegistrationType == (short)SubscriptionRegistrationType.Transition)
                    {
                        TelekomSynchronization.TelekomSynchronizationUtilities.UpdateSubscriberTelekomInfoFromWebService(db, new TelekomSynchronization.TelekomSynchronizationOptions()
                        {
                            AppUserID = registerOptions.AppUserID,
                            LogInterface = registerOptions.LogInterface,
                            LogInterfaceUsername = registerOptions.LogInterfaceUsername,
                            DBSubscription = subscription,
                            DSLNo = subscription.SubscriptionTelekomInfo.SubscriptionNo
                        });
                    }
                    // new registration
                    else
                    {
                        // Telekom registration
                        var results = TelekomRegistration(subscription.Customer, subscription, domain, registerOptions.AppUserID);
                        if (results != null)
                            throw new Exception(results.GetShortMessage());
                        addedTelekomWorkOrder = subscription.TelekomWorkOrders.OrderByDescending(two => two.CreationDate).FirstOrDefault();
                    }
                }

                // set state history
                subscription.AddStateHistory(registerOptions.NewState, registerOptions.AppUserID);
                // add system log
                db.SystemLogs.Add(SystemLogProcessor.ChangeClientState(registerOptions.AppUserID, subscriptionId, registerOptions.LogInterface, registerOptions.LogInterfaceUsername, (CustomerState)oldState, registerOptions.NewState));
                // save
                db.SaveChanges();

                if (addedTelekomWorkOrder != null)
                {
                    // work order log
                    db.SystemLogs.Add(SystemLogProcessor.CreateTelekomWorkOrder(registerOptions.AppUserID, subscription.ID, registerOptions.LogInterface, registerOptions.LogInterfaceUsername, addedTelekomWorkOrder.ID, new SystemLogs.Parameters.TelekomWorkOrderDetails((RadiusR.DB.Enums.TelekomOperations.TelekomOperationType)addedTelekomWorkOrder.OperationTypeID, (RadiusR.DB.Enums.TelekomOperations.TelekomOperationSubType)addedTelekomWorkOrder.OperationSubType, addedTelekomWorkOrder.ManagementCode, addedTelekomWorkOrder.ProvinceCode, addedTelekomWorkOrder.QueueNo, subscription.SubscriptionTelekomInfo.SubscriptionNo)));
                    // save
                    db.SaveChanges();
                }
            }
        }
        /// <summary>
        /// <para>Changes subscription state to <b>Reserved</b>.</para>
        /// <para>Usage: Registered -&gt; Reserved.</para>
        /// </summary>
        /// <param name="subscriptionId">Subscription id.</param>
        /// <param name="reserveOptions">Reserving options.</param>
        public static void ChangeSubscriptionState(long subscriptionId, ReserveSubscriptionOptions reserveOptions)
        {
            using (RadiusREntities db = new RadiusREntities())
            {
                var billingreadySubscription = db.PrepareForBilling(db.Subscriptions.Where(s => s.ID == subscriptionId)).FirstOrDefault();
                // check if it is a valid change.
                ValidateStateChange(reserveOptions, (CustomerState)billingreadySubscription.Subscription.State);
                // set activation dates.
                billingreadySubscription.Subscription.ActivationDate = DateTime.Now;
                // set subscription state
                var oldState = billingreadySubscription.Subscription.State;
                billingreadySubscription.Subscription.State = (short)reserveOptions.NewState;
                // -------last allowed date--------
                // issue bill for pre-invoiced
                if (SchedulerSettings.SchedulerBillingType == (short)SchedulerBillingTypes.PreInvoicing)
                {
                    billingreadySubscription.IssueBill();
                }
                billingreadySubscription.Subscription.UpdateLastAllowedDate();
                // --------------------------------
                if (reserveOptions.SetupServiceRequest != null)
                {
                    // add setup task
                    billingreadySubscription.Subscription.CustomerSetupTasks.Add(new CustomerSetupTask()
                    {
                        Details = reserveOptions.SetupServiceRequest.SetupTaskDescription,
                        HasModem = reserveOptions.SetupServiceRequest.HasModem,
                        ModemName = reserveOptions.SetupServiceRequest.ModemName,
                        SetupUserID = reserveOptions.SetupServiceRequest.SetupUserID,
                        TaskIssueDate = DateTime.Now,
                        TaskStatus = (short)Enums.CustomerSetup.TaskStatuses.New,
                        XDSLType = (short)reserveOptions.SetupServiceRequest.XDSLType,
                        TaskType = (short)Enums.CustomerSetup.TaskTypes.Setup
                    });

                    // add setup fee
                    var setupFee = db.FeeTypeCosts.Find((short)FeeType.Setup);
                    billingreadySubscription.Subscription.Fees.Add(new Fee()
                    {
                        Cost = setupFee.Cost,
                        Date = DateTime.Now,
                        FeeTypeID = setupFee.FeeTypeID,
                        InstallmentBillCount = 1
                    });
                }
                // issue bill for pre-invoiced
                if (SchedulerSettings.SchedulerBillingType == (short)SchedulerBillingTypes.PreInvoicing)
                {
                    billingreadySubscription.IssueBill();
                }
                // telekom only
                if (billingreadySubscription.Subscription.SubscriptionTelekomInfo != null)
                {
                    // close telekom work order
                    {
                        var openWorkOrders = billingreadySubscription.Subscription.TelekomWorkOrders.Where(two => two.IsOpen && two.OperationTypeID == (short)RadiusR.DB.Enums.TelekomOperations.TelekomOperationType.Registration).ToArray();
                        foreach (var workOrder in openWorkOrders)
                        {
                            workOrder.IsOpen = false;
                            workOrder.ClosingDate = DateTime.Now;
                            // add system log
                            db.SystemLogs.Add(SystemLogProcessor.CloseTelekomWorkOrder(reserveOptions.AppUserID, subscriptionId, reserveOptions.LogInterface, reserveOptions.LogInterfaceUsername, workOrder.ID, (Enums.TelekomOperations.TelekomOperationType)workOrder.OperationTypeID, (Enums.TelekomOperations.TelekomOperationSubType)workOrder.OperationSubType));
                        }
                    }

                    // set TT redback name
                    TelekomSynchronization.TelekomSynchronizationUtilities.UpdateSubscriberTelekomInfoFromWebService(db, new TelekomSynchronization.TelekomSynchronizationOptions()
                    {
                        AppUserID = reserveOptions.AppUserID,
                        LogInterface = reserveOptions.LogInterface,
                        LogInterfaceUsername = reserveOptions.LogInterfaceUsername,
                        DBSubscription = billingreadySubscription.Subscription,
                        DSLNo = billingreadySubscription.Subscription.SubscriptionTelekomInfo.SubscriptionNo
                    });
                }
                // set state history
                billingreadySubscription.Subscription.AddStateHistory(reserveOptions.NewState, reserveOptions.AppUserID);
                // add system log
                db.SystemLogs.Add(SystemLogProcessor.ChangeClientState(reserveOptions.AppUserID, subscriptionId, reserveOptions.LogInterface, reserveOptions.LogInterfaceUsername, (CustomerState)oldState, reserveOptions.NewState));
                // save
                db.SaveChanges();
            }
        }
        /// <summary>
        /// <para>Changes subscription state to <b>Active</b>.</para>
        /// <para>Usage: Reserved -&gt; Active.</para>
        /// <para>Usage: Disabled -&gt; Active.</para>
        /// </summary>
        /// <param name="subscriptionId">Subscription id.</param>
        /// <param name="activateOptions">Activating options.</param>
        public static void ChangeSubscriptionState(long subscriptionId, ActivateSubscriptionOptions activateOptions)
        {
            using (RadiusREntities db = new RadiusREntities())
            {
                var billingReadySubscription = db.PrepareForBilling(db.Subscriptions.Where(s => s.ID == subscriptionId)).FirstOrDefault();
                // check if it is a valid change.
                ValidateStateChange(activateOptions, (CustomerState)billingReadySubscription.Subscription.State);
                // remove state change scheduler tasks.
                db.RemoveChangeStateTask(subscriptionId);
                // change state
                var oldState = billingReadySubscription.Subscription.State;
                billingReadySubscription.Subscription.State = (short)activateOptions.NewState;
                // check for unfreeze
                if (oldState == (short)CustomerState.Disabled)
                {
                    // set activation date
                    billingReadySubscription.Subscription.ActivationDate = DateTime.Now.Date;
                    // -------last allowed date--------
                    // issue bill for pre-invoiced
                    if (SchedulerSettings.SchedulerBillingType == (short)SchedulerBillingTypes.PostInvoicing)
                    {
                        billingReadySubscription.IssueBill();
                    }
                    billingReadySubscription.Subscription.UpdateLastAllowedDate();
                    // --------------------------------

                    // check for Telekom unfreeze
                    if (billingReadySubscription.Subscription.SubscriptionTelekomInfo != null)
                    {
                        var domain = DomainsCache.DomainsCache.GetDomainByID(billingReadySubscription.Subscription.DomainID);
                        if (domain != null && domain.TelekomCredential != null)
                        {
                            var serviceClient = new RezaB.TurkTelekom.WebServices.TTApplication.TTApplicationServiceClient(domain.TelekomCredential.XDSLWebServiceUsernameInt, domain.TelekomCredential.XDSLWebServicePassword, billingReadySubscription.Subscription.SubscriptionTelekomInfo.TTCustomerCode);
                            var response = serviceClient.ReleaseClient(billingReadySubscription.Subscription.SubscriptionTelekomInfo.SubscriptionNo, DateTime.Now);
                            if (response.InternalException != null && !activateOptions.ForceUnfreeze)
                            {
                                throw response.InternalException;
                            }
                        }
                    }


                }
                // set state history
                billingReadySubscription.Subscription.AddStateHistory(activateOptions.NewState, activateOptions.AppUserID);
                // add system log
                db.SystemLogs.Add(SystemLogProcessor.ChangeClientState(activateOptions.AppUserID, subscriptionId, activateOptions.LogInterface, activateOptions.LogInterfaceUsername, (CustomerState)oldState, activateOptions.NewState));

                // save
                db.SaveChanges();

                if (oldState == (short)CustomerState.Disabled)
                {
                    if (activateOptions.ScheduleSMSes)
                    {
                        // schedule SMS
                        db.ScheduledSMS.Add(new ScheduledSMS()
                        {
                            CreationDate = DateTime.Now,
                            ExpirationDate = DateTime.Now.Date.AddDays(5),
                            SMSType = (short)SMSType.FreezeDurationEnd,
                            SubscriptionID = billingReadySubscription.Subscription.ID
                        });
                    }
                    else
                    {
                        // send reactivation SMS
                        SMSService smsClient = new SMSService();
                        db.SMSArchives.AddSafely(smsClient.SendSubscriberSMS(billingReadySubscription.Subscription, SMSType.FreezeDurationEnd));
                    }
                }
                else
                {
                    if (activateOptions.ScheduleSMSes)
                    {
                        // schedule SMS
                        db.ScheduledSMS.Add(new ScheduledSMS()
                        {
                            CreationDate = DateTime.Now,
                            ExpirationDate = DateTime.Now.Date.AddDays(5),
                            SMSType = (short)SMSType.Activation,
                            SubscriptionID = billingReadySubscription.Subscription.ID
                        });
                    }
                    else
                    {
                        // send activation sms
                        SMSService smsClient = new SMSService();
                        db.SMSArchives.AddSafely(smsClient.SendSubscriberSMS(billingReadySubscription.Subscription, SMSType.Activation));
                    }
                }

                db.SaveChanges();
            }
        }
        /// <summary>
        /// <para>Changes subscription state to <b>Disabled</b>.</para>
        /// <para>Usage: Active -&gt; Disabled.</para>
        /// </summary>
        /// <param name="subscriptionId">Subscription id.</param>
        /// <param name="freezeOptions">Freezing options.</param>
        public static void ChangeSubscriptionState(long subscriptionId, FreezeSubscriptionOptions freezeOptions)
        {
            using (RadiusREntities db = new RadiusREntities())
            {
                var billingReadySubscription = db.PrepareForBilling(db.Subscriptions.Where(s => s.ID == subscriptionId)).FirstOrDefault();
                var domain = DomainsCache.DomainsCache.GetDomainByID(billingReadySubscription.Subscription.DomainID);
                // check if it is a valid change.
                ValidateStateChange(freezeOptions, (CustomerState)billingReadySubscription.Subscription.State);
                // validate freeze duration
                if (!ValidateFreezingDuration(billingReadySubscription.Subscription, domain, freezeOptions.ReleaseDate))
                    throw new Exception("Invalid freeze duration.");
                // cancel active referral discounts
                foreach (var referralDiscount in billingReadySubscription.ValidRecurringDiscounts.Where(rd => rd.ReferralSubscriptionID.HasValue).ToArray())
                {
                    Discounts.DiscountUtilities.CancelRecurringDiscount(db, referralDiscount.RecurringDiscount, new Discounts.DiscountOperationOptions()
                    {
                        AppUserID = freezeOptions.AppUserID,
                        LogInterface = freezeOptions.LogInterface,
                        LogInterfaceUsername = freezeOptions.LogInterfaceUsername,
                        CancellationCause = Enums.RecurringDiscount.RecurringDiscountCancellationCause.SubscriptionFreeze
                    });
                }
                // create pre-freeze bill
                CreateBillsBeforeFreezing(db, billingReadySubscription);
                // remove activation date
                billingReadySubscription.Subscription.ActivationDate = null;
                // add release scheduled task
                db.AddClientStateChange(freezeOptions.ReleaseDate, subscriptionId, CustomerState.Active);
                // set state history
                billingReadySubscription.Subscription.AddStateHistory(freezeOptions.NewState, freezeOptions.AppUserID);
                // add system log
                db.SystemLogs.Add(SystemLogProcessor.ChangeClientState(freezeOptions.AppUserID, subscriptionId, freezeOptions.LogInterface, freezeOptions.LogInterfaceUsername, (CustomerState)billingReadySubscription.Subscription.State, freezeOptions.NewState));
                // change state
                billingReadySubscription.Subscription.State = (short)freezeOptions.NewState;
                // send telekom request
                if (billingReadySubscription.Subscription.SubscriptionTelekomInfo != null)
                {
                    //var domain = DomainsCache.DomainsCache.GetDomainByID(subscription.DomainID);
                    if (domain != null && domain.TelekomCredential != null)
                    {
                        var freezeClient = new RezaB.TurkTelekom.WebServices.TTApplication.TTApplicationServiceClient(domain.TelekomCredential.XDSLWebServiceUsernameInt, domain.TelekomCredential.XDSLWebServicePassword, billingReadySubscription.Subscription.SubscriptionTelekomInfo.TTCustomerCode);
                        var response = freezeClient.FreezeClient(billingReadySubscription.Subscription.SubscriptionTelekomInfo.SubscriptionNo, freezeOptions.ReleaseDate);
                        if (response.InternalException != null)
                        {
                            if (!freezeOptions.ForceThroughWebService)
                            {
                                throw response.InternalException;
                            }
                        }
                    }
                }
                // save
                db.SaveChanges();
            }
        }
        /// <summary>
        /// <para>Changes subscription state to <b>Cancelled</b>.</para>
        /// <para>Usage: Pre-Registered -&gt; Canceled.</para>
        /// <para>Usage: Registered -&gt; Canceled.</para>
        /// <para>Usage: Reserved -&gt; Canceled.</para>
        /// <para>Usage: Active -&gt; Canceled.</para>
        /// </summary>
        /// <param name="subscriptionId">Subscription id.</param>
        /// <param name="cancelOptions">Cancelling options.</param>
        public static void ChangeSubscriptionState(long subscriptionId, CancelSubscriptionOptions cancelOptions)
        {
            using (RadiusREntities db = new RadiusREntities())
            {
                var billingReadySubscription = db.PrepareForBilling(db.Subscriptions.Where(s => s.ID == subscriptionId)).FirstOrDefault();
                // check if it is a valid change.
                ValidateStateChange(cancelOptions, (CustomerState)billingReadySubscription.Subscription.State);
                // update the new state if it should be dismissed instead of cancelled
                var validForDismissed = new short[]
                {
                    (short)CustomerState.PreRegisterd,
                    (short)CustomerState.Registered
                };
                cancelOptions.IsDismissed = validForDismissed.Contains(billingReadySubscription.Subscription.State) && !billingReadySubscription.Subscription.RadiusAccountings.Any();
                // remove any scheduled task
                if (billingReadySubscription.Subscription.ChangeStateTasks.Any())
                {
                    var toRemoveTasks = billingReadySubscription.Subscription.ChangeStateTasks.Select(t => t.SchedulerTask).ToArray();
                    db.ChangeStateTasks.RemoveRange(billingReadySubscription.Subscription.ChangeStateTasks);
                    db.SchedulerTasks.RemoveRange(toRemoveTasks);
                }
                if (billingReadySubscription.Subscription.ChangeServiceTypeTasks.Any())
                {
                    var toRemoveTasks = billingReadySubscription.Subscription.ChangeServiceTypeTasks.Select(t => t.SchedulerTask).ToArray();
                    db.ChangeServiceTypeTasks.RemoveRange(billingReadySubscription.Subscription.ChangeServiceTypeTasks);
                    db.SchedulerTasks.RemoveRange(toRemoveTasks);
                }
                // remove automatic payments
                if (billingReadySubscription.Subscription.MobilExpressAutoPayment != null)
                    db.MobilExpressAutoPayments.Remove(billingReadySubscription.Subscription.MobilExpressAutoPayment);
                if (billingReadySubscription.Subscription.RecurringPaymentSubscription != null)
                    db.RecurringPaymentSubscriptions.Remove(billingReadySubscription.Subscription.RecurringPaymentSubscription);
                // force issue last bill
                if (billingReadySubscription.Subscription.IsActive)
                {
                    billingReadySubscription.ForceIssueBill(ignoreReferralDiscounts: true, settleAllInstallments: true);
                }
                // set cancellation reason
                db.AddOrUpdateClientCancellation(subscriptionId, cancelOptions.CancellationReason, cancelOptions.CancellationReasonDescription);
                // set end date
                billingReadySubscription.Subscription.EndDate = DateTime.Now;
                // set state history
                billingReadySubscription.Subscription.AddStateHistory(cancelOptions.NewState, cancelOptions.AppUserID);
                // add system log
                db.SystemLogs.Add(SystemLogProcessor.ChangeClientState(cancelOptions.AppUserID, subscriptionId, cancelOptions.LogInterface, cancelOptions.LogInterfaceUsername, (CustomerState)billingReadySubscription.Subscription.State, cancelOptions.NewState));
                // change state
                billingReadySubscription.Subscription.State = (short)cancelOptions.NewState;
                // if should cancel telekom service
                if (!cancelOptions.DoNotCancelTelekomService && !cancelOptions.IsDismissed)
                {
                    // TT cancellation request if available
                    if (billingReadySubscription.Subscription.SubscriptionTelekomInfo != null && !string.IsNullOrWhiteSpace(billingReadySubscription.Subscription.SubscriptionTelekomInfo.SubscriptionNo))
                    {
                        var domain = DomainsCache.DomainsCache.GetDomainByID(billingReadySubscription.Subscription.DomainID);
                        if (domain != null && domain.TelekomCredential != null)
                        {
                            var client = new RezaB.TurkTelekom.WebServices.TTApplication.TTApplicationServiceClient(domain.TelekomCredential.XDSLWebServiceUsernameInt, domain.TelekomCredential.XDSLWebServicePassword, billingReadySubscription.Subscription.SubscriptionTelekomInfo.TTCustomerCode);
                            var response = client.CancelCustomer(billingReadySubscription.Subscription.SubscriptionTelekomInfo.SubscriptionNo);
                            if (response.InternalException != null && !cancelOptions.ForceCancellation)
                            {
                                throw response.InternalException;
                            }
                        }

                        // close telekom work order
                        {
                            var openWorkOrders = billingReadySubscription.Subscription.TelekomWorkOrders.Where(two => two.IsOpen && two.OperationTypeID == (short)RadiusR.DB.Enums.TelekomOperations.TelekomOperationType.Registration).ToArray();
                            foreach (var workOrder in openWorkOrders)
                            {
                                workOrder.IsOpen = false;
                                workOrder.ClosingDate = DateTime.Now;
                                // add system log
                                db.SystemLogs.Add(SystemLogProcessor.CloseTelekomWorkOrder(cancelOptions.AppUserID, subscriptionId, cancelOptions.LogInterface, cancelOptions.LogInterfaceUsername, workOrder.ID, (Enums.TelekomOperations.TelekomOperationType)workOrder.OperationTypeID, (Enums.TelekomOperations.TelekomOperationSubType)workOrder.OperationSubType));
                            }
                        }
                    }
                }
                // cancel active referral discounts
                foreach (var referralDiscount in billingReadySubscription.ValidRecurringDiscounts.Where(rd => rd.ReferralSubscriptionID.HasValue).ToArray())
                {
                    Discounts.DiscountUtilities.CancelRecurringDiscount(db, referralDiscount.RecurringDiscount, new Discounts.DiscountOperationOptions()
                    {
                        AppUserID = cancelOptions.AppUserID,
                        LogInterface = cancelOptions.LogInterface,
                        LogInterfaceUsername = cancelOptions.LogInterfaceUsername,
                        CancellationCause = Enums.RecurringDiscount.RecurringDiscountCancellationCause.SubscriptionCancellation
                    });
                }
                // cancel partner allowance if has not reached valid payment
                if (billingReadySubscription.Subscription.PartnerRegisteredSubscription != null && !billingReadySubscription.Subscription.PartnerRegisteredSubscription.HasFullBills)
                {
                    billingReadySubscription.Subscription.PartnerRegisteredSubscription.AllowanceState = (short)PartnerAllowanceState.Cancelled;
                }
                // cancel pending transfers
                var pendingTransferTos = billingReadySubscription.Subscription.SubscriptionTransferredToHistories.Where(sth => !sth.Date.HasValue).ToArray();
                if (pendingTransferTos.Any())
                {
                    db.SubscriptionTransferHistories.RemoveRange(pendingTransferTos);
                }
                // change username to cancelled
                billingReadySubscription.Subscription.Username += "(c)";
                // save
                db.SaveChanges();
            }
        }

        private static void CreateBillsBeforeFreezing(RadiusREntities db, BillingReadySubscription subscription)
        {
            // freezing fee
            var freezingFee = db.FeeTypeCosts.FirstOrDefault(ft => ft.FeeTypeID == (short)FeeType.Freezing);
            // force issue last bill with freezing fee
            subscription.ForceIssueBill(ignoreReferralDiscounts: true, extraFees: new BillingExtraFee[]
            {
                new BillingExtraFee()
                {
                    Cost = freezingFee.Cost.Value,
                    IsAllTime = false,
                    FeeType = FeeType.Freezing
                }
            });

        }
        /// <summary>
        /// Validates freezing duration.
        /// </summary>
        /// <param name="subscription">The target subscriber</param>
        /// <param name="releaseDate">Realse date from freezing.</param>
        public static bool ValidateFreezingDuration(Subscription subscription, DomainsCache.CachedDomain domain, DateTime releaseDate)
        {
            var freezeDuration = releaseDate.Date - DateTime.Now.Date;
            if (releaseDate.Date <= DateTime.Now.Date || freezeDuration.Days > domain.MaxFreezeDuration)
            {
                return false;
            }
            if (subscription.SubscriptionStateHistories.Count(history => history.ChangeDate.Year == DateTime.Now.Year && (history.NewState == (short)CustomerState.Disabled || history.OldState == (short)CustomerState.Disabled)) >= domain.MaxFreezesPerYear)
                return false;
            return true;
        }

        public static IEnumerable<CustomerState> GetValidStateChanges(CustomerState oldState)
        {
            return ChangeStateOptionsBase.GetValidStateChanges(oldState);
        }

        private static void ValidateStateChange(ChangeStateOptionsBase changeStateOptions, CustomerState oldState)
        {
            if (!changeStateOptions.IsValidChange(oldState))
                throw new Exception(string.Format("Invalid state change: {0}->{1}.", Enum.GetName(typeof(CustomerState), (oldState)), Enum.GetName(typeof(CustomerState), (changeStateOptions.NewState))));
        }

        private static TTWebServiceException TelekomRegistration(Customer dbCustomer, Subscription dbSubscription, CachedDomain selectedDomain, int? appUserId)
        {
            var telekomClient = new TTApplicationServiceClient(selectedDomain.TelekomCredential.XDSLWebServiceUsernameInt, selectedDomain.TelekomCredential.XDSLWebServicePassword, selectedDomain.TelekomCredential.XDSLWebServiceCustomerCodeInt);
            var registrationTicket = TelekomRegistrationTicketFactory.CreateRegistrationTicket(dbCustomer, dbSubscription);
            var response = telekomClient.RegisterSubscriber(registrationTicket);

            // check if a registration work order already saved
            if (response.InternalException != null)
            {
                var checkClient = new TelekomSubscriberInfoServiceClient(selectedDomain.TelekomCredential.XDSLWebServiceUsernameInt, selectedDomain.TelekomCredential.XDSLWebServicePassword);
                var checkResponse = checkClient.GetWorkOrderByBBK(registrationTicket.AddressInfo.ApartmentID);

                if (checkResponse.InternalException == null)
                {
                    // remake response
                    response = new ServiceResponse<ApplicationResponseDescription>()
                    {
                        Data = new ApplicationResponseDescription()
                        {
                            ManagementCode = checkResponse.Data.ManagementCode,
                            ProvinceCode = checkResponse.Data.ProvinceCode,
                            QueueNo = checkResponse.Data.QueueNo,
                            SubscriptionNo = checkResponse.Data.XDSLNo
                        }
                    };
                }
            }
            // final result
            if (response.InternalException != null)
            {
                return response.InternalException;
                //ViewBag.TelekomError = response.InternalException.GetShortMessage();
                //ModelState.AddModelError("TelekomError", RadiusR.Localization.Validation.Common.TelekomError);
            }
            else
            {
                // add to telekom work order queue
                dbSubscription.TelekomWorkOrders = new List<TelekomWorkOrder>()
                            {
                                new TelekomWorkOrder()
                                {
                                    CreationDate = DateTime.Now,
                                    IsOpen = true,
                                    ManagementCode = response.Data.ManagementCode,
                                    ProvinceCode = response.Data.ProvinceCode,
                                    QueueNo = response.Data.QueueNo,
                                    OperationTypeID = (short)RadiusR.DB.Enums.TelekomOperations.TelekomOperationType.Registration,
                                    OperationSubType = (short)(dbSubscription.SubscriptionTelekomInfo.XDSLType == (short)XDSLType.Fiber ? RadiusR.DB.Enums.TelekomOperations.TelekomOperationSubType.FTTX : RadiusR.DB.Enums.TelekomOperations.TelekomOperationSubType.XDSL),
                                    AppUserID = appUserId
                                }
                            };
                // add xdsl no
                dbSubscription.SubscriptionTelekomInfo.SubscriptionNo = response.Data.SubscriptionNo.Trim();
            }

            return null;
        }

        private static void CopyTelekomInfo(SubscriptionTelekomInfo from, SubscriptionTelekomInfo to)
        {
            to.SubscriptionNo = from.SubscriptionNo;
            to.PSTN = from.PSTN;
            to.RedbackName = from.RedbackName;
            to.PacketCode = from.PacketCode;
            to.TariffCode = from.TariffCode;
            to.TTCustomerCode = from.TTCustomerCode;
            to.XDSLType = from.XDSLType;
            to.IsPaperWorkNeeded = from.IsPaperWorkNeeded;
        }
    }
}
