using A1QSystem.ViewModel.Machine.MachineHistory;
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

namespace A1QSystem.View.Machine.MachineHistory
{
    /// <summary>
    /// Interaction logic for OpenUploadedFilesView.xaml
    /// </summary>
    public partial class OpenUploadedFilesView : UserControl
    {
        public OpenUploadedFilesView(Int64 workOrderNo, string machineName)
        {
            InitializeComponent();
            DataContext = new OpenUploadedFilesViewModel(workOrderNo, machineName);
        }
    }
}
