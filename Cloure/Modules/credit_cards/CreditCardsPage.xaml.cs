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

namespace Cloure.Modules.credit_cards
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class CreditCardsPage : Page
    {
        ModuleInfo moduleInfo;

        public CreditCardsPage()
        {
            this.InitializeComponent();
            this.moduleInfo = CloureManager.GetModuleInfo();

            LoadData();
        }

        public async void LoadData()
        {
            List<CreditCard> receipts = await CreditCards.getList();
            lstItems.ItemsSource = receipts;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.Navigate(typeof(CreditCardAddPage));
        }

        private void lstItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            CreditCard bank = (CreditCard)e.ClickedItem;
            edit(bank);
        }

        void edit(CreditCard bank)
        {
            CloureManager.Navigate(typeof(CreditCardAddPage), bank);
        }

        private void lstItems_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            itemContextMenu.Items.Clear();

            ListView list = (ListView)sender;
            CreditCard bank = (CreditCard)((FrameworkElement)e.OriginalSource).DataContext;

            if (bank.availableCommands.Count > 0)
            {
                foreach (AvailableCommand availableCommand in bank.availableCommands)
                {
                    MenuFlyoutItem menuFlyoutItem = new MenuFlyoutItem();
                    menuFlyoutItem.Text = availableCommand.title;
                    menuFlyoutItem.Name = availableCommand.name;
                    menuFlyoutItem.Tag = bank;
                    menuFlyoutItem.Click += MenuFlyoutItem_Click;
                    itemContextMenu.Items.Add(menuFlyoutItem);
                }
            }

            if (itemContextMenu.Items.Count > 0)
            {
                itemContextMenu.ShowAt(listView, e.GetPosition(listView));
                var a = ((FrameworkElement)e.OriginalSource).DataContext;
            }

        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = (MenuFlyoutItem)sender;
            CreditCard bank = (CreditCard)menuFlyoutItem.Tag;
            if (menuFlyoutItem.Name == "edit") edit(bank);
            if (menuFlyoutItem.Name == "delete") DisplayDeleteDialog(bank);
        }

        private async void DisplayDeleteDialog(CreditCard bank)
        {
            ContentDialog deleteFileDialog = new ContentDialog
            {
                Title = "¿Está seguro que desea eliminar este registro?",
                Content = "El registro se borrará de forma permanente",
                PrimaryButtonText = "Borrar",
                CloseButtonText = "Cancelar"
            };

            ContentDialogResult result = await deleteFileDialog.ShowAsync();

            // Delete the file if the user clicked the primary button.
            /// Otherwise, do nothing.
            if (result == ContentDialogResult.Primary)
            {
                bool api_result = await CreditCards.Delete(bank.Id);
                if (api_result) LoadData();
            }

        }
    }
}
