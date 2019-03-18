using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Cloure.Core
{
    public class GlobalCommand
    {
        public int Id;
        public string Name;
        public string Title;

        public GlobalCommand(int Id, string Name, string Title)
        {
            this.Id = Id;
            this.Name = Name;
            this.Title = Title;
        }
    }
}
