using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.transports
{
    public class mod_transports : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(TransportsPage));
        }
    }
}
