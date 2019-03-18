using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace Cloure.Core
{
    public struct Price
    {
        double val { get; set; }
        string Currency { get; set; }

        public override string ToString()
        {
            string s = CloureManager.currency_symbol+" "+val.ToString();

            return s;
        }

        public Price(double v, string Currency="")
        {
            val = v;
            this.Currency = Currency;
        }


        public static Price Parse(JsonValue jsonValue, string Currency="")
        {

            double n = 0;

            if (jsonValue.ValueType == JsonValueType.Number)
            {
                n = jsonValue.GetNumber();
            }
            else if (jsonValue.ValueType == JsonValueType.String)
            {
                string number_str = jsonValue.GetString();
                if (CloureManager.GetSystemDecimalSeparator() == ",")
                {
                    //in API double always is with point
                    number_str = number_str.Replace(".", ",");
                    double.TryParse(number_str, out n);
                }
            }

            Price clourePrice = new Price(n, Currency);
            return clourePrice;
        }

    }
}
