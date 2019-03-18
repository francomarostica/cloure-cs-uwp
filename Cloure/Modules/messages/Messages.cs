using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.messages
{
    public static class Messages
    {
        public static async Task<GenericResponse> GetList(string filtro = "", string ordenar_por = "", string orden = "", int Page = 1)
        {
            GenericResponse genericResponse = new GenericResponse();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "messages"));
                cparams.Add(new CloureParam("topic", "listar"));
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
                        Message item = new Message();
                        item.Subject = register.GetNamedString("Asunto");
                        item.Id = (int)register.GetNamedNumber("Id");
                        //item.Descripcion = register.GetNamedString("Descripcion");
                        //item.Cliente = register.GetNamedString("Cliente");

                        JsonArray available_commands_arr = register.GetNamedArray("AvailableCommands");
                        item.availableCommands = new List<AvailableCommand>();
                        foreach (JsonValue available_cmd_obj in available_commands_arr)
                        {
                            JsonObject available_cmd_item = available_cmd_obj.GetObject();
                            int available_cmd_id = (int)available_cmd_item.GetNamedNumber("Id");
                            string available_cmd_name = available_cmd_item.GetNamedString("Name");
                            string available_cmd_title = available_cmd_item.GetNamedString("Title");
                            AvailableCommand availableCommand = new AvailableCommand(available_cmd_id, available_cmd_name, available_cmd_title);
                            item.availableCommands.Add(availableCommand);
                        }

                        genericResponse.Items.Add(item);
                    }
                    genericResponse.PageString = api_response.GetNamedString("PageString");
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

            return genericResponse;
        }

        public static async Task<Message> Get(int id)
        {
            Message message = new Message();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "messages"));
                cparams.Add(new CloureParam("topic", "obtener"));
                cparams.Add(new CloureParam("notificacion_id", id.ToString()));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonObject msg_obj = api_result.GetNamedObject("Response");
                    JsonArray messages = msg_obj.GetNamedArray("Mensajes");

                    foreach(JsonValue itemVal in messages)
                    {
                        JsonObject item = itemVal.GetObject();
                        message.MessageText = item.GetNamedString("Mensaje").Replace("<br />", "\n");
                    }

                    message.DateStr = msg_obj.GetNamedString("FechaStr");
                    message.User = msg_obj.GetNamedString("Usuario");
                    message.Phone = msg_obj.GetNamedString("Telefono");
                    message.Mail = msg_obj.GetNamedString("Mail");
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

            return message;
        }

        public static async Task<bool> Delete(int id)
        {
            bool response = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "messages"));
                cparams.Add(new CloureParam("topic", "borrar"));
                cparams.Add(new CloureParam("id", id.ToString()));
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
