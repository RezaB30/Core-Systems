using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Scheduling;
using NLog;
using System.Net;
using System.Net.Sockets;
using RadiusR.API.MobilExpress.DBAdapter.AdapterClient;
using RadiusR.DB;
using System.Data.Entity;
using RadiusR.DB.Enums;
using RadiusR.Scheduler.SMS;
using RadiusR.DB.Utilities.Billing;
using RadiusR.SystemLogs;

namespace RadiusR.Scheduler.Tasks
{
    class AutomaticPaymentTasks : AbortableTask
    {
        private static Logger logger = LogManager.GetLogger("automatic-payment-tasks");
        private static Logger successLogger = LogManager.GetLogger("automatic-payment-success");
        private static Logger failLogger = LogManager.GetLogger("automatic-payment-fail");
        private const int batchSize = 100;

        public override bool Run()
        {
            try
            {
                // create payment service client
                var paymentClient = new MobilExpressAdapterClient(MobilExpressSettings.MobilExpressMerchantKey, MobilExpressSettings.MobilExpressAPIPassword, new ClientConnectionDetails()
                {
                    IP = GetIPAddress(),
                    UserAgent = "RadiusR Scheduler/Windows Service"
                });

                var today = DateTime.Today;

                // on issue date automatic payments
                {
                    // set for first batch
                    long minSubsID = 0;
                    // on issue payments
                    while (true)
                    {
                        // abort by flag
                        if (_isAborted)
                        {
                            logger.Debug("Aborted by the scheduler.");
                            return false;
                        }
                        using (RadiusREntities db = new RadiusREntities())
                        {
                            //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                            // base query
                            var baseQuery = db.MobilExpressAutoPayments
                                .OrderBy(ap => ap.SubscriptionID)
                                .Include(ap => ap.Subscription.Bills.Select(b => b.BillFees.Select(bf => bf.Discount)))
                                .Include(ap => ap.Subscription.Customer)
                                .Include(ap => ap.Subscription.SubscriptionCredits)
                                .Where(ap => ap.PaymentType == (short)AutoPaymentType.OnBillIssue).Where(ap => !ap.LastOperationTime.HasValue || ap.LastOperationTime < today).OrderBy(ap => ap.SubscriptionID).Where(ap => ap.SubscriptionID > minSubsID);
                            // get the batch
                            var currentBatch = baseQuery.Take(batchSize).ToArray();
                            // finish if nothing left
                            if (currentBatch.Count() == 0)
                                break;
                            // set for next batch
                            minSubsID = currentBatch.Max(ap => ap.SubscriptionID);
                            // iterate payments
                            foreach (var automaticPayment in currentBatch)
                            {
                                // abort by flag
                                if (_isAborted)
                                {
                                    logger.Debug("Aborted by the scheduler.");
                                    return false;
                                }
                                try
                                {
                                    // get unpaid bills
                                    var unpaidBills = automaticPayment.Subscription.Bills.Where(b => b.BillStatusID == (short)BillState.Unpaid).ToArray();
                                    // skip if has an unsuccessful tried unpaid bill
                                    if (automaticPayment.LastOperationTime.HasValue && unpaidBills.Any(b => b.IssueDate < automaticPayment.LastOperationTime))
                                        continue;
                                    // iterate bills
                                    foreach (var bill in unpaidBills)
                                    {
                                        try
                                        {
                                            // send payment request
                                            var response = paymentClient.PayBill(bill.Subscription.Customer, bill, automaticPayment.CardToken);
                                            if (response.InternalException != null)
                                            {
                                                logger.Warn(response.InternalException, $"Error sending payment request through web service for bill ID '{bill.ID}': 'PayBill'");
                                                continue;
                                            }
                                            if (response.Response.ResponseCode != RezaB.API.MobilExpress.Response.ResponseCodes.Success)
                                            {
                                                automaticPayment.LastOperationTime = DateTime.Now;
                                                // send sms for unsuccessful payment
                                                var results = bill.AddFailedAutoPaymentSMS(response.Response.ErrorMessage);
                                                if (results.ToAdd != null)
                                                    db.ScheduledSMS.Add(results.ToAdd);
                                                // log results
                                                failLogger.Warn($"Automatic payment for bill ID '{bill.ID}' subscriber No '{bill.Subscription.SubscriberNo}' returned error with code: {response.Response.ResponseCode:G} MESSAGE: {response.Response.ErrorMessage}");
                                                break;
                                            }
                                            successLogger.Info($"Successful payment for bill ID '{bill.ID}' subscriber No '{bill.Subscription.SubscriberNo}'");
                                            db.PayBills(new[] { bill }, PaymentType.MobilExpress, BillPayment.AccountantType.Admin);
                                            db.SystemLogs.Add(SystemLogProcessor.BillPayment(new[] { bill.ID }, null, bill.SubscriptionID, SystemLogInterface.RadiusRScheduler, null, PaymentType.MobilExpress));
                                            bill.AddPaymentBillSMS();
                                            // save
                                            db.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error(ex, $"Error payment for bill ID '{bill.ID}' subscriber No '{bill.Subscription.SubscriberNo}'");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex, $"General error in automatic payment for subscriber no: {automaticPayment.Subscription.SubscriberNo}");
                                }
                            }
                        }
                    }
                }
                // on last date payments
                {
                    // set for first batch
                    long minSubsID = 0;
                    // on expiration payments
                    while (true)
                    {
                        // abort by flag
                        if (_isAborted)
                        {
                            logger.Debug("Aborted by the scheduler.");
                            return false;
                        }
                        using (RadiusREntities db = new RadiusREntities())
                        {
                            // base query
                            var baseQuery = db.MobilExpressAutoPayments
                                .OrderBy(ap => ap.SubscriptionID)
                                .Include(ap => ap.Subscription.Bills.Select(b => b.BillFees.Select(bf => bf.Discount)))
                                .Include(ap => ap.Subscription.Customer)
                                .Include(ap => ap.Subscription.SubscriptionCredits)
                                .Where(ap => ap.PaymentType == (short)AutoPaymentType.OnBillExpiration).Where(ap => !ap.LastOperationTime.HasValue || ap.LastOperationTime < today).OrderBy(ap => ap.SubscriptionID).Where(ap => ap.SubscriptionID > minSubsID);
                            // get the batch
                            var currentBatch = baseQuery.Take(batchSize).ToArray();
                            // finish if nothing left
                            if (currentBatch.Count() == 0)
                                break;
                            // set for next batch
                            minSubsID = currentBatch.Max(ap => ap.SubscriptionID);
                            // iterate payments
                            foreach (var automaticPayment in currentBatch)
                            {
                                // abort by flag
                                if (_isAborted)
                                {
                                    logger.Debug("Aborted by the scheduler.");
                                    return false;
                                }
                                try
                                {
                                    // get unpaid bills
                                    var unpaidBills = automaticPayment.Subscription.Bills.Where(b => b.PaymentTypeID == (short)PaymentType.None).ToArray().Where(b => b.DueDate <= today).ToArray();
                                    // skip if has an unsuccessful tried unpaid bill
                                    if (unpaidBills.Any(b => b.DueDate < automaticPayment.LastOperationTime))
                                        continue;
                                    // iterate bills
                                    foreach (var bill in unpaidBills)
                                    {
                                        try
                                        {
                                            // send payment request
                                            var response = paymentClient.PayBill(bill.Subscription.Customer, bill, automaticPayment.CardToken);
                                            if (response.InternalException != null)
                                            {
                                                logger.Warn(response.InternalException, $"Error sending payment request through web service for bill ID '{bill.ID}': 'PayBill'");
                                                continue;
                                            }
                                            if (response.Response.ResponseCode != RezaB.API.MobilExpress.Response.ResponseCodes.Success)
                                            {
                                                automaticPayment.LastOperationTime = DateTime.Now;
                                                // send sms for unsuccessful payment
                                                var results = bill.AddFailedAutoPaymentSMS(response.Response.ErrorMessage);
                                                if (results.ToAdd != null)
                                                    db.ScheduledSMS.Add(results.ToAdd);
                                                // log results
                                                failLogger.Warn($"Automatic payment for bill ID '{bill.ID}' subscriber No '{bill.Subscription.SubscriberNo}' returned error with code: {response.Response.ResponseCode:G} MESSAGE: {response.Response.ErrorMessage}");
                                                break;
                                            }
                                            successLogger.Trace($"Successful payment for bill ID '{bill.ID}' subscriber No '{bill.Subscription.SubscriberNo}'");
                                            db.PayBills(new[] { bill }, PaymentType.MobilExpress, BillPayment.AccountantType.Admin);
                                            db.SystemLogs.Add(SystemLogProcessor.BillPayment(new[] { bill.ID }, null, bill.SubscriptionID, SystemLogInterface.RadiusRScheduler, null, PaymentType.MobilExpress));
                                            bill.AddPaymentBillSMS();
                                            // save
                                            db.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error(ex, $"Error payment for bill ID '{bill.ID}' subscriber No '{bill.Subscription.SubscriberNo}'");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error(ex, $"General error in automatic payment for subscriber no: {automaticPayment.Subscription.SubscriberNo}");
                                }
                            }
                        }
                    }
                }
                logger.Info("Automatic payments done.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "General error in main body.");
                return false;
            }
        }

        private string GetIPAddress()
        {
            var PrivateIPRanges = new[]
            {
                new { Start = (uint)167772160, End = (uint)184549375}, // 10.0.0.0 - 10.255.255.255
                new { Start = (uint)2886729728, End = (uint)2887778303}, // 172.16.0.0 - 172.31.255.255
                new { Start = (uint)3232235520, End = (uint)3232301055}, // 192.168.0.0 - 192.168.255.255
            };
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList.Where(i => i.AddressFamily == AddressFamily.InterNetwork))
            {
                if (!PrivateIPRanges.Any(r => r.Start <= BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0) && r.End >= BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0)))
                {
                    return ip.ToString();
                }
            }

            return "10.0.0.0";
        }
    }
}
