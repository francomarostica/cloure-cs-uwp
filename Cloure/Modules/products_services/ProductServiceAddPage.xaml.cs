using Cloure.Core;
using Cloure.Modules.products_services_categories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.products_services
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class ProductServiceAddPage : Page
    {
        ProductService product;
        ModuleInfo moduleInfo;
       
        private List<CloureImage> images = new List<CloureImage>();
        List<ProductStock> productStocks;
        int id = 0;

        public ProductServiceAddPage() {
            this.InitializeComponent();
            this.moduleInfo = CloureManager.GetModuleInfo();

            if (CloureManager.getAccountType() == "free" || CloureManager.getAccountType() == "test_free") txtImgAdvice.Text = moduleInfo.locales.GetNamedString("warning_text_image_free");

            LoadLocales();
            LoadProductTypes();
            LoadProductUnits();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                if(e.Parameter.GetType() == typeof(int))
                {
                    id = (int)e.Parameter;
                    getItem(id);
                }
            }

            LoadStock();
        }

        public async void getItem(int id)
        {
            product = await ProductsServices.GetItem(id);
            txtTitulo.Text = product.Title;
            txtDescripcion.Text = product.Descripcion;
            txtProductoTipo.SelectedValue = product.ProductTypeId;
            txtSistemaDeMedida.SelectedValue = product.MeasureUnitId;

            txtIVA.Text = product.IVA.ToString("F2");
            txtCostoPrecio.Text = product.CostoPrecio.ToString("F2");
            txtCostoImporte.Text = product.CostoImporte.ToString("F2");
            txtVentaPrecio.Text = product.VentaPrecio.ToString("F2");
            txtVentaImporte.Text = product.VentaImporte.ToString("F2");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private async void LoadLocales()
        {
            JsonObject locales = await CloureManager.getLocales("products_services");

            tabGeneral.Header = locales.GetNamedString("general");
            tabPrices.Header = locales.GetNamedString("prices");
            tabStock.Header = locales.GetNamedString("stock");
            tabImages.Header = locales.GetNamedString("images");

            txtProductoTipoPrompt.Text = locales.GetNamedString("product_type");
            txtSistemaDeMedidaPrompt.Text = locales.GetNamedString("measure_unit");
            txtTituloPrompt.Text = locales.GetNamedString("title_prompt");
            txtCategoriaN1Prompt.Text = locales.GetNamedString("category_n1");
            txtCategoriaN2Prompt.Text = locales.GetNamedString("category_n2");
            txtCodigoInternoPrompt.Text = locales.GetNamedString("internal_code");
            txtCodigoDeBarrasPrompt.Text = locales.GetNamedString("barcode");
            txtDescripcionPrompt.Text = locales.GetNamedString("description");

            txtCostoPrecioPrompt.Text = locales.GetNamedString("cost_price");
            txtCostoSinIVAPrompt.Text = locales.GetNamedString("price_without_iva");
            txtCostoIVAImportePrompt.Text = locales.GetNamedString("iva_money");
            txtCostoConIVAPrompt.Text = locales.GetNamedString("price_with_iva");

            txtVentaPrecioPrompt.Text = locales.GetNamedString("sale_price");
            txtVentaSinIVAPrompt.Text = locales.GetNamedString("price_without_iva");
            txtVentaIVAImportePrompt.Text = locales.GetNamedString("iva_money");
            txtVentaConIVAPrompt.Text = locales.GetNamedString("price_with_iva");
        }

        public async void LoadProductTypes()
        {
            List<ProductType> productTypes = await ProductsServices.GetTypes();
            txtProductoTipo.ItemsSource = productTypes;
            txtProductoTipo.DisplayMemberPath = "Name";
            txtProductoTipo.SelectedValuePath = "Id";
            if(productTypes!=null && productTypes.Count>0) txtProductoTipo.SelectedIndex = 0;
        }

        public async void LoadStock()
        {
            productStocks = await ProductsServices.GetStock(id);
            lstStock.ItemsSource = productStocks;
        }

        public async void LoadProductUnits()
        {
            List<ProductMeasureUnit> productMeasureUnits = await ProductsServices.GetUnits();
            txtSistemaDeMedida.ItemsSource = productMeasureUnits;
            txtSistemaDeMedida.DisplayMemberPath = "Title";
            txtSistemaDeMedida.SelectedValuePath = "Id";
            if (productMeasureUnits != null && productMeasureUnits.Count > 0) txtSistemaDeMedida.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Guardar();
        }

        private async void Guardar()
        {
            double IVA = 0;
            double CostoPrecio = 0;
            double CostoImporte = 0;
            double VentaPrecio = 0;
            double VentaImporte = 0;

            if (product == null)
            {
                product = new ProductService();
                product.Id = 0;
            }

            double.TryParse(txtIVA.Text, out IVA);
            double.TryParse(txtCostoPrecio.Text, out CostoPrecio);
            double.TryParse(txtCostoImporte.Text, out CostoImporte);
            double.TryParse(txtVentaPrecio.Text, out VentaPrecio);
            double.TryParse(txtVentaImporte.Text, out VentaImporte);

            product.ProductTypeId = (int)txtProductoTipo.SelectedValue;
            product.MeasureUnitId = (int)txtSistemaDeMedida.SelectedValue;
            product.Title = txtTitulo.Text;
            product.Descripcion = txtDescripcion.Text;
            product.IVA = IVA;
            product.CostoPrecio = CostoPrecio;
            product.CostoImporte = CostoImporte;
            product.VentaPrecio = VentaPrecio;
            product.VentaImporte = VentaImporte;
            if(images!=null) product.Images = images;
            product.Stock = productStocks;

            if (await ProductsServices.save(product)) {
                CloureManager.GoBack();
            }
        }

        private void TextOnFocus(object sender, RoutedEventArgs e) {
            TextBox tb = (TextBox)sender;
            tb.SelectAll();
        }

        private void calcularCostoImporte()
        {
            double precio = 0;
            double iva = 0;
            double importe = 0;
            double iva_precio = 0;

            double.TryParse(txtCostoPrecio.Text, out precio);
            double.TryParse(txtIVA.Text, out iva);

            importe = precio + ((precio * iva) / 100);
            iva_precio = importe - precio;

            txtCostoImporte.Text = importe.ToString("F2");
            txtCostoIVA.Text = iva_precio.ToString("F2");
        }

        private void calcularCostoPrecio()
        {
            double precio = 0;
            double iva = 0;
            double importe = 0;
            double iva_precio = 0;

            double.TryParse(txtCostoImporte.Text, out importe);
            double.TryParse(txtIVA.Text, out iva);

            precio = importe / ((100 + iva) / 100);
            iva_precio = importe - precio;

            txtCostoPrecio.Text = precio.ToString("F2");
            txtCostoIVA.Text = iva_precio.ToString("F2");
        }

        private void calcularVentaImporte()
        {
            double precio = 0;
            double iva = 0;
            double importe = 0;
            double iva_precio = 0;

            double.TryParse(txtVentaPrecio.Text, out precio);
            double.TryParse(txtIVA.Text, out iva);

            importe = precio + ((precio * iva) / 100);
            iva_precio = importe - precio;

            txtVentaImporte.Text = importe.ToString("F2");
            txtVentaIVA.Text = iva_precio.ToString("F2");
        }

        private void calcularVentaPrecio()
        {
            double precio = 0;
            double iva = 0;
            double importe = 0;
            double iva_precio = 0;

            double.TryParse(txtVentaImporte.Text, out importe);
            double.TryParse(txtIVA.Text, out iva);

            precio = importe / ((100 + iva) / 100);
            iva_precio = importe - precio;

            txtVentaPrecio.Text = precio.ToString("F2");
            txtVentaIVA.Text = iva_precio.ToString("F2");
        }

        private void txtCostoPrecio_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
        {
            calcularCostoImporte();
        }

        private void txtCostoImporte_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
        {
            calcularCostoPrecio();
        }

        private void txtVentaPrecio_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
        {
            calcularVentaImporte();
        }

        private void txtVentaImporte_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
        {
            calcularVentaPrecio();
        }

        private void btnAddImage_Click(object sender, RoutedEventArgs e)
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

            if (CloureManager.getAccountType() == "free" || CloureManager.getAccountType() == "test_free")
            {
                
                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    images = new List<CloureImage>();
                    var inputStream = await file.OpenSequentialReadAsync();
                    var readStream = inputStream.AsStreamForRead();
                    var byteArray = new byte[readStream.Length];
                    await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                    CloureImage cloureImage = new CloureImage(file.Name, byteArray);

                    IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                    BitmapImage image = new BitmapImage();
                    image.SetSource(stream);
                    cloureImage.ImageSrc = image;

                    images.Add(cloureImage);

                    lstImages.ItemsSource = null;
                    lstImages.ItemsSource = images;
                }
            }
            else
            {
                IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
                foreach (StorageFile file in files)
                {
                    if (file != null)
                    {
                        var inputStream = await file.OpenSequentialReadAsync();
                        var readStream = inputStream.AsStreamForRead();
                        var byteArray = new byte[readStream.Length];
                        await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                        CloureImage cloureImage = new CloureImage(file.Name, byteArray);

                        IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                        BitmapImage image = new BitmapImage();
                        image.SetSource(stream);
                        cloureImage.ImageSrc = image;

                        images.Add(cloureImage);

                        //lstImages.ItemsSource = null;
                        //lstImages.ItemsSource = images;
                    }
                }
            }

            lstImages.ItemsSource = null;
            lstImages.ItemsSource = images;
        }

        private void btnAddCategoriaN1_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.Navigate(typeof(ProductServiceCategoryAddPage));
        }

        private void StockMin_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            CloureManager.NumberInput(tb);
            ProductStock productStock = (ProductStock)((FrameworkElement)e.OriginalSource).DataContext;
            double stock_min = 0;
            double.TryParse(tb.Text, out stock_min);
            productStock.Min = stock_min;
        }

        private void StockActual_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            CloureManager.NumberInput(tb);
            ProductStock productStock = (ProductStock)((FrameworkElement)e.OriginalSource).DataContext;
            double stock_actual = 0;
            double.TryParse(tb.Text, out stock_actual);
            productStock.Actual = stock_actual;
        }
    }
}
