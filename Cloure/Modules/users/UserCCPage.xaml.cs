using Cloure.Modules.finances;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.users
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class UserCCPage : Page
    {
        int UserId = 0;
        int TotalPages = 0;
        private CancellationTokenSource cts;

        public UserCCPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UserId = (int)e.Parameter;
            LoadData();
        }

        private async void LoadData()
        {
            FinancesResponse financesResponse = await new Finances().getListCC(UserId);
            TotalPages = financesResponse.TotalPages;

            if (financesResponse.financeMovements.Count > 0)
                grdNoRegisters.Visibility = Visibility.Collapsed;
            else
                grdNoRegisters.Visibility = Visibility.Visible;

            tbSaldo.Text = "Saldo: "+financesResponse.Saldo.ToString("C2");
            /*
            grdLoader.Visibility = Visibility.Collapsed;

            if (Page == 1)
            {
                btnFirst.IsEnabled = false;
                btnPrevious.IsEnabled = false;
            }
            if (Page > 1)
            {
                btnFirst.IsEnabled = true;
                btnPrevious.IsEnabled = true;
            }
            if (Page < TotalPages)
            {
                btnNext.IsEnabled = true;
                btnLast.IsEnabled = true;
            }
            if (Page == TotalPages)
            {
                btnNext.IsEnabled = false;
                btnLast.IsEnabled = false;
            }

            txtRegister.Text = "Mostrando página " + Page.ToString() + " de " + TotalPages.ToString();
            */

            lstFinances.ItemsSource = financesResponse.financeMovements;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            print();
        }

        private async void print()
        {
            //var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            try
            {
                LoaderText.Text = "Generando documento PDF...";
                grdLoader.Visibility = Visibility.Visible;

                JsonObject result = await Finances.printCC(UserId);
                Uri uri = new Uri(result.GetNamedString("url"));
                string filename = result.GetNamedString("file_name");

                StorageFile destinationFile = await DownloadsFolder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);

                BackgroundDownloader downloader = new BackgroundDownloader();
                DownloadOperation download = downloader.CreateDownload(uri, destinationFile);
                await HandleDownloadAsync(download, true);
                await Windows.System.Launcher.LaunchFileAsync(download.ResultFile);
                grdLoader.Visibility = Visibility.Collapsed;
                LoaderText.Text = "";
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }
        }

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

        private void DownloadProgress(DownloadOperation download)
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
