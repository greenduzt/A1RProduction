using A1QSystem.ViewModel.Productions.Mixing;
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

namespace A1QSystem.View.Production.Mixing
{
    /// <summary>
    /// Interaction logic for IBCChangeView.xaml
    /// </summary>
    public partial class IBCChangeView : UserControl
    {
        public IBCChangeView()
        {
            InitializeComponent();
            DataContext = new IBCChangeViewModel();
        }
    }
}
