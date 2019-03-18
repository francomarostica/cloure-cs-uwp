using Cloure.Core;
using Cloure.Modules.users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.shows
{
    public static class Shows
    {
        public static async Task<GenericResponse> GetList(string filtro = "", string ordenar_por = "", string orden = "", int Page = 1)
        {
            GenericResponse response = new GenericResponse();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "shows"));
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
                        Show item = new Show();
                        item.Id = (int)register.GetNamedNumber("Id");
                        item.Titulo = register.GetNamedString("Titulo");

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

                        //Carga fotografos
                        item.Fotografos = new ObservableCollection<User>();

                        JsonArray fotografos_arr = register.GetNamedArray("Fotografos");
                        foreach (JsonValue fotografo_obj in fotografos_arr)
                        {
                            JsonObject fotografo_item = fotografo_obj.GetObject();
                            User user = new User();

                            JsonValue tmpval = fotografo_item.GetNamedValue("Id");
                            if(tmpval.ValueType == JsonValueType.Number)
                            {
                                user.id = (int)fotografo_item.GetNamedNumber("Id");
                                user.nombre = fotografo_item.GetNamedString("Nombre");
                                user.apellido = fotografo_item.GetNamedString("Apellido");
                                user.razonsocial = user.apellido + ", " + user.nombre;
                                user.ImageURL = new Uri(fotografo_item.GetNamedString("Imagen"));
                                user.grupo = fotografo_item.GetNamedString("Grupo");
                                user.email = fotografo_item.GetNamedString("Mail");
                                user.Fotos = CloureManager.ParseInt(fotografo_item.GetNamedValue("Fotos"));

                                item.Fotografos.Add(user);
                            }
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

        public static async Task<Show> Get(int id)
        {
            Show item = new Show();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "shows"));
                cparams.Add(new CloureParam("topic", "obtener"));
                cparams.Add(new CloureParam("id", id));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonObject item_obj = api_result.GetNamedObject("Response");
                    item.Id = CloureManager.ParseInt(item_obj.GetNamedValue("Id"));
                    item.ArtistaId = CloureManager.ParseInt(item_obj.GetNamedValue("ArtistaId"));
                    item.LugarId = CloureManager.ParseInt(item_obj.GetNamedValue("LugarId"));
                    item.Fecha = CloureManager.ParseDate(item_obj.GetNamedValue("Fecha"));

                    JsonArray fotografosArr = item_obj.GetNamedArray("Fotografos");
                    item.Fotografos = new ObservableCollection<User>();

                    foreach (JsonValue fotografoItem in fotografosArr)
                    {
                        JsonObject fotografo = fotografoItem.GetObject();
                        User fotografoUser = new User();
                        fotografoUser.id = CloureManager.ParseInt(fotografo.GetNamedValue("Id"));
                        fotografoUser.nombre = fotografo.GetNamedString("Nombre");
                        fotografoUser.apellido = fotografo.GetNamedString("Apellido");
                        fotografoUser.razonsocial = fotografoUser.apellido + ", " + fotografoUser.nombre;
                        fotografoUser.email = fotografo.GetNamedString("Mail");
                        fotografoUser.grupo = fotografo.GetNamedString("Grupo");
                        fotografoUser.ImageURL = new Uri(fotografo.GetNamedString("Imagen"));
                        fotografoUser.Fotos = CloureManager.ParseInt(fotografo.GetNamedValue("Fotos"));
                        item.Fotografos.Add(fotografoUser);
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

            return item;
        }

        public static async Task<bool> save(Show show)
        {
            bool response = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "shows"));
                cparams.Add(new CloureParam("topic", "guardar"));
                cparams.Add(new CloureParam("id", show.Id));
                cparams.Add(new CloureParam("date_time", show.Fecha.Value.ToString("yyyy-MM-dd")));
                cparams.Add(new CloureParam("artista_id", show.ArtistaId));
                cparams.Add(new CloureParam("lugar_id", show.LugarId));
                cparams.Add(new CloureParam("images", show.Images));

                if (show.Fotografos != null)
                {
                    string ItemsArr = "[";
                    for (int i = 0; i < show.Fotografos.Count; i++)
                    {
                        ItemsArr += "{";
                        ItemsArr += "\"id\": \"" + show.Fotografos[i].id.ToString() + "\",";
                        ItemsArr += "\"fotos\": \"" + show.Fotografos[i].Fotos + "\"";
                        ItemsArr += "}";
                        if (i < show.Fotografos.Count - 1) ItemsArr += ",";
                    }

                    ItemsArr += "]";

                    cparams.Add(new CloureParam("fotografos", ItemsArr));
                }

                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                JsonObject error = api_result.GetNamedValue("Error").GetObject();
                ApiError apiError = new ApiError(error.GetNamedString("message"));

                if (apiError.message == "")
                {
                    //JsonObject api_response = api_result.GetNamedObject("Response");

                }
                else
                {
                    throw new Exception(apiError.message);
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

        public static async Task<bool> Delete(int id)
        {
            bool response = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "shows"));
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
