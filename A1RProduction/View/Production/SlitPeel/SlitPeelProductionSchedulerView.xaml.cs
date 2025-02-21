using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Productions.SlitPeel;
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

namespace A1QSystem.View.Production
{
    /// <summary>
    /// Interaction logic for SlitPeelProductionSchedulerView.xaml
    /// </summary>
    public partial class SlitPeelProductionSchedulerView : UserControl
    {
        public SlitPeelProductionSchedulerView(string UserName, string State, List<UserPrivilages> priv, List<MetaData> md)
        {
            InitializeComponent();
            this.CreatePermission();
            DataContext = new SlitPeelProductionSchedulerViewModel(UserName, State, priv, this.Dispatcher, md);
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
    }
}
