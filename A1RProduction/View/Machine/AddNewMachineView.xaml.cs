using A1QSystem.ViewModel.Machine;
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

namespace A1QSystem.View.Machine
{
    /// <summary>
    /// Interaction logic for AddNewMachineView.xaml
    /// </summary>
    public partial class AddNewMachineView : UserControl
    {
        public AddNewMachineView()
        {
            InitializeComponent();
            DataContext = new AddNewMachineViewModel();
        }
    }
}
