using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.receipts
{
    public class ReceiptsResponse
    {
        public int Pages = 1;
        public List<Receipt> Items = new List<Receipt>();
    }
}
