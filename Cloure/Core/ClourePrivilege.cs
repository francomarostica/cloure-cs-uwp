using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Core
{
    public class ClourePrivilege
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ModuleId { get; set; }
        public object Value { get; set; }
        public string Type { get; set; }
    }
}
