using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MsgBox;
using System.Windows;
using A1QSystem.View.Production;
using A1QSystem.View.Orders;
using A1QSystem.View.Production.Grading;
using A1QSystem.View.Production.Mixing;
using System.Timers;
using System.Windows.Threading;
using System.Windows.Controls;
using A1QSystem.DB;
using System.Collections.ObjectModel;
using A1QSystem.Model.RawMaterials;
using A1QSystem.View.Production.SlitPeel;
using A1QSystem.View.StockMaintenance;
using A1QSystem.View.Graded_Stock;
using A1QSystem.View.CuringProducts;
using A1QSystem.View.Production.Slitting;
using A1QSystem.View.Production.Peeling;
using A1QSystem.View.Production.ReRolling;
using A1QSystem.View.Stock.BlockLogStock;
using A1QSystem.View.VehicleWorkOrders;
using A1QSystem.View.Vehicles;
using A1QSystem.View.WorkOrders;
using A1QSystem.Model.Meta;
using A1QSystem.View.VehicleWorkOrders.History;
using A1QSystem.View;
using A1QSystem.View.Machine.MachineWorkOrders;
using A1QSystem.View.Machine.MachineHistory;
using A1QSystem.View.Machine;
using A1QSystem.View.Production.WeeklyScheduleFull;

namespace A1QSystem.ViewModel.PageSwitcher
{
    public class TopContentViewModel : ViewModelBase
    {
        public DateTime CurrentDateTime { get; set; }
        private ICommand _logOutCommand;
        private ICommand _exitCommand;
        private ICommand _mixingSchedulerCommand;
        private ICommand _viewProfileCommand;
        private ICommand _prodSchedulerCommand;
        private ICommand _newProdOrderCommand;
        private ICommand _gradingOrderCommand;
        private ICommand _mixingOrderCommand;
        private ICommand _slittingCommand;
        private ICommand _peelingCommand;
        private ICommand _slitPeelSchedulerCommand;
        private ICommand _stockMaintenanceCommand;
        private ICommand _gradingHistoryCommand;
        private ICommand _mixingHistoryCommand;
        private ICommand _viewGradedStockCommand;
        private ICommand _curingOrderCommand;
        private ICommand _flatBenchSlittingCommand;
        private ICommand _carouselSlittingCommand;
        private ICommand _reRollingCommand;
        private ICommand _amendOrderCommand;
        private ICommand _finishedGoodsCommand;
        private ICommand _blockLogCommand;
        private ICommand _reRollingHistoryCommand;
        private ICommand _peelingHistoryCommand;
        private ICommand _slittingHistoryCommand;
        private ICommand _newVehicleMaintenanceWorkOrderCommand;
        private ICommand _updateVehicleWorkOrderCommand;
        private ICommand _vehicleWorkOrdersCommand;
        private ICommand _newVehicleWorkOrderCommand;
        private ICommand _vehicleWorkOrderHistoryCommand;
        private ICommand _dashboardCommand;
        private ICommand _scheduleVehicleWorkOrderCommand;
        private ICommand _machineWorkOrdersCommand;
        private ICommand _newMachineRepairWorkOrderCommand;
        private ICommand _machineWorkOrderHistoryCommand;
        private ICommand _scheduleMachineWorkOrderCommand;
        private ICommand _addMachineCommand;
        private ICommand _updateMachineCommand;
        private ICommand _slittingSchedulerCommand;
        private ICommand _weeklyScheduleCommand;
        private bool _canExecute;
        public bool ReadyToExit =false;
        private User _user;
        private string _userWrapPanel;
        private List<MetaData> metaData;
        private bool _adminEnabled;
        private bool _productionEnabled;
        private bool _maintenanceEnabled;
        private bool _ordersEnabled;
        //private bool _workOrdersEnabled;
        private bool _stockEnabled;
        private bool _workStationsEnabled;

        private bool _productionActive;
        private bool _gradingActive;
        private bool _peelingActive;
        private bool _slittingActive;
        private bool _mixingActive;
        private string _liveTIme;
        private string _liveDate;
        private string userName;
        private string state;
        private List<UserPrivilages> privilages;
        DispatcherTimer Timer = new DispatcherTimer();
        private Grid TopContent = null;

        public TopContentViewModel(string UserName, string State, List<UserPrivilages> uPriv, Grid topContent, List<MetaData> md) 
        {           
            userName = UserName;
            state = State;
            privilages = uPriv;
            UserWrapPanel = "Hidden";
            TopContent = topContent;
            _canExecute = true;
            metaData = md;



            Timer.Tick += new EventHandler(Timer_Click);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }
        public TopContentViewModel()
        {          

            Timer.Tick += new EventHandler(Timer_Click);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
            UserWrapPanel = "Hidden";
            _canExecute = true;

        }

