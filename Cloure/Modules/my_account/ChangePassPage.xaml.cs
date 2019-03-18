using Cloure.Core;
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

namespace Cloure.Modules.my_account
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            this.InitializeComponent();

            ModuleInfo moduleInfo = CloureManager.GetModuleInfo();
            txtNewPassPrompt.Text = moduleInfo.locales.GetNamedString("new_password");
            txtOldPassPrompt.Text = moduleInfo.locales.GetNamedString("old_password");
            txtRepeatPassPrompt.Text = moduleInfo.locales.GetNamedString("repeat_password");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            change_pass();
        }

        private async void change_pass()
        {
            try
            {
                List<CloureParam> cparams = new List<CloureParam>();
                cparams.Add(new CloureParam("module", "my_account"));
                cparams.Add(new CloureParam("topic", "cambiar_clave"));
                cparams.Add(new CloureParam("old_pass", txtOldPass.Password));
                cparams.Add(new CloureParam("new_pass", txtNewPass.Password));
                cparams.Add(new CloureParam("repeat_pass", txtRepeatPass.Password));
                string res = await CloureManager.ExecuteAsync(cparams);

                JsonObject api_result = JsonObject.Parse(res);
                string error = api_result.GetNamedString("Error");
                string respuesta = api_result.GetNamedString("Response");
                if (error == "")
                {
                    var dialog = new MessageDialog(respuesta);
                    await dialog.ShowAsync();
                    CloureManager.GoBack();
                }
                else
                {
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }

        }
    }
}
