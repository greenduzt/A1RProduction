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
    /// Interaction logic for VehicleMenu.xaml
    /// </summary>
    public partial class VehicleMenuView : UserControl
    {
        public VehicleMenuView(string UserName, string State, List<UserPrivilages> uPriv, List<MetaData> metaData)
        {
            InitializeComponent();
            DataContext = new VehiclesMenuViewModel(UserName, State, uPriv, metaData);
            //childWindow.DataContext = ChildWindowManager.Instance;
        }
    }
}
