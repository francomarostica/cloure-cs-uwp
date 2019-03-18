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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.bands_artists
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class BandArtistAddPage : Page
    {
        public BandArtist bandArtist;

        public BandArtistAddPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                if (e.Parameter.GetType() == typeof(int))
                {
                    int id = (int)e.Parameter;
                    getItem(id);
                }
            }
            else
            {
                bandArtist = new BandArtist();
                bandArtist.Id = 0;
            }
        }

        public async void getItem(int id)
        {
            bandArtist = await BandsArtists.Get(id);
            txtNombre.Text = bandArtist.Nombre;
            imgLogo.Source = new BitmapImage(bandArtist.ImageUrl);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            save();
        }
        public async void save()
        {
            btnSave.IsEnabled = false;
            grdLoader.Visibility = Visibility.Visible;

            bandArtist.Nombre = txtNombre.Text;

            int response = await BandsArtists.save(bandArtist);
            if (response>0)
            {
                btnSave.IsEnabled = true;
                grdLoader.Visibility = Visibility.Collapsed;
                CloureManager.GoBack(new CloureParam("artist_id", response));
            }
            else
            {
                btnSave.IsEnabled = true;
                grdLoader.Visibility = Visibility.Collapsed;
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
                bandArtist.CloureImage = new CloureImage(file.Name, byteArray);
                imgLogo.Source = await bandArtist.CloureImage.GetBitmapImage();
            }

        }
    }
}
