using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.banks
{
    public class Bank
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OnlineBanking { get; set; }
        public string Web { get; set; }

        public List<AvailableCommand> availableCommands { get; set; }
    }
}
