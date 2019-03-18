using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.finances
{
    public class FinancesResponse
    {
        public double TotalIngresos = 0;
        public string TotalIngresosStr = "";
        public double TotalGastos = 0;
        public string TotalGastosStr = "";
        public double Saldo = 0;
        public string SaldoStr = "";

        public int TotalPages = 1;
        public string PageString = "";
        public List<FinanceMovement> financeMovements = new List<FinanceMovement>();
    }
}
