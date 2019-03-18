using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.shows_personal_history
{
    public class mod_shows_personal_history : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(ShowsPersonalHistoryPage));
        }
    }
}
