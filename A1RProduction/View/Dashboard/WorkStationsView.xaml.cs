using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Dashboard;
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

namespace A1QSystem.View.Dashboard
{
    /// <summary>
    /// Interaction logic for WorkStationsView.xaml
    /// </summary>
    public partial class WorkStationsView : UserControl
    {
        public WorkStationsView(string UserName, string State, List<UserPrivilages> uPriv, List<MetaData> metaData)
        {
            InitializeComponent();
            DataContext = new WorkStationsViewModel(UserName, State, uPriv, metaData);
        }
    }
}
