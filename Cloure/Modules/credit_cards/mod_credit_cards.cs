using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.credit_cards
{
    public class mod_credit_cards : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(CreditCardsPage));
        }
    }
}
