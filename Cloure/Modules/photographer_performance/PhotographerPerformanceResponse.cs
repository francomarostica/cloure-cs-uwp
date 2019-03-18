using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.photographer_performance
{
    public class PhotographerPerformanceResponse
    {
        public int TotalEventos = 0;
        public int TotalFotos = 0;
        public int TotalPages = 1;

        public List<PhotographerPerformanceItem> Items = new List<PhotographerPerformanceItem>();
    }
}
