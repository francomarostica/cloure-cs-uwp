using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.bands_artists
{
    public class BandArtist
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Uri ImageUrl { get; set; }
        public CloureImage CloureImage { get; set; }

        public List<AvailableCommand> AvailableCommands = new List<AvailableCommand>();
    }
}
