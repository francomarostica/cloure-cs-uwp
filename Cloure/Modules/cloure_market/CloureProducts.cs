using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.cloure_market
{
    public static class CloureProducts
    {
        public static async Task<JsonObject> GetClourePlans(string code="")
        {
            JsonObject response = null;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("topic", "get_cloure_plans"));
                cparams.Add(new CloureParam("pid", code));
                string res = await CloureManager.ExecuteAsync(cparams);

                //CloureManager.ShowDialog(res);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    response = api_result.GetNamedObject("Response");
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
