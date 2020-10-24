using RezaB.DBUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB
{
    public static class SystemSettings
    {
        public static string RadiusRServerServiceName
        {
            get
            {
                return DBSetting<RadiusREntities, SystemSetting>.Retrieve<string>("RadiusRServerServiceName");
            }
        }

        public static string RadiusRSchedulerServiceName
        {
            get
            {
                return DBSetting<RadiusREntities, SystemSetting>.Retrieve<string>("RadiusRSchedulerServiceName");
            }
        }

        public static string BTKLogSchedulerServiceName
        {
            get
            {
                return DBSetting<RadiusREntities, SystemSetting>.Retrieve<string>("BTKLogSchedulerServiceName");
            }
        }
    }
}
