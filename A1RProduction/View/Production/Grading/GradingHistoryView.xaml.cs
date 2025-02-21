using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Productions.Grading;
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

namespace A1QSystem.View.Production.Grading
{
    /// <summary>
    /// Interaction logic for GradingHistoryView.xaml
    /// </summary>
    public partial class GradingHistoryView : UserControl
    {
        public GradingHistoryView(string userName, string state, List<UserPrivilages> privilages, List<MetaData> md)
        {
            InitializeComponent();
            DataContext = new GradingHistoryViewModel(userName, state, privilages, md);
            //childWindow.DataContext = ChildWindowManager.Instance;
        }
    }
}
