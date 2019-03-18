using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.shows_news
{
    class mod_shows_news : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(ShowsNewsPage));
        }
    }
}
