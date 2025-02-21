using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel;
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

namespace A1QSystem.View.Production
{
    /// <summary>
    /// Interaction logic for ProductionView.xaml
    /// </summary>
    public partial class ProductionView : UserControl
    {

        string _userName;
        string _state;
        ViewProductionViewModel vpvm;

        public ProductionView(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            InitializeComponent();

            vpvm = new ViewProductionViewModel(UserName, State, Privilages, md);

            DataContext = vpvm;
            this.gaugeLargeMixer.DataContext = vpvm.disLargeMixerData();
            this.gaugeHighSpeedMixer.DataContext = vpvm.disHighSpeedMixerData();
            this.gaugeSmallMixer.DataContext = vpvm.disSmallMixerData();
            this.gaugeLogAndPeel.DataContext = vpvm.disLogPeelData();
            this.gaugeCSBRMachine.DataContext = vpvm.disCSBRData();
            this.gaugeSlitter.DataContext = vpvm.disSlitterData();
            this.gaugeBagging.DataContext = vpvm.disBaggingData();
            this.gaugeRollUp.DataContext = vpvm.disRollUpData();
            this.gaugeRubberGrading.DataContext = vpvm.disRubberGradingData();
            this.gaugeShredding.DataContext = vpvm.disShreddingData();

            _userName = UserName;
            _state = State;
        }


        private void ExitProdMenuTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
