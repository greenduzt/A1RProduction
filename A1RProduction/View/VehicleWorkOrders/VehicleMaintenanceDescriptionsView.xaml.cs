using A1QSystem.ViewModel.VehicleWorkOrders;
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

namespace A1QSystem.View.VehicleWorkOrders
{
    /// <summary>
    /// Interaction logic for VehicleWorkDescriptionsView.xaml
    /// </summary>
    public partial class VehicleMaintenanceDescriptionsView : UserControl
    {
        public VehicleMaintenanceDescriptionsView(int loc,int vt,string un)
        {
            InitializeComponent();
            DataContext = new VehicleMaintenanceDescriptionsViewModel(loc,vt,un);
        }
    }
}
