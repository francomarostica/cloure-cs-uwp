using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.debit_cards
{
    public class mod_debit_cards : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(DebitCardsPage));
        }
    }
}
