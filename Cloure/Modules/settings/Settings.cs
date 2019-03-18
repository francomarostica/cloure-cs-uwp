using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.settings
{
    public static class Settings
    {
        public async static Task<List<ModuleSettings>> GetList()
        {
            List<ModuleSettings> items = new List<ModuleSettings>();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "settings"));
                cparams.Add(new CloureParam("topic", "get_list"));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonObject api_response = api_result.GetNamedObject("Response");
                    JsonArray registers = api_response.GetNamedArray("Registros");

                    foreach (JsonValue jsonValue in registers)
                    {
                        JsonObject register = jsonValue.GetObject();
                        ModuleSettings moduleSettings = new ModuleSettings();
                        moduleSettings.CloureSettings = new List<CloureSetting>();
                        moduleSettings.ModuleTitle = register.GetNamedString("Title");
                        moduleSettings.ModuleId = register.GetNamedString("Id");

                        JsonArray privileges_tmp = register.GetNamedArray("Settings");
                        foreach (JsonValue privilege_tmp in privileges_tmp)
                        {
                            JsonObject privilegeObj = privilege_tmp.GetObject();

                            CloureSetting item = new CloureSetting();
                            item.Id = privilegeObj.GetNamedString("Id");
                            item.Title = privilegeObj.GetNamedString("Titulo");
                            item.Type = privilegeObj.GetNamedString("Tipo");
                            item.Value = privilegeObj.GetNamedString("Valor");
                            moduleSettings.CloureSettings.Add(item);
                        }
                        items.Add(moduleSettings);
                    }
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }

            return items;
        }

        public static async Task<bool> Save(List<ModuleSettings> moduleSettings)
        {
            bool response = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "settings"));
                cparams.Add(new CloureParam("topic", "save"));

                if (moduleSettings != null)
                {

                }
                string str_content = "[";
                foreach (ModuleSettings module in moduleSettings)
                {
                    foreach(CloureSetting setting in module.CloureSettings)
                    {
                        str_content += "{";
                        str_content += "\"module_id\":\""+module.ModuleId + "\",";
                        str_content += "\"option\":\"" + setting.Id + "\",";
                        str_content += "\"type\":\"" + setting.Type + "\",";
                        str_content += "\"value\":\"" + setting.Value + "\"";
                        str_content += "},";
                    }
                }
                str_content = str_content.TrimEnd(',');
                str_content += "]";

                cparams.Add(new CloureParam("settings", str_content));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    //JsonObject api_response = api_result.GetNamedObject("Response");
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                response = false;
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }

            return response;
        }
    }
}
