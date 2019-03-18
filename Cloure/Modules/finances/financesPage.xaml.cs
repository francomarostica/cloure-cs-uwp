using Cloure.Core;
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

namespace Cloure.Modules.finances
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class financesPage : Page
    {
        private string Filter = "";
        private string OrderBy = "date";
        private string OrderType = "desc";
        private string Since = "";
        private string Until = "";
        private string MovementType = "";
        private string CompanyBranch = "0";
        private int Page = 1;
        private int TotalPages = 1;

        ModuleInfo moduleInfo;

        public financesPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            moduleInfo = CloureManager.GetModuleInfo();

            grdFilters.Visibility = Visibility.Collapsed;
            grdFiltersContent.Children.Clear();

            /*
            foreach (GlobalCommand globalCommand in moduleInfo.globalCommands)
            {
                if (globalCommand.Name != "filters")
                {
                    Button globalCommandButton = new Button();
                    globalCommandButton.Name = globalCommand.Name;
                    globalCommandButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                    globalCommandButton.Style = (Style)Application.Current.Resources["CloureButton"];
                    globalCommandButton.Content = globalCommand.Title;
                    //Windows.UI.Xaml.Shapes.Path path = new Windows.UI.Xaml.Shapes.Path();
                    ToolBar.Children.Add(globalCommandButton);
                    globalCommandButton.Click += GlobalCommand_Click;
                }
            }
            */

            txtIngresosPrompt.Text = moduleInfo.locales.GetNamedString("incoming_prompt");
            txtGastosPrompt.Text = moduleInfo.locales.GetNamedString("outcoming_prompt");
            txtSaldoPrompt.Text = moduleInfo.locales.GetNamedString("balance_prompt");
            txtSearch.Text = moduleInfo.locales.GetNamedString("search");
            txtNoRegistersFound.Text = moduleInfo.locales.GetNamedString("no_registers_found");

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
                    button.Content = moduleInfo.locales.GetNamedString("select_date");
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if ((string)CloureManager.GetParameter() == "reload") LoadData();
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

        private void GlobalCommand_Click(object sender, RoutedEventArgs e)
        {
            Button globalCommand = (Button)sender;
            if (globalCommand.Name == "add")
            {
                CloureManager.Navigate(typeof(FinanceAdd));
            }
        }

        public async void LoadData()
        {
            FinancesResponse financesResponse = await new Finances().getList(Filter, OrderBy, OrderType, Since, Until, MovementType, CompanyBranch, Page);
            txtIngresos.Text = financesResponse.TotalIngresosStr;
            txtGastos.Text = financesResponse.TotalGastosStr;
            txtSaldo.Text = financesResponse.SaldoStr;
            TotalPages = financesResponse.TotalPages;

            if (financesResponse.financeMovements.Count > 0)
                grdNoRegisters.Visibility = Visibility.Collapsed;
            else
                grdNoRegisters.Visibility = Visibility.Visible;

            lstFinances.ItemsSource = financesResponse.financeMovements;
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

            txtRegister.Text = financesResponse.PageString;

            lstFinances.ItemsSource = financesResponse.financeMovements;
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
            if(e.Key == Windows.System.VirtualKey.Enter)
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
            Page=1;
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
            Page=TotalPages;
            LoadData();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.Navigate(typeof(FinanceAdd));
        }
    }
}
