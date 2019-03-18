using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Cloure.Modules.invoicing
{
    public class mod_invoicing : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(InvoicingPage));
        }
    }
}
