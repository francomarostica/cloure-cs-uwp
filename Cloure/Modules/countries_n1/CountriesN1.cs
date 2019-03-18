using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.countries_n1
{
    public static class CountriesN1
    {
        public static async Task<List<CountryN1>> GetList(int CountryId)
        {
            List<CountryN1> items = new List<CountryN1>();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "countries_n1"));
                cparams.Add(new CloureParam("topic", "get_list"));
                cparams.Add(new CloureParam("pais_id", CountryId.ToString()));
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
                        CountryN1 item = new CountryN1();
                        item.Id = (int)register.GetNamedNumber("Id");
                        item.Name = register.GetNamedString("Nombre");
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
