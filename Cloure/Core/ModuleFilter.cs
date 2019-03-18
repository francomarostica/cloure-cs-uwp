using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Core
{
    public class ModuleFilter
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Default { get; set; }

        public List<ModuleFilterItem> FilterItems { get; set; }

        public ModuleFilter(string Name, string Title, string Type, string Default)
        {
            this.Name = Name;
            this.Title = Title;
            this.Type = Type;
            this.Default = Default;
            this.FilterItems = new List<ModuleFilterItem>();
        }

        public void AddItem(ModuleFilterItem item)
        {
            FilterItems.Add(item); 
        }
    }
}
