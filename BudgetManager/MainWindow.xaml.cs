using BudgetManager.Models;
using BudgetManager.Pages;
using BudgetManager.Utilities;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.System;
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
            SetUp();
        }

        private void SetUp()
        {
            AppData.navigationView = NavView;
            AppData.mainFrame = ContentFrame;

            var appWindow = GetAppWindowForCurrentWindow();
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
                if (item is NavigationViewItem && item.Tag.ToString().Equals("home"))
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
                NavView_Navigate(item);
            }
        }

        private bool IsSelectedMonthPage()
        {
            var item = NavView.SelectedItem as NavigationViewItem;
            string[] selectedMonthTags = { "home", "table", "exp_list", "exp_burndown", "exp_list_and_burndown", "month_details" };
            return selectedMonthTags.Contains(item.Tag);
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

                case "exp_list_and_burndown":
                    ContentFrame.Navigate(typeof(ExpListAndBurndownPage));
                    break;

                case "month_details":
                    ContentFrame.Navigate(typeof(MonthDetailsPage));
                    break;

                case "history":
                    ContentFrame.Navigate(typeof(MonthsHistoryPage));
                    break;

                case "history_quarters":
                    ContentFrame.Navigate(typeof(MonthsHistoryPage));
                    break;

                case "prev_month":
                    if (IsSelectedMonthPage())
                    {
                        AppData.SelectPreviousMonth();
                        ContentFrame.Navigate(ContentFrame.Content.GetType(),
                                              null,
                                              new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
                    }
                    break;

                case "next_month":
                    if (IsSelectedMonthPage())
                    {
                        AppData.SelectNextMonth();
                        ContentFrame.Navigate(ContentFrame.Content.GetType(),
                                              null,
                                              new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                    }
                    break;

                case "months_list":
                    SideFrame.Navigate(typeof(MonthsListPage), null, new SuppressNavigationTransitionInfo());
                    var monthsList = SideFrame.Content as MonthsListPage;
                    monthsList.MonthSelected += new EventHandler(MonthsList_MonthSelected);
                    MainSplitView.OpenPaneLength = 200;
                    MainSplitView.IsPaneOpen = true;
                    break;

                case "settings":
                    ContentFrame.Navigate(typeof(SettingsPage));
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

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            ContentFrame.GoBack(new DrillInNavigationTransitionInfo());
            NavView.IsPaneToggleButtonVisible = true;
            NavView.IsBackEnabled = false;
            NavView.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;
            NavView.Header = (ContentFrame.Content as IPageWithInfo).header;
            Logger.Log("changed page to: " + NavView.Header);
        }

        private void Screenshot_Click(object sender, RoutedEventArgs e)
        {
            Screenshot();
        }

        private void Screenshot()
        {

        }

        private async void KeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            var closeDialog = new ContentDialog()
            {
                Title = "Zamykanie aplikacji",
                Content = "Czy chcesz zapisać wprowadzone zmiany? W przypadku braku zapisu zostaną one utracone.",
                PrimaryButtonText = "Tak",
                SecondaryButtonText = "Nie",
                CloseButtonText = "Anuluj",
                XamlRoot = AppData.navigationView.XamlRoot,
            };
            var saveDialog = new ContentDialog()
            {
                Title = "Zapisano dane.",
                CloseButtonText = "Ok",
                XamlRoot = AppData.navigationView.XamlRoot,
            };
            switch (args.KeyboardAccelerator)
            {
                case { Key: VirtualKey.S, Modifiers: VirtualKeyModifiers.Control }:
                    AppData.Save();
                    await saveDialog.ShowAsync();
                    break;
                case { Key: VirtualKey.Print, Modifiers: VirtualKeyModifiers.None }:
                    Screenshot();
                    break;
                case { Key: VirtualKey.Escape, Modifiers: VirtualKeyModifiers.None }:
                    
                    ContentDialogResult result = await closeDialog.ShowAsync();
                    switch (result)
                    {
                        case ContentDialogResult.Primary:
                            AppData.Save();
                            Close();
                            break;
                        case ContentDialogResult.Secondary:
                            Close();
                            break;
                        default:
                            break;
                    }
                    break;
            }
        }
    }
}
