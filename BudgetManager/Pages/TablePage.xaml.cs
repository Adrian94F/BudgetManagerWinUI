using BudgetManager.Models;
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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BudgetManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TablePage : Page, IPageWithInfo
    {
        public string header = "Tabela";

        private List<double> dateSums;
        private List<double> catSums;
        private List<List<double>> dateCatSums;

        string IPageWithInfo.header { get => header; set => header = value; }

        public TablePage()
        {
            this.InitializeComponent();
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

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => CountSums());
            ShowTableLabels();
            FillTableWithSumsAndlabels();
            LoadingControl.IsLoading = false;
        }

        private void ShowTableLabels()
        {
            TextBlock01.Visibility = Visibility.Visible;
            TextBlock10.Visibility = Visibility.Visible;
            TextBlock11.Visibility = Visibility.Visible;
        }

        private void CountSums()
        {
            var nOfColumns = (AppData.CurrentMonth.EndDate - AppData.CurrentMonth.StartDate).Days + 1;
            var nOfRows = AppData.categories.Count;
            
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
                var catIndex = AppData.categories.ToList().IndexOf(exp.Category);

                catSums[catIndex] += exp.Value2;
                dateSums[dateIndex] += exp.Value2;
                dateCatSums[catIndex][dateIndex] += exp.Value2;
            }
        }

        private void FillTableWithSumsAndlabels()
        {
            var columnWidth = 50;
            var padding = new Thickness(5, 1, 5, 1);
            var margin = new Thickness(0.5);
            var border = new Thickness(1);

            var date = AppData.CurrentMonth.StartDate;
            var colIdx = 0;
            foreach (var sum in dateSums)
            {
                DaysGrid.ColumnDefinitions.Add(new ColumnDefinition());
                var button = new Button()
                {
                    Content = date.ToString("d.MM"),
                    Width = columnWidth,
                    Margin = margin,
                    BorderThickness = border,
                    Padding = padding,
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };
                Grid.SetColumn(button, colIdx);
                DaysGrid.Children.Add(button);

                DaySumsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                button = new Button()
                {
                    Content = ((int)sum).ToString(),
                    Width = columnWidth,
                    Margin = margin,
                    BorderThickness = border,
                    Padding = padding,
                    HorizontalContentAlignment = HorizontalAlignment.Right
                };
                Grid.SetColumn(button, colIdx);
                DaySumsGrid.Children.Add(button);

                date = date.AddDays(1);
                colIdx++;
            }
            AddStretchColumn(DaysGrid);
            AddStretchColumn(DaySumsGrid);

            var rowIdx = 0;
            foreach (var cat in AppData.categories)
            {
                CategoriesGrid.RowDefinitions.Add(new RowDefinition());
                var button = new Button()
                {
                    Content = cat.Name,
                    Width = 300,
                    Margin = margin,
                    BorderThickness = border,
                    Padding = padding,
                    HorizontalContentAlignment = HorizontalAlignment.Left
                };
                Grid.SetRow(button, rowIdx++);
                CategoriesGrid.Children.Add(button);
            }
            AddStretchRow(CategoriesGrid);

            rowIdx = 0;
            foreach (var sum in catSums)
            {
                CategorySumsGrid.RowDefinitions.Add(new RowDefinition());
                var button = new Button()
                {
                    Content = ((int)sum).ToString(),
                    Width = columnWidth,
                    Margin = margin,
                    BorderThickness = border,
                    Padding = padding,
                    HorizontalContentAlignment = HorizontalAlignment.Right
                };
                Grid.SetRow(button, rowIdx++);
                CategorySumsGrid.Children.Add(button);
            }
            AddStretchRow(CategorySumsGrid);


            var catIdx = 0;
            while (catIdx < dateCatSums.Count)
            {
                ExpensesGrid.RowDefinitions.Add(new RowDefinition());

                var dateIdx = 0;
                while (dateIdx < dateCatSums[catIdx].Count)
                {
                    ExpensesGrid.ColumnDefinitions.Add(new ColumnDefinition());

                    var content = dateCatSums[catIdx][dateIdx] != 0
                        ? ((int)dateCatSums[catIdx][dateIdx]).ToString()
                        : " ";
                    var button = new Button() {
                        Content = content,
                        Width = columnWidth,
                        Margin = margin,
                        BorderThickness = border,
                        Padding = padding,
                        HorizontalContentAlignment = HorizontalAlignment.Right
                    };
                    Grid.SetColumn(button, dateIdx);
                    Grid.SetRow(button, catIdx);

                    ExpensesGrid.Children.Add(button);

                    dateIdx++;
                }

                catIdx++;
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
    }
}
