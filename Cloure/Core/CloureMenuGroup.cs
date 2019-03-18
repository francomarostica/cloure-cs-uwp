using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Cloure.Core
{
    [TemplatePart(Name = ExpandableItemsList, Type = typeof(StackPanel))]
    public class CloureMenuGroup : Button
    {
        private const string ExpandableItemsList = "PART_listItems";
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.RegisterAttached("IsExpanded", typeof(bool), typeof(CloureMenuGroup), new PropertyMetadata(false));
        public static void SetIsExpanded(UIElement element, bool value)
        {
            element.SetValue(IsExpandedProperty, value);
        }
        public static bool GetIsExpanded(UIElement element)
        {
            return (bool)element.GetValue(IsExpandedProperty);
        }

        public bool isExpanded
        {
            get
            {
                return (bool)GetValue(IsExpandedProperty);
            }
            set
            {
                SetValue(IsExpandedProperty, value);
                var ExpandableList = GetTemplateChild(ExpandableItemsList) as StackPanel;
                if (ExpandableList != null)
                {
                    if (value)
                    {
                        ExpandableList.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ExpandableList.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public string Name = "";
        private string _Title = "";
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
                Content = value;
            }
        }
        public string IconSVG = "";
        public List<CloureMenuItem> cloureMenuItems = new List<CloureMenuItem>();

        public CloureMenuGroup(string Name, string Title, string IconSVG)
        {
            this.Name = Name;
            this.Title = Title;
            this.IconSVG = IconSVG;
            Foreground = new SolidColorBrush(Colors.White);
            HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        public List<CloureMenuItem> getItemList()
        {
            return cloureMenuItems;
        }

        protected override void OnApplyTemplate()
        {
            var ExpandableList = GetTemplateChild(ExpandableItemsList) as StackPanel;
            if (ExpandableList != null)
            {
                foreach(CloureMenuItem cloureMenuItem in cloureMenuItems)
                {
                    cloureMenuItem.HorizontalAlignment = HorizontalAlignment.Stretch;
                    cloureMenuItem.Style = (Style)Application.Current.Resources["CloureMenuItem"];
                    ExpandableList.Children.Add(cloureMenuItem);
                }
                ExpandableList.Visibility = Visibility.Collapsed;
            }
            base.OnApplyTemplate();
        }
    }
}
