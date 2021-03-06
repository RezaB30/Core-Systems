using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR_Manager.Models.ViewModels.PDFTemplates
{
    public class Coordinates
    {
        public InvariantDecimal X { get; set; }

        public InvariantDecimal Y { get; set; }

        public Coordinates(decimal x, decimal y)
        {
            X = new InvariantDecimal() { Value = x };
            Y = new InvariantDecimal() { Value = y };
        }

        public Coordinates() { }
    }
}
