using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Cloure.Modules.messages
{
    public class mod_messages : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(MessagesPage));
        }
    }
}
