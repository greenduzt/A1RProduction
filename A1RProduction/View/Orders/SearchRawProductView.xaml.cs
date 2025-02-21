using A1QSystem.DB;
using A1QSystem.Model.Products;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace A1QSystem.View.Orders
{
    /// <summary>
    /// Interaction logic for SearchRawProductView.xaml
    /// </summary>
    public partial class SearchRawProductView : Window
    {
        public ObservableCollection<RawProduct> RawProducts { get; set; }

        public SearchRawProductView()
        {
            InitializeComponent();

            RawProducts = new ObservableCollection<RawProduct>();
            RawProducts = DBAccess.GetAllRawProducts();
                      
            txtName.ItemsSource = RawProducts.Select(x=>x.Description);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("ProductCode", typeof(string), typeof(SearchRawProductView), new UIPropertyMetadata(String.Empty));
        public string ProductCode
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }




        private void txtName_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string pDescription = txtName.Text;

            var pCode =(from at in RawProducts where at.Description == pDescription select at).Select(a => a.RawProductCode).SingleOrDefault();

            txtProductCode.Text = pCode;
            txtProductCode.Focusable = true;
            Keyboard.Focus(txtProductCode);
        }
        //Close
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //Add
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            DialogResult = true;
            Close();
        }
        //Clear
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            txtName.Text = "";
            txtProductCode.Text = "";
        }
    }
}
