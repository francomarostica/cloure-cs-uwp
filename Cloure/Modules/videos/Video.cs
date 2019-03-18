using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.videos
{
    public class Video
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public Uri Uri { get; set; }
        public List<AvailableCommand> AvailableCommands = new List<AvailableCommand>();
    }
}
