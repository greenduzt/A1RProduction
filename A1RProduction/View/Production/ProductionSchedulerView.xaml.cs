using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.RawMaterials;
using A1QSystem.ViewModel.Productions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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

namespace A1QSystem.View.Production
{
    /// <summary>
    /// Interaction logic for ProductionSchedulerView.xaml
    /// </summary>
    public partial class ProductionSchedulerView : UserControl
    {
        private ProductionSchedulerViewModel productionSchedulerViewModel;

        public ProductionSchedulerView(string uName, string uState, List<UserPrivilages> uPriv, List<MetaData> md)
        {
            InitializeComponent();
            this.CreatePermission();
            productionSchedulerViewModel = new ProductionSchedulerViewModel(uName, uState, uPriv, this.Dispatcher, md);
            DataContext = productionSchedulerViewModel;
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

        private void SelectRowDetails(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            if (row == null)
            {
                return;
            }
            row.Focusable = true;
            row.Focus();

            var focusDirection = FocusNavigationDirection.Next;
            var request = new TraversalRequest(focusDirection);
            var elementWithFocus = Keyboard.FocusedElement as UIElement;
            if (elementWithFocus != null)
            {
                elementWithFocus.MoveFocus(request);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ////string Tag4Val = Btn4.Tag.ToString(); 

            string myValue = (string)((Button)sender).Tag;

            //string[] strings = myValue.Split('|');

            //bool answer = productionSchedulerViewModel.OrderProduction.Any(x => x.ProductionDate == strings[0] && (x.Shift == 1 && x.ShiftText == "Enable") && (x.Shift == 2 && x.ShiftText == "Enable") && (x.Shift == 3 && x.ShiftText == "Enable"));

           

            ////Btn4.Content = "XX";
            ////foreach (var item in productionSchedulerViewModel.OrderProduction)
            ////{
            ////    if (item.ProductionDate == strings[0] && item.ProductionDate == Tag4Val)
            ////    {
            ////        if (item.Shift == 1 && item.ShiftText == "Disable")
            ////        {

            ////        }
            ////    }
            ////}



            //var viewModel = (ProductionSchedulerViewModel)DataContext;
            //ObservableCollection<RawProductionDetails> rmdList = DBAccess.GetProductionSchedulerData();
            //viewModel.LoadData(rmdList);
        }
    }
}
