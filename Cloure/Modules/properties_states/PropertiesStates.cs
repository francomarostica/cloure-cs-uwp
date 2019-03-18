using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.properties_states
{
    public static class PropertiesStates
    {
        public static async Task<List<PropertyState>> GetList(int OperationId)
        {
            List<PropertyState> items = new List<PropertyState>();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "properties"));
                cparams.Add(new CloureParam("topic", "get_states"));
                cparams.Add(new CloureParam("operacion_id", OperationId.ToString()));
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
                        PropertyState item = new PropertyState();
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
