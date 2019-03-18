using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Cloure.Modules.finances
{
    public class mod_finances : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(financesPage));
        }
    }
}
