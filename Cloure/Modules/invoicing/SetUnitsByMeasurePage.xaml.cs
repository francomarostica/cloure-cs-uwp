using Cloure.Modules.products_services;
using Cloure.Modules.receipts;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace Cloure.Modules.invoicing
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class SetUnitsByMeasurePage : Page
    {
        ProductService product;
        double cantidad_total;
        double total;

        public SetUnitsByMeasurePage()
        {
            this.InitializeComponent();
            this.Loaded += SetUnitsByMeasurePage_Loaded;
        }

        private void SetUnitsByMeasurePage_Loaded(object sender, RoutedEventArgs e)
        {
            txtCantidad.Focus(FocusState.Keyboard);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            product = (ProductService)e.Parameter;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private void CalcularDesperdicio()
        {
            double ancho = 0;
            double alto = 0;
            double lado_mayor = 0;
            double lado_menor = 0;
            double cantidad = 0;
            double precio_desperdicio = 0;

            double.TryParse(txtAncho.Text, out ancho);
            double.TryParse(txtAlto.Text, out alto);
            double.TryParse(txtLadoMenor.Text, out lado_menor);
            double.TryParse(txtCantidad.Text, out cantidad);
            double.TryParse(txtPrecioDesperdicio.Text, out precio_desperdicio);

            if (ancho > alto)
                lado_mayor = ancho;
            else
                lado_mayor = alto;

            double sup_requerida = ancho * alto;
            double sup_material = (alto + 0.10) * (lado_menor);
            double desperdicio = sup_material - sup_requerida;
            double importe_desperdicio = desperdicio * precio_desperdicio;
            cantidad_total = cantidad * sup_requerida;
            double ImporteTmp = product.Importe;

            total = (ImporteTmp * cantidad_total) + importe_desperdicio;

            txtTotalDesperdicio.Text = importe_desperdicio.ToString("F2");
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            finalizar();
        }

        private void finalizar()
        {
            double cant = 0;
            double.TryParse(txtCantidad.Text, out cant);

            if (txtObservaciones.Text.Length > 0) txtObservaciones.Text += " ";

            CartItem cartItem = new CartItem();
            cartItem.ProductoId = product.Id;
            cartItem.Cantidad = cantidad_total;
            cartItem.Importe = total / cantidad_total;
            cartItem.Total = total;
            cartItem.Descripcion = product.Title + "(" + txtObservaciones.Text + txtAncho.Text + " x " + txtAlto.Text + ")";
            CloureManager.GoBack(cartItem);
        }

        private void txtCantidad_TextChanged(object sender, TextChangedEventArgs e)
        {
            CloureManager.NumberInput(sender);
            CalcularDesperdicio();
        }

        private void txtAncho_TextChanged(object sender, TextChangedEventArgs e)
        {
            CloureManager.NumberInput(sender);
            CalcularDesperdicio();
        }

        private void txtAlto_TextChanged(object sender, TextChangedEventArgs e)
        {
            CloureManager.NumberInput(sender);
            CalcularDesperdicio();
        }

        private void txtLadoMenor_TextChanged(object sender, TextChangedEventArgs e)
        {
            CloureManager.NumberInput(sender);
            CalcularDesperdicio();
        }

        private void txtPrecioDesperdicio_TextChanged(object sender, TextChangedEventArgs e)
        {
            CloureManager.NumberInput(sender);
            CalcularDesperdicio();
        }

        private void txtCantidad_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void txtAncho_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void txtAlto_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void txtLadoMenor_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void txtPrecioDesperdicio_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void txtTotalDesperdicio_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                finalizar();
            }
        }
    }
}
