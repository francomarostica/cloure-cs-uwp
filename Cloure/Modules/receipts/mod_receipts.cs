using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.receipts
{
    public class mod_receipts : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(ReceiptsPage));
        }
    }
}
