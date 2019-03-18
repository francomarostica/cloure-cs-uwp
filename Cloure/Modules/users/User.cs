using Cloure.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.users
{
    public class User : INotifyPropertyChanged
    {
        public int id = 0;
        public string razonsocial { get; set; }
        public string empresa { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public double saldo { get; set; }
        public string saldo_str { get; set; }
        public string grupo_id { get; set; }
        public string grupo { get; set; }
        public double salario { get; set; }
        public double comision { get; set; }

        public Uri ImageURL { get; set; }
        public DateTime? FechaNac { get; set; }
        public int GeneroId { get; set; }
        public CloureImage CloureImage { get; set; }

        public bool Selected { get; set; }

        private int _Fotos = 0;
        public int Fotos {
            get
            {
                return _Fotos;
            }
            set
            {
                _Fotos = value;
                OnPropertyChanged("Fotos");
            }
        }


        //public Bitmap Image;
        //public Bitmap64 ImageToUpload;

        public List<AvailableCommand> availableCommands = new List<AvailableCommand>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return this.apellido + ", " + this.nombre;
        }
    }
}
