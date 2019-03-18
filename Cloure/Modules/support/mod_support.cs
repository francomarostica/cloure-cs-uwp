using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.support
{
    public class mod_support : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(SupportPage));
        }
    }
}
