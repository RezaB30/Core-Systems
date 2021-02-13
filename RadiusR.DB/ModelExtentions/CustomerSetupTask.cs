using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB
{
    public partial class CustomerSetupTask
    {
        public bool IsActive
        {
            get
            {
                return TaskStatus != (short)Enums.CustomerSetup.TaskStatuses.Cancelled && TaskStatus != (short)Enums.CustomerSetup.TaskStatuses.Completed;
            }
        }
    }
}
