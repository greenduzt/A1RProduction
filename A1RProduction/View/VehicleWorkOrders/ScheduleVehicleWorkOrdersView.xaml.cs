using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
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
    /// Interaction logic for ScheduleVehicleWorkOrdersView.xaml
    /// </summary>
    public partial class ScheduleVehicleWorkOrdersView : UserControl
    {
        public ScheduleVehicleWorkOrdersView(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            InitializeComponent();
            DataContext = new ScheduleVehicleWorkOrdersViewModel(UserName,State,up,md);
            //childWindow.DataContext = ChildWindowManager.Instance;
        }
    }
}
