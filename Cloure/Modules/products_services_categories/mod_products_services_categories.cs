using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.products_services_categories
{
    public class mod_products_services_categories : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(ProductsServicesCategoriesPage));
        }
    }
}
