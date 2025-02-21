using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Productions.Slitting;
using System;
using System.Collections.Generic;
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

namespace A1QSystem.View.Production.Slitting
{
    /// <summary>
    /// Interaction logic for FlatBenchSlitterView.xaml
    /// </summary>
    public partial class FlatBenchSlitterView : UserControl
    {
        public FlatBenchSlitterView(string userName, string state, List<UserPrivilages> priv, List<MetaData> metaData)
        {
            InitializeComponent();
            this.CreatePermission();
            DataContext = new FlatBenchSlitterViewModel(userName, state, priv, this.Dispatcher, metaData);
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
        private void selectRowDetails(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            row.Focusable = true;
            bool focused = row.Focus();

            // Creating a FocusNavigationDirection object and setting it to a
            // local field that contains the direction selected.
            FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;

            // MoveFocus takes a TraveralReqest as its argument.
            TraversalRequest request = new TraversalRequest(focusDirection);

            // Gets the element with keyboard focus.
            UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;

            // Change keyboard focus.
            if (elementWithFocus != null)
            {
                elementWithFocus.MoveFocus(request);
            }
        }
    }
}
