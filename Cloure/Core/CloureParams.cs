using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Core
{
    public class CloureParams
    {
        private List<CloureParam> cloureParams = new List<CloureParam>();

        public void Add(string key, object value)
        {
            CloureParam cloureParam = new CloureParam(key, value);
            cloureParams.Add(cloureParam);
        }

        public override string ToString()
        {
            string result = "{";

            for (int i = 0; i < cloureParams.Count; i++)
            {
                CloureParam item = cloureParams[i];

                result += "\"" + item.name + "\"";
                result += ":";
                result += "\"" + item.value.ToString() + "\"";
                if (i < (cloureParams.Count - 1)) result += ",";
            }

            result += "}";

            return result;
        }
    }
}
