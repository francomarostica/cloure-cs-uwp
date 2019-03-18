using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.company_branches
{
    public class CompanyBranches
    {
        public static async Task<int> save(CompanyBranch companyBranch)
        {
            int response = 0;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "company_branches"));
                cparams.Add(new CloureParam("topic", "guardar"));
                cparams.Add(new CloureParam("id", companyBranch.Id));
                cparams.Add(new CloureParam("nombre", companyBranch.Name));
                cparams.Add(new CloureParam("direccion", companyBranch.Address));
                cparams.Add(new CloureParam("telefono", companyBranch.Phone));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    //returns branch id
                    //JsonObject api_response = api_result.GetNamedObject("Response");
                    response = CloureManager.ParseInt(api_result.GetNamedValue("Response"));
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                response = 0;
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }

            return response;
        }

        public static async Task<CompanyBranch> Get(int id)
        {
            CompanyBranch itemResult = new CompanyBranch();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "company_branches"));
                cparams.Add(new CloureParam("topic", "obtener"));
                cparams.Add(new CloureParam("id", id));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonObject itemObj = api_result.GetNamedObject("Response");
                    itemResult.Id = CloureManager.ParseInt(itemObj.GetNamedValue("Id"));
                    itemResult.Name = itemObj.GetNamedString("Nombre");
                    itemResult.Address = itemObj.GetNamedString("Direccion");
                    itemResult.Phone = itemObj.GetNamedString("Telefono");
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

            return itemResult;
        }

        public static async Task<CompanyBranchResponse> getList(string filtro = "", string ordenar_por = "", string orden = "", string sucursal = "0")
        {
            CompanyBranchResponse response = new CompanyBranchResponse();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "company_branches"));
                cparams.Add(new CloureParam("topic", "listar"));
                if (ordenar_por.Length > 0) cparams.Add(new CloureParam("ordenar_por", ordenar_por));
                if (orden.Length > 0) cparams.Add(new CloureParam("orden", orden));
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
                        CompanyBranch companyBranch = new CompanyBranch();
                        companyBranch.Id = CloureManager.ParseInt(register.GetNamedValue("Id"));
                        companyBranch.Name = register.GetNamedString("Nombre");

                        JsonArray available_commands_arr = register.GetNamedArray("AvailableCommands");
                        companyBranch.AvailableCommands = new List<AvailableCommand>();
                        foreach (JsonValue available_cmd_obj in available_commands_arr)
                        {
                            JsonObject available_cmd_item = available_cmd_obj.GetObject();
                            int available_cmd_id = (int)available_cmd_item.GetNamedNumber("Id");
                            string available_cmd_name = available_cmd_item.GetNamedString("Name");
                            string available_cmd_title = available_cmd_item.GetNamedString("Title");
                            AvailableCommand availableCommand = new AvailableCommand(available_cmd_id, available_cmd_name, available_cmd_title);
                            companyBranch.AvailableCommands.Add(availableCommand);
                        }

                        response.Items.Add(companyBranch);
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

        public static async Task<bool> Delete(int id)
        {
            bool response = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "company_branches"));
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
