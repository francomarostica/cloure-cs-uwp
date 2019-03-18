using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.products_services
{
    public class ProductsResponse
    {
        public int TotalPaginas = 0;
        public int TotalRegistros = 0;
        public List<ProductService> Items = new List<ProductService>();
    }
}
