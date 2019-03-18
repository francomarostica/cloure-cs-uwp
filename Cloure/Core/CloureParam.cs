using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Core
{
    public class CloureParam
    {
        public string name = "";
        public object value = "";

        public CloureParam(string name, object value)
        {
            this.name = name;
            this.value = value;
        }


    }
}
