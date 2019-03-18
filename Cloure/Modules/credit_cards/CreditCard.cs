using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.credit_cards
{
    public class CreditCard
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<AvailableCommand> availableCommands { get; set; }
    }
}
