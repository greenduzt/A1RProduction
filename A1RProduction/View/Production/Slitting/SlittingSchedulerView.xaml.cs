using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Productions.Slitting;
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

namespace A1QSystem.View.Production.Slitting
{
    /// <summary>
    /// Interaction logic for SlittingSchedulerView.xaml
    /// </summary>
    public partial class SlittingSchedulerView : UserControl
    {
        public SlittingSchedulerView(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            InitializeComponent();
            DataContext = new SlittingSchedulerViewModel(UserName, State, up, md, this.Dispatcher);
            //childWindow.DataContext = ChildWindowManager.Instance;    
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
    }
}
