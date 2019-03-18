using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.bank_checks
{
    public class BankCheck
    {
        public int Id { get; set; }
        public int BancoId { get; set; }
        public string Banco { get; set; }
        public string FechaCobroStr { get; set; }
        public int ClienteId { get; set; }
        public string Cliente { get; set; }
        public string Descripcion { get; set; }

        public List<AvailableCommand> availableCommands { get; set; }
    }
}
