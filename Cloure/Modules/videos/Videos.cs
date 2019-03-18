using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.videos
{
    public static class Videos
    {
        public static async Task<GenericResponse> GetList(string filtro = "", string ordenar_por = "", string orden = "", int Page = 1)
        {
            GenericResponse response = new GenericResponse();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "videos"));
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
                        Video item = new Video();
                        item.Id = CloureManager.ParseInt(register.GetNamedValue("Id"));
                        item.Titulo = register.GetNamedString("Titulo");
                        item.Uri = new Uri(register.GetNamedString("Url"));

                        JsonArray available_commands_arr = register.GetNamedArray("AvailableCommands");
                        item.AvailableCommands = new List<AvailableCommand>();
                        foreach (JsonValue available_cmd_obj in available_commands_arr)
                        {
                            JsonObject available_cmd_item = available_cmd_obj.GetObject();
                            int available_cmd_id = (int)available_cmd_item.GetNamedNumber("Id");
                            string available_cmd_name = available_cmd_item.GetNamedString("Name");
                            string available_cmd_title = available_cmd_item.GetNamedString("Title");
                            AvailableCommand availableCommand = new AvailableCommand(available_cmd_id, available_cmd_name, available_cmd_title);
                            item.AvailableCommands.Add(availableCommand);
                        }

                        response.Items.Add(item);
                    }

                    response.TotalPages = (int)api_response.GetNamedNumber("TotalPaginas");
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

        public static async Task<bool> save(Video video)
        {
            bool response = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "videos"));
                cparams.Add(new CloureParam("topic", "guardar"));
                cparams.Add(new CloureParam("id", video.Id));
                cparams.Add(new CloureParam("titulo", video.Titulo));
                if(video.Uri!=null) cparams.Add(new CloureParam("url", video.Uri.ToString()));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    string api_response = api_result.GetNamedString("Response");
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

        public static async Task<Video> Get(int id)
        {
            Video item = new Video();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "videos"));
                cparams.Add(new CloureParam("topic", "obtener"));
                cparams.Add(new CloureParam("id", id));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonObject item_obj = api_result.GetNamedObject("Response");
                    item.Id = CloureManager.ParseInt(item_obj.GetNamedValue("Id"));
                    item.Titulo = item_obj.GetNamedString("Titulo");
                    item.Uri = new Uri(item_obj.GetNamedString("Url"));
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

            return item;
        }

        public static async Task<bool> Delete(int id)
        {
            bool response = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "videos"));
                cparams.Add(new CloureParam("topic", "borrar"));
                cparams.Add(new CloureParam("id", id));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                string response_str = api_result.GetNamedString("Response");
                if (error == "")
                {
                    var dialog = new MessageDialog(response_str);
                    await dialog.ShowAsync();
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
