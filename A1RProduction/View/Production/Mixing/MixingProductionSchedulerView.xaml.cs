using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Productions.Mixing;
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

namespace A1QSystem.View.Production.Mixing
{
    /// <summary>
    /// Interaction logic for GradingProductionScheduler.xaml
    /// </summary>
    public partial class MixingProductionSchedulerView : UserControl
    {
        public MixingProductionSchedulerView(string UserName, string State, List<UserPrivilages> priv, List<MetaData> md)
        {
            InitializeComponent();
            this.CreatePermission();
            DataContext = new MixingProductionSchedulerViewModel(UserName, State, priv, this.Dispatcher, md);
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
