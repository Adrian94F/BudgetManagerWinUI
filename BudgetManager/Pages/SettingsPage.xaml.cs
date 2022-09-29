using BudgetManager.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;

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
            FillWithCategories();
            SetTheme();
        }

        private void SetTheme()
        {
            ThemeToggleSwitch.IsOn = this.ActualTheme == ElementTheme.Light;
        }


        private void FillWithCategories()
        {
            var categoriesCollection = new ObservableCollection<Category>(AppData.Categories);
            CategoriesDataGrid.ItemsSource = categoriesCollection;
        }

        private async void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as Category;

            if (AppData.IsCategoryRemovable(item))
            {
                AppData.Categories.Remove(item);
                (CategoriesDataGrid.ItemsSource as ObservableCollection<Category>).Remove(item);
            }
            else
            {
                var contentDialog = new ContentDialog()
                {
                    Title = "Operacja niedozwolona",
                    Content = "Nie można usunąć kategorii, która jest w użytku. Usuń operacje przypisane do tej kategorii lub zmień ich kategorię we wszystkich miesiącach, a następnie spróbuj ponownie.",
                    CloseButtonText = "Ok",
                    XamlRoot = AppData.NavigationView.XamlRoot,
                };
                await contentDialog.ShowAsync();
            }
        }

        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var cat = new Category()
            {
                Name = "nowa kategoria"
            };
            AppData.Categories.Add(cat);
            FillWithCategories();
        }

        private void ThemeToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleSwitch;
            (AppData.NavigationView.XamlRoot.Content as Grid).RequestedTheme = toggle.IsOn ? ElementTheme.Light : ElementTheme.Dark;
        }
    }
}
