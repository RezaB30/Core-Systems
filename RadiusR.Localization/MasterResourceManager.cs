using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using RadiusR.Localization.Lists;

namespace RadiusR.Localization
{
    public static class MasterResourceManager
    {
        public static ResourceManager GetResourceManager(string name)
        {
            if (name.StartsWith("RadiusR.Localization.Lists"))
                return Lists.MasterResourceManager.GetResourceManager(name);
            return new ResourceManager(name, Assembly.GetExecutingAssembly());
        }
    }
}
