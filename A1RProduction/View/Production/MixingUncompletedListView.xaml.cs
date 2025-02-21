using A1QSystem.ViewModel.Productions;
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

namespace A1QSystem.View.Production
{
    /// <summary>
    /// Interaction logic for MixingUncompletedListView.xaml
    /// </summary>
    public partial class MixingUncompletedListView : UserControl
    {
        public MixingUncompletedListView()
        {
            InitializeComponent();
            DataContext = new MixingUncompletedListViewModel();
        }
    }
}
