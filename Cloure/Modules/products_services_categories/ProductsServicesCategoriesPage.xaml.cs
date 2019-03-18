﻿using Cloure.Core;
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

namespace Cloure.Modules.products_services_categories
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class ProductsServicesCategoriesPage : Page
    {
        ModuleInfo moduleInfo;
        private string Filter = "";
        private string OrderBy = "date";
        private string OrderType = "desc";
        private int Page = 1;
        private int TotalPages = 1;

        public ProductsServicesCategoriesPage()
        {
            this.InitializeComponent();

            this.moduleInfo = CloureManager.GetModuleInfo();
            txtSearch.PlaceholderText = moduleInfo.locales.GetNamedString("search");
            txtNoResults.Text = moduleInfo.locales.GetNamedString("no_results");

            LoadCategoriesTypes();
        }

        private void Applybutton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private async void LoadCategoriesTypes()
        {
            List<ProductCategoryType> productCategoryTypes = await ProductsServicesCategories.GetTypes();
            txtCategoriaNivel.ItemsSource = productCategoryTypes;
            txtCategoriaNivel.SelectedValuePath = "Id";
            txtCategoriaNivel.DisplayMemberPath = "Nombre";
            txtCategoriaNivel.SelectedIndex = 0;
        }

        public async void LoadData()
        {
            grdLoader.Visibility = Visibility.Visible;
            GenericResponse genericResponse = await ProductsServicesCategories.GetList((int)txtCategoriaNivel.SelectedValue, Filter, OrderBy, OrderType, Page);
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

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            if (comboBox.Name == "order_by") OrderBy = (string)comboBox.SelectedValue;
            if (comboBox.Name == "order_type") OrderType = (string)comboBox.SelectedValue;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.Navigate(typeof(ProductServiceCategoryAddPage));
        }

        private void btnFilters_Click(object sender, RoutedEventArgs e)
        {
            if (grdFilters.Visibility == Visibility.Collapsed)
                grdFilters.Visibility = Visibility.Visible;
            else
                grdFilters.Visibility = Visibility.Collapsed;
        }

        private void txtSearch_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Filter = txtSearch.Text;
                LoadData();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSearch.Text == "")
            {
                Filter = "";
                LoadData();
            }
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

        private void lstItems_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            itemContextMenu.Items.Clear();

            ListView list = (ListView)sender;
            ProductServiceCategory product = (ProductServiceCategory)((FrameworkElement)e.OriginalSource).DataContext;

            if (product.AvailableCommands.Count > 0)
            {
                foreach (AvailableCommand availableCommand in product.AvailableCommands)
                {
                    MenuFlyoutItem menuFlyoutItem = new MenuFlyoutItem();
                    menuFlyoutItem.Text = availableCommand.title;
                    menuFlyoutItem.Name = availableCommand.name;
                    menuFlyoutItem.Tag = product;
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
            ProductServiceCategory product = (ProductServiceCategory)menuFlyoutItem.Tag;
            if (menuFlyoutItem.Name == "edit") edit(product.Id);
            if (menuFlyoutItem.Name == "delete") DisplayDeleteDialog(product.Id);
        }

        void edit(int id)
        {
            CloureManager.Navigate(typeof(ProductServiceCategoryAddPage), id);
        }

        private async void DisplayDeleteDialog(int id)
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
                bool api_result = await ProductsServicesCategories.Delete(id);
                if (api_result) LoadData();
            }
        }

        private void lstItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            ProductServiceCategory product = (ProductServiceCategory)e.ClickedItem;
            edit(product.Id);
        }

        private void txtCategoriaNivel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }
    }
}
