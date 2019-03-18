using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.shows_personal_history
{
    public static class ShowsPersonalHistory
    {
        public static async Task<ShowPersonalHistoryResponse> GetList(string filtro = "", string ordenar_por = "", string orden = "", string desde = "", string hasta = "", string movement_type = "", string sucursal = "0", int Page = 1)
        {
            ShowPersonalHistoryResponse respuesta = new ShowPersonalHistoryResponse();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "shows_personal_history"));
                cparams.Add(new CloureParam("topic", "listar"));
                if (filtro.Length > 0) cparams.Add(new CloureParam("filtro", filtro));
                if (ordenar_por.Length > 0) cparams.Add(new CloureParam("ordenar_por", ordenar_por));
                if (orden.Length > 0) cparams.Add(new CloureParam("orden", orden));
                if (desde.Length > 0) cparams.Add(new CloureParam("desde", desde));
                if (hasta.Length > 0) cparams.Add(new CloureParam("hasta", hasta));
                if (sucursal.Length > 0) cparams.Add(new CloureParam("sucursal", sucursal));
                cparams.Add(new CloureParam("pagina", Page.ToString()));
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
                        ShowPersonalHistoryItem item = new ShowPersonalHistoryItem();
                        item.Id = CloureManager.ParseInt(register.GetNamedValue("Id"));
                        item.Fotos = CloureManager.ParseInt(register.GetNamedValue("Fotos"));
                        item.ShowId = CloureManager.ParseInt(register.GetNamedValue("ShowId"));
                        item.Titulo = register.GetNamedString("Titulo");
                        //item.Fecha = register.GetNamedObject("ShowId");

                        respuesta.Items.Add(item);
                    }
                    respuesta.TotalPages = (int)api_response.GetNamedNumber("TotalPaginas");
                    respuesta.TotalEventos = CloureManager.ParseInt(api_response.GetNamedValue("TotalEventos"));
                    respuesta.TotalFotos = CloureManager.ParseInt(api_response.GetNamedValue("TotalFotos"));
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

            return respuesta;
        }
    }
}
