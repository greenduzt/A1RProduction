using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Machine.MachineHistory;
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

namespace A1QSystem.View.Machine.MachineHistory
{
    /// <summary>
    /// Interaction logic for MachineWorkOrderHistoryView.xaml
    /// </summary>
    public partial class MachineWorkOrderHistoryView : UserControl
    {
        public MachineWorkOrderHistoryView(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            InitializeComponent();
            DataContext = new MachineWorkOrderHistoryViewModel(UserName, State, up, md);
            //childWindow.DataContext = ChildWindowManager.Instance;
        }

        private void dgQuoteDetails_Copy1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dgQuoteDetails_Copy1.UnselectAllCells();
        }
    }
}
