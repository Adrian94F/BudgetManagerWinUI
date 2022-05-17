using BudgetManager.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BudgetManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TablePage : Page, IPageWithInfo
    {
        public string header = "Tabela wydatków";

        private static List<double> dateSums;
        private static List<double> catSums;
        private static List<List<double>> dateCatSums;

        private double columnWidth = 50;
        private double rowHeight = 24;
        private Thickness padding = new Thickness(5, 1, 5, 1);
        private Thickness margin = new Thickness(1);
        private Thickness border = new Thickness(1);
        private CornerRadius cornerRadius = new CornerRadius(0);

        string IPageWithInfo.header { get => header; set => header = value; }

        public TablePage()
        {
            this.InitializeComponent();

            if (AppData.CurrentMonth != null)
            {
                var month = AppData.CurrentMonth;
                var startDate = month.StartDate.ToString("dd.MM.yyyy");
                var endDate = month.EndDate.ToString("dd.MM.yyyy");
                header += " " + startDate + "-" + endDate;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppData.CurrentMonth != null)
            {
                await Task.Run(() => CountSums());
                ShowTableLabels();
                FillTableWithSumsAndlabels();
                LoadingControl.IsLoading = false;
            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ScrollToToday();
        }

        private void ScrollToToday()
        {
            if (AppData.CurrentMonth == null || DateTime.Now.CompareTo(AppData.CurrentMonth.EndDate.AddDays(1)) > 0)
            {
                return;
            }

            var lastVisibleColumnPosition = columnWidth * ((DateTime.Today - AppData.CurrentMonth.StartDate).Days + 4);
            var scrollViewerWidth = ExpensesScrollViewer.ActualWidth;
            var remainder = (scrollViewerWidth - 1) % columnWidth + 1;
            var rightTodayMargin = columnWidth + remainder;
            var offset = lastVisibleColumnPosition - scrollViewerWidth + rightTodayMargin;
            ExpensesScrollViewer.ScrollToHorizontalOffset(offset);
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var scrolledObject = sender as ScrollViewer;

            if (scrolledObject != null)
            {
                if (scrolledObject == ExpensesScrollViewer)
                {
                    DaysScrollViewer.ScrollToHorizontalOffset(scrolledObject.HorizontalOffset);
                    DaySumsScrollViewer.ScrollToHorizontalOffset(scrolledObject.HorizontalOffset);
                    CategoriesScrollViewer.ScrollToVerticalOffset(scrolledObject.VerticalOffset);
                    CategorySumsScrollViewer.ScrollToVerticalOffset(scrolledObject.VerticalOffset);
                }
                else if (scrolledObject == DaysScrollViewer || scrolledObject == DaySumsScrollViewer)
                {
                    ExpensesScrollViewer.ScrollToHorizontalOffset(scrolledObject.HorizontalOffset);
                }
                else if (scrolledObject == CategoriesScrollViewer || scrolledObject == CategorySumsScrollViewer)
                {
                    ExpensesScrollViewer.ScrollToVerticalOffset(scrolledObject.VerticalOffset);
                }
            }
        }

        private void ShowTableLabels()
        {
            TextBlock01.Visibility = Visibility.Visible;
            TextBlock10.Visibility = Visibility.Visible;
            TextBlock11.Visibility = Visibility.Visible;
        }

        private static void CountSums()
        {
            var nOfColumns = (AppData.CurrentMonth.EndDate - AppData.CurrentMonth.StartDate).Days + 1;
            var nOfRows = AppData.Categories.Count;

            dateSums = Enumerable.Repeat(0d, nOfColumns).ToList();
            catSums = Enumerable.Repeat(0d, nOfRows).ToList();

            dateCatSums = new List<List<double>>();
            while (nOfRows-- > 0)
            {
                var emptyRow = Enumerable.Repeat(0d, nOfColumns).ToList();
                dateCatSums.Add(emptyRow);
            }

            foreach (var exp in AppData.CurrentMonth.Expenses)
            {
                var dateIndex = (exp.Date - AppData.CurrentMonth.StartDate).Days;
                dateIndex = dateIndex < 0 ? 0 : dateIndex;
                var catIndex = AppData.Categories.ToList().IndexOf(exp.Category);

                catSums[catIndex] += exp.Value2;
                dateSums[dateIndex] += exp.Value2;
                dateCatSums[catIndex][dateIndex] += exp.Value2;
            }
        }

        private void NavigateToExpListPageWithParam(object par)
        {
            AppData.NavigationView.IsBackEnabled = true;
            AppData.NavigationView.IsBackButtonVisible = NavigationViewBackButtonVisible.Visible;
            AppData.NavigationView.IsPaneToggleButtonVisible = false;
            AppData.MainFrame.Navigate(typeof(ExpListPage), par, new DrillInNavigationTransitionInfo());
            AppData.NavigationView.Header = (AppData.MainFrame.Content as IPageWithInfo).header;
        }

        private void FillTableWithSumsAndlabels()
        {
            AddWeekendsRectangles(DaysGrid, AppData.CurrentMonth);
            AddWeekendsRectangles(DaySumsGrid, AppData.CurrentMonth);
            AddTodayRectangle(DaysGrid, AppData.CurrentMonth);
            AddTodayRectangle(DaySumsGrid, AppData.CurrentMonth);

            var date = AppData.CurrentMonth.StartDate;
            var colIdx = 0;
            foreach (var sum in dateSums)
            {
                var sumDate = new DateTime(date.Ticks);
                DaysGrid.ColumnDefinitions.Add(new ColumnDefinition());
                var button = new Button()
                {
                    Content = sumDate.ToString("d.MM"),
                    Width = columnWidth,
                    Margin = margin,
                    BorderThickness = border,
                    CornerRadius = cornerRadius,
                    Padding = padding,
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };
                button.Click += (sender, e) =>
                {
                    NavigateToExpListPageWithParam(sumDate);
                };
                Grid.SetColumn(button, colIdx);
                DaysGrid.Children.Add(button);

                DaySumsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                button = new Button()
                {
                    Content = sum == 0 ? " " : ((int)sum).ToString(),
                    Width = columnWidth,
                    Margin = margin,
                    BorderThickness = border,
                    CornerRadius = cornerRadius,
                    Padding = padding,
                    HorizontalContentAlignment = HorizontalAlignment.Right
                };
                button.Click += (sender, e) =>
                {
                    NavigateToExpListPageWithParam(sumDate);
                };
                Grid.SetColumn(button, colIdx);
                DaySumsGrid.Children.Add(button);

                date = date.AddDays(1);
                colIdx++;
            }
            AddStretchColumn(DaysGrid);
            AddStretchColumn(DaySumsGrid);

            var rowIdx = 0;
            foreach (var cat in AppData.Categories)
            {
                var row = rowIdx++;
                CategoriesGrid.RowDefinitions.Add(new RowDefinition());
                var button = new Button()
                {
                    Content = cat.Name,
                    Width = 300,
                    Margin = margin,
                    BorderThickness = border,
                    CornerRadius = cornerRadius,
                    Padding = padding,
                    HorizontalContentAlignment = HorizontalAlignment.Left
                };
                button.Click += (sender, e) =>
                {
                    NavigateToExpListPageWithParam(cat);
                };
                Grid.SetRow(button, row);
                CategoriesGrid.Children.Add(button);
            }
            AddStretchRow(CategoriesGrid);

            rowIdx = 0;
            foreach (var sum in catSums)
            {
                var row = rowIdx++;
                CategorySumsGrid.RowDefinitions.Add(new RowDefinition());
                var button = new Button()
                {
                    Content = sum == 0 ? " " : ((int)sum).ToString(),
                    Width = columnWidth,
                    Margin = margin,
                    BorderThickness = border,
                    CornerRadius = cornerRadius,
                    Padding = padding,
                    HorizontalContentAlignment = HorizontalAlignment.Right
                };
                button.Click += (sender, e) =>
                {
                    NavigateToExpListPageWithParam(AppData.Categories.ToArray()[row]);
                };
                Grid.SetRow(button, row);
                CategorySumsGrid.Children.Add(button);
            }
            AddStretchRow(CategorySumsGrid);

            foreach (var row in dateCatSums)
            {
                ExpensesGrid.RowDefinitions.Add(new RowDefinition());
            }
            AddWeekendsRectangles(ExpensesGrid, AppData.CurrentMonth);
            AddTodayRectangle(ExpensesGrid, AppData.CurrentMonth);

            var catIdx = 0;
            while (catIdx < dateCatSums.Count)
            {
                var row = catIdx++;
                var dateIdx = 0;
                while (dateIdx < dateCatSums[row].Count)
                {
                    var column = dateIdx++;
                    ExpensesGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    var content = dateCatSums[row][column] != 0
                        ? ((int)dateCatSums[row][column]).ToString()
                        : " ";
                    var button = new Button()
                    {
                        Content = content,
                        Width = columnWidth,
                        Margin = margin,
                        BorderThickness = border,
                        CornerRadius = cornerRadius,
                        Padding = padding,
                        HorizontalContentAlignment = HorizontalAlignment.Right
                    };
                    button.Click += (sender, e) =>
                    {
                        var sumDate = AppData.CurrentMonth.StartDate.AddDays(column);
                        var category = AppData.Categories.ToArray().GetValue(row) as Category;
                        var param = new Tuple<DateTime, Category>(sumDate, category);
                        NavigateToExpListPageWithParam(param);
                    };
                    Grid.SetColumn(button, column);
                    Grid.SetRow(button, row);

                    ExpensesGrid.Children.Add(button);
                }
            }

            AddStretchColumn(ExpensesGrid);
            AddStretchRow(ExpensesGrid);
        }

        private void AddStretchRow(Grid grid)
        {
            var rowDef = new RowDefinition
            {
                Height = new GridLength(100, GridUnitType.Star),
                MinHeight = 16
            };
            grid.RowDefinitions.Add(rowDef);
        }

        private void AddStretchColumn(Grid grid)
        {
            var colDef = new ColumnDefinition
            {
                Width = new GridLength(100, GridUnitType.Star),
                MinWidth = 16,
            };
            grid.ColumnDefinitions.Add(colDef);
        }

        private static Microsoft.UI.Xaml.Shapes.Rectangle AddRectangleAt(int row, int col, int rowSpan, int colSpan, SolidColorBrush fill, Grid grid)
        {
            if (row < 0 || col < 0)
            {
                return null;
            }
            var rect = new Microsoft.UI.Xaml.Shapes.Rectangle()
            {
                Fill = fill,
            };

            if (rowSpan < 1)
            {
                rowSpan = 1;
            }
            if (colSpan < 1)
            {
                colSpan = 1;
            }
            Grid.SetRow(rect, row);
            Grid.SetRowSpan(rect, rowSpan);
            Grid.SetColumn(rect, col);
            Grid.SetColumnSpan(rect, colSpan);

            grid.Children.Add(rect);

            return rect;
        }

        private static void AddWeekendsRectangles(Grid grid, Month month)
        {
            var row = 0;
            var rowSpan = grid.RowDefinitions.Count;
            var col = 0;
            var colSpan = 1;
            var fill = new SolidColorBrush(new Windows.UI.Color()
            {
                A = 50,
                R = 128,
                G = 128,
                B = 128,
            });

            var day = month.StartDate;
            while (day.Date <= month.EndDate.Date)
            {
                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                {
                    AddRectangleAt(row, col, rowSpan, colSpan, fill, grid);
                }
                day = day.AddDays(1);
                col++;
            }
        }

        private static void AddTodayRectangle(Grid grid, Month month)
        {
            if (DateTime.Today.Date > month.EndDate.Date)
            {
                return;
            }
            var row = 0;
            var rowSpan = grid.RowDefinitions.Count;
            var col = (DateTime.Now - month.StartDate).Days;
            var colSpan = 1;
            var fill = new SolidColorBrush(new Windows.UI.Color()
            {
                A = 100,
                R = 144,
                G = 238,
                B = 144,
            });
            AddRectangleAt(row, col, rowSpan, colSpan, fill, grid);
        }

    }
}
