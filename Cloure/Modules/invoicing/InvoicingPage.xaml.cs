using Cloure.Core;
using Cloure.Modules.company_branches;
using Cloure.Modules.company_branches_receipts;
using Cloure.Modules.products_services;
using Cloure.Modules.receipts;
using Cloure.Modules.users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.invoicing
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class InvoicingPage : Page
    {
        private List<CompanyBranch> companyBranches = new List<CompanyBranch>();
        public List<CartItem> carrito = new List<CartItem>();
        private int CustomerId = 0;
        ContentDialog finishedDialog;
        Receipt receipt = new Receipt();

        public InvoicingPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            LoadCompanyBranches();
            LoadCompanyBranchesReceipts();
            lstCarrito.ItemsSource = carrito;
            this.Loaded += InvoicingPage_Loaded;
        }

        private void InvoicingPage_Loaded(object sender, RoutedEventArgs e)
        {
            txtCliente.Focus(FocusState.Keyboard);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadLocales();

            object cloure_param = CloureManager.GetParameter();
            txtCliente.Focus(FocusState.Programmatic);
            if (cloure_param != null)
            {
                if (cloure_param.GetType() == typeof(CartItem))
                {
                    CartItem item = (CartItem)CloureManager.GetParameter();
                    addProductToCarrito(item);
                }
                if (cloure_param.GetType() == typeof(CloureParam))
                {
                    CloureParam cloureParam = (CloureParam)CloureManager.GetParameter();
                    if (cloureParam.name == "finish")
                    {
                        Receipt receipt = (Receipt)cloureParam.value;
                        ShowFinishedDialog(receipt);
                        Clear();
                    }
                }
            }

            check_monthly_exceed();
        }

        private async void LoadLocales()
        {
            JsonObject json_res = await AvailableLanguages.getLocales("invoicing", CloureManager.lang);
            txtAdvice.Text = json_res.GetNamedString("press_f2_to_finish");
            tbDatePrompt.Text = json_res.GetNamedString("date");
            tbCompanyBranchPrompt.Text = json_res.GetNamedString("branch");
            tbReceiptType.Text = json_res.GetNamedString("receipt_type");
            tbCustomerPrompt.Text = json_res.GetNamedString("customer");
            tbCustomerNamePrompt.Text = json_res.GetNamedString("customer_name");
            tbSaldoPrompt.Text = json_res.GetNamedString("balance");
            tbProductsServicesPrompt.Text = json_res.GetNamedString("products_services");
            tbQuantityPrompt.Text = json_res.GetNamedString("quantity");
            tbProductPrompt.Text = json_res.GetNamedString("product_service");
            tbObservationsPrompt.Text = json_res.GetNamedString("observations");
        }

        private async void check_monthly_exceed()
        {
            bool exceed = await finances.Finances.isMonthlyIncomingExceded();
            if (CloureManager.getAccountType() == "free" || CloureManager.getAccountType() == "test_free")
            {
                if (exceed)
                {
                    btnAceptar.IsEnabled = false;
                    txtAdvice.Text = "";
                }
            }
        }

        private async void ShowFinishedDialog(Receipt receipt)
        {
            string receipt_type = "";
            if (receipt.TypeId == 2) receipt_type = "pedido";
            Run run = new Run();
            run.Text = "El ";

            Run runPedido = new Run();
            runPedido.Text = receipt_type + " Nº " + receipt.Id.ToString();
            
            Hyperlink hyperlinkPedido = new Hyperlink();
            hyperlinkPedido.Inlines.Add(runPedido);
            hyperlinkPedido.Click += HyperlinkPedido_Click;

            TextBlock tbContent = new TextBlock();
            tbContent.Inlines.Add(run);
            tbContent.Inlines.Add(hyperlinkPedido);
            tbContent.Inlines.Add(new Run() { Text=" ha sido generado!" });

            finishedDialog = new ContentDialog
            {
                Title = "Operación realizada",
                Content = tbContent,
                CloseButtonText = "Cerrar"
            };

            finishedDialog.PrimaryButtonClick += FinishedDialog_PrimaryButtonClick;

            ContentDialogResult result = await finishedDialog.ShowAsync();
        }

        private void FinishedDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            
        }

        private void HyperlinkPedido_Click(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            finishedDialog.Hide();
            CloureManager.Navigate(typeof(ReceiptDetails), receipt.Id);
            //throw new NotImplementedException();
        }

        private void Clear()
        {
            carrito = new List<CartItem>();
            lstCarrito.ItemsSource = null;
            lstCarrito.ItemsSource = carrito;
            txtCliente.Text = "";
            txtProducto.Text = "";
            txtProductoCantidad.Text = "1";
            CustomerId = 0;
            tbClienteRazonSocial.Text = "";
            tbClienteSaldo.Text = "";
        }

        private void txtCliente_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            
        }

        private async void loadCustomer()
        {
            UsersResponse usersResponse = await Users.getList(txtCliente.Text);

            if (usersResponse.TotalRegistros == 0)
            {
                var dialog = new MessageDialog("No se encontraron registros");
                await dialog.ShowAsync();
            }
            else if (usersResponse.TotalRegistros == 1)
            {
                /*
                var dialog = new MessageDialog("Se encontro un registro");
                await dialog.ShowAsync();
                */
                User user = usersResponse.Items[0];
                txtCliente.Text = user.id.ToString();
                tbClienteRazonSocial.Text = user.apellido + ", " + user.nombre;
                tbClienteSaldo.Text = user.saldo_str;
                CustomerId = usersResponse.Items[0].id;
                txtObservaciones.Focus(FocusState.Keyboard);
            }
            else
            {
                //var dialog = new MessageDialog("Se encontraron varios registros");
                //await dialog.ShowAsync();
                lstClientes.Visibility = Visibility.Visible;
                lstClientes.ItemsSource = usersResponse.Items;
                
            }
        }

        private async void LoadCompanyBranches()
        {
            CompanyBranchResponse response = await CompanyBranches.getList();
            companyBranches = response.Items;
            txtSucursal.ItemsSource = null;
            txtSucursal.ItemsSource = companyBranches;
            txtSucursal.SelectedValuePath = "Id";
            txtSucursal.DisplayMemberPath = "Name";
            txtSucursal.SelectedIndex = 0;
        }

        private async void LoadCompanyBranchesReceipts()
        {
            List<CompanyBranchReceipt> receipts = await CompanyBranchesReceipts.GetList();
            txtComprobante.ItemsSource = null;
            txtComprobante.ItemsSource = receipts;
            txtComprobante.SelectedValuePath = "Id";
            txtComprobante.DisplayMemberPath = "Name";
            if(receipts.Count>0) txtComprobante.SelectedValue = 2;
        }

        private async void loadProduct()
        {
            GenericResponse productsResponse = await ProductsServices.GetList(txtProducto.Text);

            if (productsResponse.Items.Count == 0)
            {
                var dialog = new MessageDialog("No se encontraron registros");
                await dialog.ShowAsync();
            }
            else if (productsResponse.Items.Count == 1)
            {
                product_selected((ProductService)productsResponse.Items[0]);
            }
            else
            {
                lstProductos.Visibility = Visibility.Visible;
                lstProductos.ItemsSource = productsResponse.Items;
            }
        }

        private void product_selected(ProductService product)
        {
            lstProductos.Visibility = Visibility.Collapsed;
            txtProducto.Text = product.Id.ToString();
            if (product.MeasureUnitId == 3)
            {
                CloureManager.Navigate(typeof(SetUnitsByMeasurePage), product);
            }
            else
            {
                CartItem cartItem = new CartItem();
                double cant = 0;
                double.TryParse(txtProductoCantidad.Text, out cant);
                cartItem.ImagenPath = product.ImagePath;
                cartItem.Cantidad = cant;
                cartItem.ProductoId = product.Id;
                cartItem.Descripcion = product.Title;
                cartItem.Importe = product.Importe;
                cartItem.Total = product.Importe * cant;
                addProductToCarrito(cartItem);
            }
        }

        private void addProductToCarrito(CartItem cartItem)
        {
            carrito.Add(cartItem);
            lstCarrito.ItemsSource = null;
            lstCarrito.ItemsSource = carrito;
            txtProducto.Text = "";
            calcular_total();
        }

        private void lstClientes_ItemClick(object sender, ItemClickEventArgs e)
        {
            User user = (User)e.ClickedItem;
            LoadUserData(user);
        }

        private void txtProducto_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            
        }

        private void txtCliente_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtCliente.Text == "") lstClientes.Visibility = Visibility.Collapsed;
        }

        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.Navigate(typeof(UserAddPage));
        }

        private void lstProductos_ItemClick(object sender, ItemClickEventArgs e)
        {
            product_selected((ProductService)e.ClickedItem);
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            finalizar();
        }

        private async void finalizar()
        {
            if (btnAceptar.IsEnabled)
            {
                receipt.cartItems = carrito;
                receipt.CustomerId = CustomerId;
                receipt.CompanyBranchId = (int)txtSucursal.SelectedValue;
                receipt.TypeId = (int)txtComprobante.SelectedValue;

                if (receipt.cartItems.Count == 0)
                {
                    var dialog = new MessageDialog("Debes agregar items al carrito");
                    await dialog.ShowAsync();
                }
                else if (receipt.CustomerId == 0)
                {
                    var dialog = new MessageDialog("Debes agregar un cliente al comprobante");
                    await dialog.ShowAsync();
                }
                else
                {
                    CloureManager.Navigate(typeof(InvoicingFinishPage), receipt);
                }
            }

        }

        private void btnProductAdd_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.Navigate(typeof(ProductServiceAddPage));
        }

        private void txtCarritoCant_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void txtCarritoCant_PreviewKeyUp(object sender, KeyRoutedEventArgs e)
        {
            double cant = 0;
            TextBox txtCant = (TextBox)sender;
            CloureManager.NumberInput(txtCant);

            double.TryParse(txtCant.Text, out cant);
            CartItem cartItem = (CartItem)((FrameworkElement)e.OriginalSource).DataContext;
            double importe = cartItem.Importe * cant;
            cartItem.Cantidad = cant;
            cartItem.Total = importe;
            calcular_total();
        }

        private void calcular_importe_item(double cant, CartItem cartItem)
        {
            
            //lstCarrito.ItemsSource = null;
            //lstCarrito.ItemsSource = carrito;
            //var dialog = new MessageDialog(importe.ToString("F2"));
            //await dialog.ShowAsync();
        }

        private void btnDeleteCartItem_Click(object sender, RoutedEventArgs e)
        {
            CartItem cartItem = (CartItem)((FrameworkElement)e.OriginalSource).DataContext;
            carrito.Remove(cartItem);
            lstCarrito.ItemsSource = null;
            lstCarrito.ItemsSource = carrito;
            calcular_total();
        }

        private void calcular_total()
        {
            double total = 0;

            foreach (CartItem item in carrito)
            {
                total += item.Total;
            }
            receipt.Total = total;
            txtTotal.Text = "Total $ " + total.ToString("F2");
        }

        private void Page_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.F2)
            {
                finalizar();
            }
        }

        private void txtProductoCantidad_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void txtCarritoCant_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private void txtCliente_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Down)
            {
                if(lstClientes.Visibility == Visibility.Visible)
                {
                    if (lstClientes.SelectedIndex < lstClientes.Items.Count-1)
                    {
                        lstClientes.SelectedIndex++;
                    }
                    else
                    {
                        lstClientes.SelectedIndex = 0;
                    }
                    lstClientes.ScrollIntoView(lstClientes.Items[lstClientes.SelectedIndex]);
                }
            }
            if (e.Key == Windows.System.VirtualKey.Up)
            {
                if (lstClientes.Visibility == Visibility.Visible)
                {
                    if (lstClientes.SelectedIndex >0)
                    {
                        lstClientes.SelectedIndex--;
                    }
                    else
                    {
                        lstClientes.SelectedIndex = lstClientes.Items.Count-1;
                    }
                    lstClientes.ScrollIntoView(lstClientes.Items[lstClientes.SelectedIndex]);
                }
            }
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if(lstClientes.Visibility == Visibility.Collapsed)
                {
                    loadCustomer();
                }
                else
                {
                    //User user = (User)e.ClickedItem;
                    User user = (User)lstClientes.Items[lstClientes.SelectedIndex];
                    LoadUserData(user);
                }
            }
        }

        private void LoadUserData(User user)
        {
            txtCliente.Text = user.id.ToString();
            tbClienteRazonSocial.Text = user.apellido + ", " + user.nombre;
            tbClienteSaldo.Text = user.saldo_str;
            lstClientes.Visibility = Visibility.Collapsed;
            CustomerId = user.id;
            txtObservaciones.Focus(FocusState.Keyboard);
            receipt.Customer = user;
        }

        private void txtProducto_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Down)
            {
                if (lstProductos.Visibility == Visibility.Visible)
                {
                    if (lstProductos.SelectedIndex < lstProductos.Items.Count - 1)
                    {
                        lstProductos.SelectedIndex++;
                    }
                    else
                    {
                        lstProductos.SelectedIndex = 0;
                    }
                    lstProductos.ScrollIntoView(lstProductos.Items[lstProductos.SelectedIndex]);
                }
            }
            if (e.Key == Windows.System.VirtualKey.Up)
            {
                if (lstProductos.Visibility == Visibility.Visible)
                {
                    if (lstProductos.SelectedIndex > 0)
                    {
                        lstProductos.SelectedIndex--;
                    }
                    else
                    {
                        lstProductos.SelectedIndex = lstProductos.Items.Count - 1;
                    }
                    lstProductos.ScrollIntoView(lstProductos.Items[lstProductos.SelectedIndex]);
                }
            }
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (lstProductos.Visibility == Visibility.Collapsed)
                {
                    loadProduct();
                }
                else
                {
                    ProductService product = (ProductService)lstProductos.Items[lstProductos.SelectedIndex];
                    product_selected(product);
                }
            }
        }
    }
}
