using A1QSystem.Model.Stock;
using A1QSystem.ViewModel.StockMaintenance;
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

namespace A1QSystem.View.StockMaintenance
{
    /// <summary>
    /// Interaction logic for EditStockMaintenance.xaml
    /// </summary>
    public partial class EditStockMaintenance : UserControl
    {
        public EditStockMaintenance(StockMaintenanceDetails stockMaintenanceDetails)
        {
            InitializeComponent();
            DataContext = new EditStockMaintenanceViewModel(stockMaintenanceDetails);
        }

        //private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
           
        //}

        
        //private void TextBox_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
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

        private void txtAvailableQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox _this = (sender as TextBox);

            if (string.IsNullOrWhiteSpace(txtAvailableQty.Text))
            {
                _this.Text = "0";
            }
        }

        //private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    TextBox _this = (sender as TextBox);

        //    if (string.IsNullOrWhiteSpace(txtReOrderQty.Text))
        //    {
        //        _this.Text = "0";
        //    }
        //}

        private void txtAvailableQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
