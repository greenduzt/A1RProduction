
using A1QSystem.Model.FormulaGeneration;
using A1QSystem.ViewModel.Manufacturing.FormulaGeneration;
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

namespace A1QSystem.View.Manufacturing.RubberFormula
{
    /// <summary>
    /// Interaction logic for RubberFormulaTreeView.xaml
    /// </summary>
    public partial class RubberFormulaTreeView : UserControl
    {
        public RubberFormulaTreeView()
        {
            InitializeComponent();

            DataContext = new RubberFormulaTreeViewModel();            
            
        }


        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            RubberFormulaTreeViewModel presenter = this.DataContext as RubberFormulaTreeViewModel;
            presenter.SelectedTreeItem = (TreeItem)e.NewValue;
        }
       
        
    }
}
