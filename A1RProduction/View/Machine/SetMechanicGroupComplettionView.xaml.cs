using A1QSystem.Model.Machine;
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
    /// Interaction logic for SetMechanicGroupComplettionView.xaml
    /// </summary>
    public partial class SetMechanicGroupComplettionView : UserControl
    {
        public SetMechanicGroupComplettionView()
        {
            InitializeComponent();
            DataContext = new SetMechanicGroupComplettionViewModel();
        }
    }
}
