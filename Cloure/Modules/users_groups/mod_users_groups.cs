using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.users_groups
{
    public class mod_users_groups : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(UsersGroupsPage));
        }
    }
}
