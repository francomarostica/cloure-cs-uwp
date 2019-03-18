using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Core
{
    public static class AvailableLanguages
    {
        public static async Task<List<AvailableLanguage>> getList(string filtro = "")
        {
            List<AvailableLanguage> response = new List<AvailableLanguage>();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("topic", "get_available_languages"));

                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonArray registers = api_result.GetNamedArray("Response");

                    foreach (JsonValue jsonValue in registers)
                    {
                        JsonObject register = jsonValue.GetObject();
                        AvailableLanguage item = new AvailableLanguage();
                        item.Id = register.GetNamedString("id");
                        item.Name = register.GetNamedString("name");
                        response.Add(item);
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

            return response;
        }

        public static async Task<JsonObject> getLocales(string module, string lang="en")
        {
            JsonObject api_result = null;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("topic", "get_locales"));
                cparams.Add(new CloureParam("module_name", module));
                cparams.Add(new CloureParam("lang", lang));

                string res = await CloureManager.ExecuteAsync(cparams);
                //CloureManager.ShowDialog(res);
                api_result = JsonObject.Parse(res);
                /*
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonArray registers = api_result.GetNamedArray("Response");

                    foreach (JsonValue jsonValue in registers)
                    {
                        JsonObject register = jsonValue.GetObject();
                        Locale item = new Locale();
                        item.Name = register.GetNamedString("name");
                        item.Value = register.GetNamedString("value");
                        response.Add(item);
                    }
                }
                else
                {
                    throw new Exception(error);
                }
                */
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }

            return api_result;
        }
    }
}
