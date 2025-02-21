using A1QSystem.Model.Vehicles;
using A1QSystem.ViewModel.WorkOrders;
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

namespace A1QSystem.View.WorkOrders
{
    /// <summary>
    /// Interaction logic for ViewVehicleWorkOrderItemsView.xaml
    /// </summary>
    public partial class ViewVehicleWorkOrderItemsView : UserControl
    {
        ViewVehicleWorkOrderItemsViewModel vm;

        public ViewVehicleWorkOrderItemsView(VehicleWorkOrder vwo)
        {
            InitializeComponent();

            vm = new ViewVehicleWorkOrderItemsViewModel(vwo);

            DataContext = vm;
        }

        //private void MenuItem_Del(object sender, RoutedEventArgs e)
        //{
        //    VehicleRepairDescription item = ((sender as MenuItem).DataContext) as VehicleRepairDescription;
        //    //item.FindDirectParent(vm.VehicleRepairDescriptions).Remove(item);
        //    vm.VehicleRepairDescriptions.Remove(item);
        //}

      
    }
}
