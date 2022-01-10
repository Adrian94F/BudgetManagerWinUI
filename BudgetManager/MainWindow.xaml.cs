using BudgetManager.Pages;
using BudgetManager.Utilities;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
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
        private NavigationViewItem previousItem;

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
            appWindow.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
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
                    NavView.SelectedItem = previousItem = item as NavigationViewItem;
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
                NavView_Navigate(item);

                if (sender.FooterMenuItems.Contains(item))
                {
                    NavView.SelectedItem = previousItem;
                }
                else
                {
                    previousItem = NavView.SelectedItem as NavigationViewItem;
                }
            }
        }

        private void NavView_Navigate(NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "home":
                    ContentFrame.Navigate(typeof(SummaryPage));
                    break;

                case "table":
                    ContentFrame.Navigate(typeof(TablePage));
                    break;

                case "exp_list":
                    ContentFrame.Navigate(typeof(ExpListPage));
                    break;

                case "exp_burndown":
                    ContentFrame.Navigate(typeof(ExpBurndownPage));
                    break;

                case "month_details":
                    ContentFrame.Navigate(typeof(MonthDetailsPage));
                    break;

                case "history":
                    ContentFrame.Navigate(typeof(MonthsHistoryPage));
                    break;

                case "months_list":
                    SideFrame.Navigate(typeof(MonthsListPage), null, new SuppressNavigationTransitionInfo());
                    var monthsList = SideFrame.Content as MonthsListPage;
                    monthsList.MonthSelected += new EventHandler(MonthsList_MonthSelected);
                    MainSplitView.OpenPaneLength = 200;
                    MainSplitView.IsPaneOpen = true;
                    break;

                case "settings":
                    SideFrame.Navigate(typeof(SettingsPage), null, new SuppressNavigationTransitionInfo());
                    MainSplitView.OpenPaneLength = 600;
                    MainSplitView.IsPaneOpen = true;
                    break;

                default:
                    SideFrame.Navigate(typeof(DebugPage), null, new SuppressNavigationTransitionInfo());
                    MainSplitView.OpenPaneLength = 500;
                    MainSplitView.IsPaneOpen = true;
                    break;
            }

            var page = ContentFrame.Content as IPageWithInfo;
            NavView.Header = page.header;
            Logger.Log("changed page to: " + page.header);
        }

        private void MonthsList_MonthSelected(object sender, EventArgs args)
        {
            MainSplitView.IsPaneOpen = false;
            NavView_Navigate(NavView.SelectedItem as NavigationViewItem);
        }
    }
}
