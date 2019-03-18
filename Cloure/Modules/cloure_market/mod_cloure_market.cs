using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.cloure_market
{
    public class mod_cloure_market : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(CloureMarketPage));
        }
    }
}
