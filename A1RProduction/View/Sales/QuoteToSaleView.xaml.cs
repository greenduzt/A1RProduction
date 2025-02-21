using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Sales;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace A1QSystem.View.Sales
{
    /// <summary>
    /// Interaction logic for QuoteToSale.xaml
    /// </summary>
    public partial class QuoteToSaleView : UserControl
    {

        public QuoteToSaleView(string username, string state, List<UserPrivilages> uPrivilages, List<MetaData> md)
        {
            InitializeComponent();
            this.CreatePermission();
            DataContext = new QuoteToSaleViewModel(username, state, uPrivilages, this.Dispatcher, md);
            //childWindow.DataContext = ChildWindowManager.Instance;

        }

        public void CreatePermission()
        {
            // Make sure client has permissions 
            try
            {
                SqlClientPermission perm = new SqlClientPermission(System.Security.Permissions.PermissionState.Unrestricted);
                perm.Demand();
            }
            catch
            {
                throw new ApplicationException("No permission");
            }
        }

        private void ExitMainMenuTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxImage icon = MessageBoxImage.Warning;
            if (MessageBox.Show("Do you want to exit from A1 Rubber System?", "Exit Confirmation", MessageBoxButton.YesNo, icon) == MessageBoxResult.Yes)
            {
                //Window parentWindow = (Window)this.Parent;
                //parentWindow.Close();
                Window parent = Window.GetWindow(this);
                parent.Close();
            }
            else
            {
                // Do not close the window
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                Convert.ToInt32(e.Text);
            }
            catch
            {
                e.Handled = true;
            }
        }
    }
}
