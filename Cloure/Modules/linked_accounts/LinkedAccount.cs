using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.linked_accounts
{
    public class LinkedAccount
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string ImageURL { get; set; }
        public string Status { get; set; }
        public List<LinkedAccountField> linkedAccountFields { get; set; }
    }

    public class LinkedAccountField
    {
        public string Nombre { get; set; }
        public string Titulo { get; set; }
        public string Valor { get; set; }
        public string Tipo { get; set; }
    }
}
