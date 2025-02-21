using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.ViewModel;
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

namespace A1QSystem.View.Quoting
{
    /// <summary>
    /// Interaction logic for SearchProductByName.xaml
    /// </summary>
    public partial class SearchProductByName : Window
    {
        public ObservableCollection<Product> Product { get; set; }
        public SearchProductByName()
        {
            InitializeComponent();

            //DataView pdv = new DataView();
            //pdv = DBAccess.GetAllProducts().Tables["Products"].DefaultView;
            //pdv.Sort = "ProductDescription ASC";

            var productDescription = new ObservableCollection<string>();

            //for (int x = 0; x < pdv.Count; x++)
            //{
            //    productDescription.Add(pdv[x]["ProductDescription"].ToString());

            //}
            Product = DBAccess.GetAllProds();

            foreach (var item in Product)
	        {
		        productDescription.Add(item.ProductDescription);

               
	        }

            txtName.ItemsSource = productDescription;

        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("ProductCode", typeof(string), typeof(SearchProductByName), new UIPropertyMetadata(String.Empty));
        public string ProductCode
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }       


        private void txtName_SelectionChanged(object sender, RoutedEventArgs e)
        {
            string pDescription = txtName.Text;

            if (!string.IsNullOrEmpty(pDescription))
            {
                var query =
                         (from c in Product
                          where c.ProductDescription == pDescription
                          select new { c.ProductCode }).Single().ProductCode;

                txtProductCode.Text = query.ToString();
                txtProductCode.Focusable = true;
                Keyboard.Focus(txtProductCode);
            }
            else
            {
                txtProductCode.Text = "";
            }
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
