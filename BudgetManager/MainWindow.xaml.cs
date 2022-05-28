using BudgetManager.Models;
using BudgetManager.Pages;
using BudgetManager.Utilities;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Composition;
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
            AppData.NavigationView = NavView;
            AppData.MainFrame = ContentFrame;

            Title = "Menedżer Budżetu";
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(TitleBar);

            var appWindow = GetAppWindowForCurrentWindow();
            appWindow.SetIcon("calculator.ico");
            appWindow.Title = Title;
        }

        private AppWindow GetAppWindowForCurrentWindow()
        {
            var handler = WindowNative.GetWindowHandle(this);
            var id = Win32Interop.GetWindowIdFromWindow(handler);
            return AppWindow.GetFromWindowId(id);
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            GoHome();
        }

        public void GoHome()
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

        private async void NavView_Navigate(NavigationViewItem item)
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
                    ContentFrame.Navigate(typeof(QuartersHistoryPage));
                    break;

                case "save":
                    Save();
                    break;

                case "prev_month":
                    PreviousMonth();
                    break;

                case "next_month":
                    NextMonth();
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

            DisableBackButton();

            var page = ContentFrame.Content as IPageWithInfo;
            NavView.Header = page.header;
            Logger.Log("changed page to: " + page.header);
        }

        private void PreviousMonth()
        {
            if (IsSelectedMonthPage() && AppData.SelectPreviousMonth())
            {
                ContentFrame.Navigate(
                    ContentFrame.Content.GetType(),
                    null,
                    new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
            }
        }

        private void NextMonth()
        {
            if (IsSelectedMonthPage() && AppData.SelectNextMonth())
            {
                ContentFrame.Navigate(
                    ContentFrame.Content.GetType(),
                    null,
                    new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
            }
        }

        private void MonthsList_MonthSelected(object sender, EventArgs args)
        {
            MainSplitView.IsPaneOpen = false;
            NavView_Navigate(NavView.SelectedItem as NavigationViewItem);
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            GoBack();
        }

        private void GoBack()
        {
            ContentFrame.GoBack(new DrillInNavigationTransitionInfo());
            DisableBackButton();
            NavView.Header = (ContentFrame.Content as IPageWithInfo).header;
            Logger.Log("changed page to: " + NavView.Header);
        }

        private void DisableBackButton()
        {
            NavView.IsBackEnabled = false;
        }

        private void Screenshot_Click(object sender, RoutedEventArgs e)
        {
            Screenshot();
        }

        private async void Screenshot()
        {
            var element = ContentFrame;
            var bg = element.Background;
            element.Background = ContentFrame.ActualTheme == ElementTheme.Light ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);

            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(element);
            element.Background = bg;

            var pixels = await renderTargetBitmap.GetPixelsAsync();
            var stream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            byte[] bytes = pixels.ToArray();
            encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                    BitmapAlphaMode.Ignore,
                                    (uint)element.ActualWidth, (uint)element.ActualHeight,
                                    96, 96, bytes);
            await encoder.FlushAsync();

            var dp = new DataPackage();
            dp.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
            Clipboard.SetContent(dp);
        }

        private async Task Save()
        {
            AppData.Save();
            var saveDialog = new ContentDialog()
            {
                Title = "Zapisano dane.",
                CloseButtonText = "Ok",
                XamlRoot = AppData.NavigationView.XamlRoot,
            };
            await saveDialog.ShowAsync();
        }

        private async void KeyboardAccelerator_Invoked(Microsoft.UI.Xaml.Input.KeyboardAccelerator sender, Microsoft.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            switch (args.KeyboardAccelerator)
            {
                case { Key: VirtualKey.S, Modifiers: VirtualKeyModifiers.Control }:
                    await Save();
                    break;
                case { Key: VirtualKey.Escape, Modifiers: VirtualKeyModifiers.None }:
                    if (NavView.IsBackEnabled)
                    {
                        GoBack();
                    }
                    break;
                case { Key: VirtualKey.PageUp, Modifiers: VirtualKeyModifiers.None }:
                    AppData.SelectPreviousMonth();
                    break;
            }
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                AppTitleTextBlock.Foreground =
                    (SolidColorBrush)App.Current.Resources["WindowCaptionForegroundDisabled"];
            }
            else
            {
                AppTitleTextBlock.Foreground =
                    (SolidColorBrush)App.Current.Resources["WindowCaptionForeground"];
            }
        }
    }
}
