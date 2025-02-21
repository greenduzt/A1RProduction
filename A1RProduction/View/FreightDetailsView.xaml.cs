using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.View.Quoting;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;


namespace A1QSystem.View
{
    /// <summary>
    /// Interaction logic for FreightDetailsView.xaml
    /// </summary>
    public partial class FreightDetailsView : Window
    {
        NewQuoteView _qv = null;

        public FreightDetailsView(NewQuoteView Qv)
        {
            InitializeComponent();
            _qv = Qv;
            txtAddFreightPrice.Text = "0.00";
           
        }

        private void btnAddFreigth_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtAddFreightName.Text))
            {
                txtAddFreightName.Background = Brushes.Yellow;
                txtAddFreightName.BorderBrush = Brushes.Red;
                lblAddFreightNameError.Visibility = Visibility.Visible;
                lblAddFreightNameError.Content = "Freight name is required";
            }
            else if (string.IsNullOrWhiteSpace(this.txtAddFreightPrice.Text)){
                txtAddFreightPrice.Background = Brushes.Yellow;
                txtAddFreightPrice.BorderBrush = Brushes.Red;
                lblAddFreightPriceError.Visibility = Visibility.Visible;
                lblAddFreightPriceError.Content = "Freight price is required";
            }
            else if (string.IsNullOrWhiteSpace(this.txtAddFreightDescription.Text))
            {
                txtAddFreightDescription.Background = Brushes.Yellow;
                txtAddFreightDescription.BorderBrush = Brushes.Red;
                lblAddFreightDesError.Visibility = Visibility.Visible;
                lblAddFreightDesError.Content = "Freight description is required";
            }
            else
            {
             
                string freightName = txtAddFreightName.Text;
                decimal freightPrice = Convert.ToDecimal(txtAddFreightPrice.Text);
                string freightDescription = txtAddFreightDescription.Text;
                string freightUnit = txtFreightUnit.Text;

                A1QSystem.Model.Freight newFreight = new A1QSystem.Model.Freight();
                newFreight.FreightName = freightName;
                newFreight.FreightPrice = freightPrice;
                newFreight.FreightUnit = freightUnit;
                newFreight.FreightDescription = freightDescription;
               
                int result = DBAccess.InsertFreightDetails(newFreight);

                //Updating freight names in the combobox

                DataView dvF = DBAccess.GetFreightNames().Tables["Freight"].DefaultView;
                dvF.Sort = "FreightName asc";
                _qv.cmbFreightName.ItemsSource = dvF;

                MessageBox.Show(result.ToString());

                this.Close();

                txtAddFreightName.Text = "";
                txtAddFreightPrice.Text = "";
                txtFreightUnit.Text = "";
                txtAddFreightDescription.Text = "";

            } 
        }

        private void txtAddFreightPrice_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(this.txtAddFreightPrice.Text))
            {
                txtAddFreightPrice.Background = Brushes.Yellow;
                txtAddFreightPrice.BorderBrush = Brushes.Red;
                lblAddFreightPriceError.Visibility = Visibility.Visible;
                lblAddFreightPriceError.Content = "Freight price is required";
            }
            else
            {
                txtAddFreightPrice.Background = Brushes.AliceBlue;
                txtAddFreightPrice.BorderBrush = Brushes.LightGray;
                lblAddFreightPriceError.Content = "";
                lblAddFreightPriceError.Visibility = Visibility.Collapsed;

                TextBox textBox = sender as TextBox;
                Int32 selectionStart = textBox.SelectionStart;
                Int32 selectionLength = textBox.SelectionLength;
                String newText = String.Empty;
                int count = 0;
                foreach (Char c in textBox.Text.ToCharArray())
                {
                    if (Char.IsDigit(c) || Char.IsControl(c) || (c == '.' && count == 0))
                    {
                        newText += c;
                        if (c == '.')
                            count += 1;
                        
                    }
                    
                }
                textBox.Text = newText;
                textBox.SelectionStart = selectionStart <= textBox.Text.Length ? selectionStart : textBox.Text.Length;
            }
        }

        private void txtAddFreightName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtAddFreightName.Text))
            {
                txtAddFreightName.Background = Brushes.Yellow;
                txtAddFreightName.BorderBrush = Brushes.Red;
                lblAddFreightNameError.Visibility = Visibility.Visible;
                lblAddFreightNameError.Content = "Freight name is required";
            }
            else
            {
                lblAddFreightNameError.Visibility = Visibility.Collapsed;
                txtAddFreightName.Background = Brushes.AliceBlue;
                txtAddFreightName.BorderBrush = Brushes.LightGray;
                lblAddFreightNameError.Content = "";
            }
        }

        private void txtAddFreightDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtAddFreightDescription.Text))
            {
                txtAddFreightDescription.Background = Brushes.Yellow;
                txtAddFreightDescription.BorderBrush = Brushes.Red;
                lblAddFreightDesError.Visibility = Visibility.Visible;
                lblAddFreightDesError.Content = "Freight description is required";
            }
            else
            {
                lblAddFreightDesError.Visibility = Visibility.Collapsed;
                txtAddFreightDescription.Background = Brushes.AliceBlue;
                txtAddFreightDescription.BorderBrush = Brushes.LightGray;
                lblAddFreightDesError.Content = "";
            }
        }

        private void btnCancelFreight_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
