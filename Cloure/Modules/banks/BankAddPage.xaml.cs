using Cloure.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.banks
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class BankAddPage : Page
    {
        Bank bank;
        ModuleInfo moduleInfo;

        public BankAddPage()
        {
            this.InitializeComponent();

            moduleInfo = CloureManager.GetModuleInfo();

            txtNombrePrompt.Text = moduleInfo.locales.GetNamedString("name");
            txtWebPrompt.Text = moduleInfo.locales.GetNamedString("web");
            txtOnlineBankingPrompt.Text = moduleInfo.locales.GetNamedString("online_banking");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                if (e.Parameter.GetType() == typeof(Bank))
                {
                    bank = (Bank)e.Parameter;
                    txtNombre.Text = bank.Name;
                    txtWeb.Text = bank.Web;
                    txtOnlineBanking.Text = bank.OnlineBanking;
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private async void Save() {
            if(bank == null)
            {
                bank = new Bank();
                bank.Id = 0;
            }
            bank.Name = txtNombre.Text;
            bank.Web = txtWeb.Text;
            bank.OnlineBanking = txtOnlineBanking.Text;

            if (await Banks.save(bank))
            {
                CloureManager.GoBack("reload");
            }
        }
    }
}
