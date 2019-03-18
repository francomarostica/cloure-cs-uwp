using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.products_services
{
    public static class ProductsServices
    {
        public static async Task<bool> save(ProductService product)
        {
            bool response = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "products_services"));
                cparams.Add(new CloureParam("topic", "guardar"));
                cparams.Add(new CloureParam("id", product.Id.ToString()));
                cparams.Add(new CloureParam("tipo_producto_id", product.ProductTypeId.ToString()));
                cparams.Add(new CloureParam("sistema_medida_id", product.MeasureUnitId.ToString()));
                cparams.Add(new CloureParam("titulo", product.Title));
                cparams.Add(new CloureParam("descripcion", product.Descripcion));
                cparams.Add(new CloureParam("iva", product.IVA));
                cparams.Add(new CloureParam("costo_precio", product.CostoPrecio));
                cparams.Add(new CloureParam("costo_importe", product.CostoImporte));
                cparams.Add(new CloureParam("venta_precio", product.VentaPrecio));
                cparams.Add(new CloureParam("venta_importe", product.VentaImporte));
                cparams.Add(new CloureParam("images", product.Images));

                if (product.Stock != null)
                {
                    string stock_str = "[";
                    foreach (ProductStock productStock in product.Stock)
                    {
                        stock_str += "{";
                        stock_str += "\"inmueble_id\":\""+productStock.PropiedadId+"\",";
                        stock_str += "\"min\":\""+productStock.Min.ToString("F2", CultureInfo.InvariantCulture)+ "\",";
                        stock_str += "\"actual\":\""+productStock.Actual.ToString("F2", CultureInfo.InvariantCulture)+ "\"";
                        stock_str += "},";
                    }
                    stock_str = stock_str.TrimEnd(',');
                    stock_str += "]";
                    cparams.Add(new CloureParam("stock", stock_str));
                }
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
                cparams.Add(new CloureParam("module", "products_services"));
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

        public static async Task<GenericResponse> GetList(string filtro="", string ordenar_por = "", string orden = "", int Page = 1)
        {
            GenericResponse response = new GenericResponse();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "products_services"));
                cparams.Add(new CloureParam("topic", "listar"));
                if(filtro.Length>0) cparams.Add(new CloureParam("filtro", filtro));
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
                        ProductService item = new ProductService();
                        item.Id = (int)register.GetNamedNumber("Id");
                        item.Title = register.GetNamedString("Titulo");
                        item.MeasureUnitId = (int)register.GetNamedNumber("SistemaMedidaId");
                        item.Importe = (double)register.GetNamedNumber("Importe");
                        item.ImporteStr = register.GetNamedString("ImporteStr");
                        string url_image_path = register.GetNamedString("ImagenPath");
                        item.ImagePath = new Uri(url_image_path);
                        item.StockTotal = CloureManager.ParseNumber(register.GetNamedValue("StockTotal"));
                        item.StockTotalStr = register.GetNamedString("StockTotalStr");
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

        public static async Task<ProductService> GetItem(int id)
        {
            ProductService item = new ProductService();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "products_services"));
                cparams.Add(new CloureParam("topic", "obtener"));
                cparams.Add(new CloureParam("id", id.ToString()));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonObject item_obj = api_result.GetNamedObject("Response");
                    item.Id = (int)item_obj.GetNamedNumber("Id");
                    item.ProductTypeId = (int)item_obj.GetNamedNumber("TipoProductoId");
                    item.MeasureUnitId = (int)item_obj.GetNamedNumber("SistemaMedidaId");

                    item.Title = item_obj.GetNamedString("Titulo");
                    item.Descripcion = item_obj.GetNamedString("Descripcion");
                    item.CategoriaN1Id = (int)item_obj.GetNamedNumber("CategoriaN1Id");
                    item.CategoriaN2Id = (int)item_obj.GetNamedNumber("CategoriaN2Id");
                    item.CodigoInterno = item_obj.GetNamedString("Codigo");
                    item.CodigoBarras = item_obj.GetNamedString("CodigoBarras");

                    item.IVA = CloureManager.ParseNumber(item_obj.GetNamedValue("Iva"));

                    item.CostoPrecio = CloureManager.ParseNumber(item_obj.GetNamedValue("CostoPrecio"));
                    item.CostoImporte = CloureManager.ParseNumber(item_obj.GetNamedValue("CostoImporte"));
                    item.VentaPrecio = CloureManager.ParseNumber(item_obj.GetNamedValue("VentaPrecio"));
                    item.VentaImporte = CloureManager.ParseNumber(item_obj.GetNamedValue("VentaImporte"));

                    item.StockTotal = CloureManager.ParseNumber(item_obj.GetNamedValue("StockActual"));
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

        public static async Task<List<ProductType>> GetTypes()
        {
            List<ProductType> productTypes = new List<ProductType>();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "products_services_types"));
                cparams.Add(new CloureParam("topic", "get_list"));
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
                        ProductType item = new ProductType();
                        item.Id = (int)register.GetNamedNumber("Id");
                        item.Name = register.GetNamedString("Title");
                        productTypes.Add(item);
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

            return productTypes;
        }

        public static async Task<List<ProductStock>> GetStock(int id)
        {
            List<ProductStock> productStocks = new List<ProductStock>();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "products_services"));
                cparams.Add(new CloureParam("topic", "cargar_stock"));
                cparams.Add(new CloureParam("producto_id", id));
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

                        ProductStock item = new ProductStock();
                        item.PropiedadId = (int)register.GetNamedNumber("PropiedadId");
                        item.PropiedadNombre = register.GetNamedString("PropiedadNombre");
                        item.MinPrompt = register.GetNamedString("MinPrompt");
                        item.CurrentPrompt = register.GetNamedString("CurrentPrompt");
                        item.Actual = CloureManager.ParseNumber(register.GetNamedValue("Actual"));
                        item.Min = CloureManager.ParseNumber(register.GetNamedValue("Min"));

                        productStocks.Add(item);
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

            return productStocks;
        }

        public static async Task<List<ProductMeasureUnit>> GetUnits()
        {
            List<ProductMeasureUnit> productTypes = new List<ProductMeasureUnit>();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "products_services_units"));
                cparams.Add(new CloureParam("topic", "get_list"));
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
                        ProductMeasureUnit item = new ProductMeasureUnit();
                        item.Id = (int)register.GetNamedNumber("Id");
                        item.Title = register.GetNamedString("Title");
                        productTypes.Add(item);
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

            return productTypes;
        }
    }
}
