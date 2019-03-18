using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Core
{
    public class GenericResponse
    {
        public int TotalPages = 1;
        public string PageString = "";
        public List<object> Items = new List<object>();
        //List<itemType> Items = new List<itemType>();
    }
}
