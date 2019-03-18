using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Cloure.Core
{
    public class CloureMenuItem : Button
    {
        public string Nombre;
        public string Titulo;
        public string GroupId;
        public string ModuleName;

        public CloureMenuItem(string name, string title, string group_id)
        {
            Nombre = name;
            ModuleName = name;
            Titulo = title;
            GroupId = group_id;

            Content = Titulo;
        }
    }
}
