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

namespace Cloure.Modules.invoicing
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class InvoicingFinishPage : Page
    {
        Receipt receipt;
        int SelectedPaymentMethodId = 0;

        public InvoicingFinishPage()
        {
            this.InitializeComponent();
            LoadPaymentsMethods();
            this.Loaded += InvoicingFinishPage_Loaded;
        }

        private void InvoicingFinishPage_Loaded(object sender, RoutedEventArgs e)
        {
            txtPaymentMethod.Focus(FocusState.Keyboard);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            double saldo_cte = 0;
            receipt = (Receipt)e.Parameter;
            if (receipt.Customer != null)
            {
                tbCliente.Text = receipt.Customer.ToString();
                tbSaldo.Text = receipt.Customer.saldo.ToString("F2");
                saldo_cte = receipt.Customer.saldo;
            }
            
            tbTotalComprobante.Text = receipt.Total.ToString("F2");

            double TotalAPagar = receipt.Total + saldo_cte;
            tbTotal.Text = TotalAPagar.ToString("F2");
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
            double entrega = 0;
            int FormaDePagoId = 0;
            int EntidadId = 0;

            double.TryParse(txtEntrega.Text, out entrega);
            if (txtPaymentMethod.SelectedValue != null) FormaDePagoId = (int)txtPaymentMethod.SelectedValue;
            if (txtEntidad.SelectedValue != null) EntidadId = (int)txtEntidad.SelectedValue;

            receipt.Entrega = entrega;
            receipt.FormaDePagoId = FormaDePagoId;
            receipt.FormaDePagoEntidadId = EntidadId;
            receipt.FormaDePagoData = txtEntidadData.Text;
            receipt.FormaDePagoCobro = txtFechaCobro.Date.DateTime;

            int result = await Invoicing.save(receipt);
            if (result>0)
            {
                receipt.Id = result;
                CloureParam cloureParam = new CloureParam("finish", receipt);
                CloureManager.GoBack(cloureParam);
            }
        }

        private void txtPaymentMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            PaymentMethod paymentMethod = (PaymentMethod)comboBox.SelectedItem;
            SelectedPaymentMethodId = paymentMethod.Id;

            if (paymentMethod.Id == 1) //Efectivo
            {
                grdEntidad.Visibility = Visibility.Collapsed;
                grdEntidadData.Visibility = Visibility.Collapsed;
                grdFechaCobro.Visibility = Visibility.Collapsed;
            }
            if (paymentMethod.Id == 2) //Tarjeta debito
            {
                grdEntidad.Visibility = Visibility.Visible;
                grdEntidadData.Visibility = Visibility.Collapsed;
                grdFechaCobro.Visibility = Visibility.Collapsed;
                LoadCreditCards();
            }
            if (paymentMethod.Id == 3) //Tarjeta credito
            {
                grdEntidad.Visibility = Visibility.Visible;
                grdEntidadData.Visibility = Visibility.Collapsed;
                grdFechaCobro.Visibility = Visibility.Collapsed;
                LoadCreditCards();
            }
            if (paymentMethod.Id == 4) //Cuenta corriente
            {
                grdEntidad.Visibility = Visibility.Collapsed;
                grdEntidadData.Visibility = Visibility.Collapsed;
                grdFechaCobro.Visibility = Visibility.Collapsed;
            }
            if (paymentMethod.Id == 5) //Cheque
            {
                grdEntidad.Visibility = Visibility.Visible;
                grdEntidadData.Visibility = Visibility.Visible;
                grdFechaCobro.Visibility = Visibility.Visible;
                LoadBanksForCheks();
            }
            if (paymentMethod.Id == 6) //Contrarembolso
            {
                grdEntidad.Visibility = Visibility.Collapsed;
                grdEntidadData.Visibility = Visibility.Collapsed;
                grdFechaCobro.Visibility = Visibility.Collapsed;
            }
            if (paymentMethod.Id == 7) //Deposito-transferencia
            {
                grdEntidad.Visibility = Visibility.Visible;
                grdEntidadData.Visibility = Visibility.Collapsed;
                grdFechaCobro.Visibility = Visibility.Collapsed;
                LoadBanksForTransfers();
            }
        }

        private async void LoadCreditCards()
        {
            lblEntidadPrompt.Text = "Tarjeta";
            txtEntidad.ItemsSource = null;
            txtEntidad.ItemsSource = await CreditCards.getList();
            txtEntidad.DisplayMemberPath = "Name";
            txtEntidad.SelectedValuePath = "Id";
            txtEntidad.SelectedIndex = 0;
        }

        private async void LoadDebitCards()
        {
            lblEntidadPrompt.Text = "Tarjeta";
            txtEntidad.ItemsSource = null;
            txtEntidad.ItemsSource = await DebitCards.getList();
            txtEntidad.DisplayMemberPath = "Name";
            txtEntidad.SelectedValuePath = "Id";
            txtEntidad.SelectedIndex = 0;
        }

        private async void LoadBanksForCheks()
        {
            lblEntidadPrompt.Text = "Banco";
            txtEntidad.ItemsSource = null;
            txtEntidad.ItemsSource = await Banks.getList();
            txtEntidad.DisplayMemberPath = "Name";
            txtEntidad.SelectedValuePath = "Id";
            lblEntidadDataPrompt.Text = "Cheque N°";
            txtEntidad.SelectedIndex = 0;
        }

        private async void LoadBanksForTransfers()
        {
            lblEntidadPrompt.Text = "Banco";
            txtEntidad.ItemsSource = null;
            txtEntidad.ItemsSource = await Banks.getList();
            txtEntidad.DisplayMemberPath = "Name";
            txtEntidad.SelectedValuePath = "Id";
            txtEntidad.SelectedIndex = 0;
        }

        private void btnAddEntity_Click(object sender, RoutedEventArgs e)
        {
            if(SelectedPaymentMethodId == 5 || SelectedPaymentMethodId == 7)
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

        private void txtEntrega_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.F2)
            {
                Guardar();
            }
        }
    }
}
