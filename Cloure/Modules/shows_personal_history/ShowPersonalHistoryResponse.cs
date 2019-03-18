using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.shows_personal_history
{
    public class ShowPersonalHistoryResponse
    {
        public int TotalEventos = 0;
        public int TotalFotos = 0;
        public int TotalPages = 1;

        public List<ShowPersonalHistoryItem> Items = new List<ShowPersonalHistoryItem>();
    }
}
