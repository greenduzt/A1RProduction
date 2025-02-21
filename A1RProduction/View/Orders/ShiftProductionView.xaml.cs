using A1QSystem.Model.Orders;
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
    /// Interaction logic for ShiftOrderView.xaml
    /// </summary>
    public partial class ShiftProductionView : UserControl
    {
        public ShiftProductionView(RawProductionDetails rawProductionDetails,string type)
        {
            InitializeComponent();
            DataContext = new ShiftProductionViewModel(rawProductionDetails, type);
        }
    }
}
