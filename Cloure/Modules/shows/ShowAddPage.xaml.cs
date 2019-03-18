using Cloure.Core;
using Cloure.Modules.bands_artists;
using Cloure.Modules.places;
using Cloure.Modules.users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.shows
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class ShowAddPage : Page
    {
        Show show;
        private List<CloureImage> images = new List<CloureImage>();
        private ObservableCollection<User> fotografos = new ObservableCollection<User>();

        public ShowAddPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            LoadBandArtists();
            LoadPlaces();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            lstPhotographers.ItemsSource = null;

            if (e.Parameter != null)
            {
                if(e.Parameter.GetType() == typeof(int))
                {
                    int id = (int)e.Parameter;
                    show = await Shows.Get(id);
                    txtFecha.Date = show.Fecha.Value;
                    txtBandaArtista.SelectedValue = show.ArtistaId;
                    txtLugar.SelectedValue = show.LugarId;
                    lstPhotographers.ItemsSource = null;
                    lstPhotographers.ItemsSource = show.Fotografos;
                    //LoadData(id);
                }
            }
            else
            {
                show = new Show();
                show.Id = 0;
                txtFecha.Date = DateTime.Now;
                txtBandaArtista.SelectedIndex = -1;
                txtLugar.SelectedIndex = -1;
            }

            //Verify GoBack Method
            var cParameter = CloureManager.GetParameter();
            if (cParameter!=null)
            {
                if(cParameter.GetType() == typeof(CloureParam))
                {
                    CloureParam cloureParam = (CloureParam)cParameter;
                    if (cloureParam.name == "place_id") LoadPlaces((int)cloureParam.value);
                    if (cloureParam.name == "artist_id") LoadBandArtists((int)cloureParam.value);
                }
                else if (cParameter.GetType() == typeof(ObservableCollection<User>))
                {
                    fotografos = (ObservableCollection<User>)cParameter;
                    show.Fotografos = fotografos;
                    lstPhotographers.ItemsSource = null;
                    lstPhotographers.ItemsSource = fotografos;
                }
            }
        }

        private async void LoadData(int id)
        {
            show = await Shows.Get(id);
            txtFecha.Date = show.Fecha.Value;
            txtBandaArtista.SelectedValue = show.ArtistaId;
            txtLugar.SelectedValue = show.LugarId;

            lstPhotographers.ItemsSource = null;
            lstPhotographers.ItemsSource = show.Fotografos;
            //lstPhotographers.ItemsSource = fotografos;
        }

        private async void Save()
        {
            if(txtBandaArtista.SelectedValue == null)
            {
                CloureManager.ShowDialog("Debes seleccionar una banda/artista");
            }
            else
            {
                show.ArtistaId = (int)txtBandaArtista.SelectedValue;
                show.LugarId = (int)txtLugar.SelectedValue;
                show.Fecha = txtFecha.Date.Date;
                show.Images = images;
                show.Fotografos = fotografos;

                if (await Shows.save(show))
                {
                    CloureManager.GoBack("reload");
                }
            }
        }

        private async void LoadBandArtists(int SelectedId=0)
        {
            GenericResponse genericResponse = await BandsArtists.GetList("","","",1,0);
            List<BandArtist> bandArtists = genericResponse.Items.Cast<BandArtist>().ToList();
            txtBandaArtista.ItemsSource = bandArtists;
            txtBandaArtista.DisplayMemberPath = "Nombre";
            txtBandaArtista.SelectedValuePath = "Id";
            if (SelectedId > 0) txtBandaArtista.SelectedValue = SelectedId;
        }

        private async void LoadPlaces(int SelectedId=0)
        {
            GenericResponse genericResponse = await Places.GetList("", "", "", 1, 0);
            txtLugar.ItemsSource = genericResponse.Items;
            txtLugar.DisplayMemberPath = "Nombre";
            txtLugar.SelectedValuePath = "Id";
            if (SelectedId > 0) txtLugar.SelectedValue = SelectedId;
        }

        private void btnAddLugar_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.Navigate(typeof(PlaceAddPage));
        }

        private void btnAddBandaArtista_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.Navigate(typeof(BandArtistAddPage));
        }

        private void btnAddImage_Click(object sender, RoutedEventArgs e)
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

            if (CloureManager.getAccountType() == "free")
            {

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    images = new List<CloureImage>();
                    var inputStream = await file.OpenSequentialReadAsync();
                    var readStream = inputStream.AsStreamForRead();
                    var byteArray = new byte[readStream.Length];
                    await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                    CloureImage cloureImage = new CloureImage(file.Name, byteArray);

                    IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                    BitmapImage image = new BitmapImage();
                    image.SetSource(stream);
                    cloureImage.ImageSrc = image;

                    images.Add(cloureImage);

                    lstImages.ItemsSource = null;
                    lstImages.ItemsSource = images;
                }
            }
            else
            {
                IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
                foreach (StorageFile file in files)
                {
                    if (file != null)
                    {
                        var inputStream = await file.OpenSequentialReadAsync();
                        var readStream = inputStream.AsStreamForRead();
                        var byteArray = new byte[readStream.Length];
                        await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                        CloureImage cloureImage = new CloureImage(file.Name, byteArray);

                        IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                        BitmapImage image = new BitmapImage();
                        image.SetSource(stream);
                        cloureImage.ImageSrc = image;

                        images.Add(cloureImage);

                        //lstImages.ItemsSource = null;
                        //lstImages.ItemsSource = images;
                    }
                }
            }

            lstImages.ItemsSource = null;
            lstImages.ItemsSource = images;
        }

        private void BtnAddPhotographer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSelectPhotographer_Click(object sender, RoutedEventArgs e)
        {
            CloureParam cloureParam = new CloureParam("miembros", "1");
            CloureManager.Navigate(typeof(UsersSelectionPage), cloureParam);
        }

        private void TxtFotos_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
        {
            int fotos = 0;
            TextBox textBox = (TextBox)sender;
            int.TryParse(textBox.Text, out fotos);

            User user = (User)((FrameworkElement)e.OriginalSource).DataContext;
            user.Fotos = fotos;
        }
    }
}
