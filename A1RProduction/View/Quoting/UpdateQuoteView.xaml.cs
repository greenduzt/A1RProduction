using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel;
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

namespace A1QSystem.View.Quoting
{
    /// <summary>
    /// Interaction logic for UpdateQuoteView.xaml
    /// </summary>
    public partial class UpdateQuoteView : UserControl
    {
        public UpdateQuoteView(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            InitializeComponent();

            DataContext = new UpdateQuoteViewModel(UserName, State, Privilages, md);
            //childWindow.DataContext = ChildWindowManager.Instance;
          
        }

        private void ExitProdMenuTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxImage icon = MessageBoxImage.Warning;
            if (MessageBox.Show("Do you want to exit from A1 Rubber Console?", "Exit Confirmation", MessageBoxButton.YesNo, icon) == MessageBoxResult.Yes)
            {
                Window parentWindow = (Window)this.Parent;
                parentWindow.Close();
            }
            else
            {
                // Do not close the window
            }
        }

        
        private void txtFreightQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox _this = (sender as TextBox);

            if (string.IsNullOrWhiteSpace(txtFreightQty.Text))
            {
                _this.Text="0";
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
