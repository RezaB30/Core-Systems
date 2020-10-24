using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.Address.QueryInterface
{
    public class RadiusAddress<T>
    {
        public bool ErrorOccured { get; set; }

        public string ErrorMessage { get; set; }

        public T Data { get; set; }
    }
}
