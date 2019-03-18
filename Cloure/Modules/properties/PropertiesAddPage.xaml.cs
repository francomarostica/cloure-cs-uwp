using Cloure.Core;
using Cloure.Modules.countries;
using Cloure.Modules.countries_n1;
using Cloure.Modules.currencies;
using Cloure.Modules.properties_operations;
using Cloure.Modules.properties_states;
using Cloure.Modules.properties_types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Cloure.Modules.properties
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class PropertiesAddPage : Page
    {
        private List<properties_types.PropertyType> propertyTypes;
        private List<PropertyOperation> propertyOperations;
        private List<CloureImage> images = new List<CloureImage>();

        public PropertiesAddPage()
        {
            this.InitializeComponent();

            LoadPropertiesTypes();
            LoadOperations();
            LoadCountries();
            LoadCurrencies();

            if (CloureManager.getAccountType() == "free") txtImgAdvice.Text = "En esta versión de cloure solo podrás cargar una imagen";
        }

        private async void LoadPropertiesTypes()
        {
            propertyTypes = await PropertiesTypes.getList();
            txtPropertyType.ItemsSource = propertyTypes;
            txtPropertyType.DisplayMemberPath = "Nombre";
            txtPropertyType.SelectedValuePath = "Id";
            txtPropertyType.SelectedValue = 1;
        }

        private async void LoadOperations()
        {
            propertyOperations = await PropertiesOperations.getList();
            txtOperation.ItemsSource = propertyOperations;
            txtOperation.DisplayMemberPath = "Nombre";
            txtOperation.SelectedValuePath = "Id";
            txtOperation.SelectedValue = 1;
        }

        private async void LoadStates(int OperationId)
        {
            List<PropertyState> propertyStates = new List<PropertyState>();
            propertyStates = await PropertiesStates.GetList(OperationId);
            txtEstado.ItemsSource = propertyStates;
            txtEstado.DisplayMemberPath = "Name";
            txtEstado.SelectedValuePath = "Id";
            txtEstado.SelectedIndex = 0;
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

        private async void LoadCurrencies()
        {
            List<Currency> currencies = new List<Currency>();
            currencies = await Currencies.GetList();

            txtAlquilerMoneda.ItemsSource = currencies;
            txtAlquilerMoneda.DisplayMemberPath = "Name";
            txtAlquilerMoneda.SelectedValuePath = "Id";
            txtAlquilerMoneda.SelectedValue = 32;

            txtVentaMoneda.ItemsSource = currencies;
            txtVentaMoneda.DisplayMemberPath = "Name";
            txtVentaMoneda.SelectedValuePath = "Id";
            txtVentaMoneda.SelectedValue = 32;
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

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

        private async void save()
        {
            Property property = new Property();
            property.TipoId = (int)txtPropertyType.SelectedValue;
            property.OperacionId = (int)txtOperation.SelectedValue;
            property.Titulo = txtTitulo.Text;

            bool result = await Properties.save(property);
            if (result) CloureManager.GoBack("load");
        }

        private void btnAddImage_Click(object sender, RoutedEventArgs e){
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

            if (CloureManager.getAccountType() == "free")
            {
                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    images = new List<CloureImage>();
                    IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                    //CloureImage cloureImage = await CloureImage.CreateFromStream(stream, file.Name);
                    //images.Add(cloureImage);
                }
            }
            else
            {
                IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
                foreach (StorageFile file in files)
                {
                    IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                    //CloureImage cloureImage = await CloureImage.CreateFromStream(stream, file.Name);
                    //images.Add(cloureImage);
                    //stream.Dispose();
                }
            }

            lstImages.ItemsSource = null;
            lstImages.ItemsSource = images;
        }

        private void txtPais_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            Country country = (Country)comboBox.SelectedItem;
            LoadCountriesN1(country.Id);
        }

        private void txtOperation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            PropertyOperation propertyOperation = (PropertyOperation)comboBox.SelectedItem;
            LoadStates(propertyOperation.Id);
        }
    }
}
