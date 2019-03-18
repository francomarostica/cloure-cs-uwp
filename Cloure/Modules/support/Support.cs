using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.support
{
    public static class Support
    {
        public static async Task<List<SupportType>> GetSupportTypes()
        {
            List<SupportType> supportTypes = new List<SupportType>();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "support"));
                cparams.Add(new CloureParam("topic", "get_support_types"));
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
                        SupportType item = new SupportType();
                        item.Id = register.GetNamedString("id");
                        item.Title = register.GetNamedString("title");
                        supportTypes.Add(item);
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

            return supportTypes;
        }

        public static async Task<bool> Send(string Type, string Message)
        {
            bool result = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "support"));
                cparams.Add(new CloureParam("topic", "send"));
                cparams.Add(new CloureParam("message_type", Type));
                cparams.Add(new CloureParam("message", Message));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    string api_response = api_result.GetNamedString("Response");
                }
                else
                {
                    result = false;
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }

            return result;
        }
    }
}
