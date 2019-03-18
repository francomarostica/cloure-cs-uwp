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

namespace Cloure.Modules.company_branches
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class CompanyBranchAddPage : Page
    {
        CompanyBranch companyBranch;
        ModuleInfo moduleInfo;

        public CompanyBranchAddPage()
        {
            this.InitializeComponent();

            moduleInfo = CloureManager.GetModuleInfo();

            txtNombrePrompt.Text = moduleInfo.locales.GetNamedString("name");
            txtDireccionPrompt.Text = moduleInfo.locales.GetNamedString("address");
            txtTelefonoPrompt.Text = moduleInfo.locales.GetNamedString("phone");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                if (e.Parameter.GetType() == typeof(int))
                {
                    int id = (int)e.Parameter;
                    LoadData(id);
                }
            }
            else
            {
                companyBranch = new CompanyBranch();
                companyBranch.Id = 0;
            }
        }

        private async void LoadData(int branch_id)
        {
            companyBranch = await CompanyBranches.Get(branch_id);
            txtNombre.Text = companyBranch.Name;
            txtDireccion.Text = companyBranch.Address;
            txtTelefono.Text = companyBranch.Phone;
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
            companyBranch.Name = txtNombre.Text;
            companyBranch.Address = txtDireccion.Text;
            companyBranch.Phone = txtTelefono.Text;

            int res = await CompanyBranches.save(companyBranch);
            if (res > 0)
            {
                CloureManager.GoBack();
            }
        }
    }
}
