using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Cloure.Modules.products_services
{
    public class mod_products_services : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(ProductsServicesPage));
        }
    }
}
