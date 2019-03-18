﻿using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.debit_cards
{
    public class DebitCard
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<AvailableCommand> availableCommands { get; set; }
    }
}
