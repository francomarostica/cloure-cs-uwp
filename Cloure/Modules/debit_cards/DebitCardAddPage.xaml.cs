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

namespace Cloure.Modules.debit_cards
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class DebitCardAddPage : Page
    {
        DebitCard creditCard;
        ModuleInfo moduleInfo;

        public DebitCardAddPage()
        {
            this.InitializeComponent();

            moduleInfo = CloureManager.GetModuleInfo();
            txtNombrePrompt.Text = moduleInfo.locales.GetNamedString("name");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                if (e.Parameter.GetType() == typeof(DebitCard))
                {
                    creditCard = (DebitCard)e.Parameter;
                    txtNombre.Text = creditCard.Name;
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

        private async void Save()
        {
            if (creditCard == null)
            {
                creditCard = new DebitCard();
                creditCard.Id = 0;
            }
            creditCard.Name = txtNombre.Text;

            if (await DebitCards.save(creditCard))
            {
                CloureManager.GoBack("reload");
            }
        }
    }
}
