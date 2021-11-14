using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager.Pages
{
    public interface IPageWithInfo
    {
        public string header { get; set; }
    }
}
