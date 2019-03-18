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
    public sealed partial class UsersGroupsPage : Page
    {
        ModuleInfo moduleInfo;

        public UsersGroupsPage()
        {
            this.InitializeComponent();
            this.moduleInfo = CloureManager.GetModuleInfo();

            LoadData();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            /*
            if (Core.Core.accountType == "free" || Core.Core.accountType == "test_free")
            {
                Run run = new Run();
                run.Text = "No puedes agregar grupos de usuarios en esta versión de Cloure. ";

                Run runVerPlanes = new Run();
                runVerPlanes.Text = "Ver planes";
                Hyperlink hyperlinkVerPlanes = new Hyperlink();
                hyperlinkVerPlanes.Inlines.Add(runVerPlanes);
                hyperlinkVerPlanes.NavigateUri = new Uri("https://cloure.com/plans/?app_token=" + Core.Core.appToken);

                txtAdvice.Inlines.Add(run);
                txtAdvice.Inlines.Add(hyperlinkVerPlanes);

                btnAdd.IsEnabled = false;
            }
            */
        }

        public async void LoadData()
        {
            List<UserGroup> items = await UsersGroups.GetList();
            lstItems.ItemsSource = items;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.Navigate(typeof(UserGroupAddPage));
        }

        private void lstItems_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            itemContextMenu.Items.Clear();

            ListView list = (ListView)sender;
            UserGroup item = (UserGroup)((FrameworkElement)e.OriginalSource).DataContext;

            if (item.Type == "system")
            {
                CloureManager.ShowDialog("No puedes realizar acciones en grupos de sistema");
            }
            else
            {
                if (item.AvailableCommands.Count > 0)
                {
                    foreach (AvailableCommand availableCommand in item.AvailableCommands)
                    {
                        MenuFlyoutItem menuFlyoutItem = new MenuFlyoutItem();
                        menuFlyoutItem.Text = availableCommand.title;
                        menuFlyoutItem.Name = availableCommand.name;
                        menuFlyoutItem.Tag = item;
                        menuFlyoutItem.Click += MenuFlyoutItem_Click;
                        itemContextMenu.Items.Add(menuFlyoutItem);
                    }
                }

                if (itemContextMenu.Items.Count > 0)
                {
                    itemContextMenu.ShowAt(listView, e.GetPosition(listView));
                    var a = ((FrameworkElement)e.OriginalSource).DataContext;
                }
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = (MenuFlyoutItem)sender;
            UserGroup item = (UserGroup)menuFlyoutItem.Tag;
            if (menuFlyoutItem.Name == "edit") edit(item.Id);
            if (menuFlyoutItem.Name == "delete") DisplayDeleteDialog(item);
        }

        private void edit(string id)
        {
            CloureManager.Navigate(typeof(UserGroupAddPage), id);
        }

        private async void DisplayDeleteDialog(UserGroup item)
        {
            ContentDialog deleteFileDialog = new ContentDialog
            {
                Title = "¿Está seguro que desea eliminar este registro?",
                Content = "El registro se borrará de forma permanente",
                PrimaryButtonText = "Borrar",
                CloseButtonText = "Cancelar"
            };

            ContentDialogResult result = await deleteFileDialog.ShowAsync();

            // Delete the file if the user clicked the primary button.
            /// Otherwise, do nothing.
            if (result == ContentDialogResult.Primary)
            {
                bool api_result = await UsersGroups.Delete(item.Id);
                if (api_result) LoadData();
            }

        }

    }
}
