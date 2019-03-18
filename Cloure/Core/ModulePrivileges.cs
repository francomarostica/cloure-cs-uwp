using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Core
{
    public class ModulePrivileges
    {
        public string ModuleId { get; set; }
        public string ModuleTitle { get; set; }
        public string GroupId { get; set; }
        public string GroupTitle { get; set; }

        public List<ClourePrivilege> ClourePrivileges { get; set; }
    }
}
