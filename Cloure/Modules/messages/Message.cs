using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.messages
{
    public class Message
    {
        public int Id { get; set; }
        public string DateStr { get; set; }
        public string Subject { get; set; }
        public string User { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string MessageText { get; set; }

        public List<AvailableCommand> availableCommands { get; set; }
    }
}
