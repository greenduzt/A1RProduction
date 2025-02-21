using A1QSystem.Model.Production.Slitting;
using A1QSystem.ViewModel.Productions.Slitting;
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

namespace A1QSystem.View.Production.Slitting
{
    /// <summary>
    /// Interaction logic for SlittingConfirmationView.xaml
    /// </summary>
    public partial class SlittingConfirmationView : UserControl
    {
        public SlittingConfirmationView(SlittingOrder slittingProduction)
        {
            InitializeComponent();
            DataContext = new SlittingConfirmationViewModel(slittingProduction);
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

        private void txtOffSpecTiles_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox _this = (sender as TextBox);

            if (string.IsNullOrWhiteSpace(txtOffSpecTiles.Text))
            {
                _this.Text = "0";
            }
        }

        private void txtOffSpecTiles_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        //private void txtExtraTiles_TextChanged(object sender, TextChangedEventArgs e)
        //{

        //    TextBox _this = (sender as TextBox);

        //    if (string.IsNullOrWhiteSpace(txtExtraTiles.Text))
        //    {
        //        _this.Text = "0";
        //    }
        //}

        private void txtExtraTiles_PreviewTextInput(object sender, TextCompositionEventArgs e)
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

        //private void txtShreaddingTiles_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    TextBox _this = (sender as TextBox);

        //    if (string.IsNullOrWhiteSpace(txtExtraTiles.Text))
        //    {
        //        _this.Text = "0";
        //    }

        //    var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
        //    binding.UpdateSource();
        //}


        //private void txtShreaddingTiles_PreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    try
        //    {
        //        Convert.ToInt32(e.Text);
        //    }
        //    catch
        //    {
        //        e.Handled = true;
        //    }
        //}

        //private void txtShreddedTiles_PreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    try
        //    {
        //        Convert.ToInt32(e.Text);
        //    }
        //    catch
        //    {
        //        e.Handled = true;
        //    }
        //}

        //private void txtShreddedTiles_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    TextBox _this = (sender as TextBox);

        //    if (string.IsNullOrWhiteSpace(txtShreddedTiles.Text))
        //    {
        //        _this.Text = "0";
        //    }
        //}
    }
}
