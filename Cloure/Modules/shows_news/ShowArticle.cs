using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.shows_news
{
    public class ShowArticle
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public string ImagePath { get; set; }
        public int ArtistaId { get; set; }

        public List<AvailableCommand> AvailableCommands { get; set; }
    }
}
