using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.products_services_categories
{
    public class ProductServiceCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public Uri Image { get; set; }
        public CloureImage CloureImage { get; set; }

        public List<AvailableCommand> AvailableCommands { get; set; }
    }
}
