using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BudgetManager.Controls
{
    public sealed partial class ExpenseListViewItem : UserControl
    {
        public Expense OriginalExpense { get; set; }

        public ExpenseListViewItem()
        {
            this.InitializeComponent();
        }
    }
}
