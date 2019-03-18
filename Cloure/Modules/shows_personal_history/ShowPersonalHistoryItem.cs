using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.shows_personal_history
{
    public class ShowPersonalHistoryItem
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Fotos { get; set; }
        public int ShowId { get; set; }
        public DateTime? Fecha { get; set; }
        public string BandaArtista { get; set; }
        public string Lugar { get; set; }
    }
}
