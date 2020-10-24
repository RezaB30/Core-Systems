using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.API.MobilExpress.DBAdapter.AdapterParameters
{
    public class AdapterCard
    {
        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public int CardMonth { get; set; }

        public int CardYear { get; set; }
    }
}
