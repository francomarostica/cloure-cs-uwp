using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.countries
{
    public static class Countries
    {
        public static async Task<List<Country>> GetList()
        {
            List<Country> items = new List<Country>();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "countries"));
                cparams.Add(new CloureParam("topic", "list"));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    //JsonObject api_response = api_result.GetNamedObject("Response");
                    JsonArray registers = api_result.GetNamedArray("response");

                    foreach (JsonValue jsonValue in registers)
                    {
                        JsonObject register = jsonValue.GetObject();
                        Country item = new Country();
                        item.Id = (int)register.GetNamedNumber("id");
                        item.Name = register.GetNamedString("name");
                        items.Add(item);
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

        public static async Task<List<Country>> GetCloureList()
        {
            List<Country> items = new List<Country>();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "countries"));
                cparams.Add(new CloureParam("topic", "list"));
                cparams.Add(new CloureParam("available", "1"));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("error");
                if (error == "")
                {
                    //JsonObject api_response = api_result.GetNamedObject("Response");
                    JsonArray registers = api_result.GetNamedArray("response");

                    foreach (JsonValue jsonValue in registers)
                    {
                        JsonObject register = jsonValue.GetObject();
                        Country item = new Country();
                        item.Id = (int)register.GetNamedNumber("id");
                        item.Name = register.GetNamedString("name");
                        //item.Codigo = register.GetNamedString("Dominio");
                        items.Add(item);
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
    }
}
