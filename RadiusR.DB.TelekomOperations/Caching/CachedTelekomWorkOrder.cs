using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.TelekomOperations.Caching
{
    public class CachedTelekomWorkOrder
    {
        public long ID { get; private set; }

        public short State { get; private set; }

        public string CancellationReason { get; private set; }

        public CachedTelekomWorkOrder(long id, short state, string cancellationReason = null)
        {
            ID = id;
            State = state;
            CancellationReason = cancellationReason;
        }
    }
}
