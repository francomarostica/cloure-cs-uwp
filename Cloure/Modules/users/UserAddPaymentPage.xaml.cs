using Cloure.Core;
using Cloure.Modules.banks;
using Cloure.Modules.credit_cards;
using Cloure.Modules.debit_cards;
using Cloure.Modules.payments_methods;
using Cloure.Modules.receipts;
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

namespace Cloure.Modules.users
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class UserAddPaymentPage : Page
    {
        Receipt receipt;
        int SelectedPaymentMethodId = 0;
        int UsuarioId = 0;

        public UserAddPaymentPage()
        {
            this.InitializeComponent();
            grdAditionalData.Visibility = Visibility.Collapsed;
            LoadPaymentsMethods();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter.GetType() == typeof(int))
            {
                UsuarioId = (int)e.Parameter;
            }
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            Guardar();
        }

        private async void LoadPaymentsMethods()
        {
            List<PaymentMethod> paymentMethods = await PaymentsMethods.getList();
            txtPaymentMethod.ItemsSource = paymentMethods;
            txtPaymentMethod.SelectedValuePath = "Id";
            txtPaymentMethod.DisplayMemberPath = "Name";
            txtPaymentMethod.SelectedValue = 1;
        }

        private async void Guardar()
        {
            double importe = 0;
            double.TryParse(txtImporte.Text, out importe);
            int _FormaDePagoEntidadId = 0;

            try
            {
                _FormaDePagoEntidadId = (int)txtEntidad.SelectedValue;
            }
            catch
            {

            }

            bool result = await Users.AddPayment(UsuarioId, importe, SelectedPaymentMethodId, _FormaDePagoEntidadId);
            if (result)
            {
                CloureManager.ShowDialog("La operación ha sido realizada");
                CloureManager.GoBack();
            }
        }

        private void txtPaymentMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            PaymentMethod paymentMethod = (PaymentMethod)comboBox.SelectedItem;
            SelectedPaymentMethodId = paymentMethod.Id;

            if (paymentMethod.Id == 1) //Efectivo
            {
                grdAditionalData.Visibility = Visibility.Collapsed;
            }
            if (paymentMethod.Id == 2) //Tarjeta debito
            {
                LoadCreditCards();
                grdAditionalData.Visibility = Visibility.Visible;
            }
            if (paymentMethod.Id == 3) //Tarjeta credito
            {
                LoadCreditCards();
                grdAditionalData.Visibility = Visibility.Visible;
            }
            if (paymentMethod.Id == 4) //Cuenta corriente
            {
                grdAditionalData.Visibility = Visibility.Collapsed;
            }
            if (paymentMethod.Id == 5) //Cheque
            {
                LoadBanksForCheks();
                grdAditionalData.Visibility = Visibility.Visible;
            }
            if (paymentMethod.Id == 6) //Contrarembolso
            {
                grdAditionalData.Visibility = Visibility.Collapsed;
            }
            if (paymentMethod.Id == 7) //Deposito-transferencia
            {
                LoadBanksForTransfers();
                grdAditionalData.Visibility = Visibility.Visible;
            }
        }

        private async void LoadCreditCards()
        {
            List<CreditCard> creditCards = await CreditCards.getList();
            lblEntidadPrompt.Text = "Tarjeta";
            txtEntidad.ItemsSource = null;
            txtEntidad.ItemsSource = creditCards;
            txtEntidad.DisplayMemberPath = "Name";
            txtEntidad.SelectedValuePath = "Id";
            lblEntidadDataPrompt.Visibility = Visibility.Collapsed;
            txtEntidadData.Visibility = Visibility.Collapsed;
            if(creditCards.Count>0) txtEntidad.SelectedIndex = 0;
        }

        private async void LoadDebitCards()
        {
            List<DebitCard> debitCards = await DebitCards.getList();
            lblEntidadPrompt.Text = "Tarjeta";
            txtEntidad.ItemsSource = null;
            txtEntidad.ItemsSource = debitCards;
            txtEntidad.DisplayMemberPath = "Name";
            txtEntidad.SelectedValuePath = "Id";
            lblEntidadDataPrompt.Visibility = Visibility.Collapsed;
            txtEntidadData.Visibility = Visibility.Collapsed;
            if(debitCards.Count>0) txtEntidad.SelectedIndex = 0;
        }

        private async void LoadBanksForCheks()
        {
            List<Bank> banks = await Banks.getList();
            lblEntidadPrompt.Text = "Banco";
            txtEntidad.ItemsSource = null;
            txtEntidad.ItemsSource = banks;
            txtEntidad.DisplayMemberPath = "Name";
            txtEntidad.SelectedValuePath = "Id";
            lblEntidadDataPrompt.Text = "Cheque N°";
            lblEntidadDataPrompt.Visibility = Visibility.Visible;
            txtEntidadData.Visibility = Visibility.Visible;
            if(banks.Count>0) txtEntidad.SelectedIndex = 0;
        }

        private async void LoadBanksForTransfers()
        {
            List<Bank> banks = await Banks.getList();
            lblEntidadPrompt.Text = "Banco";
            txtEntidad.ItemsSource = null;
            txtEntidad.ItemsSource = banks;
            txtEntidad.DisplayMemberPath = "Name";
            txtEntidad.SelectedValuePath = "Id";
            lblEntidadDataPrompt.Visibility = Visibility.Collapsed;
            txtEntidadData.Visibility = Visibility.Collapsed;
            if (banks.Count > 0) txtEntidad.SelectedIndex = 0;
        }

        private void btnAddEntity_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPaymentMethodId == 5 || SelectedPaymentMethodId == 7)
            {
                CloureManager.Navigate(typeof(BankAddPage));
            }
            if (SelectedPaymentMethodId == 2)
            {
                CloureManager.Navigate(typeof(DebitCardAddPage));
            }
            if (SelectedPaymentMethodId == 3)
            {
                CloureManager.Navigate(typeof(CreditCardAddPage));
            }
        }
    }
}
