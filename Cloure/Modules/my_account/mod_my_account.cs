using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.my_account
{
    public class mod_my_account : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(MyAccountPage));
        }
    }
}
