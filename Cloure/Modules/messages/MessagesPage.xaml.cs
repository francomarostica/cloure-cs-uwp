using Cloure.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.messages
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MessagesPage : Page
    {
        ModuleInfo moduleInfo;
        private string Filter = "";
        private string OrderBy = "date";
        private string OrderType = "desc";
        private int Page = 1;
        private int TotalPages = 1;
        private string Since = "";

        public MessagesPage()
        {
            this.InitializeComponent();
            grdFilters.Visibility = Visibility.Collapsed;

            this.moduleInfo = CloureManager.GetModuleInfo();
            txtSearch.Text = moduleInfo.locales.GetNamedString("search");
            txtNoResults.Text = moduleInfo.locales.GetNamedString("no_results");

            foreach (ModuleFilter moduleFilters in moduleInfo.moduleFilters)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = moduleFilters.Title;
                textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                textBlock.VerticalAlignment = VerticalAlignment.Top;
                textBlock.Margin = new Thickness(5, 5, 5, 0);
                textBlock.Height = 20;
                textBlock.Foreground = new SolidColorBrush(Colors.White);
                textBlock.FontSize = 14;
                grdFiltersContent.Children.Add(textBlock);

                if (moduleFilters.Type == "combo")
                {
                    //showDialog(moduleFilters.Default);
                    ComboBox comboFilter = new ComboBox();
                    comboFilter.Name = moduleFilters.Name;
                    comboFilter.HorizontalAlignment = HorizontalAlignment.Stretch;
                    comboFilter.Margin = new Thickness(5, 0, 5, 10);
                    comboFilter.ItemsSource = moduleFilters.FilterItems;
                    comboFilter.SelectedValuePath = "Id";
                    comboFilter.DisplayMemberPath = "Title";
                    //comboBox.SelectedIndex = 0;
                    comboFilter.SelectionChanged += ComboFilter_SelectionChanged;
                    grdFiltersContent.Children.Add(comboFilter);
                    comboFilter.SelectedValue = moduleFilters.Default;
                }
                if (moduleFilters.Type == "date")
                {
                    Button button = new Button();
                    DatePickerFlyout pickerFlyout = new DatePickerFlyout();
                    button.Flyout = pickerFlyout;
                    button.Content = "Seleccione una fecha";
                    button.Name = moduleFilters.Name;
                    button.Foreground = new SolidColorBrush(Colors.White);
                    button.HorizontalAlignment = HorizontalAlignment.Stretch;
                    button.Margin = new Thickness(5, 0, 5, 10);
                    pickerFlyout.DatePicked += PickerFlyout_DatePicked;
                    pickerFlyout.Closed += PickerFlyout_Closed;
                    grdFiltersContent.Children.Add(button);
                }
            }

            Button applybutton = new Button();
            applybutton.Content = moduleInfo.locales.GetNamedString("apply");
            applybutton.HorizontalAlignment = HorizontalAlignment.Center;
            applybutton.Style = (Style)Application.Current.Resources["CloureFilterButton"];
            applybutton.Click += Applybutton_Click;
            grdFiltersContent.Children.Add(applybutton);

            LoadData();
        }

        private void PickerFlyout_Closed(object sender, object e)
        {
            
        }

        private void PickerFlyout_DatePicked(DatePickerFlyout sender, DatePickedEventArgs args)
        {
            Button button = (Button)sender.Target;
            if (button.Name == "since")
            {
                Since = sender.Date.ToString("yyyy-MM-dd");
            }
            button.Content = sender.Date.ToString("dd/MM/yyyy");
        }

        private void Applybutton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            if (comboBox.Name == "order_by") OrderBy = (string)comboBox.SelectedValue;
            if (comboBox.Name == "order_type") OrderType = (string)comboBox.SelectedValue;
        }

        public async void LoadData()
        {
            grdLoader.Visibility = Visibility.Visible;
            GenericResponse genericResponse = await Messages.GetList(Filter, OrderBy, OrderType, Page);
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

        private void lstItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            Message message = (Message)e.ClickedItem;
            edit(message.Id);
        }

        void edit(int message_id)
        {
            CloureManager.Navigate(typeof(MessageDetailPage), message_id);
        }

        private void lstItems_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            itemContextMenu.Items.Clear();

            ListView list = (ListView)sender;
            Message bank = (Message)((FrameworkElement)e.OriginalSource).DataContext;

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
            Message message = (Message)menuFlyoutItem.Tag;
            if (menuFlyoutItem.Name == "edit") edit(message.Id);
            if (menuFlyoutItem.Name == "delete") DisplayDeleteDialog(message);
        }

        private async void DisplayDeleteDialog(Message message)
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
                bool api_result = await Messages.Delete(message.Id);
                if (api_result) LoadData();
            }

        }

        private void btnFilters_Click(object sender, RoutedEventArgs e)
        {
            if (grdFilters.Visibility == Visibility.Collapsed)
                grdFilters.Visibility = Visibility.Visible;
            else
                grdFilters.Visibility = Visibility.Collapsed;
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
