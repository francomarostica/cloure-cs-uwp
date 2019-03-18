using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.shows
{
    public class mod_shows : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(ShowsPage));
        }
    }
}
