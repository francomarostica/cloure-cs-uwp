using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.countries
{
    public class mod_countries : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(CountriesPage));
        }
    }
}
