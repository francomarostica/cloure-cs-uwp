using Cloure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.receipts
{
    public class CartItem : INotifyPropertyChanged
    {
        public double Cantidad { get; set; }
        public int ProductoId { get; set; }
        public string Descripcion { get; set; }
        public double PrecioUnitario { get; set; }
        public double Iva { get; set; }
        public double Importe { get; set; }

        private double total = 0;
        public double Total
        {
            get
            {
                return total;
            }
            set
            {
                total = value;
                OnPropertyChanged("Total");
            }
        }
        public string TotalStr
        {
            get
            {
                return Total.ToString("F2");
            }
        }
        public Uri ImagenPath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
