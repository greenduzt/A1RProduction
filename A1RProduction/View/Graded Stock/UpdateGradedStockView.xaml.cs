using A1QSystem.ViewModel.Graded_Stock;
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

namespace A1QSystem.View.Graded_Stock
{
    /// <summary>
    /// Interaction logic for UpdateGradedStockView.xaml
    /// </summary>
    public partial class UpdateGradedStockView : UserControl
    {
        public UpdateGradedStockView()
        {
            InitializeComponent();
            DataContext = new UpdateGradedStockViewModel();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (((TextBox)sender).Text == "")
            {
                ((TextBox)sender).Text = "0";
            }
        }


        private void txtMesh4_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox _this = (sender as TextBox);

            if (string.IsNullOrWhiteSpace(txtMesh4.Text))
            {
                _this.Text = "0";
            }
        }

        private void txtMesh12_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox _this = (sender as TextBox);

            if (string.IsNullOrWhiteSpace(txtMesh12.Text))
            {
                _this.Text = "0";
            }
        }

        private void txtMesh16_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox _this = (sender as TextBox);

            if (string.IsNullOrWhiteSpace(txtMesh16.Text))
            {
                _this.Text = "0";
            }
        }

        private void txtMesh30_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox _this = (sender as TextBox);

            if (string.IsNullOrWhiteSpace(txtMesh30.Text))
            {
                _this.Text = "0";
            }
        }

        private void txtRegrind_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox _this = (sender as TextBox);

            if (string.IsNullOrWhiteSpace(txtRegrind.Text))
            {
                _this.Text = "0";
            }
        }
    }
}
