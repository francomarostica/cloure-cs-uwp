using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.products_services_categories
{
    public static class ProductsServicesCategories
    {
        public static async Task<GenericResponse> GetList(int TipoId, string filtro = "", string ordenar_por = "", string orden = "", int Page = 1)
        {
            GenericResponse response = new GenericResponse();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "products_services_categories"));
                cparams.Add(new CloureParam("topic", "listar"));
                cparams.Add(new CloureParam("tipo_id", TipoId));
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
                        ProductServiceCategory item = new ProductServiceCategory();
                        item.Id = (int)register.GetNamedNumber("Id");
                        item.Name = register.GetNamedString("Nombre");

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
                    response.PageString = api_response.GetNamedString("PageString");
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

        public static async Task<List<ProductCategoryType>> GetTypes()
        {
            List<ProductCategoryType> response = new List<ProductCategoryType>();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "products_services_categories"));
                cparams.Add(new CloureParam("topic", "listar_tipos"));
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
                        ProductCategoryType item = new ProductCategoryType();
                        item.Id = (int)register.GetNamedNumber("Id");
                        item.Nombre = register.GetNamedString("Nombre");

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

        public static async Task<bool> save(ProductServiceCategory product)
        {
            bool response = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "products_services_categories"));
                cparams.Add(new CloureParam("topic", "guardar"));
                cparams.Add(new CloureParam("id", product.Id));
                cparams.Add(new CloureParam("nombre", product.Name));
                if(product.CloureImage!=null) cparams.Add(new CloureParam("image", product.CloureImage));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonObject api_response = api_result.GetNamedObject("Response");
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

        public static async Task<bool> Delete(int id)
        {
            bool response = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "products_services_categories"));
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
