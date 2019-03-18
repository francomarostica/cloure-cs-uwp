using Cloure.Core;
using Cloure.Modules.receipts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;

namespace Cloure.Modules.invoicing
{
    public static class Invoicing
    {
        public static async Task<int> save(Receipt receipt)
        {
            int result = 0;

            try
            {
                string ItemsArr = "[";
                for (int i = 0; i < receipt.cartItems.Count; i++)
                {
                    ItemsArr += "{";
                    ItemsArr += "\"id\": \"" + receipt.cartItems[i].ProductoId.ToString() + "\",";
                    ItemsArr += "\"cantidad\": \"" + receipt.cartItems[i].Cantidad + "\",";
                    ItemsArr += "\"detalle\": \"" + receipt.cartItems[i].Descripcion + "\",";
                    ItemsArr += "\"precio\": \"" + receipt.cartItems[i].PrecioUnitario + "\",";
                    ItemsArr += "\"iva\": \"" + receipt.cartItems[i].Iva + "\",";
                    ItemsArr += "\"importe\": \"" + receipt.cartItems[i].Importe + "\"";
                    ItemsArr += "}";
                    if (i < receipt.cartItems.Count - 1) ItemsArr += ",";
                }

                ItemsArr += "]";

                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "receipts"));
                cparams.Add(new CloureParam("topic", "guardar"));
                cparams.Add(new CloureParam("tipo_comprobante_id",receipt.TypeId.ToString()));
                cparams.Add(new CloureParam("cliente_id", receipt.CustomerId.ToString()));
                cparams.Add(new CloureParam("items", ItemsArr));
                cparams.Add(new CloureParam("entrega", receipt.Entrega.ToString()));
                cparams.Add(new CloureParam("forma_de_pago_id", receipt.FormaDePagoId.ToString()));
                cparams.Add(new CloureParam("forma_de_pago_entidad_id", receipt.FormaDePagoEntidadId.ToString()));
                cparams.Add(new CloureParam("forma_de_pago", receipt.FormaDePagoEntidadId.ToString()));
                cparams.Add(new CloureParam("forma_de_pago_data", receipt.FormaDePagoData));
                cparams.Add(new CloureParam("forma_de_pago_cobro", receipt.FormaDePagoCobro.Value.ToString("yyyy-mm-dd")));
                cparams.Add(new CloureParam("sucursal_id", receipt.CompanyBranchId.ToString()));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonObject api_response = api_result.GetNamedObject("Response");
                    result = CloureManager.ParseInt(api_response.GetNamedValue("ComprobanteId"));
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

            return result;
        }
    }
}
