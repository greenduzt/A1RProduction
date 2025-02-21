using A1QSystem.ViewModel.Productions.Grading;
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

namespace A1QSystem.View.Production.Grading
{
    /// <summary>
    /// Interaction logic for AddToRegrindStockView.xaml
    /// </summary>
    public partial class AddToRegrindStockView : UserControl
    {
        public AddToRegrindStockView()
        {
            InitializeComponent();
            DataContext = new AddToRegrindStockViewModel();
          

        }
    }
}
