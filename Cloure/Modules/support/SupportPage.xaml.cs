using Cloure.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace Cloure.Modules.support
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class SupportPage : Page
    {
        ModuleInfo moduleInfo;

        public SupportPage()
        {
            this.InitializeComponent();
            moduleInfo = CloureManager.GetModuleInfo();

            txtInquiryTypePrompt.Text = moduleInfo.locales.GetNamedString("inquiry_type");
            txtCommentsPrompt.Text = moduleInfo.locales.GetNamedString("comments");

            GetSupportTypes();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private async void SendMessage()
        {
            bool res = await Support.Send((string)txtTipoMensaje.SelectedValue, txtWeb.Text);
            if (res)
            {
                var dialog = new MessageDialog("Su mensaje ha sido enviado!");
                await dialog.ShowAsync();
                txtWeb.Text = "";
            }
        }

        private async void GetSupportTypes()
        {
            txtTipoMensaje.DisplayMemberPath = "Title";
            txtTipoMensaje.SelectedValuePath = "Id";
            txtTipoMensaje.ItemsSource = await Support.GetSupportTypes();
            txtTipoMensaje.SelectedIndex = 0;
        }
    }
}
