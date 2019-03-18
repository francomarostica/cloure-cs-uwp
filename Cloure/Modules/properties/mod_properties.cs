using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.properties
{
    public class mod_properties : CloureModule
    {
        public override void OnModuleCreated(){
            CloureManager.Navigate(typeof(PropertiesPage));
        }
    }
}
