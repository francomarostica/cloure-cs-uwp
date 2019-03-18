using Cloure.Core;
using Cloure.Modules.users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.receipts
{
    public class Receipt
    {
        public int Id { get; set; }
        public string FechaStr { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public int CustomerId { get; set; }
        public User Customer { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public int StateId { get; set; }
        public string State { get; set; }
        public int CustomStateId { get; set; }
        public string CustomState { get; set; }
        public string Pin { get; set; }
        public string Description { get; set; }
        public double Total { get; set; }
        public string TotalStr { get; set; }
        public int CompanyBranchId { get; set; }
        public double Entrega { get; set; }
        public int FormaDePagoId { get; set; }
        public int FormaDePagoEntidadId { get; set; }
        public string FormaDePagoData { get; set; }
        public DateTime? FormaDePagoCobro { get; set; }
        public string Currency { get; set; }

        public List<CartItem> cartItems { get; set; }

    }
}
