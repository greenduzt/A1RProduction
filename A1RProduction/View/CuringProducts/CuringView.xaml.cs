using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.CuringProducts;
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

namespace A1QSystem.View.CuringProducts
{
    /// <summary>
    /// Interaction logic for CuringView.xaml
    /// </summary>
    public partial class CuringView : UserControl
    {
        public CuringView(string userName, string state, List<UserPrivilages> privilages, List<MetaData> metaData)
        {
            InitializeComponent();
            DataContext = new CuringViewModel(userName, state, privilages, metaData);
        }
    }
}
