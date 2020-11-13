//using NLog;
//using RadiusR.DB;
//using RadiusR.DB.Enums;
//using RadiusR.DB.Utilities.Billing;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Data.Entity;
//using RadiusR.API.MobilExpress.DBAdapter.AdapterClient;
//using System.Net;
//using System.Net.Sockets;
//using RadiusR.SystemLogs;
//using RadiusR.SMS;
//using RadiusR.Scheduler.SMS;

//namespace RadiusR.Scheduler
//{
//    public static partial class Scheduler
//    {
//        private static bool _isAutomaticPaymentRunning = false;
//        private static DateTime _lastSuccessfulAutomaticPayment = DateTime.MinValue;
//        private static Logger AutomaticPaymentExceptionLogger = LogManager.GetLogger("automatic_payments_exceptions");
//        private static Logger AutomaticPaymentSuccessLogger = LogManager.GetLogger("automatic_payments_success");
//        private static Logger AutomaticPaymentFailLogger = LogManager.GetLogger("automatic_payments_fail");
//        public static void AutomaticPayments()
//        {
//            try
//            {
//                // create payment service client
//                var paymentClient = new MobilExpressAdapterClient(MobilExpressSettings.MobilExpressMerchantKey, MobilExpressSettings.MobilExpressAPIPassword, new ClientConnectionDetails()
//                {
//                    IP = GetIPAddress(),
//                    UserAgent = "RadiusR Scheduler/Windows Service"
//                });
                
//                var today = DateTime.Today;

//                // on issue date automatic payments
//                {
//                    // set for first batch
//                    long minSubsID = 0;
//                    // on issue payments
//                    while (true)
//                    {
//                        using (RadiusREntities db = new RadiusREntities())
//                        {
//                            //db.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
//                            // base query
//                            var baseQuery = db.MobilExpressAutoPayments
//                                .OrderBy(ap => ap.SubscriptionID)
//                                .Include(ap => ap.Subscription.Bills.Select(b => b.BillFees.Select(bf => bf.Discount)))
//                                .Include(ap => ap.Subscription.Customer)
//                                .Include(ap => ap.Subscription.SubscriptionCredits)
//                                .Where(ap => ap.PaymentType == (short)AutoPaymentType.OnBillIssue).Where(ap => !ap.LastOperationTime.HasValue || ap.LastOperationTime < today).OrderBy(ap => ap.SubscriptionID).Where(ap => ap.SubscriptionID > minSubsID);
//                            // get the batch
//                            var currentBatch = baseQuery.Take(batchSize).ToArray();
//                            // finish if nothing left
//                            if (currentBatch.Count() == 0)
//                                break;
//                            // set for next batch
//                            minSubsID = currentBatch.Max(ap => ap.SubscriptionID);
//                            // batch changes
//                            var toAddScheduleSMSes = new List<ScheduledSMS>();
//                            var toAddSystemLogs = new List<SystemLog>();
//                            // iterate payments
//                            foreach (var automaticPayment in currentBatch)
//                            {
//                                // get unpaid bills
//                                var unpaidBills = automaticPayment.Subscription.Bills.Where(b => b.BillStatusID == (short)BillState.Unpaid).ToArray();
//                                // skip if has an unsuccessful tried unpaid bill
//                                if (automaticPayment.LastOperationTime.HasValue && unpaidBills.Any(b => b.IssueDate < automaticPayment.LastOperationTime))
//                                    continue;
//                                // iterate bills
//                                foreach (var bill in unpaidBills)
//                                {
//                                    try
//                                    {
//                                        // send payment request
//                                        var response = paymentClient.PayBill(bill.Subscription.Customer, bill, automaticPayment.CardToken);
//                                        if (response.InternalException != null)
//                                        {
//                                            AutomaticPaymentExceptionLogger.Warn(response.InternalException, "Error sending payment request through web service for bill ID '{0}': 'PayBill'", bill.ID.ToString());
//                                            continue;
//                                        }
//                                        if (response.Response.ResponseCode != RezaB.API.MobilExpress.Response.ResponseCodes.Success)
//                                        {
//                                            automaticPayment.LastOperationTime = DateTime.Now;
//                                            // send sms for unsuccessful payment
//                                            var results = bill.AddFailedAutoPaymentSMS(response.Response.ErrorMessage);
//                                            if (results.ToAdd != null)
//                                                toAddScheduleSMSes.Add(results.ToAdd);
//                                            // log results
//                                            AutomaticPaymentFailLogger.Warn("Automatic payment for bill ID '{0}' subscriber No '{1}' returned error with code: {2} MESSAGE: {3}", bill.ID.ToString(), bill.Subscription.SubscriberNo, response.Response.ResponseCode.ToString(), response.Response.ErrorMessage);
//                                            break;
//                                        }
//                                        AutomaticPaymentSuccessLogger.Trace("Successful payment for bill ID '{0}' subscriber No '{1}'", bill.ID.ToString(), bill.Subscription.SubscriberNo);
//                                        db.PayBills(new[] { bill }, PaymentType.MobilExpress, BillPayment.AccountantType.Admin);
//                                        toAddSystemLogs.Add(SystemLogProcessor.BillPayment(new[] { bill.ID }, null, bill.SubscriptionID, SystemLogInterface.RadiusRScheduler, null, PaymentType.MobilExpress));
//                                        bill.AddPaymentBillSMS();
//                                    }
//                                    catch (Exception ex)
//                                    {
//                                        AutomaticPaymentExceptionLogger.Error(ex, "Error payment for bill ID '{0}' subscriber No '{1}'", bill.ID.ToString(), bill.Subscription.SubscriberNo);
//                                    }
//                                }
//                            }

