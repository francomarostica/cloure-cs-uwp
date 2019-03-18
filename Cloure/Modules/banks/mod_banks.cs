using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.banks
{
    public class mod_banks : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(BanksPage));
        }
    }
}
