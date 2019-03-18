using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Core
{
    public class ModuleFilterItem
    {
        public string Id { get; set; }
        public string Title { get; set; }

        public ModuleFilterItem(string Id, string Title)
        {
            this.Id = Id;
            this.Title = Title;
        }
    }
}
