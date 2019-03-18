using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace Cloure.Core
{
    public class ModuleInfo
    {
        public string Name = "";
        public string Title = "";
        public List<GlobalCommand> globalCommands = new List<GlobalCommand>();
        public List<ModuleFilter> moduleFilters = new List<ModuleFilter>();
        public JsonObject locales = null;
    }
}
