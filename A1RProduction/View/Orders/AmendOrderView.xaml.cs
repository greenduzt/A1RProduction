using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Orders;
using A1QSystem.ViewModel.Orders;
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
using System.Windows.Shapes;

namespace A1QSystem.View.Orders
{
    /// <summary>
    /// Interaction logic for AmendOrderView.xaml
    /// </summary>
    public partial class AmendOrderView : UserControl
    {

        public List<Order> Orders { get; set; }

        public AmendOrderView(string UserName, string State, List<UserPrivilages> UserPrivilages, List<MetaData> md)
        {
            InitializeComponent();
            DataContext = new AmendOrderViewModel(UserName, State, UserPrivilages, md);
            //Orders = DBAccess.GetAllOrders();
            //txtName.ItemsSource = Orders.Select(x => x.Customer.CompanyName + " " + x.SalesNo + " " + x.OrderNo + " " + x.Comments);
        }

        private void DataGrid_CellGotFocus(object sender, RoutedEventArgs e)
        {
            // Lookup for the source to be DataGridCell
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                // Starts the Edit on the row;
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);

                Control control = GetFirstChildByType<Control>(e.OriginalSource as DataGridCell);
                if (control != null)
                {
                    control.Focus();
                }
            }
        }

        private T GetFirstChildByType<T>(DependencyObject prop) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(prop); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild((prop), i) as DependencyObject;
                if (child == null)
                    continue;

                T castedProp = child as T;
                if (castedProp != null)
                    return castedProp;

                castedProp = GetFirstChildByType<T>(child);

                if (castedProp != null)
                    return castedProp;
            }
            return null;
        }
      
    }
}
