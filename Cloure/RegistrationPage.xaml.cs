using Cloure.Core;
using Cloure.Modules.countries;
using Cloure.Modules.users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            this.InitializeComponent();

            loadBusinessTypes();
            loadCountries();
            LoadLocales();
        }

        private async void LoadLocales()
        {
            JsonObject json_res = await AvailableLanguages.getLocales("users", CloureManager.lang);
            tbNamePrompt.Text = json_res.GetNamedString("name");
            tbLastNamePrompt.Text = json_res.GetNamedString("last_name");
            tbEmailPrompt.Text = json_res.GetNamedString("email");
            tbBusinessNamePrompt.Text = json_res.GetNamedString("business_name");
            tbBusinessTypePrompt.Text = json_res.GetNamedString("business_type");
            tbCountryPrompt.Text = json_res.GetNamedString("country");
            tbPassPrompt.Text = json_res.GetNamedString("password");
            tbRepeatPassPrompt.Text = json_res.GetNamedString("repeat_password");
            tbCloureURLPrompt.Text = json_res.GetNamedString("cloure_url_prompt");
            txtCloureURL.PlaceholderText = json_res.GetNamedString("cloure_url_example");
            btnLogin.Content = json_res.GetNamedString("login_button");
            btnRegister.Content = json_res.GetNamedString("register_button");
        }

        public async void loadBusinessTypes()
        {
            txtTipoEmpresa.ItemsSource = await new BusinessTypes().getList();
            txtTipoEmpresa.DisplayMemberPath = "Title";
            txtTipoEmpresa.SelectedValuePath = "Id";
            txtTipoEmpresa.SelectedValue = "generic";
        }

        public async void loadCountries()
        {
            txtPais.ItemsSource = await Countries.GetCloureList();
            txtPais.DisplayMemberPath = "Name";
            txtPais.SelectedValuePath = "Id";
            txtPais.SelectedValue = "9";
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            loginProgress.IsActive = true;
            btnLogin.IsEnabled = false;
            btnRegister.IsEnabled = false;
            register_account();
        }

        private async void register_account()
        {
            bool registrationResult = await CloureManager.registerAccount(
                txtNombre.Text,
                txtApellido.Text,
                txtEmail.Text,
                txtPass.Password.ToString(),
                txtRepeatPass.Password.ToString(),
                txtEmpresa.Text,
                (string)txtTipoEmpresa.SelectedValue,
                txtCloureURL.Text,
                ((int)txtPais.SelectedValue).ToString()
            );

            if (registrationResult)
            {
                Frame.Navigate(typeof(MainPage));
            }
            else
            {
                btnLogin.IsEnabled = true;
                btnRegister.IsEnabled = true;
                loginProgress.IsActive = false;
            }
        }
    }
}
