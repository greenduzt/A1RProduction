using A1QSystem.Core;
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
    /// Interaction logic for AddToGradedStockView.xaml
    /// </summary>
    public partial class AddToGradedStockView : UserControl
    {
        public AddToGradedStockView()
        {
            InitializeComponent();
            DataContext = new AddToGradedStockViewModel();
           
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

        private void txtRed4Mesh_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox _this = (sender as TextBox);

            if (string.IsNullOrWhiteSpace(txtRed4Mesh.Text))
            {
                _this.Text = "0";
            }
        }

        private void txtMesh12Red_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox _this = (sender as TextBox);

            if (string.IsNullOrWhiteSpace(txtMesh12Red.Text))
            {
                _this.Text = "0";
            }
        }

      
        private void txtRedFines_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox _this = (sender as TextBox);

            if (string.IsNullOrWhiteSpace(txtRedFines.Text))
            {
                _this.Text = "0";
            }
        }

        

        //private void txtRegrind_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    TextBox _this = (sender as TextBox);

        //    if (string.IsNullOrWhiteSpace(txtRegrind.Text))
        //    {
        //        _this.Text = "0";
        //    }
        //}

       
    }
}
