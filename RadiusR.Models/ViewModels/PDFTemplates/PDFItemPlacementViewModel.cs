using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.PDFTemplates
{
    public class PDFItemPlacementViewModel
    {
        public string Name { get; set; }

        public int ID { get; set; }

        public Coordinates Placement { get; set; }
    }
}
