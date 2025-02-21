using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.Productions.WeeklyScheduleFull;
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

namespace A1QSystem.View.Production.WeeklyScheduleFull
{
    /// <summary>
    /// Interaction logic for WeeklyScheduleFullView.xaml
    /// </summary>
    public partial class WeeklyScheduleFullView : UserControl
    {
        public WeeklyScheduleFullView(string UserName, string State, List<UserPrivilages> uPriv, List<MetaData> md)
        {
            InitializeComponent();
            DataContext = new WeeklyScheduleFullViewModel(UserName, State, uPriv,md);
        }
    }
}
