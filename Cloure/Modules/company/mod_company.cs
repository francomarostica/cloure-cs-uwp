using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.company
{
    public class mod_company : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(CompanyPage));
        }
    }
}
