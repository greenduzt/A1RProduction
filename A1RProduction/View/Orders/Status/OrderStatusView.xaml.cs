using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Orders.Status;
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

namespace A1QSystem.View.Orders.Status
{
    /// <summary>
    /// Interaction logic for OrderStatusView.xaml
    /// </summary>
    public partial class OrderStatusView : UserControl
    {
        public OrderStatusView(string UserName, string State, List<UserPrivilages> UserPrivilages, List<MetaData> md)
        {
            InitializeComponent();
            DataContext = new OrderStatusViewModel(UserName, State, UserPrivilages, md);

        }
    }
}
