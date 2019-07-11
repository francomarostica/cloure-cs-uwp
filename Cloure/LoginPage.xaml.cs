using Cloure.Core;
using Cloure.Modules.users;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public LoginPage()
        {
            this.InitializeComponent();

            //Core.Core.cloureManager = new CloureManager();

            txtVersion.Text = CloureManager.GetAppVersionString();
            LoadLanguages();
        }

        private async void LoadLanguages()
        {
            string lang = Windows.Globalization.Language.CurrentInputMethodLanguageTag;
            lang = lang.Substring(0, 2);

            List<AvailableLanguage> availableLanguages = await AvailableLanguages.getList();
            txtLanguage.ItemsSource = availableLanguages;
            txtLanguage.DisplayMemberPath = "Name";
            txtLanguage.SelectedValuePath = "Id";

            txtLanguage.SelectedValue = lang;

            Windows.Storage.ApplicationDataCompositeValue composite = (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["user_data"];
            if (composite != null)
            {
                //CloureManager.ShowDialog("Hay valor!");
                txtUser.Text = (string)composite["user"];
                txtPass.Password = (string)composite["pass"];
                chKeepConnected.IsChecked = true;
                CloureManager.account_data_saved = true;
                //attemptLoginV2();
                attemptLogin();
            }
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            attemptLogin();
            //attemptLoginV2();
        }
        public async void attemptLogin()
        {
            loginProgress.IsActive = true;
            btnLogin.IsEnabled = false;
            btnRegister.IsEnabled = false;

            string user = txtUser.Text;
            string pass = txtPass.Password;

            //bool loginResult = await CloureManager.login(user, pass, chKeepConnected.IsChecked.Value);

            List<CloureParam> cloureParams = new List<CloureParam>();
            cloureParams.Add(new CloureParam("module", "cloure_login"));
            cloureParams.Add(new CloureParam("topic", "login"));
            cloureParams.Add(new CloureParam("user", user));
            cloureParams.Add(new CloureParam("pass", pass));

            string res = await CloureManager.ExecuteAsync(cloureParams);

            JsonObject api_result = JsonObject.Parse(res);
            string error = api_result.GetNamedString("error");
            if (error == "")
            {
                
            }
            else
            {
                btnLogin.IsEnabled = true;
                btnRegister.IsEnabled = true;
                loginProgress.IsActive = false;
            }

            /*
            if (loginResult)
            {
                Frame.Navigate(typeof(MainPage));
            }
            else
            {
                btnLogin.IsEnabled = true;
                btnRegister.IsEnabled = true;
                loginProgress.IsActive = false;
            }
            */
        }

        /*
        public void attemptLoginV2()
        {
            loginProgress.IsActive = true;
            btnLogin.IsEnabled = false;
            btnRegister.IsEnabled = false;

            List<CloureParam> cloureParams = new List<CloureParam>();
            cloureParams.Add(new CloureParam("user", txtUser.Text));
            cloureParams.Add(new CloureParam("pass", txtPass.Password));

            if (CloureManager.cloureClient.Connected)
            {
                CloureManager.cloureClient.SendData("login", "", cloureParams);
            }
            else
            {
                CloureManager.ShowDialog("No estás conectado al servidor!. Comprueba tu conexión a internet, si el problema persiste ponte en contacto con nosotros a info@cloure.com");
                loginProgress.IsActive = false;
                btnLogin.IsEnabled = true;
                btnRegister.IsEnabled = true;
            }
        }
        */

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegistrationPage));
        }

        private void txtPass_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                attemptLogin();
                //attemptLoginV2();
            }
        }

        private void txtLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            CloureManager.lang = (string)comboBox.SelectedValue;
            LoadLocales();
        }

        private async void LoadLocales()
        {
            /*
            JsonObject json_res  = await AvailableLanguages.getLocales("users", CloureManager.lang);
            tbUserPrompt.Text = json_res.GetNamedString("user_login_field");
            tbPassPromp.Text = json_res.GetNamedString("password");
            btnLogin.Content = json_res.GetNamedString("login_button");
            btnRegister.Content = json_res.GetNamedString("register_button");
            chKeepConnected.Content = json_res.GetNamedString("keep_connected");
            */
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CloureManager.cloureClient.Connect();
            CloureManager.cloureClient.OnLoginError += CloureClient_OnLoginError;
            CloureManager.cloureClient.OnLoginSuccess += CloureClient_OnLoginSuccess;
        }

        private async void CloureClient_OnLoginSuccess()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                Frame.Navigate(typeof(MainPage));
            });
        }

        private async void CloureClient_OnLoginError(string error, string errorType)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                btnLogin.IsEnabled = true;
                btnRegister.IsEnabled = true;
                loginProgress.IsActive = false;
                CloureManager.ShowDialog(error);
            });
        }
    }
}
