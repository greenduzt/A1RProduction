using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model.Vehicles;
using A1QSystem.View;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.VehicleWorkOrders
{
    public class NewVehicleWorkOrderViewModel : ViewModelBase
    {
        private VehicleWorkOrder _vehicleWorkOrder;
        private ObservableCollection<Vehicle> _vehicles;
        private ObservableCollection<MaintenanceType> _maintenanceType;
        private int _selectedVehicle;
        private int _selectedInspectionType;
        private DateTime _currentDate;
        private DateTime _selectedDate;
        private bool _onceChecked;
        private bool _sixMonthChecked;
        private bool _twoYearsChecked;
        private bool _dailyChecked;
        private bool _oneYearChecked;
        private bool _oneMonthChecked;
        private int _maintenanceFrequency;
        private bool _maintenanceSeqEnabled;
        private int selectedVehicleCategoryId;
        private string _vehicleType;
        private bool _createBtnEnabled;
        private bool _datagridActive;
        private string userName;
        private string _radioVisiblity;
        private string _vehicleLocation;

        private bool canExecute;
        public event Action<int> Closed;
        private DelegateCommand _closeCommand;
        private ICommand _createWorkOrderCommand;
        private ICommand _clearCommand;
        private ICommand _selectionChangedCommand;


        public NewVehicleWorkOrderViewModel(string un)
        {
            userName = un;
            VehicleWorkOrder = new VehicleWorkOrder();
            VehicleWorkOrder.VehicleMaintenanceInfo = new ObservableCollection<VehicleMaintenanceInfo>();
            Vehicles = new ObservableCollection<Vehicle>();
            MaintenanceType = new ObservableCollection<MaintenanceType>();
            CurrentDate = DateTime.Now;
            SelectedDate = CurrentDate;
            MaintenanceSeqEnabled = false;
            _closeCommand = new DelegateCommand(CloseForm);
            canExecute = true;
            CreateBtnEnabled = false;  
            LoadVehicles();                       
        }
        private void LoadVehicles()
        {
            Vehicles=DBAccess.GetAllVehicles();
            Vehicles.Add(new Vehicle() { ID=0,VehicleString="Select"});
            SelectedVehicle = 0;
        }
        private void LoadMaintenanceInfo(int num)
        {
            if (num > 0)
            {              
                if (selectedVehicleCategoryId > 0)
                {
                    Vehicle v = Vehicles.FirstOrDefault(x=>x.ID == SelectedVehicle);
                    VehicleWorkOrder.VehicleMaintenanceInfo = DBAccess.GetMaintenanceInfoBySequence(num, selectedVehicleCategoryId,v.StockLocation.ID);
                    if (VehicleWorkOrder.VehicleMaintenanceInfo.Count == 0)
                    {
                        CreateBtnEnabled = false;
                        Msg.Show("No work description found for the vehicle", "Work Description Not Found", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
                    }
                    else
                    {
                        CreateBtnEnabled = true;
                    }
                }
                else
                {
                    Msg.Show("Cannot load vehicle category id. Please try again later ", "Cannot Load Vehicle Category Id", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }
            }
            else
            {               
                Msg.Show("Cannot load vehicle sequence id. Please try again later ", "Cannot Load Vehicle Sequence Id", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
        }      

        private void AddOrder()
        {

        }

        //private void MaintenanceGroupSelected(object parameter)
        //{
        //    //int index = VehicleWorkOrder.MaintenanceGroup.IndexOf(parameter as VehicleMaintenanceInfop);
        //    //if (index > -1 && index < VehicleWorkOrder.MaintenanceGroup.Count)
        //    //{
        //    //    //MaintenanceType[index] = null;
        //    //    var data1 = MaintenanceGroup.FirstOrDefault(c => c.Code == VehicleWorkOrder.MaintenanceGroup[index].Code);
        //    //    //var data2 = MaintenanceGroup.FirstOrDefault(c => c.Code == MaintenanceTypeList[index].VehicleMaintenanceGroupID == data1.ID);

        //    //    foreach (var item in MaintenanceTypeList)
        //    //    {
        //    //        if(item.VehicleMaintenanceGroupID == data1.ID)
        //    //        {
        //    //            MaintenanceType.Add(new MaintenanceType() { ID = item.ID,Code = item.Code,Description = item.Description,VehicleMaintenanceGroupID = item.VehicleMaintenanceGroupID});
        //    //        }
        //    //    }

        //    //    //Console.WriteLine(data2.ID);
        //    //}
        //}

        private void ClearOrder()
        {
            MaintenanceSeqEnabled = false;
            OneMonthChecked = false;
            SixMonthChecked = false;
            OneYearChecked = false;
            TwoYearsChecked = false;
            SelectedDate = DateTime.Now;
            SelectedVehicle = 0;
            VehicleWorkOrder.VehicleMaintenanceInfo.Clear();
            CreateBtnEnabled = false;
            DatagridActive = false;
            VehicleLocation = string.Empty;
        }

        private void GetVehicleCategory()
        {
            var data = Vehicles.SingleOrDefault(c => c.ID == SelectedVehicle);
            selectedVehicleCategoryId= data.VehicleCategory.ID;
            VehicleType = data.VehicleCategory.VehicleType;
        }

        private void CreateWorkOrder()
        {
            Int32 res = 0;
            bool recExists = false;
            BackgroundWorker worker = new BackgroundWorker();
            ChildWindowView LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Working");

            worker.DoWork += (_, __) =>
            {
                //Check if there is a work order existing for the vehicle
                recExists=CheckWorkOrderExist();
                if (recExists == false)
                {
                    VehicleWorkOrder.FirstServiceDate = SelectedDate;
                    VehicleWorkOrder.Vehicle = new Vehicle() { ID = SelectedVehicle };
                    VehicleWorkOrder.MaintenanceFrequency = GetMaintenanceFreq(MaintenanceFrequency);
                    VehicleWorkOrder.IsCompleted = false;
                    VehicleWorkOrder.CreatedDate = DateTime.Now;
                    VehicleWorkOrder.CreatedBy = userName;
                    VehicleWorkOrder.NextServiceDate = SelectedDate;//CalculateNextServiceDate(MaintenanceFrequency);
                    VehicleWorkOrder.Status = VehicleWorkOrderEnum.Pending.ToString();
                    VehicleWorkOrder.WorkOrderType = VehicleWorkOrderTypesEnum.Maintenance.ToString();
                    VehicleWorkOrder.Urgency = 2;

                    res = DBAccess.InsertNewVehicleWorkOrder(VehicleWorkOrder);
                }                
            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();

                if (recExists)
                {
                    Msg.Show("Work order exists for this vehicle " + System.Environment.NewLine + "Please select a different date", "Work Order Exist", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.OK);
                }
                else
                {
                    if (res > 0)
                    {
                        Msg.Show("Work order no " + res + " was created", "Work Order Created", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
                        ClearOrder();
                    }
                }
            };
            worker.RunWorkerAsync();            
        }

        private string GetMaintenanceFreq(int n)
        {
            string str = string.Empty;

            switch (n)
            {
                case 1: str = "Daily";
                    break;
                case 2: str = "1 Month";
                    break;
                case 3: str = "6 Months";
                    break;
                case 4: str = "1 Year";
                    break;
                case 5: str = "2 Years";
                    break;
                case 6: str = "As Required";
                    break;
                case 7: str = "Once";
                    break;
            }
            return str;
        }

        private DateTime? CalculateNextServiceDate(int n)
        {
            DateTime? d = null;
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();

            switch (n)
            {
                case 1: d = bdg.SkipWeekends(SelectedDate.AddDays(1));
                    break;
                case 2: d = bdg.SkipWeekends(SelectedDate.AddMonths(1));
                    break;
                case 3: d = bdg.SkipWeekends(SelectedDate.AddMonths(6));
                    break;
                case 4: d = bdg.SkipWeekends(SelectedDate.AddYears(1));
                    break;
                case 5: d = bdg.SkipWeekends(SelectedDate.AddYears(2));
                    break;
            }

            return d;
        }

        private bool CheckWorkOrderExist()
        {
            bool exist = false;
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            DateTime startDate = CurrentDate;
            DateTime endDate = CurrentDate;

            switch (MaintenanceFrequency)
            {
                case 2: startDate = bdg.SkipWeekends(SelectedDate.AddDays(-30)); endDate = bdg.SkipWeekends(SelectedDate.AddDays(30));
                    break;
                case 3: startDate = bdg.SkipWeekends(SelectedDate.AddDays(-180)); endDate = bdg.SkipWeekends(SelectedDate.AddDays(180));
                    break;
                case 4: startDate = bdg.SkipWeekends(SelectedDate.AddDays(-365)); endDate = bdg.SkipWeekends(SelectedDate.AddDays(365));
                    break;
                case 5: startDate = bdg.SkipWeekends(SelectedDate.AddDays(-730)); endDate = bdg.SkipWeekends(SelectedDate.AddDays(730));
                    break;
            }

            //Check db for dates
            exist = DBAccess.CheckWorkOrderExistBeforInsert(SelectedVehicle, startDate, endDate);

            //Console.WriteLine(startDate + " " + endDate + " " + exist);

            return exist;
        }

        private void CheckIfRadioChecked()
        {

            if (OneMonthChecked == true || SixMonthChecked == true || OneYearChecked == true || TwoYearsChecked == true)
            {
                DatagridActive = true;
            }
            else
            {
                DatagridActive = false;
            }

        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                int r = 1;
                Closed(r);
            }
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        #region PUBLIC_PROPERTIES       

        

        public string RadioVisiblity
        {
            get
            {
                return _radioVisiblity;
            }
            set
            {
                _radioVisiblity = value;
                RaisePropertyChanged(() => this.RadioVisiblity);                
            }
        }

        public string VehicleType
        {
            get
            {
                return _vehicleType;
            }
            set
            {
                _vehicleType = value;
                RaisePropertyChanged(() => this.VehicleType);                
            }
        }

        public bool MaintenanceSeqEnabled
        {
            get
            {
                return _maintenanceSeqEnabled;
            }
            set
            {
                _maintenanceSeqEnabled = value;
                RaisePropertyChanged(() => this.MaintenanceSeqEnabled);                
            }
        }

        public int MaintenanceFrequency
        {
            get
            {
                return _maintenanceFrequency;
            }
            set
            {
                _maintenanceFrequency = value;
                RaisePropertyChanged(() => this.MaintenanceFrequency);                
            }
        }

        public bool OnceChecked
        {
            get
            {
                return _onceChecked;
            }
            set
            {
                _onceChecked = value;
                RaisePropertyChanged(() => this.OnceChecked);

                if (OnceChecked == true)
                {
                    LoadMaintenanceInfo(7);
                    MaintenanceFrequency = 7;
                }

                CheckIfRadioChecked();                   
            }
        }

        public bool SixMonthChecked
        {
            get
            {
                return _sixMonthChecked;
            }
            set
            {
                _sixMonthChecked = value;
                RaisePropertyChanged(() => this.SixMonthChecked);

                if (SixMonthChecked == true)
                {
                    LoadMaintenanceInfo(3);
                    MaintenanceFrequency = 3;
                }
                CheckIfRadioChecked();
            }
        }

        public bool TwoYearsChecked
        {
            get
            {
                return _twoYearsChecked;
            }
            set
            {
                _twoYearsChecked = value;
                RaisePropertyChanged(() => this.TwoYearsChecked);

                if (TwoYearsChecked == true)
                {
                    LoadMaintenanceInfo(5);
                    MaintenanceFrequency = 5;
                }
                CheckIfRadioChecked();
            }
        }

        public bool DailyChecked
        {
            get
            {
                return _dailyChecked;
            }
            set
            {
                _dailyChecked = value;
                RaisePropertyChanged(() => this.DailyChecked);

                if (DailyChecked == true)
                {
                    LoadMaintenanceInfo(1);
                    MaintenanceFrequency = 1;
                }                    
            }
        }

        public bool OneYearChecked
        {
            get
            {
                return _oneYearChecked;
            }
            set
            {
                _oneYearChecked = value;
                RaisePropertyChanged(() => this.OneYearChecked);

                if (OneYearChecked == true)
                {
                    LoadMaintenanceInfo(4);
                    MaintenanceFrequency = 4;
                }
                CheckIfRadioChecked();
            }
        }

        public bool OneMonthChecked
        {
            get
            {
                return _oneMonthChecked;
            }
            set
            {
                _oneMonthChecked = value;
                RaisePropertyChanged(() => this.OneMonthChecked);

                if (OneMonthChecked == true)
                {
                    LoadMaintenanceInfo(2);
                    MaintenanceFrequency = 2;
                }
                CheckIfRadioChecked();
            }
        }     

        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                RaisePropertyChanged(() => this.SelectedDate);
            }
        }

        public DateTime CurrentDate
        {
            get
            {
                return _currentDate;
            }
            set
            {
                _currentDate = value;
                RaisePropertyChanged(() => this.CurrentDate);               
            }
        }      

        public ObservableCollection<MaintenanceType> MaintenanceType
        {
            get
            {
                return _maintenanceType;
            }
            set
            {
                _maintenanceType = value;
                RaisePropertyChanged(() => this.MaintenanceType);               
            }
        }       

        public VehicleWorkOrder VehicleWorkOrder
        {
            get
            {
                return _vehicleWorkOrder;
            }
            set
            {
                _vehicleWorkOrder = value;
                RaisePropertyChanged(() => this.VehicleWorkOrder);
            }
        }       

        public ObservableCollection<Vehicle> Vehicles
        {
            get
            {
                return _vehicles;
            }
            set
            {
                _vehicles = value;
                RaisePropertyChanged(() => this.Vehicles);
            }
        }

        public int SelectedVehicle
        {
            get
            {
                return _selectedVehicle;
            }
            set
            {
                _selectedVehicle = value;
                RaisePropertyChanged(() => this.SelectedVehicle);

                MaintenanceSeqEnabled = false;
                OneMonthChecked = false;
                SixMonthChecked = false;
                OneYearChecked = false;
                TwoYearsChecked = false;
                SelectedDate = DateTime.Now;
                VehicleType = string.Empty;
                VehicleWorkOrder.VehicleMaintenanceInfo.Clear();
               
                if(SelectedVehicle > 0)
                {
                   
                    MaintenanceSeqEnabled = true;
                    GetVehicleCategory();

                    Vehicle v = Vehicles.FirstOrDefault(x=>x.ID == SelectedVehicle);
                    if(v.StockLocation.ID == 1)
                    {
                        RadioVisiblity = "Visible";
                    }
                    else if(v.StockLocation.ID == 2)
                    {
                        RadioVisiblity = "Collapsed";
                    }

                    VehicleLocation = v.StockLocation.StockName;
                }
            }
        }

        public int SelectedInspectionType
        {
            get
            {
                return _selectedInspectionType;
            }
            set
            {
                _selectedInspectionType = value;
                RaisePropertyChanged(() => this.SelectedInspectionType);               
            }
        }

        public string VehicleLocation
        {
            get
            {
                return _vehicleLocation;
            }
            set
            {
                _vehicleLocation = value;
                RaisePropertyChanged(() => this.VehicleLocation);               
            }
        }

        

        public bool CreateBtnEnabled
        {
            get
            {
                return _createBtnEnabled;
            }
            set
            {
                _createBtnEnabled = value;
                RaisePropertyChanged(() => this.CreateBtnEnabled);
            }
        }

        public bool DatagridActive
        {
            get
            {
                return _datagridActive;
            }
            set
            {
                _datagridActive = value;
                RaisePropertyChanged(() => this.DatagridActive);
            }
        }       

        #endregion


        #region COMMANDS

        public ICommand CreateWorkOrderCommand
        {
            get
            {
                return _createWorkOrderCommand ?? (_createWorkOrderCommand = new A1QSystem.Commands.LogOutCommandHandler(() => CreateWorkOrder(), canExecute));
            }
        }

        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new A1QSystem.Commands.LogOutCommandHandler(() => ClearOrder(), canExecute));
            }
        }
     
        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

       
        //public ICommand SelectionChangedCommand
        //{
        //    get
        //    {
        //        if (_selectionChangedCommand == null)
        //        {
        //            _selectionChangedCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, MaintenanceGroupSelected);
        //        }
        //        return _selectionChangedCommand;
        //    }
        //}
        

        #endregion
    }
}
