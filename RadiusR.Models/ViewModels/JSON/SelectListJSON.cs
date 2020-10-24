using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.JSON
{
    public class SelectListJSON
    {
        public IEnumerable<SelectListItem> Items { get; set; }

        public long? selectedValue { get; set; }

        public class SelectListItem
        {
            public long Value { get; set; }

            public string Name { get; set; }
        }
    }
}
