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
            FillTable();
        }

        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            
        }

        private void FillTable()
        {
            CountSums();
            FillTableWithSums();
        }

        private void CountSums()
        {
            var nOfColumns = (AppData.CurrentMonth.EndDate - AppData.CurrentMonth.StartDate).Days;
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

        private void FillTableWithSums()
        {
            var columnWidth = 40;
            var padding = new Thickness(1);
            var border = new Thickness(0);

            var date = AppData.CurrentMonth.StartDate;
            foreach (var sum in dateSums)
            {
                DaysGrid.Children.Add(new Button()
                {
                    Content = date.ToString("d.MM"),
                    Width = columnWidth,
                    Background = null,
                    BorderThickness = border,
                    Padding = padding,
                });

                DaySumsGrid.Children.Add(new Button()
                {
                    Content = ((int)sum).ToString(),
                    Width = columnWidth,
                    Background = null,
                    BorderThickness = border,
                    Padding = padding
                });

                date = date.AddDays(1);
            }

            foreach (var cat in AppData.categories)
            {
                CategoriesGrid.Children.Add(new Button()
                {
                    Content = cat.Name,
                    Background = null,
                    BorderThickness = border,
                    Padding = padding,
                });
            }

            foreach (var sum in catSums)
            {
                CategorySumsGrid.Children.Add(new Button()
                {
                    Content = ((int)sum).ToString(),
                    Width = columnWidth,
                    Background = null,
                    BorderThickness = border,
                    Padding = padding,
                });
            }

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
                        : "";
                    var button = new Button() {
                        Content = content,
                        Width = columnWidth,
                        Background = null,
                        BorderThickness = border,
                        Padding = padding,
                    };
                    Grid.SetColumn(button, dateIdx);
                    Grid.SetRow(button, catIdx);

                    ExpensesGrid.Children.Add(button);

                    dateIdx++;
                }

                catIdx++;
            }
        }
    }
}
