using BudgetManager.Pages;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BudgetManager
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // set the initial SelectedItem 
            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "home")
                {
                    NavView.SelectedItem = item;
                    NavView_Navigate(item as NavigationViewItem);
                    break;
                }
            }
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (!args.IsSettingsInvoked)
            {
                // find NavigationViewItem with Content that equals InvokedItem
                var items = sender.MenuItems.Concat(sender.FooterMenuItems);
                var item = items.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
                NavView_Navigate(item as NavigationViewItem);
            }
        }

        private void NavView_Navigate(NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "home":
                    ContentFrame.Navigate(typeof(BlankPage));
                    break;

                case "table":
                    ContentFrame.Navigate(typeof(BlankPage));
                    break;

                case "exp_list":
                    ContentFrame.Navigate(typeof(BlankPage));
                    break;

                case "exp_burndown":
                    ContentFrame.Navigate(typeof(BlankPage));
                    break;

                case "history":
                    ContentFrame.Navigate(typeof(BlankPage));
                    break;

                case "months_list":
                    ContentFrame.Navigate(typeof(BlankPage));
                    break;

                case "categories":
                    ContentFrame.Navigate(typeof(BlankPage));
                    break;

                case "settings":
                    ContentFrame.Navigate(typeof(BlankPage));
                    break;
            }
        }
    }
}
