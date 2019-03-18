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

namespace Cloure.Modules.receipts
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class ReceiptDetails : Page
    {
        int ComprobanteId = 0;
        public ReceiptDetails()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if(e.Parameter!=null && e.Parameter.GetType() == typeof(int))
            {
                ComprobanteId = (int)e.Parameter;
                LoadData(ComprobanteId);
            }
        }

        private async void LoadData(int id)
        {
            Receipt receipt = await Receipts.Get(id);
            txtCliente.Text = receipt.CustomerName;
            txtDireccion.Text = receipt.CustomerAddress;
            txtTotal.Text = receipt.Total.ToString("C2");
            lstItems.ItemsSource = receipt.cartItems;
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            print();
        }

        private async void print()
        {
            CloureManager.ShowLoader("Generando documento PDF...");
            await new Receipts().print(ComprobanteId);
            CloureManager.HideLoader();
        }
    }
}
