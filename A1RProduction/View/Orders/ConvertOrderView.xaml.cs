using A1QSystem.Model.RawMaterials;
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
    /// Interaction logic for ConvertOrderView.xaml
    /// </summary>
    public partial class ConvertOrderView : UserControl
    {
        public ConvertOrderView(RawProductionDetails rawProductionDetails)
        {
            InitializeComponent();
            DataContext = new ConvertOrderViewModel(rawProductionDetails);
        }
    }
}
