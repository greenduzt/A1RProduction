using A1QSystem.Core;
using A1QSystem.ViewModel.Sales;
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

namespace A1QSystem.View.Sales
{
    /// <summary>
    /// Interaction logic for EditQuoteDetailsPopUpView.xaml
    /// </summary>
    public partial class EditQuoteDetailsPopUpView : UserControl
    {
        public EditQuoteDetailsPopUpView(int QuoteNo, string UserName, string State)
        {
            InitializeComponent();
            DataContext = new EditQuoteDetailsPopUpViewModel(QuoteNo, UserName, State);
            //childWindow.DataContext = ChildWindowManager.Instance;
            
        }

        private void txtFreightQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox _this = (sender as TextBox);

            if (string.IsNullOrWhiteSpace(txtFreightQty.Text))
            {
                _this.Text = "0";
            }
        }

        private void txtFreightDisc_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox _this = (sender as TextBox);

            if (string.IsNullOrWhiteSpace(txtFreightDisc.Text))
            {
                _this.Text = "0";
            }
        }


        private void txtSearchProjectName_KeyUp(object sender, KeyEventArgs e)
        {
            txtSearchQuoteNumber.Text = "";
            txtSearchCustomer.Text = "";
        }

        private void txtSearchQuoteNumber_KeyUp(object sender, KeyEventArgs e)
        {
            txtSearchProjectName.Text = "";
            txtSearchCustomer.Text = "";
        }

        private void txtSearchCustomer_KeyUp(object sender, KeyEventArgs e)
        {
            txtSearchProjectName.Text = "";
            txtSearchQuoteNumber.Text = "";
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
