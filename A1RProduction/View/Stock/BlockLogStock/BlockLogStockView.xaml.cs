using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Stock.BlockLogStock;
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

namespace A1QSystem.View.Stock.BlockLogStock
{
    /// <summary>
    /// Interaction logic for BlockLogStockView.xaml
    /// </summary>
    public partial class BlockLogStockView : UserControl
    {
        public BlockLogStockView(string uName, string uState, List<UserPrivilages> uPriv, List<MetaData> metaData)
        {
            InitializeComponent();
            DataContext = new BlockLogStockViewModel(uName, uState, uPriv, metaData);
            //childWindow.DataContext = ChildWindowManager.Instance;
            
        }
    }
}
