using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for MachineLargrMiverView.xaml
    /// </summary>
    public partial class MachineLargeMixerView : UserControl
    {

        public MachineLargeMixerView(string UserName, string State, string MachineName, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            InitializeComponent();
           // DataContext = new MachineViewModel(); 

            this.AddChild(new DayShiftView(MachineName, UserName, State, Privilages, md));
         
  
        }
    }    
}
