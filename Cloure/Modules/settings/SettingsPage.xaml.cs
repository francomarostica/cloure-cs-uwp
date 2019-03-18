using Cloure.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace Cloure.Modules.settings
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        List<ModuleSettings> moduleSettings;

        public SettingsPage()
        {
            this.InitializeComponent();
            LoadSettings();
        }

        private async void LoadSettings()
        {
            stackModulesOptions.Children.Clear();
            moduleSettings = await Settings.GetList();

            foreach (ModuleSettings module in moduleSettings)
            {
                TextBlock txtModuleTitle = new TextBlock();
                txtModuleTitle.HorizontalAlignment = HorizontalAlignment.Stretch;
                txtModuleTitle.HorizontalTextAlignment = TextAlignment.Center;
                txtModuleTitle.Text = module.ModuleTitle;

                if (module.CloureSettings != null)
                {
                    StackPanel stackPrivileges = new StackPanel();

                    if (module.CloureSettings.Count > 0)
                    {
                        foreach (CloureSetting setting in module.CloureSettings)
                        {
                            Grid grid = new Grid();
                            TextBlock txtPrivilegeTitle = new TextBlock();
                            ColumnDefinition colTitle = new ColumnDefinition();
                            ColumnDefinition colControl = new ColumnDefinition();
                            colTitle.Width = new GridLength(1, GridUnitType.Star);
                            colControl.Width = new GridLength(200, GridUnitType.Pixel);
                            grid.ColumnDefinitions.Add(colTitle);
                            grid.ColumnDefinitions.Add(colControl);

                            txtPrivilegeTitle.Text = setting.Title;
                            Grid.SetColumn(txtPrivilegeTitle, 0);

                            grid.Children.Add(txtPrivilegeTitle);

                            if (setting.Type == "bool")
                            {
                                ToggleSwitch toggleSwitch = new ToggleSwitch();
                                toggleSwitch.Tag = setting;
                                toggleSwitch.IsOn = CloureManager.ParseBoolObject(setting.Value);
                                toggleSwitch.Toggled += ToggleSwitch_Toggled;
                                Grid.SetColumn(toggleSwitch, 1);
                                grid.Children.Add(toggleSwitch);
                            }

                            stackPrivileges.Children.Add(grid);
                        }
                        stackModulesOptions.Children.Add(txtModuleTitle);
                        stackModulesOptions.Children.Add(stackPrivileges);
                    }
                }
            }
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = (ToggleSwitch)sender;
            CloureSetting setting = (CloureSetting)toggleSwitch.Tag;
            setting.Value = toggleSwitch.IsOn;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private async void Save()
        {
            if(await Settings.Save(moduleSettings))
            {
                var dialog = new MessageDialog("Cambios guardados!");
                await dialog.ShowAsync();
            }
        }
    }
}
