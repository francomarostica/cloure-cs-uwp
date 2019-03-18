using Cloure.Core;
using Cloure.Modules.payments_methods;
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

namespace Cloure.Modules.finances
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class FinanceAdd : Page
    {
        ModuleInfo moduleInfo;
        public FinanceAdd()
        {
            this.InitializeComponent();

            moduleInfo = CloureManager.GetModuleInfo();

            cboOperationPrompt.Text = moduleInfo.locales.GetNamedString("operation");
            cboPaymentMethodPrompt.Text = moduleInfo.locales.GetNamedString("payment_method");
            txtDescriptionPrompt.Text = moduleInfo.locales.GetNamedString("description");
            txtAmountPrompt.Text = moduleInfo.locales.GetNamedString("amount");

            GetOperations();
            GetPaymentsMethods();
        }

        private async void GetOperations()
        {
            cboOperation.DisplayMemberPath = "Name";
            cboOperation.SelectedValuePath = "Id";
            cboOperation.ItemsSource = await new Operations().getList();
            cboOperation.SelectedIndex = 0;
        }

        private async void GetPaymentsMethods()
        {
            cboPaymentMethod.DisplayMemberPath = "Name";
            cboPaymentMethod.SelectedValuePath = "Id";
            cboPaymentMethod.ItemsSource = await PaymentsMethods.getList();
            cboPaymentMethod.SelectedIndex = 0;
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            save();
        }

        private async void save()
        {
            FinanceMovement finance = new FinanceMovement();
            finance.FechaStr = DateTime.Now.ToString("yyyy-MM-dd");
            finance.FormaDePagoId = (int)cboPaymentMethod.SelectedValue;
            finance.TipoMovimientoId = (string)cboOperation.SelectedValue;
            finance.Detalles = txtDescription.Text;
            finance.ImporteStr = txtAmount.Text;

            if(await new Finances().save(finance))
            {
                CloureManager.GoBack("reload");
            }
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }
    }
}
