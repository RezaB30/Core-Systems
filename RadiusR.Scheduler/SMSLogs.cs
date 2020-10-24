using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.Scheduler
{
    public static partial class Scheduler
    {
        private static Logger SMSLogger = LogManager.GetLogger("SMS");

        private static void LogSMSException(Exception ex, string message = "Error in sms batches!")
        {
            SMSLogger.Error(ex, message);
        }

        private static void LogSMSTimeout(long billId)
        {
            SMSLogger.Error("SMS queue timeout for bill id: {0}", billId);
        }
    }
}
