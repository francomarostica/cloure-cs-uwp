using Cloure.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Popups;

namespace Cloure.Modules.receipts
{
    public class Receipts
    {
        public static async Task<GenericResponse> GetList(string filtro = "", string ordenar_por = "", string orden = "", int Page=1)
        {
            GenericResponse genericResponse = new GenericResponse();

            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "receipts"));
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
                    genericResponse.TotalPages = (int)api_response.GetNamedNumber("TotalPaginas");

                    foreach (JsonValue jsonValue in registers)
                    {
                        JsonObject register = jsonValue.GetObject();
                        Receipt receipt = new Receipt();
                        receipt.Id = (int)register.GetNamedNumber("Id");
                        receipt.FechaStr = register.GetNamedString("FechaStr");
                        receipt.Description = register.GetNamedString("Detalles");
                        receipt.Total = CloureManager.ParseNumber(register.GetNamedValue("Total"));
                        receipt.TotalStr = register.GetNamedString("TotalStr");
                        genericResponse.Items.Add(receipt);
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

        public static async Task<Receipt> Get(int id)
        {
            Receipt receipt = new Receipt();
            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "receipts"));
                cparams.Add(new CloureParam("topic", "obtener"));
                cparams.Add(new CloureParam("id", id.ToString()));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonObject api_response = api_result.GetNamedObject("Response");
                    receipt.CustomerName = api_response.GetNamedString("Usuario");
                    receipt.CustomerAddress = api_response.GetNamedString("UsuarioDomicilio");
                    receipt.Total = CloureManager.ParseNumber(api_response.GetNamedValue("Total"));
                    List<CartItem> items = new List<CartItem>();

                    JsonArray values = api_response.GetNamedArray("Items");
                    foreach(JsonValue jsonValue in values)
                    {
                        double cant = 0;

                        JsonObject item_obj = jsonValue.GetObject();
                        CartItem item = new CartItem();
                        item.ProductoId = (int)item_obj.GetNamedNumber("ProductoId");
                        item.Cantidad = CloureManager.ParseNumber(item_obj.GetNamedValue("Cantidad"));
                        item.Descripcion = item_obj.GetNamedString("Detalles");
                        item.Importe = CloureManager.ParseNumber(item_obj.GetNamedValue("Importe"));
                        item.Total = CloureManager.ParseNumber(item_obj.GetNamedValue("Total"));
                        items.Add(item);
                    }

                    receipt.cartItems = items;
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

            return receipt;
        }

        public async Task print(int comprobante_id)
        {
            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "receipts"));
                cparams.Add(new CloureParam("topic", "export_pdf"));
                cparams.Add(new CloureParam("comprobante_id", comprobante_id));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonObject response = api_result.GetNamedObject("Response");
                    Uri uri = new Uri(response.GetNamedString("url"));
                    string filename = response.GetNamedString("file_name");

                    StorageFile destinationFile = await DownloadsFolder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);

                    BackgroundDownloader downloader = new BackgroundDownloader();
                    DownloadOperation download = downloader.CreateDownload(uri, destinationFile);
                    await HandleDownloadAsync(download, true);
                    await Windows.System.Launcher.LaunchFileAsync(download.ResultFile);
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
        }

        private CancellationTokenSource cts;
        private async Task HandleDownloadAsync(DownloadOperation download, bool start)
        {
            try
            {
                Progress<DownloadOperation> progressCallback = new Progress<DownloadOperation>(DownloadProgress);
                if (start)
                {
                    // Start the download and attach a progress handler.
                    await download.StartAsync().AsTask(cts.Token, progressCallback);
                }
                else
                {
                    // The download was already running when the application started, re-attach the progress handler.
                    await download.AttachAsync().AsTask(cts.Token, progressCallback);
                }

                ResponseInformation response = download.GetResponseInformation();
                string statusCode = response != null ? response.StatusCode.ToString() : String.Empty;
            }
            catch (TaskCanceledException)
            {
                //LogStatus("Canceled: " + download.Guid, NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                /*
                if (!IsExceptionHandled("Execution error", ex, download))
                {
                    throw;
                }
                */
            }
        }

        private static void DownloadProgress(DownloadOperation download)
        {
            BackgroundDownloadProgress currentProgress = download.Progress;

            double percent = 100;
            if (currentProgress.TotalBytesToReceive > 0)
            {
                percent = currentProgress.BytesReceived * 100 / currentProgress.TotalBytesToReceive;
            }

            if (currentProgress.HasResponseChanged)
            {
                ResponseInformation response = download.GetResponseInformation();
                int headersCount = response != null ? response.Headers.Count : 0;
            }
        }
    }



}
