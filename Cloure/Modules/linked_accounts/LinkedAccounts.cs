using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.linked_accounts
{
    public static class LinkedAccounts
    {
        public static async Task<GenericResponse> GetList(string filtro = "", string ordenar_por = "", string orden = "", int Page = 1)
        {
            GenericResponse response = new GenericResponse();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "linked_accounts"));
                cparams.Add(new CloureParam("topic", "listar"));
                if (filtro.Length > 0) cparams.Add(new CloureParam("filtro", filtro));
                if (ordenar_por.Length > 0) cparams.Add(new CloureParam("ordenar_por", ordenar_por));
                if (orden.Length > 0) cparams.Add(new CloureParam("orden", orden));
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
                        LinkedAccount item = new LinkedAccount();
                        item.Name = register.GetNamedString("Name");
                        item.Title = register.GetNamedString("Title");
                        item.ImageURL = register.GetNamedString("Image");
                        item.Status = register.GetNamedString("Status");

                        response.Items.Add(item);
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

        public static async Task<LinkedAccount> Obtener(string nombre = "")
        {
            LinkedAccount response = new LinkedAccount();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "linked_accounts"));
                cparams.Add(new CloureParam("topic", "obtener"));
                cparams.Add(new CloureParam("id", nombre));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonObject api_response = api_result.GetNamedObject("Response");

                    response.Name = api_response.GetNamedString("Name");
                    response.Title = api_response.GetNamedString("Title");
                    response.ImageURL = api_response.GetNamedString("Image");
                    response.Status = api_response.GetNamedString("Status");
                    response.linkedAccountFields = new List<LinkedAccountField>();

                    JsonArray campos = api_response.GetNamedArray("Data");
                    foreach (JsonValue campo in campos)
                    {
                        JsonObject campo_obj = campo.GetObject();
                        LinkedAccountField linkedAccountField = new LinkedAccountField();

                        linkedAccountField.Nombre = campo_obj.GetNamedString("nombre");
                        linkedAccountField.Titulo = campo_obj.GetNamedString("titulo");
                        linkedAccountField.Valor = campo_obj.GetNamedString("valor");
                        linkedAccountField.Tipo = campo_obj.GetNamedString("tipo");
                        response.linkedAccountFields.Add(linkedAccountField);
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

        public static async Task<JsonObject> Save(LinkedAccount linkedAccount)
        {
            JsonObject response = new JsonObject();

            string Data = "";

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "linked_accounts"));
                cparams.Add(new CloureParam("topic", "guardar"));
                cparams.Add(new CloureParam("id", linkedAccount.Name));

                if (linkedAccount.linkedAccountFields != null)
                {
                    Data = "[";
                    foreach (LinkedAccountField campo in linkedAccount.linkedAccountFields)
                    {
                        Data += "{";
                        Data += "\"id\":\"" + campo.Nombre + "\",";
                        Data += "\"valor\":\"" + campo.Valor + "\"";
                        Data += "},";
                    }
                    Data = Data.TrimEnd(',');
                    Data += "]";
                    cparams.Add(new CloureParam("data", Data));
                }

                string res = await CloureManager.ExecuteAsync(cparams);
                response = JsonObject.Parse(res);
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
