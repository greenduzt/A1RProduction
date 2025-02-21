using A1QSystem.Model.Vehicles;
using A1QSystem.ViewModel.VehicleWorkOrders.History;
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

namespace A1QSystem.View.VehicleWorkOrders.History
{
    /// <summary>
    /// Interaction logic for VehicleWorkOrderHistoryItemsView.xaml
    /// </summary>
    public partial class VehicleWorkOrderHistoryItemsView : UserControl
    {
        public VehicleWorkOrderHistoryItemsView(VehicleWorkOrderHistory vwoh)
        {
            InitializeComponent();
            DataContext = new VehicleWorkOrderHistoryItemsViewModel(vwoh);
        }
    }
}
