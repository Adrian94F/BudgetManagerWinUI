using BudgetManager.Pages;
using Microsoft.UI;
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
using WinRT.Interop;

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
            SetUpTitleBar();
        }

        private void SetUpTitleBar()
        {
            var appWindow = this.GetAppWindowForCurrentWindow();
            appWindow.SetIcon("calculator.ico");
            appWindow.Title = "Menedżer Budżetu";
            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            appWindow.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            var handler = WindowNative.GetWindowHandle(this);
            var id = Win32Interop.GetWindowIdFromWindow(handler);
            return AppWindow.GetFromWindowId(id);
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
                    ContentFrame.Navigate(typeof(DebugPage));
                    break;

                case "table":
                    ContentFrame.Navigate(typeof(DebugPage));
                    break;

                case "exp_list":
                    ContentFrame.Navigate(typeof(DebugPage));
                    break;

                case "exp_burndown":
                    ContentFrame.Navigate(typeof(DebugPage));
                    break;

                case "history":
                    ContentFrame.Navigate(typeof(DebugPage));
                    break;

                case "months_list":
                    ContentFrame.Navigate(typeof(DebugPage));
                    break;

                case "categories":
                    ContentFrame.Navigate(typeof(DebugPage));
                    break;

                case "settings":
                    ContentFrame.Navigate(typeof(SettingsPage));
                    break;

                default:
                    ContentFrame.Navigate(typeof(DebugPage));
                    break;
            }

            var page = ContentFrame.Content as IPageWithInfo;

            NavView.Header = page.header != null ? page.header : null;
        }
    }
}
