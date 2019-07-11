using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Core
{
    public class BusinessTypes
    {
        public async Task<List<BusinessType>> getList(string filtro = "")
        {
            List<BusinessType> response = new List<BusinessType>();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "business_types"));
                cparams.Add(new CloureParam("topic", "list"));
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
                        BusinessType item = new BusinessType();
                        item.Id = register.GetNamedString("id");
                        item.Title = register.GetNamedString("title");
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
    }
}
