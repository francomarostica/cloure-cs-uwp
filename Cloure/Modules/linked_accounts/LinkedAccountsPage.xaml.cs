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

namespace Cloure.Modules.linked_accounts
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class LinkedAccountsPage : Page
    {
        public LinkedAccountsPage()
        {
            this.InitializeComponent();
            LoadData();
        }

        private async void LoadData()
        {
            GenericResponse genericResponse = await LinkedAccounts.GetList();

            if (genericResponse.Items.Count > 0)
                grdNoRegisters.Visibility = Visibility.Collapsed;
            else
                grdNoRegisters.Visibility = Visibility.Visible;

            grdItems.ItemsSource = genericResponse.Items;
        }

        private void GrdItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            LinkedAccount linkedAccount = (LinkedAccount)e.ClickedItem;
            edit(linkedAccount.Name);
        }

        void edit(string name)
        {
            CloureManager.Navigate(typeof(LinkedAccountLink), name);
        }
    }
}
