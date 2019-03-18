using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.users
{
    public static class Users
    {
        public static async Task<UsersResponse> getList(string filtro="", string ordenar_por = "", string orden = "", int pagina = 1, int Limite=50, string grupoId="", string Miembros="all")
        {
            UsersResponse response = new UsersResponse();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "users"));
                cparams.Add(new CloureParam("topic", "get_list"));
                cparams.Add(new CloureParam("filtro", filtro));
                cparams.Add(new CloureParam("ordenar_por", ordenar_por));
                cparams.Add(new CloureParam("orden", orden));
                cparams.Add(new CloureParam("pagina", pagina));
                cparams.Add(new CloureParam("limite", Limite));
                cparams.Add(new CloureParam("grupo_id", grupoId));
                cparams.Add(new CloureParam("miembros", Miembros));

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
                        User item = new User();

                        item.id = (int)register.GetNamedNumber("id");
                        item.empresa = register.GetNamedString("empresa");
                        item.nombre = register.GetNamedString("nombre");
                        item.apellido = register.GetNamedString("apellido");
                        item.razonsocial = item.apellido + ", " + item.nombre;
                        item.saldo_str = register.GetNamedString("saldo_str");
                        item.saldo = CloureManager.ParseNumber(register.GetNamedValue("saldo"));
                        item.grupo = register.GetNamedString("grupo");
                        item.email = register.GetNamedString("mail");
                        string img_path = register.GetNamedString("imagen");
                        item.ImageURL = new Uri(img_path);

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

                        response.Items.Add(item);
                    }

                    response.PageString = api_response.GetNamedString("PageString");
                    response.TotalRegistros = (int)api_response.GetNamedNumber("TotalRegistros");
                    response.TotalPaginas = (int)api_response.GetNamedNumber("TotalPaginas");
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

        public static async Task<User> GetUser(string userToken)
        {
            User user = new User();

            try
            {
                List <CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "users"));
                cparams.Add(new CloureParam("topic", "get_by_token"));
                cparams.Add(new CloureParam("usr_token", userToken));

                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error=="")
                {
                    JsonObject user_obj = api_result.GetNamedObject("Response");
                    user = load_data(user_obj);
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

            return user;
        }

        public static async Task<User> GetUserById(int id)
        {
            User user = new User();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "users"));
                cparams.Add(new CloureParam("topic", "get_by_id"));
                cparams.Add(new CloureParam("id", id.ToString()));

                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonObject user_obj = api_result.GetNamedObject("Response");
                    user = load_data(user_obj);
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

            return user;
        }

        public static async Task<List<PersonGender>> GetPersonGenders()
        {
            List<PersonGender> items = new List<PersonGender>();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "users"));
                cparams.Add(new CloureParam("topic", "get_genders"));

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
                        PersonGender item = new PersonGender();
                        item.Id = (int)register.GetNamedNumber("id");
                        item.Name = register.GetNamedString("name");
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

        private static User load_data(JsonObject user_obj)
        {
            User user = new User();

            user.id = (int)user_obj.GetNamedNumber("id");
            user.razonsocial = user_obj.GetNamedString("razon_social");
            user.nombre = user_obj.GetNamedString("nombre");
            user.apellido = user_obj.GetNamedString("apellido");
            user.grupo = user_obj.GetNamedString("grupo");
            user.grupo_id = user_obj.GetNamedString("grupo_id");
            user.telefono = user_obj.GetNamedString("telefono");
            user.email = user_obj.GetNamedString("mail");
            user.empresa = user_obj.GetNamedString("empresa");
            user.ImageURL = new Uri(user_obj.GetNamedString("imagen"));
            user.salario = CloureManager.ParseNumber(user_obj.GetNamedValue("salario_bruto"));
            user.comision = CloureManager.ParseNumber(user_obj.GetNamedValue("comision"));

            return user;
        }

        public static async Task<bool> save(User user)
        {
            bool response = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "users"));
                cparams.Add(new CloureParam("topic", "guardar"));
                cparams.Add(new CloureParam("id", user.id));
                cparams.Add(new CloureParam("nombre", user.nombre));
                cparams.Add(new CloureParam("apellido", user.apellido));
                cparams.Add(new CloureParam("grupo_id", user.grupo_id));
                cparams.Add(new CloureParam("empresa", user.empresa));
                cparams.Add(new CloureParam("genero_id", user.GeneroId));
                cparams.Add(new CloureParam("mail", user.email));
                cparams.Add(new CloureParam("salario_bruto", user.salario));
                cparams.Add(new CloureParam("comision", user.comision));
                if (user.CloureImage!=null) cparams.Add(new CloureParam("uploaded_image", user.CloureImage));

                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonObject api_response = api_result.GetNamedObject("Response");

                    if (user.id == 0)
                    {
                        string msg = "Usuario guardado con éxito!\nClave generada: " + api_response.GetNamedString("clave_raw");
                        CloureManager.ShowDialog(msg);
                    }
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

        public static async Task<bool> AddPayment(int usuarioId, double importe, int formaDePagoId, int formaDePagoEntidadId, string Cheque="")
        {
            bool result = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "users"));
                cparams.Add(new CloureParam("topic", "agregar_pago"));
                cparams.Add(new CloureParam("id", 0));
                cparams.Add(new CloureParam("usuario_id", usuarioId));
                cparams.Add(new CloureParam("importe", importe));
                cparams.Add(new CloureParam("forma_de_pago_id", formaDePagoId));
                cparams.Add(new CloureParam("forma_de_pago_entidad_id", formaDePagoEntidadId));
                cparams.Add(new CloureParam("cheque", Cheque));

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
                result = false;
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }

            return result;
        }

        public static async Task<bool> Delete(int id)
        {
            bool response = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "users"));
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

        public static async Task<bool> GeneratePass(int id)
        {
            bool response = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "users"));
                cparams.Add(new CloureParam("topic", "reasignar_clave_random"));
                cparams.Add(new CloureParam("id", id.ToString()));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                string response_str = api_result.GetNamedString("Response");
                if (error == "")
                {
                    CloureManager.ShowDialog("Se ha generado la nueva clave: " + response_str);
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
