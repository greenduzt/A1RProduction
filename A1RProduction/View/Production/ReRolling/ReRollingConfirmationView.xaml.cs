using A1QSystem.Model.Production.ReRoll;
using A1QSystem.ViewModel.Productions.ReRolling;
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

namespace A1QSystem.View.Production.ReRolling
{
    /// <summary>
    /// Interaction logic for ReRollingConfirmationView.xaml
    /// </summary>
    public partial class ReRollingConfirmationView : UserControl
    {
        public ReRollingConfirmationView(ReRollingOrder reRollingProd)
        {
            InitializeComponent();
            DataContext = new ReRollingConfirmationViewModel(reRollingProd);
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
