using A1QSystem.Model;
using A1QSystem.Model.Meta;
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
    /// Interaction logic for MiscellaneousWorkOrderView.xaml
    /// </summary>
    public partial class MiscellaneousWorkOrderView : UserControl
    {
        public MiscellaneousWorkOrderView(string uName, string uState, List<UserPrivilages> privilages, List<MetaData> metaData)
        {
            InitializeComponent();
            DataContext = new MiscellaneousWorkOrderViewModel(uName, uState, privilages, metaData);
        }
    }
}
