using Cloure.Core;
using Cloure.Modules.cloure_market;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.company_branches
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class CompanyBranchesPage : Page
    {
        LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;
        ModuleInfo ModuleInfo;
        public CompanyBranchesPage()
        {
            this.InitializeComponent();
            this.ModuleInfo = CloureManager.GetModuleInfo();

            LoadData();
        }

        public async void LoadData()
        {
            CompanyBranchResponse companyBranchResponse = await CompanyBranches.getList();
            List<CompanyBranch> items = companyBranchResponse.Items;
            lstItems.ItemsSource = items;
        }

        async void BuyFeature()
        {
            if (!licenseInformation.ProductLicenses["featureName"].IsActive)
            {
                try
                {
                    // The customer doesn't own this feature, so
                    // show the purchase dialog.
                    await CurrentAppSimulator.RequestProductPurchaseAsync("featureName", false);

                    //Check the license state to determine if the in-app purchase was successful.
                }
                catch (Exception)
                {
                    // The in-app purchase was not completed because
                    // an error occurred.
                }
            }
            else
            {
                // The customer already owns this feature.
            }
        }

        async void BuyFeature2()
        {
            PurchaseResults purchaseResults = await CurrentAppSimulator.RequestProductPurchaseAsync("product1");
            Guid product1TempTransactionId;

            switch (purchaseResults.Status)
            {

                case ProductPurchaseStatus.Succeeded:
                    product1TempTransactionId = purchaseResults.TransactionId;

                    // Grant the user their purchase here, and then pass the product ID and transaction ID to
                    // CurrentAppSimulator.ReportConsumableFulfillment to indicate local fulfillment to the
                    // Windows Store.
                    break;

                case ProductPurchaseStatus.NotFulfilled:
                    product1TempTransactionId = purchaseResults.TransactionId;

                    // First check for unfulfilled purchases and grant any unfulfilled purchases from an
                    // earlier transaction. Once products are fulfilled pass the product ID and transaction ID
                    // to CurrentAppSimulator.ReportConsumableFulfillment to indicate local fulfillment to the
                    // Windows Store.
                    break;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.Navigate(typeof(CompanyBranchAddPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            /*
            if (Core.Core.accountType == "free" || Core.Core.accountType == "test_free")
            {
                Run run = new Run();
                run.Text = "No puedes agregar sucursales en esta versión de Cloure. ";

                Run runVerPlanes = new Run();
                runVerPlanes.Text = "Ver planes";
                Hyperlink hyperlinkVerPlanes = new Hyperlink();
                hyperlinkVerPlanes.Inlines.Add(runVerPlanes);
                hyperlinkVerPlanes.NavigateUri = new Uri("https://cloure.com/plans/?app_token="+Core.Core.appToken);

                txtAdvice.Inlines.Add(run);
                txtAdvice.Inlines.Add(hyperlinkVerPlanes);

                btnAdd.IsEnabled = false;
            }
            */
        }

        

        private void lstItems_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            itemContextMenu.Items.Clear();

            ListView list = (ListView)sender;
            CompanyBranch product = (CompanyBranch)((FrameworkElement)e.OriginalSource).DataContext;

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
            CompanyBranch product = (CompanyBranch)menuFlyoutItem.Tag;
            if (menuFlyoutItem.Name == "edit") edit(product.Id);
            if (menuFlyoutItem.Name == "delete") DisplayDeleteDialog(product.Id);
        }

        void edit(int id)
        {
            CloureManager.Navigate(typeof(CompanyBranchAddPage), id);
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
                bool api_result = await CompanyBranches.Delete(id);
                if (api_result) LoadData();
            }
        }

        private void lstItems_ItemClick(object sender, ItemClickEventArgs e)
        {
            CompanyBranch product = (CompanyBranch)e.ClickedItem;
            edit(product.Id);
        }
    }
}
