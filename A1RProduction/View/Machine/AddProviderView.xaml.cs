using A1QSystem.Model;
using A1QSystem.Model.Meta;
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
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace A1QSystem.View.Machine
{
    /// <summary>
    /// Interaction logic for AddProviderView.xaml
    /// </summary>
    public partial class AddProviderView : UserControl
    {
        public AddProviderView()
        {
            InitializeComponent();
            DataContext = new AddNewProviderViewModel();
        }
    }
}
