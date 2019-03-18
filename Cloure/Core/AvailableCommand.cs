using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Core
{
    public class AvailableCommand
    {
        public int id=0;
        public string name="";
        public string title="";

        public AvailableCommand(int id, string name, string title)
        {
            this.id = id;
            this.name = name;
            this.title = title;
        }
    }
}
