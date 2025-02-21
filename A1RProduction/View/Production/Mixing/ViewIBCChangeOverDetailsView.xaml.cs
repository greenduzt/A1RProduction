using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Productions.Mixing;
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

namespace A1QSystem.View.Production.Mixing
{
    /// <summary>
    /// Interaction logic for ViewIBCChangeOverDetailsView.xaml
    /// </summary>
    public partial class ViewIBCChangeOverDetailsView : UserControl
    {
        public ViewIBCChangeOverDetailsView(string userName, string state, List<UserPrivilages> priv, List<MetaData> metaData)
        {
            InitializeComponent();
            DataContext = new ViewIBCChangeOverDetailsViewModel(userName, state, priv, metaData);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
