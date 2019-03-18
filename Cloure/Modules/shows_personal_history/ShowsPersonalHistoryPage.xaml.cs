using Cloure.Core;
using Cloure.Modules.shows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.shows_personal_history
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class ShowsPersonalHistoryPage : Page
    {
        ModuleInfo moduleInfo;

        private string Filter = "";
        private string OrderBy = "date";
        private string OrderType = "desc";
        private string Since = "";
        private string Until = "";
        private string MovementType = "";
        private string CompanyBranch = "0";
        private int Page = 1;
        private int TotalPages = 1;

        public ShowsPersonalHistoryPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            moduleInfo = CloureManager.GetModuleInfo();

            grdFilters.Visibility = Visibility.Collapsed;
            grdFiltersContent.Children.Clear();

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
            applybutton.Content = "Aplicar";
            applybutton.HorizontalAlignment = HorizontalAlignment.Center;
            applybutton.Style = (Style)Application.Current.Resources["CloureSecondaryButton"];
            applybutton.Click += Applybutton_Click;
            grdFiltersContent.Children.Add(applybutton);

            LoadData();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (CloureManager.GetParameter() == "reload") LoadData();
        }

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            if (comboBox.Name == "order_by") OrderBy = (string)comboBox.SelectedValue;
            if (comboBox.Name == "order_type") OrderType = (string)comboBox.SelectedValue;
            if (comboBox.Name == "movement_type") MovementType = (string)comboBox.SelectedValue;
        }

        private void Applybutton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void PickerFlyout_Closed(object sender, object e)
        {
            //DatePickerFlyout pickerFlyout = (DatePickerFlyout)sender;
            //Button button = (Button)pickerFlyout.Target;
            //button.Content = "Seleccione una fecha";
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

        public async void LoadData()
        {
            ShowPersonalHistoryResponse historyResponse = await ShowsPersonalHistory.GetList(Filter, OrderBy, OrderType, Since, Until, MovementType, CompanyBranch, Page);
            txtIngresos.Text = historyResponse.TotalEventos.ToString();
            txtGastos.Text = historyResponse.TotalFotos.ToString();
            TotalPages = historyResponse.TotalPages;

            if (historyResponse.Items.Count > 0)
                grdNoRegisters.Visibility = Visibility.Collapsed;
            else
                grdNoRegisters.Visibility = Visibility.Visible;

            lstFinances.ItemsSource = historyResponse.Items;
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

            txtRegister.Text = "Mostrando página " + Page.ToString() + " de " + TotalPages.ToString();

            lstFinances.ItemsSource = historyResponse.Items;
        }

        private async void showDialog(string value)
        {
            var dialog = new MessageDialog(value);
            await dialog.ShowAsync();
        }

        private void btnFilters_Click(object sender, RoutedEventArgs e)
        {
            if (grdFilters.Visibility == Visibility.Collapsed)
                grdFilters.Visibility = Visibility.Visible;
            else
                grdFilters.Visibility = Visibility.Collapsed;
        }

        private void btnApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
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
        
    }
}
