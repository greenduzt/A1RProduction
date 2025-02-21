using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Customers;
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

namespace A1QSystem.View.Customers
{
    /// <summary>
    /// Interaction logic for CustomerDetailsView.xaml
    /// </summary>
    public partial class CustomerDetailsView : UserControl
    {
        public CustomerDetailsView(string userName, List<MetaData> metaData)
        {
            InitializeComponent();
            DataContext = new CustomerDetailsViewModel(userName, metaData);
        }
    }
}
