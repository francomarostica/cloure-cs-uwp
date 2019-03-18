using Cloure.Core;
using Cloure.Modules.places;
using Cloure.Modules.users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.shows
{
    public class Show
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTimeOffset FechaOffset
        {
            get
            {
                return Fecha.Value;
            }
            set
            {
                Fecha = value.Date;
            }
        }
        public string FechaStr { get; set; }
        public int ArtistaId { get; set; }
        public string Artista { get; set; }
        public int LugarId { get; set; }
        public string Lugar { get; set; }

        public List<CloureImage> Images = new List<CloureImage>();
        public List<AvailableCommand> AvailableCommands = new List<AvailableCommand>();
        public ObservableCollection<User> Fotografos { get; set; }

        public List<Place> Places { get; set; } //Used for bathc events addition
    }
}
