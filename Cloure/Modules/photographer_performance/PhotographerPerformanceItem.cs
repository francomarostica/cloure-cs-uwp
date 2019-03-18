using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.photographer_performance
{
    public class PhotographerPerformanceItem
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int Fotos { get; set; }
        public int ShowId { get; set; }
        public DateTime? Fecha { get; set; }
        public string BandaArtista { get; set; }
        public string Lugar { get; set; }
        public Uri FotografoImagen { get; set; }
        public string FotografoNombre { get; set; }
        public string FotografoApellido { get; set; }
        public int FotografoEventos { get; set; }
        public string PromFotos { get; set; }
    }
}
