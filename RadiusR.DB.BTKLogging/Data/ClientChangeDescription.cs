using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.BTKLogging.Data
{
    internal class ClientChangeDescription
    {
        public int? Code { get; set; }

        public string Description { get; set; }

        public DateTime Time { get; set; }
    }
}
