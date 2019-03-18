using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.bands_artists
{
    public class mod_bands_artists : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(BandsArtistsPage));
        }
    }
}
