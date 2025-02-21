using A1QSystem.Model.Production.SlitingPeeling;
using A1QSystem.ViewModel.Productions.SlitPeel;
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

namespace A1QSystem.View.Production.SlitPeel
{
    /// <summary>
    /// Interaction logic for ShiftSlitPeelView.xaml
    /// </summary>
    public partial class ShiftSlitPeelView : UserControl
    {
        public ShiftSlitPeelView(SlitPeelSchedule slitPeelSchedule)
        {
            InitializeComponent();
            DataContext = new ShiftSlitPeelViewModel(slitPeelSchedule);
        }
    }
}
