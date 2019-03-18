using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.bank_checks
{
    public class mod_bank_checks : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(BankChecksPage));
        }
    }
}
