using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.products_services
{
    public class ProductService
    {
        public int Id { get; set; }
        public int ProductTypeId { get; set; }
        public int MeasureUnitId { get; set; }
        public string Title { get; set; }
        public int CategoriaN1Id { get; set; }
        public int CategoriaN2Id { get; set; }

        public string Descripcion { get; set; }

        public string CodigoBarras { get; set; }
        public string CodigoInterno { get; set; }

        public string CategoriaN1 { get; set; }
        public string CategoriaN2 { get; set; }

        public Uri ImagePath { get; set; }

        public double IVA { get; set; }
        public double CostoPrecio { get; set; }
        public double CostoImporte { get; set; }
        public double VentaPrecio { get; set; }
        public double VentaImporte { get; set; }

        public double Importe { get; set; }
        public string ImporteStr { get; set; }

        public double StockTotal { get; set; }
        public string StockTotalStr { get; set; }

        public List<ProductStock> Stock = new List<ProductStock>();
        public List<CloureImage> Images = new List<CloureImage>();
        public List<AvailableCommand> AvailableCommands = new List<AvailableCommand>();
    }
}
