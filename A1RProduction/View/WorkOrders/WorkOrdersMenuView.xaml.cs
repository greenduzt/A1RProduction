using A1QSystem.Model;
using A1QSystem.Model.Meta;
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
    /// Interaction logic for WorkOrdersMenuView.xaml
    /// </summary>
    public partial class WorkOrdersMenuView : UserControl
    {
        public WorkOrdersMenuView(string UserName, string State, List<UserPrivilages> UserPrivilages,List<MetaData> metaData)
        {
            InitializeComponent();
            DataContext = new WorkOrdersMenuViewModel(UserName, State, UserPrivilages, metaData);
        }
    }
}
