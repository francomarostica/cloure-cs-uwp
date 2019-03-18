using Cloure.Modules.users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.products_services
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class ProductsServicesBuyStockPage : Page
    {
        int ProveedorId = 0;

        public ProductsServicesBuyStockPage()
        {
            this.InitializeComponent();
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnProductAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtProveedor_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtProducto_KeyUp(object sender, KeyRoutedEventArgs e)
        {

        }

        private void lstProveedores_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void lstProductos_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void txtProveedor_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Down)
            {
                if (lstProveedores.Visibility == Visibility.Visible)
                {
                    if (lstProveedores.SelectedIndex < lstProveedores.Items.Count - 1)
                    {
                        lstProveedores.SelectedIndex++;
                    }
                    else
                    {
                        lstProveedores.SelectedIndex = 0;
                    }
                    lstProveedores.ScrollIntoView(lstProveedores.Items[lstProveedores.SelectedIndex]);
                }
            }
            if (e.Key == Windows.System.VirtualKey.Up)
            {
                if (lstProveedores.Visibility == Visibility.Visible)
                {
                    if (lstProveedores.SelectedIndex > 0)
                    {
                        lstProveedores.SelectedIndex--;
                    }
                    else
                    {
                        lstProveedores.SelectedIndex = lstProveedores.Items.Count - 1;
                    }
                    lstProveedores.ScrollIntoView(lstProveedores.Items[lstProveedores.SelectedIndex]);
                }
            }
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (lstProveedores.Visibility == Visibility.Collapsed)
                {
                    loadProvider();
                }
                else
                {
                    //User user = (User)e.ClickedItem;
                    User user = (User)lstProveedores.Items[lstProveedores.SelectedIndex];
                    //LoadUserData(user);
                }
            }
        }

        private async void loadProvider()
        {
            UsersResponse usersResponse = await Users.getList(txtProveedor.Text, "empresa", "asc", 1, 10, "providers");

            if (usersResponse.TotalRegistros == 0)
            {
                var dialog = new MessageDialog("No se encontraron registros");
                await dialog.ShowAsync();
            }
            else if (usersResponse.TotalRegistros == 1)
            {
                User user = usersResponse.Items[0];
                txtProveedor.Text = user.id.ToString();
                tbClienteRazonSocial.Text = user.apellido + ", " + user.nombre;
                tbClienteSaldo.Text = user.saldo_str;
                ProveedorId = usersResponse.Items[0].id;
            }
            else
            {
                //var dialog = new MessageDialog("Se encontraron varios registros");
                //await dialog.ShowAsync();
                lstProveedores.Visibility = Visibility.Visible;
                lstProveedores.ItemsSource = usersResponse.Items;
            }
        }

        private void txtProveedor_KeyUp(object sender, KeyRoutedEventArgs e)
        {

        }
    }
}