//                            if (toAddScheduleSMSes.Any())
//                                db.ScheduledSMS.AddRange(toAddScheduleSMSes);
//                            if (toAddSystemLogs.Any())
//                                db.SystemLogs.AddRange(toAddSystemLogs);

//                            db.SaveChanges();
//                        }
//                    }
//                }
//                // on last date payments
//                {
//                    // set for first batch
//                    long minSubsID = 0;
//                    // on expiration payments
//                    while (true)
//                    {
//                        using (RadiusREntities db = new RadiusREntities())
//                        {
//                            // base query
//                            var baseQuery = db.MobilExpressAutoPayments
//                                .Include(ap => ap.Subscription.Bills)
//                                .Include(ap => ap.Subscription.Customer)
//                                .Where(ap => ap.PaymentType == (short)AutoPaymentType.OnBillExpiration).Where(ap => !ap.LastOperationTime.HasValue || ap.LastOperationTime < today).OrderBy(ap => ap.SubscriptionID).Where(ap => ap.SubscriptionID > minSubsID);
//                            // get the batch
//                            var currentBatch = baseQuery.Take(batchSize).ToArray();
//                            // finish if nothing left
//                            if (currentBatch.Count() == 0)
//                                break;
//                            // set for next batch
//                            minSubsID = currentBatch.Max(ap => ap.SubscriptionID);
//                            // batch changes
//                            var toAddScheduleSMSes = new List<ScheduledSMS>();
//                            var toAddSystemLogs = new List<SystemLog>();
//                            // iterate payments
//                            foreach (var automaticPayment in currentBatch)
//                            {
//                                // get unpaid bills
//                                var unpaidBills = automaticPayment.Subscription.Bills.Where(b => b.PaymentTypeID == (short)PaymentType.None).ToArray().Where(b => b.DueDate <= today).ToArray();
//                                // skip if has an unsuccessful tried unpaid bill
//                                if (unpaidBills.Any(b => b.DueDate < automaticPayment.LastOperationTime))
//                                    continue;
//                                // iterate bills
//                                foreach (var bill in unpaidBills)
//                                {
//                                    try
//                                    {
//                                        // send payment request
//                                        var response = paymentClient.PayBill(bill.Subscription.Customer, bill, automaticPayment.CardToken);
//                                        if (response.InternalException != null)
//                                        {
//                                            AutomaticPaymentExceptionLogger.Warn(response.InternalException, "Error sending payment request through web service for bill ID '{0}': 'PayBill'", bill.ID.ToString());
//                                            continue;
//                                        }
//                                        if (response.Response.ResponseCode != RezaB.API.MobilExpress.Response.ResponseCodes.Success)
//                                        {
//                                            automaticPayment.LastOperationTime = DateTime.Now;
//                                            // send sms for unsuccessful payment
//                                            var results = bill.AddFailedAutoPaymentSMS(response.Response.ErrorMessage);
//                                            if (results.ToAdd != null)
//                                                toAddScheduleSMSes.Add(results.ToAdd);
//                                            // log results
//                                            AutomaticPaymentFailLogger.Warn("Automatic payment for bill ID '{0}' subscriber No '{1}' returned error with code: {2} MESSAGE: {3}", bill.ID.ToString(), bill.Subscription.SubscriberNo, response.Response.ResponseCode.ToString(), response.Response.ErrorMessage);

//                                            break;
//                                        }
//                                        AutomaticPaymentSuccessLogger.Trace("Successful payment for bill ID '{0}' subscriber No '{1}'", bill.ID.ToString(), bill.Subscription.SubscriberNo);
//                                        db.PayBills(new[] { bill }, PaymentType.MobilExpress, BillPayment.AccountantType.Admin);
//                                        toAddSystemLogs.Add(SystemLogProcessor.BillPayment(new[] { bill.ID }, null, bill.SubscriptionID, SystemLogInterface.RadiusRScheduler, null, PaymentType.MobilExpress));
//                                        bill.AddPaymentBillSMS();
//                                    }
//                                    catch (Exception ex)
//                                    {
//                                        AutomaticPaymentExceptionLogger.Error(ex, "Error payment for bill ID '{0}' subscriber No '{1}'", bill.ID.ToString(), bill.Subscription.SubscriberNo);
//                                    }
//                                }
//                            }

//                            if (toAddScheduleSMSes.Any())
//                                db.ScheduledSMS.AddRange(toAddScheduleSMSes);
//                            if (toAddSystemLogs.Any())
//                                db.SystemLogs.AddRange(toAddSystemLogs);

//                            db.SaveChanges();
//                        }
//                    }
//                }
//                _lastSuccessfulAutomaticPayment = DateTime.Now;
//                logger.Trace("Automatic payments done.");
//            }
//            catch (Exception ex)
//            {
//                AutomaticPaymentExceptionLogger.Fatal(ex, "error in main body.");
//            }
//            finally
//            {
//                _isAutomaticPaymentRunning = false;
//            }
//        }

//        private static string GetIPAddress()
//        {
//            var PrivateIPRanges = new[]
//            {
//                new { Start = (uint)167772160, End = (uint)184549375}, // 10.0.0.0 - 10.255.255.255
//                new { Start = (uint)2886729728, End = (uint)2887778303}, // 172.16.0.0 - 172.31.255.255
//                new { Start = (uint)3232235520, End = (uint)3232301055}, // 192.168.0.0 - 192.168.255.255
//            };
//            var host = Dns.GetHostEntry(Dns.GetHostName());

//            foreach (var ip in host.AddressList.Where(i => i.AddressFamily == AddressFamily.InterNetwork))
//            {
//                if (!PrivateIPRanges.Any(r => r.Start <= BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0) && r.End >= BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0)))
//                {
//                    return ip.ToString();
//                }
//            }

//            return "10.0.0.0";
//        }
//    }
//}
