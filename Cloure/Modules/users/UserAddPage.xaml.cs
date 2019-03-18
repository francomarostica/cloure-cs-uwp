using Cloure.Core;
using Cloure.Modules.users_groups;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.users
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class UserAddPage : Page
    {
        User user;
        ModuleInfo moduleInfo;
        JsonObject locales = new JsonObject();

        public UserAddPage()
        {
            this.InitializeComponent();
            moduleInfo = CloureManager.GetModuleInfo();

            LoadUsersGroups();
            LoadGenders();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            locales = await CloureManager.getLocales("users");

            lblApellido.Text = locales.GetNamedString("last_name");
            lblNombre.Text = locales.GetNamedString("name");
            lblEmpresa.Text = locales.GetNamedString("company");
            lblBirthDate.Text = locales.GetNamedString("birthdate");
            btnFechaNacDatePicker.Content = locales.GetNamedString("select");
            lblGender.Text = locales.GetNamedString("gender");
            lblUserGroup.Text = locales.GetNamedString("user_group");
            lblEmail.Text = locales.GetNamedString("email");

            if (e.Parameter != null)
            {
                if (e.Parameter.GetType() == typeof(int))
                {
                    int id = (int)e.Parameter;
                    get_user(id);
                }
            }
            else
            {
                user = new User();
                user.id = 0;
            }

            if (CloureManager.getAccountType() == "free")
            {
                txtFreeAdvice.Text = "";

                Run run = new Run();
                run.Text = moduleInfo.locales.GetNamedString("user_free_warning_group");
                Hyperlink hyperlink = new Hyperlink();

                Run hyperlinkInline = new Run();
                hyperlinkInline.Text = moduleInfo.locales.GetNamedString("learn_about_our_plans");
                hyperlink.Inlines.Add(hyperlinkInline);
                //hyperlink.NavigateUri = new Uri("https://cloure.com/?app_token=" + Core.Core.appToken);
                hyperlink.NavigateUri = new Uri("https://cloure.com/plans");

                txtFreeAdvice.Inlines.Add(run);
                txtFreeAdvice.Inlines.Add(hyperlink);

                txtFreeAdvice.Visibility = Visibility.Visible;
                txtGrupo.SelectedValue = "user";
                txtGrupo.IsEnabled = true;
                btnGrupoAdd.IsEnabled = false;
                pivotMain.Margin = new Thickness(0, 70, 0, 0);
            }
        }

        private async void get_user(int user_id)
        {
            user = await Users.GetUserById(user_id);
            txtNombre.Text = user.nombre;
            txtApellido.Text = user.apellido;
            txtEmpresa.Text = user.empresa;
            txtGrupo.SelectedValue = user.grupo_id;
            imgPhoto.Source = new BitmapImage(user.ImageURL);
            txtMail.Text = user.email;
            txtSalarioBruto.Text = user.salario.ToString();
            txtComision.Text = user.comision.ToString();

        }

        private async void LoadUsersGroups()
        {
            List<UserGroup> userGroups = await UsersGroups.GetList();
            txtGrupo.ItemsSource = userGroups;
            txtGrupo.DisplayMemberPath = "Name";
            txtGrupo.SelectedValuePath = "Id";
            txtGrupo.SelectedValue = "user";
        }

        private async void LoadGenders()
        {
            List<PersonGender> personGenders = await Users.GetPersonGenders();
            txtGenero.ItemsSource = personGenders;
            txtGenero.DisplayMemberPath = "Name";
            txtGenero.SelectedValuePath = "Id";
            txtGenero.SelectedValue = 0;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private async void Save()
        {
            user.nombre = txtNombre.Text;
            user.apellido = txtApellido.Text;
            user.grupo_id = (string)txtGrupo.SelectedValue;
            user.empresa = txtEmpresa.Text;
            user.email = txtMail.Text;
            user.GeneroId = (int)txtGenero.SelectedValue;

            double comision = 0;
            double salario = 0;

            double.TryParse(txtComision.Text, out comision);
            double.TryParse(txtSalarioBruto.Text, out salario);

            user.comision = comision;
            user.salario = salario;

            if (await Users.save(user))
            {
                CloureManager.GoBack("reload");
            }
        }

        private void btnImage_Click(object sender, RoutedEventArgs e)
        {
            get_file();
        }

        private async void get_file()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            StorageFile file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                var inputStream = await file.OpenSequentialReadAsync();
                var readStream = inputStream.AsStreamForRead();
                var byteArray = new byte[readStream.Length];
                await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                user.CloureImage = new CloureImage(file.Name, byteArray);
                imgPhoto.Source = await user.CloureImage.GetBitmapImage();
            }

        }

        private void btnGrupoAdd_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.Navigate(typeof(UserGroupAddPage));
        }

        private void TxtGrupo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            if (combo.SelectedItem != null)
            {
                UserGroup userGroup = (UserGroup)combo.SelectedItem;
                if (userGroup.IsStaff)
                {
                    grdSalarios.Visibility = Visibility.Visible;
                }
                else
                {
                    grdSalarios.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
