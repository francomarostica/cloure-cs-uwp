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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.shows
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class ShowsAddMultiplePage : Page
    {
        ObservableCollection<Show> shows = new ObservableCollection<Show>();
        public List<Place> places = new List<Place>();
        private Show selectedShow;

        public ShowsAddMultiplePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            lstShows.ItemsSource = shows;
            LoadBandArtists();
            LoadPlaces();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //Verify GoBack Method
            var cParameter = CloureManager.GetParameter();
            if (cParameter != null)
            {
                if (cParameter.GetType() == typeof(CloureParam))
                {
                    CloureParam cloureParam = (CloureParam)cParameter;
                    if (cloureParam.name == "place_id") LoadPlaces((int)cloureParam.value);
                    if (cloureParam.name == "artist_id") LoadBandArtists((int)cloureParam.value);
                }
                else if (cParameter.GetType() == typeof(List<User>))
                {
                    List<User> users_tmp = (List<User>)cParameter;
                    selectedShow.Fotografos.Clear();
                    foreach (User item in users_tmp)
                    {
                        selectedShow.Fotografos.Add(item);
                    }
                }
            }
        }

        private async void LoadBandArtists(int SelectedId = 0)
        {
            GenericResponse genericResponse = await BandsArtists.GetList("", "", "", 1, 0);
            List<BandArtist> bandArtists = genericResponse.Items.Cast<BandArtist>().ToList();
            txtBandaArtista.ItemsSource = bandArtists;
            txtBandaArtista.DisplayMemberPath = "Nombre";
            txtBandaArtista.SelectedValuePath = "Id";
            if (SelectedId > 0) txtBandaArtista.SelectedValue = SelectedId;
        }

        private async void LoadPlaces(int SelectedId = 0)
        {
            GenericResponse genericResponse = await Places.GetList("", "", "", 1, 0);
            places = genericResponse.Items.Cast<Place>().ToList();
        }

        private void BtnAddEvent_Click(object sender, RoutedEventArgs e)
        {
            Show show = new Show();
            show.Fecha = DateTime.Now;
            show.Places = places;
            show.Fotografos = new ObservableCollection<User>();
            shows.Add(show);
        }

        private void BtnAddBandaArtista_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.ShowLoader("Guardando");
            foreach (var item in shows)
            {
                if (txtBandaArtista.SelectedValue != null) item.ArtistaId = (int)txtBandaArtista.SelectedValue;
                bool res = await Shows.save(item);
                //CloureManager.ShowDialog("Fecha: "+item.Fecha.Value.ToString()+" Lugar: "+item.LugarId.ToString());
            }
            CloureManager.HideLoader();
            CloureManager.GoBack();
        }

        private void BtnDeleteShow_Click(object sender, RoutedEventArgs e)
        {
            Show show = (Show)((FrameworkElement)e.OriginalSource).DataContext;
            shows.Remove(show);
        }

        private void BtnAddPhotographer_Click(object sender, RoutedEventArgs e)
        {
            selectedShow = (Show)((FrameworkElement)e.OriginalSource).DataContext;
            CloureParam cloureParam = new CloureParam("miembros", "1");
            CloureManager.Navigate(typeof(UsersSelectionPage), cloureParam);
        }
    }
}
