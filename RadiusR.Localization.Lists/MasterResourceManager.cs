using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.Localization.Lists
{
    public static class MasterResourceManager
    {
        public static ResourceManager GetResourceManager(string name)
        {
            return new ResourceManager(name, Assembly.GetExecutingAssembly());
        }
    }
}
