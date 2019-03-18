using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.users
{
    public class UsersResponse
    {
        public int TotalPaginas = 0;
        public int TotalRegistros = 0;
        public string PageString = "";
        public List<User> Items = new List<User>();
    }
}