        private void Timer_Click(object sender, EventArgs e)
        {          
            DateTime d;
            d = DateTime.Now;
            LiveTIme = string.Format("{0:HH\\:mm\\:ss}", d);

            CurrentDateTime = d;
           // CheckCuringEndTime();
        }

        private void ShowAddNewMachine()
        {
            ChildWindowView myChildWindow = new ChildWindowView();
            myChildWindow.ShowAddNewMachine();
        }

        private void ShowUpdateMachine()
        {
            ChildWindowView myChildWindow = new ChildWindowView();
            myChildWindow.ShowUpdateMachine();
        }

        private void LogOut()
        {

             //Msg.Show("User Profile is under construction", "Under Construction", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);

            string fName = this.user.FirstName;
            if (Msg.Show("Hi " + fName + System.Environment.NewLine + "Are you sure you want to Log out from A1 Rubber Console?", "Log Out Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Information_Red, MsgBoxResult.Yes) == MsgBoxResult.Yes)
            {
                UserWrapPanel = "Hidden";
                Switcher.Switch(new LoginView(TopContent));
            }   
           
        }
        private void ExitApplication()
        {

            if (Msg.Show("Are you sure you want to exit from A1 Rubber Console", "Exit Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Information_Red, MsgBoxResult.Yes) == MsgBoxResult.Yes)
            {
                ReadyToExit = true;
                Timer.Stop();
                Application.Current.Shutdown();
            }
            else
            {
                ReadyToExit = false;
            }
        }
       

        private void ViewProfile()
        {
            Msg.Show("User Profile is under construction", "Under Construction", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);

        }

        

       

        //public bool WorkOrdersEnabled
        //{
        //    get
        //    {
        //        return _workOrdersEnabled;
        //    }
        //    set
        //    {
        //        _workOrdersEnabled = value;
        //        RaisePropertyChanged(() => this.WorkOrdersEnabled);
        //    }
        //}

        public bool MaintenanceEnabled
        {
            get
            {
                return _maintenanceEnabled;
            }
            set
            {
                _maintenanceEnabled = value;
                RaisePropertyChanged(() => this.MaintenanceEnabled);
            }
        }

        public bool AdminEnabled
        {
            get
            {
                return _adminEnabled;
            }
            set
            {
                _adminEnabled = value;
                RaisePropertyChanged(() => this.AdminEnabled);
            }
        }

        public bool ProductionEnabled
        {
            get
            {
                return _productionEnabled;
            }
            set
            {
                _productionEnabled = value;
                RaisePropertyChanged(() => this.ProductionEnabled);
            }
        }

        public bool StockEnabled
        {
            get
            {
                return _stockEnabled;
            }
            set
            {
                _stockEnabled = value;
                RaisePropertyChanged(() => this.StockEnabled);
            }
        }

        public bool OrdersEnabled
        {
            get
            {
                return _ordersEnabled;
            }
            set
            {
                _ordersEnabled = value;
                RaisePropertyChanged(() => this.OrdersEnabled);
            }
        }
        
        

        public string LiveTIme
        {
            get
            {
                return _liveTIme;
            }
            set
            {
                _liveTIme = value;
                RaisePropertyChanged(() => this.LiveTIme);
            }
        }

        public string LiveDate
        {
            get
            {
                return _liveDate;
            }
            set
            {
                _liveDate = value;
                RaisePropertyChanged(() => this.LiveDate);
            }
        }

        public bool ProductionActive
        {
            get
            {
                return _productionActive;
            }
            set
            {
                _productionActive = value;
                RaisePropertyChanged(() => this.ProductionActive);
            }
        }

        public bool GradingActive
        {
            get
            {
                return _gradingActive;
            }
            set
            {
                _gradingActive = value;
                RaisePropertyChanged(() => this.GradingActive);
            }
        }
        public bool MixingActive
        {
            get
            {
                return _mixingActive;
            }
            set
            {
                _mixingActive = value;
                RaisePropertyChanged(() => this.MixingActive);
            }
        }

        public bool SlittingActive
        {
            get
            {
                return _slittingActive;
            }
            set
            {
                _slittingActive = value;
                RaisePropertyChanged(() => this.SlittingActive);
            }
        }

        public bool PeelingActive
        {
            get
            {
                return _peelingActive;
            }
            set
            {
                _peelingActive = value;
                RaisePropertyChanged(() => this.PeelingActive);
            }
        }

        public User user
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                RaisePropertyChanged(() => this.user);
            }

        }

        public string UserWrapPanel 
        {
            get 
            { 
                return _userWrapPanel;            
            }
            set 
            {
                _userWrapPanel = value;
                RaisePropertyChanged(() => this.UserWrapPanel);
            } 
        }
        public bool WorkStationsEnabled 
        {
            get 
            {
                return _workStationsEnabled;            
            }
            set 
            {
                _workStationsEnabled = value;
                RaisePropertyChanged(() => this.WorkStationsEnabled);
            } 
        }

        
        #region COMMANDS



        public ICommand SlittingSchedulerCommand
        {
            get
            {

                return _slittingSchedulerCommand ?? (_slittingSchedulerCommand = new LogOutCommandHandler(() => Switcher.Switch(new SlittingSchedulerView(userName, state, privilages, metaData)), _canExecute));
            }
        }


        public ICommand VehicleWorkOrderHistoryCommand
        {
            get
            {

                return _vehicleWorkOrderHistoryCommand ?? (_vehicleWorkOrderHistoryCommand = new LogOutCommandHandler(() => Switcher.Switch(new VehicleWorkOrderHistoryView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand ReRollingCommand
        {
            get
            {

                return _reRollingCommand ?? (_reRollingCommand = new LogOutCommandHandler(() => Switcher.Switch(new ReRollingOrdersView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand CarouselSlittingCommand
        {
            get
            {

                return _carouselSlittingCommand ?? (_carouselSlittingCommand = new LogOutCommandHandler(() => Switcher.Switch(new CarouselSlitterView(userName, state, privilages, metaData)), _canExecute));
            }
        }
        public ICommand FlatBenchSlittingCommand
        {
            get
            {

                return _flatBenchSlittingCommand ?? (_flatBenchSlittingCommand = new LogOutCommandHandler(() => Switcher.Switch(new FlatBenchSlitterView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand CuringOrdersCommand
        {
            get
            {

                return _curingOrderCommand ?? (_curingOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new CuringView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand ExitCommand
        {
            get
            {

                return _exitCommand ?? (_exitCommand = new LogOutCommandHandler(() => ExitApplication(), _canExecute));
            }
        }

        public ICommand LogOutCommand
        {
            get
            {

                return _logOutCommand ?? (_logOutCommand = new LogOutCommandHandler(() => LogOut(), _canExecute));
            }
        }
        public ICommand ViewProfileCommand
        {
            get
            {
                return _viewProfileCommand ?? (_viewProfileCommand = new LogOutCommandHandler(() => ViewProfile(), _canExecute));
            }
        }


        public ICommand MixingSchedulerCommand
        {
            get
            {
                return _mixingSchedulerCommand ?? (_mixingSchedulerCommand = new LogOutCommandHandler(() => Switcher.Switch(new MixingProductionSchedulerView(userName, state, privilages, metaData)), _canExecute));
            }           
        }

        public ICommand ProdSchedulerCommand
        {
            get
            {
                return _prodSchedulerCommand ?? (_prodSchedulerCommand = new LogOutCommandHandler(() => Switcher.Switch(new ProductionSchedulerView(userName, state, privilages, metaData)), _canExecute));
            }           
        }

        public ICommand GradingOrderCommand
        {
            get
            {
                return _gradingOrderCommand ?? (_gradingOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new GradingScheduleView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand MixingOrderCommand
        {
            get
            {
                return _mixingOrderCommand ?? (_mixingOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new MixingScheduleView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand SlittingCommand
        {
            get
            {
                return _slittingCommand ?? (_slittingCommand = new LogOutCommandHandler(() => Switcher.Switch(new SlitingScheduleView(userName, state, privilages, metaData)), _canExecute));
            }
        }
        public ICommand PeelingCommand
        {
            get
            {
                return _peelingCommand ?? (_peelingCommand = new LogOutCommandHandler(() => Switcher.Switch(new PeelingOrdersView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand NewProdOrderCommand
        {
            get
            {
                return _newProdOrderCommand ?? (_newProdOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new AddProductionOrderView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand SlitPeelSchedulerCommand
        {
            get
            {
                return _slitPeelSchedulerCommand ?? (_slitPeelSchedulerCommand = new LogOutCommandHandler(() => Switcher.Switch(new SlitPeelProductionSchedulerView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand StockMaintenanceCommand
        {
            get
            {
                return null;// _stockMaintenanceCommand ?? (_stockMaintenanceCommand = new LogOutCommandHandler(() => Switcher.Switch(new StockMaintenanceView(userName, state, privilages)), _canExecute));
            }
        }

        public ICommand GradingHistoryCommand
        {
            get
            {
                return _gradingHistoryCommand ?? (_gradingHistoryCommand = new LogOutCommandHandler(() => Switcher.Switch(new GradingHistoryView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand MixingHistoryCommand
        {
            get
            {
                return _mixingHistoryCommand ?? (_mixingHistoryCommand = new LogOutCommandHandler(() => Switcher.Switch(new MixingHistoryView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand ViewGradedStockCommand
        {
            get
            {
                return _viewGradedStockCommand ?? (_viewGradedStockCommand = new LogOutCommandHandler(() => Switcher.Switch(new ViewGradedStockView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand AmendOrderCommand
        {
            get
            {
                return _amendOrderCommand ?? (_amendOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new AmendOrderView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand FinishedGoodsCommand
        {
            get
            {
                return _finishedGoodsCommand ?? (_finishedGoodsCommand = new LogOutCommandHandler(() => Switcher.Switch(new ViewGradedStockView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand BlockLogCommand
        {
            get
            {
                return _blockLogCommand ?? (_blockLogCommand = new LogOutCommandHandler(() => Switcher.Switch(new BlockLogStockView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand ReRollingHistoryCommand
        {
            get
            {
                return _reRollingHistoryCommand ?? (_reRollingHistoryCommand = new LogOutCommandHandler(() => Switcher.Switch(new ReRollingHistoryView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand PeelingHistoryCommand
        {
            get
            {
                return _peelingHistoryCommand ?? (_peelingHistoryCommand = new LogOutCommandHandler(() => Switcher.Switch(new PeelingHistoryView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand SlittingHistoryCommand
        {
            get
            {
                return _slittingHistoryCommand ?? (_slittingHistoryCommand = new LogOutCommandHandler(() => Switcher.Switch(new SlittingHistoryView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand NewVehicleMaintenanceWorkOrderCommand
        {
            get
            {
                return _newVehicleMaintenanceWorkOrderCommand ?? (_newVehicleMaintenanceWorkOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new NewRoutineVehicleWorkOrderView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand UpdateVehicleWorkOrderCommand
        {
            get
            {
                return _updateVehicleWorkOrderCommand ?? (_updateVehicleWorkOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new AddVehicleWorkOrderView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand VehicleWorkOrdersCommand
        {
            get
            {
                return _vehicleWorkOrdersCommand ?? (_vehicleWorkOrdersCommand = new LogOutCommandHandler(() => Switcher.Switch(new VehicleWorkOrdersView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand NewVehicleWorkOrderCommand
        {
            get
            {
                return _newVehicleWorkOrderCommand ?? (_newVehicleWorkOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new NewVehicleRepairWorkOrderView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand DashboardCommand
        {
            get
            {
                return _dashboardCommand ?? (_dashboardCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand ScheduleVehicleWorkOrderCommand
        {
            get
            {
                return _scheduleVehicleWorkOrderCommand ?? (_scheduleVehicleWorkOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new ScheduleVehicleWorkOrdersView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand MachineWorkOrdersCommand
        {
            get
            {
                return _machineWorkOrdersCommand ?? (_machineWorkOrdersCommand = new LogOutCommandHandler(() => Switcher.Switch(new MachineWorkOrdersView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand NewMachineRepairWorkOrderCommand
        {
            get
            {
                return _newMachineRepairWorkOrderCommand ?? (_newMachineRepairWorkOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new NewMachineRepairWorkOrderView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand MachineWorkOrderHistoryCommand
        {
            get
            {
                return _machineWorkOrderHistoryCommand ?? (_machineWorkOrderHistoryCommand = new LogOutCommandHandler(() => Switcher.Switch(new MachineWorkOrderHistoryView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand ScheduleMachineWorkOrderCommand
        {
            get
            {
                return _scheduleMachineWorkOrderCommand ?? (_scheduleMachineWorkOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new ScehduleMachineWorkOrdersView(userName, state, privilages, metaData)), _canExecute));
            }
        }

        public ICommand AddMachineCommand
        {
            get
            {
                return _addMachineCommand ?? (_addMachineCommand = new LogOutCommandHandler(() => ShowAddNewMachine(), _canExecute));
            }
        }

        public ICommand UpdateMachineCommand
        {
            get
            {
                return _updateMachineCommand ?? (_updateMachineCommand = new LogOutCommandHandler(() => ShowUpdateMachine(), _canExecute));
            }
        }

        public ICommand WeeklyScheduleCommand
        {
            get
            {
                return _weeklyScheduleCommand ?? (_weeklyScheduleCommand = new LogOutCommandHandler(() => Switcher.Switch(new WeeklyScheduleFullView(userName, state, privilages, metaData)), _canExecute));
            }
        }

       
        
        #endregion

    }
}
