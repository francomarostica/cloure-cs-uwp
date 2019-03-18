using Cloure.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.products_services_categories
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class ProductServiceCategoryAddPage : Page
    {
        ProductServiceCategory category = new ProductServiceCategory();
        ModuleInfo moduleInfo;

        public ProductServiceCategoryAddPage()
        {
            this.InitializeComponent();
            moduleInfo = CloureManager.GetModuleInfo();
            txtNombrePrompt.Text = moduleInfo.locales.GetNamedString("name");

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

        private async void save()
        {
            category.Name = txtNombre.Text;

            await ProductsServicesCategories.save(category);
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
                category.CloureImage = new CloureImage(file.Name, byteArray);
                imgControl.Source = await category.CloureImage.GetBitmapImage();
            }

        }
    }
}
