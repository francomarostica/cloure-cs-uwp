using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.places
{
    public class mod_places : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(PlacesPage));
        }
    }
}
