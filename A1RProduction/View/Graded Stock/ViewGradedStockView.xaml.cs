using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Graded_Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace A1QSystem.View.Graded_Stock
{
    /// <summary>
    /// Interaction logic for ViewGradedStockView.xaml
    /// </summary>
    public partial class ViewGradedStockView : UserControl
    {
        public ViewGradedStockView(string UserName, string State, List<UserPrivilages> UserPrivilages, List<MetaData> md)
        {
            InitializeComponent();
            DataContext = new ViewGradedStockViewModel(UserName, State, UserPrivilages, md);
        }
    }
}
