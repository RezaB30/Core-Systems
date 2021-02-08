using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.Utilities.Extentions
{
    public static class CustomerSetupTaskUtilities
    {
        public static void CancelCustomerSetupTask(this CustomerSetupTask task)
        {
            if (task.IsActive)
            {
                task.TaskStatus = (short)Enums.CustomerSetup.TaskStatuses.Cancelled;
                if (task.AllowanceState == (short)Enums.PartnerAllowanceState.OnHold)
                    task.AllowanceState = (short)Enums.PartnerAllowanceState.Cancelled;
                task.CompletionDate = DateTime.Now;
            }
        }

        public static void CompleteCustomerSetupTask(this CustomerSetupTask task)
        {
            if (task.IsActive)
            {
                task.TaskStatus = (short)Enums.CustomerSetup.TaskStatuses.Completed;
                task.CompletionDate = DateTime.Now;
            }
        }
    }
}
