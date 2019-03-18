using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.places
{
    public class Place
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public List<AvailableCommand> AvailableCommands = new List<AvailableCommand>();
    }
}
