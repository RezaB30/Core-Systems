using RadiusR.DB;
using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.Scheduler.SMS
{
    public static class SMSSchedulerManager
    {
        public static void AddNewBillSMS(this Bill dbBill)
        {
            dbBill.ScheduledSMSes.Add(new ScheduledSMS()
            {
                CreationDate = DateTime.Now,
                SMSType = (short)SMSType.NewBill,
                SubscriptionID = dbBill.SubscriptionID
            });
        }

        public static ScheduledSMSResult AddPaymentReminderSMS(this Bill dbBill)
        {
            var oldScheduledSMS = dbBill.Subscription.ScheduledSMSes.FirstOrDefault(ss => ss.SMSType == (short)SMSType.PaymentReminder);

            //dbBill.Subscription.ScheduledSMSes.Add(new ScheduledSMS()
            //{
            //    BillID = dbBill.ID,
            //    CreationDate = DateTime.Now,
            //    SMSType = (short)SMSType.PaymentReminder,
            //    ExpirationDate = dbBill.DueDate
            //});

            return new ScheduledSMSResult()
            {
                ToRemove = oldScheduledSMS,
                ToAdd = new ScheduledSMS()
                {
                    BillID = dbBill.ID,
                    SubscriptionID = dbBill.SubscriptionID,
                    CreationDate = DateTime.Now,
                    SMSType = (short)SMSType.PaymentReminder,
                    ExpirationDate = dbBill.DueDate
                }
            };
        }

        public static void AddPaymentBillSMS(this Bill dbBill)
        {
            dbBill.ScheduledSMSes.Add(new ScheduledSMS()
            {
                CreationDate = DateTime.Now,
                SMSType = (short)SMSType.PaymentDone,
                SubscriptionID = dbBill.SubscriptionID
            });
        }

        public static ScheduledSMSResult AddPrepaidReminderSMS(this Subscription dbSubscription)
        {
            var oldScheduledSMS = dbSubscription.ScheduledSMSes.FirstOrDefault(ss => ss.SMSType == (short)SMSType.PrePaidExpiration);

            //dbSubscription.ScheduledSMSes.Add(new ScheduledSMS()
            //{
            //    CreationDate = DateTime.Now,
            //    ExpirationDate = dbSubscription.ExpirationDate,
            //    SMSType = (short)SMSType.PrePaidExpiration
            //});

            return new ScheduledSMSResult()
            {
                ToRemove = oldScheduledSMS,
                ToAdd = new ScheduledSMS()
                {
                    SubscriptionID = dbSubscription.ID,
                    CreationDate = DateTime.Now,
                    ExpirationDate = dbSubscription.LastAllowedDate,
                    SMSType = (short)SMSType.PrePaidExpiration
                }
            };
        }

        public static ScheduledSMSResult AddFailedAutoPaymentSMS(this Bill dbBill, string errorMessage)
        {
            //dbBill.ScheduledSMSes.Add(new ScheduledSMS()
            //{
            //    SubscriptionID = dbBill.SubscriptionID,
            //    CreationDate = DateTime.Now,
            //    SMSType = (short)SMSType.FailedAutomaticPayment,
            //    CalculatedParameters = errorMessage
            //});

            return new ScheduledSMSResult()
            {
                ToAdd = new ScheduledSMS()
                {
                    BillID = dbBill.ID,
                    SubscriptionID = dbBill.SubscriptionID,
                    CreationDate = DateTime.Now,
                    SMSType = (short)SMSType.FailedAutomaticPayment,
                    CalculatedParameters = errorMessage
                }
            };
        }

        //public static UpdateScheduledSMSresult UpdateScheduledSMSOnSend(this DbSet<ScheduledSMS> scheduledSMSes, ScheduledSMS scheduledSMS)
        //{
        //    switch ((SMSType)scheduledSMS.SMSType)
        //    {
        //        case SMSType.PaymentDone:
        //        case SMSType.FailedAutomaticPayment:
        //        case SMSType.NewBill:
        //            return UpdateScheduledSMSresult.Remove;
        //        case SMSType.PaymentReminder:
        //        case SMSType.PrePaidExpiration:
        //            return UpdateScheduledSMSresult.SetDate;
        //        default:
        //            return UpdateScheduledSMSresult.Invalid;
        //    }
        //}

        public enum UpdateScheduledSMSresult
        {
            Invalid = 0,
            SetDate = 1,
            Remove = 2
        }

        public class ScheduledSMSResult
        {
            public ScheduledSMS ToAdd { get; set; }

            public ScheduledSMS ToRemove { get; set; }
        }
    }
}
