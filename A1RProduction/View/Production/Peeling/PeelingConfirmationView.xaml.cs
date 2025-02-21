using A1QSystem.Model.Production.Peeling;
using A1QSystem.ViewModel.Productions.Peeling;
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

namespace A1QSystem.View.Production.Peeling
{
    /// <summary>
    /// Interaction logic for PeelingConfirmationView.xaml
    /// </summary>
    public partial class PeelingConfirmationView : UserControl
    {
        public PeelingConfirmationView(PeelingOrder po)
        {
            InitializeComponent();
            DataContext = new PeelingConfirmationViewModel(po);
        }

        private void txtTotYieldCut_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox _this = (sender as TextBox);

            if (string.IsNullOrWhiteSpace(txtTotYieldCut.Text))
            {
                _this.Text = "0";
            }
        }

        private void txtTotYieldCut_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                Convert.ToInt32(e.Text);
            }
            catch
            {
                e.Handled = true;
            }
        }

    }
}
