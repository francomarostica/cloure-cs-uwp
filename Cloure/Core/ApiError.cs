using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Core
{
    public class ApiError
    {
        public string message;
        public string type;

        public ApiError(string message, string type="")
        {
            this.message = message;
            this.type = type;
        }
    }
}
