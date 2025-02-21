using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Machine.MachineWorkOrders;
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

namespace A1QSystem.View.Machine.MachineWorkOrders
{
    /// <summary>
    /// Interaction logic for NewMachineRepairWorkOrderView.xaml
    /// </summary>
    public partial class NewMachineRepairWorkOrderView : UserControl
    {
        public NewMachineRepairWorkOrderView(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            InitializeComponent();
            DataContext = new NewMachineRepairWorkOrderViewModel(UserName,State,up,md);
        }
    }
}
