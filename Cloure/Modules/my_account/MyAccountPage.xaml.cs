using Cloure.Core;
using Cloure.Modules.countries;
using Cloure.Modules.countries_n1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.my_account
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MyAccountPage : Page
    {
        ModuleInfo moduleInfo;
        CloureImage cloureImage;

        public MyAccountPage()
        {
            this.InitializeComponent();

            moduleInfo = CloureManager.GetModuleInfo();
            txtNombrePrompt.Text = moduleInfo.locales.GetNamedString("name");
            txtApellidoPrompt.Text = moduleInfo.locales.GetNamedString("last_name");
            txtTelefonoPrompt.Text = moduleInfo.locales.GetNamedString("phone");
            txtMailPrompt.Text = moduleInfo.locales.GetNamedString("email");
            txtPaisPrompt.Text = moduleInfo.locales.GetNamedString("country");
            txtPaisN1Prompt.Text = moduleInfo.locales.GetNamedString("state_province");

            LoadCountries();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadData();
        }

        private async void LoadCountries()
        {
            List<Country> countries = new List<Country>();
            countries = await Countries.GetList();
            txtPais.ItemsSource = countries;
            txtPais.DisplayMemberPath = "Name";
            txtPais.SelectedValuePath = "Id";
            txtPais.SelectedValue = 9;
        }

        private async void LoadCountriesN1(int CountryId)
        {
            List<CountryN1> countries = new List<CountryN1>();
            countries = await CountriesN1.GetList(CountryId);
            txtPaisN1.ItemsSource = countries;
            txtPaisN1.DisplayMemberPath = "Name";
            txtPaisN1.SelectedValuePath = "Id";
            txtPaisN1.SelectedValue = 1;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

        private async void save()
        {
            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "my_account"));
                cparams.Add(new CloureParam("topic", "guardar"));
                cparams.Add(new CloureParam("nombre", txtNombre.Text));
                cparams.Add(new CloureParam("apellido", txtApellido.Text));
                cparams.Add(new CloureParam("telefono", txtTelefono.Text));
                cparams.Add(new CloureParam("mail", txtMail.Text));
                cparams.Add(new CloureParam("pais_id", txtPais.SelectedValue));
                cparams.Add(new CloureParam("pais_n1_id", txtPaisN1.SelectedValue));
                if(cloureImage!=null) cparams.Add(new CloureParam("image", cloureImage));
                string res = await CloureManager.ExecuteAsync(cparams);
                JsonObject api_result = JsonObject.Parse(res);

                string error = api_result.GetNamedString("Error");
                string response = api_result.GetNamedString("Response");

                if (error == "")
                {
                    var dialog = new MessageDialog(response);
                    await dialog.ShowAsync();
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

        private void btnImage_Click(object sender, RoutedEventArgs e)
        {
            get_file();
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
                imgPhoto.Source = await cloureImage.GetBitmapImage();
            }
        }

        private void btnChangePass_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.Navigate(typeof(ChangePassPage));
        }

        private async void LoadData()
        {
            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "my_account"));
                cparams.Add(new CloureParam("topic", "get_data"));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                if (error == "")
                {
                    JsonObject item_obj = api_result.GetNamedObject("Response");
                    txtNombre.Text = item_obj.GetNamedString("nombre");
                    txtApellido.Text = item_obj.GetNamedString("apellido");
                    txtMail.Text = item_obj.GetNamedString("mail");
                    txtTelefono.Text = item_obj.GetNamedString("telefono");
                    imgPhoto.Source = new BitmapImage(new Uri(item_obj.GetNamedString("imagen")));

                    txtPais.SelectedValue = CloureManager.ParseInt(item_obj.GetNamedValue("pais_id"));
                    txtPaisN1.SelectedValue = CloureManager.ParseInt(item_obj.GetNamedValue("pais_n1_id"));
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

        private void txtPais_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            Country country = (Country)comboBox.SelectedItem;
            if(country!=null) LoadCountriesN1(country.Id);
        }
    }
}
