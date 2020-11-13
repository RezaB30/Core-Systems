using RadiusR.DB.Enums;
using RadiusR.DB.ModelExtentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadiusR.DB.Utilities.Billing;
using RadiusR.SystemLogs;

namespace RadiusR.DB.Utilities.ComplexOperations.Subscriptions.TariffChanges
{
    public static class SubscriptionTariffUtilities
    {
        public static TariffChangeResult ChangeSubscriptionTariff(this RadiusREntities db, TariffChangeOptions options)
        {
            var dbSubscription = db.Subscriptions.Find(options.SubscriptionID);
            var newTariff = db.Services.Find(options.TariffID);
            if (!newTariff.HasBilling)
                options.NewBillingPeriod = 1;
            if (options.Gateway == null || options.Gateway.InterfaceType == null || newTariff == null || dbSubscription == null || (newTariff.HasBilling && !newTariff.ServiceBillingPeriods.Any(sbp => sbp.DayOfMonth == options.NewBillingPeriod)))
            {
                return TariffChangeResult.InvalidInput;
            }
            if (!newTariff.Domains.Any(d => d.ID == dbSubscription.DomainID))
            {
                return TariffChangeResult.InvalidDomain;
            }
            if (dbSubscription.ServiceID == newTariff.ID && options.NewBillingPeriod == dbSubscription.PaymentDay)
            {
                return TariffChangeResult.TariffChanged;
            }

            var results = TariffChangeResult.TariffChanged;
            // remove any scheduled change tariff task
            if (dbSubscription.ChangeServiceTypeTasks.Any())
            {
                var allScheduledChangeTariffTasks = dbSubscription.ChangeServiceTypeTasks.ToArray();
                foreach (var scheduledTask in allScheduledChangeTariffTasks)
                {
                    var task = scheduledTask.SchedulerTask;
                    db.ChangeServiceTypeTasks.Remove(scheduledTask);
                    db.SchedulerTasks.Remove(task);

                    db.SystemLogs.Add(SystemLogProcessor.CancelScheduledChangeService(options.Gateway.UserID, dbSubscription.ID, options.Gateway.InterfaceType.Value, options.Gateway.InterfaceUsername));
                }
            }
            // invalid for cancelled subs
            if (dbSubscription.IsCancelled)
            {
                return TariffChangeResult.InvalidSubscriptionState;
            }
            // instant change for inactive subs
            else if (!dbSubscription.IsActive)
            {
                var oldTariffName = dbSubscription.Service.Name;
                // add to history and update last tariff change date
                dbSubscription.LastTariffChangeDate = DateTime.Now;
                dbSubscription.SubscriptionTariffHistories.Add(new SubscriptionTariffHistory()
                {
                    Date = DateTime.Now,
                    NewTariffID = newTariff.ID,
                    OldTariffID = dbSubscription.ServiceID,
                    SubscriptionID = dbSubscription.ID
                });
                // change tariff
                dbSubscription.ServiceID = newTariff.ID;
                dbSubscription.PaymentDay = options.NewBillingPeriod;

                db.SystemLogs.Add(SystemLogProcessor.ChangeService(options.Gateway.UserID, dbSubscription.ID, options.Gateway.InterfaceType.Value, options.Gateway.InterfaceUsername, oldTariffName, newTariff.Name));
            }
            // process for active subs
            else
            {
                //------------------------ from pre-paid tariff
                if (!dbSubscription.Service.HasBilling)
                {
                    if (options.ForceNow)
                    {
                        var oldTariffName = dbSubscription.Service.Name;
                        // add to history and update last tariff change date
                        dbSubscription.LastTariffChangeDate = DateTime.Now;
                        dbSubscription.SubscriptionTariffHistories.Add(new SubscriptionTariffHistory()
                        {
                            Date = DateTime.Now,
                            NewTariffID = newTariff.ID,
                            OldTariffID = dbSubscription.ServiceID,
                            SubscriptionID = dbSubscription.ID
                        });
                        // change tariff
                        dbSubscription.Service = newTariff;
                        dbSubscription.PaymentDay = options.NewBillingPeriod;
                        // change activation date
                        dbSubscription.ActivationDate = DateTime.Now.Date;
                        // create bill
                        var billReadySubscription = db.PrepareForBilling(db.Subscriptions.Where(s => s.ID == dbSubscription.ID)).FirstOrDefault();
                        billReadySubscription.Subscription = dbSubscription;
                        billReadySubscription.IssueBill();
                        // update last allowed date
                        dbSubscription.UpdateLastAllowedDate(forceChange: true);

                        db.SystemLogs.Add(SystemLogProcessor.ChangeService(options.Gateway.UserID, dbSubscription.ID, options.Gateway.InterfaceType.Value, options.Gateway.InterfaceUsername, oldTariffName, newTariff.Name));
                    }
                    else
                    {
                        // add new scheduled tariff task
                        db.ChangeServiceTypeTasks.Add(new ChangeServiceTypeTask()
                        {
                            NewServiceID = newTariff.ID,
                            SubscriptionID = dbSubscription.ID,
                            NewBillingPeriod = options.NewBillingPeriod,
                            SchedulerTask = new SchedulerTask()
                            {
                                ExecuteDate = dbSubscription.LastAllowedDate.Value
                            }
                        });

                        db.SystemLogs.Add(SystemLogProcessor.ChangeServiceSchedule(options.Gateway.UserID, dbSubscription.ID, options.Gateway.InterfaceType.Value, options.Gateway.InterfaceUsername, dbSubscription.Service.Name, newTariff.Name));

                        results = TariffChangeResult.TariffChangeScheduled;
                    }
                }
                //-------------------------- from billed tariff
                // destination tariff is pre-paid
                else if (!newTariff.HasBilling)
                {
                    var oldTariffName = dbSubscription.Service.Name;
                    // create bill
                    var billReadySubscription = db.PrepareForBilling(db.Subscriptions.Where(s => s.ID == dbSubscription.ID)).FirstOrDefault();
                    billReadySubscription.Subscription = dbSubscription;
                    billReadySubscription.ForceIssueBill(settleAllInstallments: true);
                    // remove activation date
                    dbSubscription.ActivationDate = null;
                    // add to history and update last tariff change date
                    dbSubscription.LastTariffChangeDate = DateTime.Now;
                    dbSubscription.SubscriptionTariffHistories.Add(new SubscriptionTariffHistory()
                    {
                        Date = DateTime.Now,
                        NewTariffID = newTariff.ID,
                        OldTariffID = dbSubscription.ServiceID,
                        SubscriptionID = dbSubscription.ID
                    });
                    // change tariff
                    dbSubscription.ServiceID = newTariff.ID;
                    dbSubscription.PaymentDay = options.NewBillingPeriod;
                    // update last allowed date
                    dbSubscription.LastAllowedDate = DateTime.Today;

                    db.SystemLogs.Add(SystemLogProcessor.ChangeService(options.Gateway.UserID, dbSubscription.ID, options.Gateway.InterfaceType.Value, options.Gateway.InterfaceUsername, oldTariffName, newTariff.Name));
                }
                // post-invoicing
                else if (SchedulerSettings.SchedulerBillingType == (short)SchedulerBillingTypes.PostInvoicing)
                {
                    var oldTariffName = dbSubscription.Service.Name;
                    // issue bills if any is due
                    var billReadySubscription = db.PrepareForBilling(db.Subscriptions.Where(s => s.ID == dbSubscription.ID)).FirstOrDefault();
                    billReadySubscription.Subscription = dbSubscription;
                    billReadySubscription.IssueBill();
                    // add to history and update last tariff change date
                    dbSubscription.LastTariffChangeDate = DateTime.Now;
                    dbSubscription.SubscriptionTariffHistories.Add(new SubscriptionTariffHistory()
                    {
                        Date = DateTime.Now,
                        NewTariffID = newTariff.ID,
                        OldTariffID = dbSubscription.ServiceID,
                        SubscriptionID = dbSubscription.ID
                    });
                    // add previous usage
                    var beforeChangePeriod = dbSubscription.GetCurrentBillingPeriod(DateTime.Today.AddDays(-1));
                    if (!beforeChangePeriod.IsBilled)
                    {
                        var addingFeeCost = dbSubscription.GetCurrentTariffCost(db.RadiusDailyAccountings.Where(rda => rda.SubscriptionID == dbSubscription.ID && rda.Date >= beforeChangePeriod.StartDate && rda.Date < beforeChangePeriod.EndDate).Select(rda => new SubscriptionUtilities.DailyUsageInfo() { Date = rda.Date, DownloadBytes = rda.DownloadBytes, UploadBytes = rda.UploadBytes }).ToArray(), beforeChangePeriod);
                        if (addingFeeCost.HasValue && addingFeeCost > 0m)
                        {
                            dbSubscription.Fees.Add(new Fee()
                            {
                                Cost = addingFeeCost.Value,
                                FeeTypeID = (short)FeeType.TariffDebt,
                                Date = DateTime.Now,
                                Description = dbSubscription.Service.Name,
                                InstallmentBillCount = 1,
                                IsCancelled = false,
                                StartDate = beforeChangePeriod.StartDate,
                                EndDate = beforeChangePeriod.EndDate
                            });
                        }
                    }
                    // change tariff
                    dbSubscription.ServiceID = newTariff.ID;
                    dbSubscription.PaymentDay = options.NewBillingPeriod;
                    // update last allowed date
                    dbSubscription.UpdateLastAllowedDate();

                    db.SystemLogs.Add(SystemLogProcessor.ChangeService(options.Gateway.UserID, dbSubscription.ID, options.Gateway.InterfaceType.Value, options.Gateway.InterfaceUsername, oldTariffName, newTariff.Name));
                }
                // pre-invoicing
                else if (SchedulerSettings.SchedulerBillingType == (short)SchedulerBillingTypes.PreInvoicing)
                {
                    // if is forced
                    if (options.ForceNow)
                    {
                        var oldTariffName = dbSubscription.Service.Name;
                        // issue bills if any is due
                        var billReadySubscription = db.PrepareForBilling(db.Subscriptions.Where(s => s.ID == dbSubscription.ID)).FirstOrDefault();
                        billReadySubscription.Subscription = dbSubscription;
                        // add to history and update last tariff change date
                        dbSubscription.LastTariffChangeDate = DateTime.Now;
                        dbSubscription.SubscriptionTariffHistories.Add(new SubscriptionTariffHistory()
                        {
                            Date = DateTime.Now,
                            NewTariffID = newTariff.ID,
                            OldTariffID = dbSubscription.ServiceID,
                            SubscriptionID = dbSubscription.ID
                        });
                        // change tariff
                        dbSubscription.Service = newTariff;
                        dbSubscription.PaymentDay = options.NewBillingPeriod;
                        // update last allowed date
                        dbSubscription.UpdateLastAllowedDate(forceChange: true);
                        // curb last bill period end
                        var relevantBill = billReadySubscription.Subscription.Bills.Where(b => b.PeriodStart < dbSubscription.LastTariffChangeDate && b.PeriodEnd > dbSubscription.LastTariffChangeDate).FirstOrDefault();
                        relevantBill.PeriodEnd = dbSubscription.LastTariffChangeDate;
                        // issue new bill
                        billReadySubscription.IssueBill();
                        // update last allowed date
                        dbSubscription.UpdateLastAllowedDate();

                        db.SystemLogs.Add(SystemLogProcessor.ChangeService(options.Gateway.UserID, dbSubscription.ID, options.Gateway.InterfaceType.Value, options.Gateway.InterfaceUsername, oldTariffName, newTariff.Name));
                    }
                    // not forced
                    else
                    {
                        var currentBillingPeriod = dbSubscription.GetCurrentBillingPeriod();
                        db.ChangeServiceTypeTasks.Add(new ChangeServiceTypeTask()
                        {
                            SubscriptionID = dbSubscription.ID,
                            NewBillingPeriod = options.NewBillingPeriod,
                            NewServiceID = newTariff.ID,
                            SchedulerTask = new SchedulerTask()
                            {
                                ExecuteDate = currentBillingPeriod.EndDate
                            }
                        });

                        db.SystemLogs.Add(SystemLogProcessor.ChangeServiceSchedule(options.Gateway.UserID, dbSubscription.ID, options.Gateway.InterfaceType.Value, options.Gateway.InterfaceUsername, dbSubscription.Service.Name, newTariff.Name));

                        results = TariffChangeResult.TariffChangeScheduled;
                    }
                }
            }



            db.SaveChanges();
            return results;
        }
    }
}
