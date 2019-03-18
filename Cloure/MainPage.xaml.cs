using Cloure.Core;
using Cloure.Modules.cloure_market;
using Cloure.Modules.users;
using Microsoft.Advertising.WinRT.UI;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace Cloure
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        InterstitialAd myInterstitialAd = null;
        JsonObject locales = null;
        JsonObject ClourePremium = null;
        string accountType = CloureManager.getAccountType();

        // Assign this variable to the Store ID of your subscription add-on.
        public string ActiveModule = "";

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            grdLoader.Visibility = Visibility.Collapsed;
            grdPayment.Visibility = Visibility.Collapsed;
            LoadLocales();

            CloureManager.SetLoaderUI(grdLoader);
            CloureManager.SetFrame(contentFrame);
            CloureManager.SetTitleControl(tbModuleTitle);

            txtCloureAccountType.Text = "Cloure " + accountType;

            txtUser.Text = CloureManager.cloureClient.Name + " " + CloureManager.cloureClient.LastName;
            txtUserGroup.Text = CloureManager.cloureClient.Group;
            txtUserMail.Text = CloureManager.cloureClient.Email;

            if (CloureManager.account_data_saved)
            {
                txtAccountAddOn.Content = "Olvidar sesión";
            }
            else
            {
                txtAccountAddOn.Visibility = Visibility.Collapsed;
            }

            await CloureManager.LoadAccountInfo();

            if (accountType != "free" && accountType != "test_free")
            {
                //Difference between days to expire subscription and advice days
                if (CloureManager.getVencimientoDias() <= CloureManager.getPaymentAdviceDays()) {
                    //LoadCloureProducts(accountType);
                }
                btnSubscribePremium.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnSubscribePremium.Visibility = Visibility.Visible;
            }

            if (CloureManager.getCompanyType() == "generic" || CloureManager.getCompanyType() == "printing_houses")
            {
                LoadModuleInfo("invoicing");
            }

            //Load Menu
            foreach (JsonValue jsonValue in CloureManager.cloureClient.modulesGroupsArr)
            {
                JsonObject groupItem = jsonValue.GetObject();
                string group_id = groupItem.GetNamedString("Id");
                string group_title = groupItem.GetNamedString("Title");
                string group_icon = groupItem.GetNamedString("Icon");

                JsonArray menuItems = groupItem.GetNamedArray("Items");

                CloureMenuGroup group_model = new CloureMenuGroup(group_id, group_title, group_icon);

                if (menuItems.Count > 1)
                {
                    for (uint j = 0; j < menuItems.Count; j++)
                    {
                        JsonObject menuItem = menuItems.GetObjectAt(j);

                        string menu_id = menuItem.GetNamedString("Id");
                        string menu_title = menuItem.GetNamedString("Title");
                        string menu_group_id = menuItem.GetNamedString("GroupId");

                        if (menu_group_id == group_id)
                        {
                            CloureMenuItem mnu = new CloureMenuItem(menu_id, menu_title, menu_group_id);
                            mnu.Click += Mnu_Click;
                            group_model.cloureMenuItems.Add(mnu);
                        }
                    }
                }
                else
                {
                    JsonObject menuItem = menuItems.GetObjectAt(0);
                    string menu_id = menuItem.GetNamedString("Id");
                    string menu_title = menuItem.GetNamedString("Title");
                    group_model.Name = menu_id;
                    group_model.Title = menu_title;
                }
                group_model.Style = (Style)Application.Current.Resources["CloureMenuGroup"];
                group_model.isExpanded = false;
                group_model.Click += Group_model_Click;
                spMenuItems.Children.Add(group_model);
            }

            //Ad settings
            if (accountType == "test_free")
            {
                BannerAd.ApplicationId = "3f83fe91-d6be-434d-a0ae-7351c5a997f1";
                BannerAd.AdUnitId = "test";
                BannerAd.IsAutoRefreshEnabled = true;

                string myAppId = "d25517cb-12d4-4699-8bdc-52040c712cab";
                string myAdUnitId = "test";

                myInterstitialAd = new InterstitialAd();
                myInterstitialAd.RequestAd(AdType.Video, myAppId, myAdUnitId);
                myInterstitialAd.AdReady += MyInterstitialAd_AdReady;
                myInterstitialAd.ErrorOccurred += MyInterstitialAd_ErrorOccurred;
                myInterstitialAd.Completed += MyInterstitialAd_Completed;
                myInterstitialAd.Cancelled += MyInterstitialAd_Cancelled;

                setUIAds(false);
            }
            else if (accountType == "free")
            {
                string AppId = "9phmgghfsgxp";
                string BannerUnitId = "1100035377";
                string VideoUnitId = "1100035195";

                BannerAd.ApplicationId = AppId;
                BannerAd.AdUnitId = BannerUnitId;
                BannerAd.IsAutoRefreshEnabled = true;

                myInterstitialAd = new InterstitialAd();
                myInterstitialAd.RequestAd(AdType.Video, AppId, VideoUnitId);
                myInterstitialAd.AdReady += MyInterstitialAd_AdReady;
                myInterstitialAd.ErrorOccurred += MyInterstitialAd_ErrorOccurred;
                myInterstitialAd.Completed += MyInterstitialAd_Completed;
                myInterstitialAd.Cancelled += MyInterstitialAd_Cancelled;

                setUIAds(false);
            }
            else
            {
                setUIAds(true);
            }

            //CloureManager.sendToast("Titulo", "Contenido");
            txtAccountAddOn.Click += TxtAccountAddOn_Click;
        }

        private async void LoadLocales()
        {
            locales = await AvailableLanguages.getLocales("", CloureManager.lang);
            btnSubscribePremium.Content = locales.GetNamedString("subscribe_premium");
            txtAccountAddOn.Content = locales.GetNamedString("forget_session");
        }

        private void TxtAccountAddOn_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Windows.Storage.ApplicationDataCompositeValue composite = (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["user_data"];
            localSettings.Values["user_data"] = null;

            txtAccountAddOn.Content = "";
            CloureManager.ShowDialog("Sus datos de accesos han sido olvidados.");
        }

        private async void LoadCloureProducts(string pid)
        {
            ClourePremium = await CloureProducts.GetClourePlans(pid);
            txtVencimientoAviso.Text = locales.GetNamedString("subscribe_cloure_today");

            var products = ClourePremium.GetNamedArray("Registros");
            gridPlans.Items.Clear();

            foreach (var product in products)
            {
                JsonObject productObj = product.GetObject();
                var features = productObj.GetNamedArray("features");
                var prices = productObj.GetNamedArray("prices");

                if (prices.GetArray().Count > 0)
                {
                    StackPanel stackProductItem = new StackPanel();
                    stackProductItem.Padding = new Thickness(20);

                    TextBlock textBlockTitle = new TextBlock();
                    textBlockTitle.Text = productObj.GetNamedString("title");
                    textBlockTitle.TextAlignment = TextAlignment.Center;
                    textBlockTitle.Foreground = new SolidColorBrush(Colors.White);
                    textBlockTitle.FontSize = 18;
                    textBlockTitle.Margin = new Thickness(0, 0, 0, 20);
                    textBlockTitle.FontWeight = FontWeights.Bold;

                    stackProductItem.Children.Add(textBlockTitle);

                    foreach (var feature in features)
                    {
                        JsonObject featureObj = feature.GetObject();

                        TextBlock textBlockFeature = new TextBlock();
                        textBlockFeature.FontSize = 14;
                        textBlockFeature.Foreground = new SolidColorBrush(Colors.White);
                        textBlockFeature.Text = featureObj.GetNamedString("title");
                        textBlockFeature.TextAlignment = TextAlignment.Center;
                        stackProductItem.Children.Add(textBlockFeature);
                    }

                    StackPanel stackPanelPricesContainer = new StackPanel();
                    stackPanelPricesContainer.Orientation = Orientation.Horizontal;
                    stackPanelPricesContainer.Margin = new Thickness(0, 20, 0, 0);

                    foreach (var price in prices)
                    {
                        JsonObject priceObj = price.GetObject();

                        StackPanel stackPanelPrice = new StackPanel();
                        stackPanelPrice.Padding = new Thickness(20);

                        TextBlock textBlockPriceStr = new TextBlock();
                        textBlockPriceStr.Text = priceObj.GetNamedString("price_str");
                        textBlockPriceStr.TextAlignment = TextAlignment.Center;
                        textBlockPriceStr.Foreground = new SolidColorBrush(Colors.White);
                        textBlockPriceStr.FontSize = 18;

                        TextBlock textBlockBillingTypeStr = new TextBlock();
                        textBlockBillingTypeStr.Text = priceObj.GetNamedString("billing_type_str");
                        textBlockBillingTypeStr.TextAlignment = TextAlignment.Center;
                        textBlockBillingTypeStr.Foreground = new SolidColorBrush(Colors.White);

                        stackPanelPrice.Children.Add(textBlockPriceStr);
                        stackPanelPrice.Children.Add(textBlockBillingTypeStr);

                        Button btnSubscribe = new Button();
                        btnSubscribe.Margin = new Thickness(0, 30, 0, 0);
                        btnSubscribe.FontSize = 22;
                        btnSubscribe.Style = (Style)Application.Current.Resources["CloureGreenButton"];
                        btnSubscribe.Tag = priceObj;
                        btnSubscribe.Content = locales.GetNamedString("subscribe");

                        btnSubscribe.Click += BtnSubscribe_Click;

                        stackPanelPrice.Children.Add(btnSubscribe);
                        
                        stackPanelPricesContainer.Children.Add(stackPanelPrice);
                    }

                    stackProductItem.Children.Add(stackPanelPricesContainer);

                    gridPlans.Items.Add(stackProductItem);
                }
            }

            grdPayment.Visibility = Visibility.Visible;
        }

        private async void BtnSubscribe_Click(object sender, RoutedEventArgs e)
        {
            JsonObject priceObject = (JsonObject)((Button)sender).Tag;
            bool res = await CloureManager.RequestPurchase(priceObject);
            if (res)
            {
                btnSubscribePremium.Visibility = Visibility.Collapsed;
                grdPayment.Visibility = Visibility.Collapsed;
                CloureManager.ShowDialog("Tu suscripción ha sido realizada!");
            }
        }

        void setUIAds(bool disable)
        {
            if (!disable)
            {
                BannerAd.HorizontalAlignment = HorizontalAlignment.Stretch;
                BannerAd.Height = 120;
                contentFrame.Margin = new Thickness(350, 50, 0, 120);
            }
            else
            {
                BannerAd.Visibility = Visibility.Collapsed;
                contentFrame.Margin = new Thickness(350, 50, 0, 0);
            }
        }

        void MyInterstitialAd_AdReady(object sender, object e)
        {
            if (InterstitialAdState.Ready == myInterstitialAd.State)
            {
                myInterstitialAd.Show();
            }
        }

        void MyInterstitialAd_ErrorOccurred(object sender, AdErrorEventArgs e)
        {
            // Your code goes here.
        }

        void MyInterstitialAd_Completed(object sender, object e)
        {
            // Your code goes here.
        }

        void MyInterstitialAd_Cancelled(object sender, object e)
        {
            // Your code goes here.
        }

        

        private void Mnu_Click(object sender, RoutedEventArgs e)
        {
            CloureMenuItem cloureMenuItem = (CloureMenuItem)sender;
            tbModuleTitle.Text = cloureMenuItem.Titulo;
            //contentFrame.Navigate(typeof(PageContent), cloureMenuItem.Nombre);
            string mod_name = cloureMenuItem.ModuleName;
            LoadModuleInfo(mod_name);
        }

        private void Group_model_Click(object sender, RoutedEventArgs e)
        {
            CloureMenuGroup cloureMenuGroup = (CloureMenuGroup)sender;
            
            if (cloureMenuGroup.cloureMenuItems.Count == 0)
            {
                tbModuleTitle.Text = cloureMenuGroup.Title;
                //contentFrame.Navigate(typeof(PageContent), cloureMenuGroup.Name);
                LoadModuleInfo(cloureMenuGroup.Name);
            }
            else
            {
                if (cloureMenuGroup.isExpanded)
                    cloureMenuGroup.isExpanded = false;
                else
                    cloureMenuGroup.isExpanded = true;
            }
        }

        private async void LoadModuleInfo(string ModuleName)
        {
            string namespace_name = "Cloure.Modules." + ModuleName;
            string class_name = "mod_" + ModuleName;
            ActiveModule = ModuleName;

            ModuleInfo moduleInfo = new ModuleInfo();

            try
            {
                var module_obj = Activator.CreateInstance(Type.GetType(namespace_name + "." + class_name));

                if (module_obj is CloureModule)
                {
                    CloureModule module = (CloureModule)module_obj;

                    List<CloureParam> cloureParams = new List<CloureParam>();
                    cloureParams.Add(new CloureParam("topic", "get_module_info"));
                    cloureParams.Add(new CloureParam("module", ModuleName));

                    string api_response = await CloureManager.ExecuteAsync(cloureParams);
                    JsonObject ApiResponse = JsonObject.Parse(api_response);
                    moduleInfo.Title = ApiResponse.GetNamedString("title");
                    JsonArray globalCommandsArr = ApiResponse.GetNamedArray("global_commands");
                    JsonArray filtersArr = ApiResponse.GetNamedArray("filters");

                    foreach (JsonValue value in globalCommandsArr)
                    {
                        JsonObject jobj = value.GetObject();
                        int cmd_id = (int)jobj.GetNamedNumber("Id");
                        string cmd_name = jobj.GetNamedString("Name");
                        string cmd_title = jobj.GetNamedString("Title");
                        GlobalCommand globalCommand = new GlobalCommand(cmd_id, cmd_name, cmd_title);
                        moduleInfo.globalCommands.Add(globalCommand);
                    }

                    JsonValue localesValue = ApiResponse.GetNamedValue("locales");
                    if(localesValue.ValueType != JsonValueType.Null)
                    {
                        moduleInfo.locales = ApiResponse.GetNamedObject("locales");
                    }

                    if (filtersArr.Count > 0)
                    {
                        foreach (JsonValue value in filtersArr)
                        {
                            JsonObject jobj = value.GetObject();
                            string filter_name = jobj.GetNamedString("Name");
                            string filter_title = jobj.GetNamedString("Title");
                            string filter_type = jobj.GetNamedString("Type");
                            string filter_default = jobj.GetNamedString("Default");

                            ModuleFilter moduleFilter = new ModuleFilter(filter_name, filter_title, filter_type, filter_default);

                            JsonArray filterItems = jobj.GetNamedArray("Items");
                            foreach (JsonValue item in filterItems)
                            {
                                JsonObject item_obj = item.GetObject();
                                JsonValue item_id = item_obj.GetNamedValue("Id");
                                string item_title = item_obj.GetNamedString("Title");
                                string item_id_str = "";
                                if (item_id.ValueType.ToString() == "String") item_id_str = item_id.GetString();
                                if (item_id.ValueType.ToString() == "Number") item_id_str = item_id.GetNumber().ToString();

                                string item_title_str = item_title.ToString();

                                ModuleFilterItem moduleFilterItem = new ModuleFilterItem(item_id_str, item_title_str);
                                moduleFilter.AddItem(moduleFilterItem);
                            }

                            moduleInfo.moduleFilters.Add(moduleFilter);
                        }
                    }

                    //Core.Core.ModuleInfo = moduleInfo;
                    CloureManager.SetModuleInfo(moduleInfo);
                    module.OnModuleCreated();
                }
                else
                {
                    var dialog = new MessageDialog("Module " + ModuleName + " doesn't implement CloureModule interface");
                    await dialog.ShowAsync();
                }
            }
            catch (Exception e)
            {
                var dialog = new MessageDialog(e.Message);
                await dialog.ShowAsync();
            }
        }

        private void btnClosePaymentAdvice_Click(object sender, RoutedEventArgs e)
        {
            ClosePaymentAdvice();
        }

        private void ClosePaymentAdvice()
        {
            grdPayment.Visibility = Visibility.Collapsed;
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Escape && grdPayment.Visibility == Visibility.Visible)
            {
                ClosePaymentAdvice();
            }
        }

        private void btnSubscribePremium_Click(object sender, RoutedEventArgs e)
        {
            LoadCloureProducts("");
        }

        private async void btnConfirmSubscribe_Click(object sender, RoutedEventArgs e)
        {
            /*
            var uri = new Uri(@"https://cloure.com/"+CloureManager.lang+"-"+CloureManager.country+"/payment?app_token="+CloureManager.appToken+"&pid=starter");
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            */
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CloureManager.cloureClient.OnBroadcastMessageReceived += CloureClient_OnBroadcastMessageReceived;
        }

        private void CloureClient_OnBroadcastMessageReceived(string message)
        {
            ToastContent content = new ToastContent()
            {
                Launch = "app-defined-string",
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = message,
                                HintMaxLines = 1
                            }
                        }
                    }
                },
            };

            var toast = new ToastNotification(content.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
