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
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.users_groups
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class UserGroupAddPage : Page
    {
        List<ModulePrivileges> modulePrivileges;
        private UserGroup userGroup;

        public UserGroupAddPage()
        {
            this.InitializeComponent();
            tgAdminGroup.IsOn = false;
            stackModulesPrivileges.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string grupo_id = "";

            base.OnNavigatedTo(e);
            if (e.Parameter == null)
            {
                userGroup = new UserGroup();
            }
            else
            {
                if(e.Parameter.GetType() == typeof(string))
                {
                    grupo_id = (string)e.Parameter;
                    LoadData(grupo_id);
                }
            }

            if (CloureManager.getAccountType() == "free" || CloureManager.getAccountType() == "test_free")
            {
                Run run = new Run();
                run.Text = "Para poder usar esta característica debes contar con una ";

                Run runVerPlanes = new Run();
                runVerPlanes.Text = "cuenta premium";
                Hyperlink hyperlinkVerPlanes = new Hyperlink();
                hyperlinkVerPlanes.Inlines.Add(runVerPlanes);
                //hyperlinkVerPlanes.NavigateUri = new Uri("https://cloure.com/plans/?app_token=" + Core.Core.appToken);

                txtAdvice.Inlines.Add(run);
                txtAdvice.Inlines.Add(hyperlinkVerPlanes);

                tgAdminGroup.IsOn = false;
                tgAdminGroup.IsEnabled = false;
                stackModulesPrivileges.Visibility = Visibility.Collapsed;
            }
            else
            {
                GetPrivileges(grupo_id);
            }
        }

        private async void LoadData(string id)
        {
            userGroup = await UsersGroups.Get(id);
            txtNombre.Text = userGroup.Name;
            tgAdminGroup.IsOn = userGroup.IsStaff;
        }

        private void btnAceptar_Click(object sender, RoutedEventArgs e)
        {
            Guardar();
        }

        private async void Guardar()
        {
            userGroup.Name = txtNombre.Text;
            userGroup.Id = txtNombre.Text;
            userGroup.IsStaff = tgAdminGroup.IsOn;
            userGroup.ModulePrivileges = modulePrivileges;

            if(await UsersGroups.save(userGroup))
            {
                CloureManager.GoBack();
            }
        }

        private async void GetPrivileges(string grupo_id="")
        {
            stackModulesPrivileges.Children.Clear();
            modulePrivileges = await UsersGroups.GetPrivileges(grupo_id);

            foreach (ModulePrivileges module in modulePrivileges)
            {
                TextBlock txtModuleTitle = new TextBlock();
                txtModuleTitle.HorizontalAlignment = HorizontalAlignment.Stretch;
                txtModuleTitle.HorizontalTextAlignment = TextAlignment.Center;
                txtModuleTitle.Text = module.ModuleTitle;

                if (module.ClourePrivileges != null)
                {
                    StackPanel stackPrivileges = new StackPanel();

                    if (module.ClourePrivileges.Count > 0)
                    {
                        foreach (ClourePrivilege privilege in module.ClourePrivileges)
                        {
                            Grid grid = new Grid();
                            TextBlock txtPrivilegeTitle = new TextBlock();
                            ColumnDefinition colTitle = new ColumnDefinition();
                            ColumnDefinition colControl = new ColumnDefinition();
                            colTitle.Width = new GridLength(1, GridUnitType.Star);
                            colControl.Width = new GridLength(200, GridUnitType.Pixel);
                            grid.ColumnDefinitions.Add(colTitle);
                            grid.ColumnDefinitions.Add(colControl);

                            txtPrivilegeTitle.Text = privilege.Title;
                            Grid.SetColumn(txtPrivilegeTitle, 0);

                            grid.Children.Add(txtPrivilegeTitle);

                            if (privilege.Type == "bool")
                            {
                                ToggleSwitch toggleSwitch = new ToggleSwitch();
                                toggleSwitch.Tag = privilege;
                                toggleSwitch.IsOn = CloureManager.ParseBoolObject(privilege.Value);
                                toggleSwitch.Toggled += ToggleSwitch_Toggled;
                                Grid.SetColumn(toggleSwitch, 1);
                                grid.Children.Add(toggleSwitch);
                            }

                            stackPrivileges.Children.Add(grid);
                        }
                        stackModulesPrivileges.Children.Add(txtModuleTitle);
                        stackModulesPrivileges.Children.Add(stackPrivileges);
                    }
                }
            }
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender;
            ClourePrivilege privilege = (ClourePrivilege)toggleSwitch.Tag;
            privilege.Value = toggleSwitch.IsOn;
        }

        private void AppBarToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

        }

        private void tgAdminGroup_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender;
            if (toggleSwitch.IsOn)
                stackModulesPrivileges.Visibility = Visibility.Visible;
            else
                stackModulesPrivileges.Visibility = Visibility.Collapsed;
        }
    }
}
