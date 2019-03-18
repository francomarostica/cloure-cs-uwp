using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.company_branches
{
    public class CompanyBranch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public List<AvailableCommand> AvailableCommands = new List<AvailableCommand>();
    }
}
