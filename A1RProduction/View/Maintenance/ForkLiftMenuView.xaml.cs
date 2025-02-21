using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel;
using A1QSystem.ViewModel.Maintenance;
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

namespace A1QSystem.View.Maintenance
{
    /// <summary>
    /// Interaction logic for MaintenanceMenuView.xaml
    /// </summary>
    public partial class ForkLiftMenuView : UserControl
    {

        public ForkLiftMenuView(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            InitializeComponent();
            DataContext = new ForkLiftMenuViewModel(UserName, State, Privilages, md);

        }
        
    }
}
