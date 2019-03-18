using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloure.Modules.videos
{
    public class mod_videos : CloureModule
    {
        public override void OnModuleCreated()
        {
            CloureManager.Navigate(typeof(VideosPage));
        }
    }
}
