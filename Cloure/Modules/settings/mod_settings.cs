using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.settings
{
    public class mod_settings : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(SettingsPage));
        }
    }
}
