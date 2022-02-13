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
using System.Collections.ObjectModel;
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
    public sealed partial class SettingsPage : Page, IPageWithInfo
    {
        public string header = "Ustawienia";

        string IPageWithInfo.header { get => header; set => header = value; }

        public SettingsPage()
        {
            this.InitializeComponent();
            PathTextBox.Text = AppSettings.dataPath;
            FillWithCategories();
        }

        private void FillWithCategories()
        {
            var categoriesCollection = new ObservableCollection<Category>(AppData.categories);
            CategoriesDataGrid.ItemsSource = categoriesCollection;
        }

        private async void ChangePathButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as Category;

            if (AppData.IsCategoryRemovable(item))
            {
                AppData.categories.Remove(item);
                (CategoriesDataGrid.ItemsSource as ObservableCollection<Category>).Remove(item);
            }
            else
            {
                var flyout = new Flyout()
                {
                    Content = new TextBlock()
                    {
                        Text = "Nie można usunąć kategorii, która jest w użytku."
                    }
                };
                flyout.ShowAt(this);
            }
        }

        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var cat = new Category()
            {
                Name = "nowa kategoria"
            };
            AppData.categories.Add(cat);
            FillWithCategories();
        }
    }
}
