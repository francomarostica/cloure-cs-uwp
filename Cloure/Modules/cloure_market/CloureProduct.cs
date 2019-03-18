using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.cloure_market
{
    public class CloureProduct
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Currency { get; set; }
        public double Price { get; set; }
        public string PriceStr { get; set; }
        public string PaymentPeriod { get; set; }
        public Uri ImagePath { get; set; }
    }
}
