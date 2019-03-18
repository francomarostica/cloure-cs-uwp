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

namespace Cloure.Modules.transports
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class TransportsPage : Page
    {
        ModuleInfo moduleInfo;
        private string Filter = "";
        private string OrderBy = "date";
        private string OrderType = "desc";
        private int Page = 1;
        private int TotalPages = 1;

        public TransportsPage()
        {
            this.InitializeComponent();
            moduleInfo = CloureManager.GetModuleInfo();

            txtNoResults.Text = moduleInfo.locales.GetNamedString("no_results");


            LoadData();
        }

        public async void LoadData()
        {
            grdLoader.Visibility = Visibility.Visible;
            GenericResponse genericResponse = await Transports.GetList(Filter, OrderBy, OrderType, Page);
            TotalPages = genericResponse.TotalPages;

            if (genericResponse.Items.Count > 0)
                grdNoRegisters.Visibility = Visibility.Collapsed;
            else
                grdNoRegisters.Visibility = Visibility.Visible;

            lstItems.ItemsSource = genericResponse.Items;
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

            txtRegister.Text = genericResponse.PageString;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.Navigate(typeof(TransportAddPage));
        }

        private void lstItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            Transport bank = (Transport)e.ClickedItem;
            edit(bank);
        }

        void edit(Transport bank)
        {
            //CloureManager.Navigate(typeof(BankAddPage), bank);
        }

        private void lstItems_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            itemContextMenu.Items.Clear();

            ListView list = (ListView)sender;
            Transport bank = (Transport)((FrameworkElement)e.OriginalSource).DataContext;

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
            Transport bank = (Transport)menuFlyoutItem.Tag;
            if (menuFlyoutItem.Name == "edit") edit(bank);
            if (menuFlyoutItem.Name == "delete") DisplayDeleteDialog(bank);
        }

        private async void DisplayDeleteDialog(Transport bank)
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
                bool api_result = await Transports.Delete(bank.Id);
                if (api_result) LoadData();
            }

        }

        private void btnFilters_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtSearch_KeyDown(object sender, KeyRoutedEventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


    }
}
