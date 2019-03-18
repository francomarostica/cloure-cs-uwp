using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.photographer_performance
{
    public class mod_photographer_performance : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(PhotographersPerformancePage));
        }
    }
}
