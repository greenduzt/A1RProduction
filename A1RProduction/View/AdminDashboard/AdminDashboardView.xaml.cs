using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.AdminDashboard;
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

namespace A1QSystem.View.AdminDashboard
{
    /// <summary>
    /// Interaction logic for AdminDashboardView.xaml
    /// </summary>
    public partial class AdminDashboardView : UserControl
    {
        public AdminDashboardView(string UserName, string State, List<UserPrivilages> up, List<MetaData> metaData)
        {
            InitializeComponent();
            DataContext = new AdminDashboardViewModel(UserName, State, up, metaData);
            //childWindow.DataContext = ChildWindowManager.Instance;
        }
    }
}
