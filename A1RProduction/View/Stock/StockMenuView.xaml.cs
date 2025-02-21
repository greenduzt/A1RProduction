using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Stock;
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

namespace A1QSystem.View.Stock
{
    /// <summary>
    /// Interaction logic for StockMenuView.xaml
    /// </summary>
    public partial class StockMenuView : UserControl
    {
        public StockMenuView(string uName, string uState, List<UserPrivilages> uPriv, List<MetaData> md)
        {
            InitializeComponent();
            DataContext = new StockMenuViewModel(uName, uState, uPriv, md);
        }
    }
}
