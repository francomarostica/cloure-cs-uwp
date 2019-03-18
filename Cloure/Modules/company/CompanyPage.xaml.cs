using Cloure.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.company
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class CompanyPage : Page
    {
        public string Logo = "";
        public CloureImage cloureImage;
        private ModuleInfo moduleInfo;

        public CompanyPage()
        {
            this.InitializeComponent();

            moduleInfo = CloureManager.GetModuleInfo();
            txtTipoEmpresaPrompt.Text = moduleInfo.locales.GetNamedString("company_type");
            txtCompanyNamePrompt.Text = moduleInfo.locales.GetNamedString("company_name");
            txtWebPrompt.Text = moduleInfo.locales.GetNamedString("website");

            Run run = new Run();
            run.Text = CloureManager.getPrimaryDomain();
            
            Hyperlink hyperlink = new Hyperlink();
            hyperlink.Inlines.Add(run);
            hyperlink.NavigateUri = new Uri("http://"+CloureManager.getPrimaryDomain());

            txtWeb.Text = "";
            txtWeb.Inlines.Add(hyperlink);

            txtCloureAccountDetails.Text = "Cloure " + CloureManager.getAccountType();

            GetBusinessTypes();

            LoadData();
        }

        public async void LoadData()
        {
            List<CloureParam> cloureParams = new List<CloureParam>();
            cloureParams.Add(new CloureParam("topic", "get_account_info")); 

            try
            {
                string api_response = await CloureManager.ExecuteAsync(cloureParams);
                JsonObject api_response_obj = JsonObject.Parse(api_response);
                Logo = api_response_obj.GetNamedString("logo");
                txtCompanyName.Text = api_response_obj.GetNamedString("company_name");
                txtTipoEmpresa.SelectedValue = api_response_obj.GetNamedString("business_type_id");

                imgLogo.Source = new BitmapImage(new Uri(Logo));
            }
            catch (Exception e)
            {
                var dialog = new MessageDialog(e.Message);
                await dialog.ShowAsync();
            }
        }

        public async void GetBusinessTypes()
        {
            txtTipoEmpresa.ItemsSource = await new BusinessTypes().getList();
            txtTipoEmpresa.DisplayMemberPath = "Title";
            txtTipoEmpresa.SelectedValuePath = "Id";
            txtTipoEmpresa.SelectedValue = "generic";
        }

        private async void get_file()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                var inputStream = await file.OpenSequentialReadAsync();
                var readStream = inputStream.AsStreamForRead();
                var byteArray = new byte[readStream.Length];
                await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                cloureImage = new CloureImage(file.Name, byteArray);
                imgLogo.Source = await cloureImage.GetBitmapImage();
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

        public async void save()
        {
            btnSave.IsEnabled = false;
            grdLoader.Visibility = Visibility.Visible;

            BusinessType businessType = (BusinessType)txtTipoEmpresa.SelectedItem;

            List<CloureParam> cloureParams = new List<CloureParam>();
            cloureParams.Add(new CloureParam("module", "company"));
            cloureParams.Add(new CloureParam("topic", "save"));
            cloureParams.Add(new CloureParam("company_name", txtCompanyName.Text));
            cloureParams.Add(new CloureParam("company_type", (string)txtTipoEmpresa.SelectedValue));
            if (cloureImage!=null) cloureParams.Add(new CloureParam("image", cloureImage));

            try
            {
                string api_response = await CloureManager.ExecuteAsync(cloureParams);
                JsonObject api_response_obj = JsonObject.Parse(api_response);
                string Error = api_response_obj.GetNamedString("Error");

                if (Error == "")
                {

                }
                else
                {
                    throw new Exception(Error);
                }
            }
            catch (Exception e)
            {
                var dialog = new MessageDialog(e.Message);
                await dialog.ShowAsync();
            }

            btnSave.IsEnabled = true;
            grdLoader.Visibility = Visibility.Collapsed;
        }

        private void btnImage_Click(object sender, RoutedEventArgs e)
        {
            get_file();
        }
    }
}
