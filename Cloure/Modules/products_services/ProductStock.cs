using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.products_services
{
    public class ProductStock
    {
        public int PropiedadId { get; set; }
        public string PropiedadNombre { get; set; }
        public double Actual { get; set; }
        public double Min { get; set; }

        public string MinPrompt { get; set; }
        public string CurrentPrompt { get; set; }
    }
}
