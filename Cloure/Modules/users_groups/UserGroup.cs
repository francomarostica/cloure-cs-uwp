using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.users_groups
{
    public class UserGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsStaff { get; set; }

        public List<ModulePrivileges> ModulePrivileges { get; set; }
        public List<AvailableCommand> AvailableCommands { get; set; }
    }
}
