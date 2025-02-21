using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.WorkOrders;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for VehicleWorkOrdersView.xaml
    /// </summary>
    public partial class VehicleWorkOrdersView : UserControl
    {
        public VehicleWorkOrdersView(string UserName, string State, List<UserPrivilages> UserPrivilages, List<MetaData> metaData)
        {
            InitializeComponent();
            this.CreatePermission();
            DataContext = new VehicleWorkOrdersViewModel(UserName, State, UserPrivilages, metaData, this.Dispatcher);
            //childWindow.DataContext = ChildWindowManager.Instance;
        }

        public void CreatePermission()
        {
            // Make sure client has permissions 
            try
            {
                SqlClientPermission perm = new SqlClientPermission(System.Security.Permissions.PermissionState.Unrestricted);
                perm.Demand();
            }
            catch
            {
                throw new ApplicationException("No permission");
            }
        }

        private void dgQuoteDetails_Copy1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dgQuoteDetails_Copy1.UnselectAllCells();
        }
    }
}
