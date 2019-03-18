using Cloure.Core;
using System;
using System.Collections.Generic;
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

namespace Cloure.Modules.users
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class UsersSelectionPage : Page
    {
        private int Page = 1;
        private int TotalPages = 1;

        private List<User> users = new List<User>();

        public UsersSelectionPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                if (e.Parameter.GetType() == typeof(CloureParam))
                {
                    CloureParam cloureParam = (CloureParam)e.Parameter;
                    if (cloureParam.name == "miembros")
                    {
                        string miembros = (string)cloureParam.value;
                        LoadData("", "apellido", "asc", 1, 50, "all");
                    }
                }
            }
        }

        private async void LoadData(string Filtro = "", string OrdenarPor = "apellido", string Orden = "asc", int Pagina = 1, int Limite = 50, string Miembros="all")
        {
            grdLoader.Visibility = Visibility.Visible;
            UsersResponse response = await Users.getList(Filtro, OrdenarPor, Orden, Pagina, Limite);
            users = response.Items;

            TotalPages = response.TotalPaginas;
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

            lstItems.ItemsSource = null;
            lstItems.ItemsSource = response.Items;

            //CloureManager.ShowDialog(response.Items.Count.ToString());

            txtRegister.Text = "Mostrando página " + Page.ToString() + " de " + TotalPages.ToString();
        }

        private void txtSearch_KeyDown(object sender, KeyRoutedEventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnFilters_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            Page = 1;
            LoadData();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            Page--;
            LoadData();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            Page++;
            LoadData();
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            Page = TotalPages;
            LoadData();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            List<User> selectedUsers = new List<User>();

            foreach (User user in users)
            {
                if (user.Selected)
                {
                    selectedUsers.Add(user);
                }
            }

            CloureManager.GoBack(selectedUsers);

            //CloureManager.ShowDialog(selectedUsers.Count.ToString());
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            User user = (User)((FrameworkElement)e.OriginalSource).DataContext;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.IsChecked.HasValue)
                user.Selected = checkBox.IsChecked.Value;
            else
                user.Selected = false;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            User user = (User)((FrameworkElement)e.OriginalSource).DataContext;
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox.IsChecked.HasValue)
                user.Selected = checkBox.IsChecked.Value;
            else
                user.Selected = false;
        }
    }
}
