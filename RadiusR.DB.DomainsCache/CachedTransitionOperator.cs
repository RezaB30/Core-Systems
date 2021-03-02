using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.DomainsCache
{
    public class CachedTransitionOperator
    {
        public int ID { get; set; }

        public string DisplayName { get; set; }

        public string Username { get; set; }

        public IEnumerable<string> RemoteFolders { get; set; }
    }
}
