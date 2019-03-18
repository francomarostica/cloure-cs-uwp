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

namespace Cloure.Modules.linked_accounts
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class LinkedAccountLink : Page
    {
        LinkedAccount linkedAccount = new LinkedAccount();

        public LinkedAccountLink()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                if(e.Parameter.GetType() == typeof(string))
                {
                    string value = (string)e.Parameter;
                    linkedAccount = await LinkedAccounts.Obtener(value);
                    foreach (LinkedAccountField item in linkedAccount.linkedAccountFields)
                    {
                        if (item.Tipo == "text")
                        {
                            TextBlock textBlock = new TextBlock();
                            textBlock.Margin = new Thickness(20, 5, 20, 5);
                            textBlock.Text = item.Titulo;
                            stackElems.Children.Add(textBlock);

                            TextBox textBox = new TextBox();
                            textBox.Margin = new Thickness(20, 5, 20, 5);
                            textBox.Tag = item.Nombre;
                            textBox.Text = item.Valor;
                            stackElems.Children.Add(textBox);
                        }
                        else if (item.Tipo == "bool")
                        {

                        }
                    }
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            foreach (LinkedAccountField campo in linkedAccount.linkedAccountFields)
            {
                foreach (UIElement element in stackElems.Children)
                {
                    if (element.GetType() == typeof(TextBox))
                    {
                        TextBox textBox = (TextBox)element;
                        if (((string)textBox.Tag) == campo.Nombre)
                        {
                            campo.Valor = textBox.Text;
                            break;
                        }
                    }
                }
            }

            var response = await LinkedAccounts.Save(linkedAccount);
            if (response.GetNamedString("Error") == "")
            {
                CloureManager.GoBack("reload");
            }
            else
            {
                CloureManager.ShowDialog(response.GetNamedString("Error"));
            }
        }
    }
}
