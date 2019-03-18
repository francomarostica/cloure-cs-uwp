using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.finances
{
    public class Finances
    {
        public async Task<bool> save(FinanceMovement finance)
        {
            bool response = true;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "finances"));
                cparams.Add(new CloureParam("topic", "guardar"));
                cparams.Add(new CloureParam("fecha", finance.FechaStr));
                cparams.Add(new CloureParam("operacion", ""));
                cparams.Add(new CloureParam("descripcion", finance.Detalles));
                cparams.Add(new CloureParam("importe", finance.ImporteStr));
                cparams.Add(new CloureParam("forma_de_pago", finance.FormaDePagoId.ToString()));
                cparams.Add(new CloureParam("forma_de_pago_entidad_id", "0"));
                cparams.Add(new CloureParam("tipo_movimiento", finance.TipoMovimientoId));
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

        public async Task<FinancesResponse> getList(string filtro="", string ordenar_por="", string orden="", string desde="", string hasta="", string movement_type="", string sucursal="0", int Page=1)
        {
            FinancesResponse financesResponse = new FinancesResponse();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "finances"));
                cparams.Add(new CloureParam("topic", "listar"));
                if (filtro.Length > 0) cparams.Add(new CloureParam("filtro", filtro));
                if (ordenar_por.Length > 0) cparams.Add(new CloureParam("ordenar_por", ordenar_por));
                if (orden.Length > 0) cparams.Add(new CloureParam("orden", orden));
                if (desde.Length > 0) cparams.Add(new CloureParam("desde", desde));
                if (hasta.Length > 0) cparams.Add(new CloureParam("hasta", hasta));
                if (movement_type.Length > 0) cparams.Add(new CloureParam("tipo_movimiento", movement_type));
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
                        FinanceMovement financeMovement = new FinanceMovement();
                        financeMovement.FechaStr = register.GetNamedString("FechaStr");
                        financeMovement.Detalles = register.GetNamedString("Detalles");
                        financeMovement.ImporteStr = register.GetNamedString("ImporteStr");

                        financesResponse.financeMovements.Add(financeMovement);
                    }
                    financesResponse.TotalPages = (int)api_response.GetNamedNumber("TotalPaginas");

                    //financesResponse.TotalIngresos = api_response.GetNamedNumber("TotalIngresos");
                    //financesResponse.TotalGastos = api_response.GetNamedNumber("TotalEgresos");
                    //financesResponse.Saldo = api_response.GetNamedNumber("Saldo");

                    financesResponse.TotalIngresosStr = api_response.GetNamedString("TotalIngresosStr");
                    financesResponse.TotalGastosStr = api_response.GetNamedString("TotalEgresosStr");
                    financesResponse.SaldoStr = api_response.GetNamedString("SaldoStr");
                    financesResponse.PageString = api_response.GetNamedString("PageString");
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

            return financesResponse;
        }

        public async Task<FinancesResponse> getListCC(int UsuarioId, string filtro = "", string ordenar_por = "", string orden = "", string desde = "", string hasta = "", int Page = 1)
        {
            FinancesResponse financesResponse = new FinancesResponse();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "finances"));
                cparams.Add(new CloureParam("topic", "listarCC"));
                cparams.Add(new CloureParam("usuario_id", UsuarioId));
                if (filtro.Length > 0) cparams.Add(new CloureParam("filtro", filtro));
                if (ordenar_por.Length > 0) cparams.Add(new CloureParam("ordenar_por", ordenar_por));
                if (orden.Length > 0) cparams.Add(new CloureParam("orden", orden));
                if (desde.Length > 0) cparams.Add(new CloureParam("desde", desde));
                if (hasta.Length > 0) cparams.Add(new CloureParam("hasta", hasta));
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
                        FinanceMovement financeMovement = new FinanceMovement();
                        financeMovement.FechaStr = register.GetNamedString("FechaStr");
                        financeMovement.Detalles = register.GetNamedString("Detalles");
                        financeMovement.ImporteStr = register.GetNamedString("ImporteStr");

                        financesResponse.financeMovements.Add(financeMovement);
                    }
                    financesResponse.TotalPages = (int)api_response.GetNamedNumber("TotalPaginas");
                    financesResponse.Saldo = CloureManager.ParseNumber(api_response.GetNamedValue("SaldoCC"));
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

            return financesResponse;
        }

        public static async Task<JsonObject> printCC(int user_id)
        {
            JsonObject response = null;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "finances"));
                cparams.Add(new CloureParam("topic", "export_pdf_cc"));
                cparams.Add(new CloureParam("usuario_id", user_id));
                string res = await CloureManager.ExecuteAsync(cparams);

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

        public static async Task<bool> isMonthlyIncomingExceded()
        {
            bool response = false;

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "finances"));
                cparams.Add(new CloureParam("topic", "is_monthly_incoming_exceeded"));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    response = CloureManager.ParseBoolObject(api_result.GetNamedValue("Response"));
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
