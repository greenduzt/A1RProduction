using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Products;
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

namespace A1QSystem.View.Products
{
    /// <summary>
    /// Interaction logic for UpdateProductView.xaml
    /// </summary>
    public partial class UpdateProductView : UserControl
    {
        public UpdateProductView(string UserName, string Password, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            InitializeComponent();

            DataContext = new UpdateProductViewModel(UserName, Password, Privilages, md);
        }


        private void ExitMainMenuTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxImage icon = MessageBoxImage.Warning;
            if (MessageBox.Show("Do you want to exit from A1 Rubber Console?", "Exit Confirmation", MessageBoxButton.YesNo, icon) == MessageBoxResult.Yes)
            {
                Window parentWindow = (Window)this.Parent;
                parentWindow.Close();
            }
            else
            {
                // Do not close the window
            }
        }
    }

}
