﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.SystemLogs.Parameters
{
    public class TelekomWorkOrderDetails
    {
        public RadiusR.DB.Enums.TelekomOperations.TelekomOperationType OperationType { get; private set; }

        public RadiusR.DB.Enums.TelekomOperations.TelekomOperationSubType OperationSubType { get; private set; }

        public int ManagementCode { get; private set; }

        public int ProvinceCode { get; private set; }

        public long QueueNo { get; private set; }

        public string TelekomSubscriberNo { get; private set; }

        public TelekomWorkOrderDetails(RadiusR.DB.Enums.TelekomOperations.TelekomOperationType operationType, RadiusR.DB.Enums.TelekomOperations.TelekomOperationSubType operationSubType, int managementCode, int provinceCode, long queueNo, string telekomSubscriberNo)
        {
            OperationType = operationType;
            OperationSubType = operationSubType;
            ManagementCode = managementCode;
            ProvinceCode = provinceCode;
            QueueNo = queueNo;
            TelekomSubscriberNo = telekomSubscriberNo;
        }
    }
}
