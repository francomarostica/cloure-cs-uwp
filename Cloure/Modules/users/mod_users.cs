using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Cloure.Modules.users
{
    public class mod_users : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(UsersPage));
        }
    }
}
