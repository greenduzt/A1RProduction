using A1QSystem.ViewModel.Orders;
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

namespace A1QSystem.View.Orders
{
    /// <summary>
    /// Interaction logic for OrderResultView.xaml
    /// </summary>
    public partial class OrderResultView : UserControl
    {
        public OrderResultView(List<Tuple<DateTime,string, int, string>> data)
        {
            InitializeComponent();
            DataContext = new OrderResultViewModel(data);
        }
    }
}
