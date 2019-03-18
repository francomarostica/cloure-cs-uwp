using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.company_branches
{
    public class mod_company_branches : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(CompanyBranchesPage));
        }
    }
}
