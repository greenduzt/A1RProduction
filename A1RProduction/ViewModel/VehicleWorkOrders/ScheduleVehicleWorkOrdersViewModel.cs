using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Vehicles;
using A1QSystem.View;
using A1QSystem.View.AdminDashboard;
using A1QSystem.View.VehicleWorkOrders;
using A1QSystem.ViewModel.Stock;
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
    public class ScheduleVehicleWorkOrdersViewModel : ViewModelBase
    {
        private VehicleWorkOrderSchedule _vehicleWorkOrderSchedule;
        private List<VehicleCategory> _vehicleCategory;
        private string userName;
        private string state;
        private List<UserPrivilages> userPrivilages;
        private bool canExecute;
        private bool _tickAll;
        private bool _clearEnabled;
        private bool _submitEnabled;

        private List<StockLocation> _stockLocation;
        private ObservableCollection<VehicleMaintenanceInfo> vehicleMaintenanceInfo;
        private List<MetaData> metaData;
        private string _version;
        private string _submitBackground;
        private string _clearBackground;
        private int _selectedVehicleType;
        private DateTime? _selectedDate;
        private DateTime _currentDate;
        private bool _oneMonthChecked;
        private bool _sixMonthChecked;
        private bool _oneYearChecked;
        private bool _twoYearsChecked;
        private int _selectedLocationID;
        private string _mainDesBackground;
        private bool _mainDisEnabled;
        private string _radioVisibility;

        private ICommand _homeCommand;
        private ICommand _vehiclesCommand;
        private ICommand _adminDashboardCommand;
        private ICommand _createWorkOrderCommand;
        private ICommand _clearCommand;
        private ICommand _viewMaintenanceCommand;

        public ScheduleVehicleWorkOrdersViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            userPrivilages = up;
            canExecute = true;
            metaData = md;
            CurrentDate = DateTime.Now;
            
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;

            vehicleMaintenanceInfo = new ObservableCollection<VehicleMaintenanceInfo>();            
            VehicleWorkOrderSchedule = new VehicleWorkOrderSchedule();
            VehicleWorkOrderSchedule.ScheduledVehicle = new ObservableCollection<ScheduledVehicle>();
            VehicleWorkOrderSchedule.StartDate = CurrentDate;
            VehicleCategory = new List<VehicleCategory>();
            VehicleCategory = DBAccess.CheckVehicleWorkOrderCompleted();
            VehicleCategory.Add(new VehicleCategory() { ID=0,VehicleType="Select"});
            SelectedVehicleType = 0;

            RadioVisibility = "Visible";
            SubmitBackground = "#FFDEDEDE";
            ClearBackground = "#FFDEDEDE";
            MainDesBackground = "#FFDEDEDE";
            MainDisEnabled = false;
            ClearEnabled = false;
            SubmitEnabled = false;
            LoadLocations();
        }


        private void GetVehciles()
        {
            if (SelectedVehicleType > 0)
            {
                VehicleWorkOrderSchedule.ScheduledVehicle = DBAccess.GetVehiclesByCategoryID(SelectedVehicleType,SelectedLocationID);
            }
            else
            {
                VehicleWorkOrderSchedule.ScheduledVehicle.Clear();
            }
        }

        private void LoadLocations()
        {
            StockLocation = DBAccess.GetStockLocations();
            StockLocation.Add(new StockLocation() { ID=0,StockName="Select"});
        }

        private void CreateWorkOrder()
        {
            ChildWindowView LoadingScreen;
            List<ScheduledVehicleDetails> scheduledVehicleDetailsList = new List<ScheduledVehicleDetails>();
            vehicleMaintenanceInfo = DBAccess.GetAllMaintenanceInfo(SelectedVehicleType,SelectedLocationID);
            List<Tuple<int, DateTime, Int32,int>> exFreqList = DBAccess.GetExistingVehiclesWorkFrequencies((DateTime)VehicleWorkOrderSchedule.StartDate);
            List<int> freqList = CrateFrequencyList();
            List<Int32> cancelOrders = new List<int>();
            int noOfItems = freqList.Count;            
            int x = 0;
            //(noOfRec, cancelledOrders)
            List<int> unDone = new List<int>();
            Tuple<int, int> result = null;
            BackgroundWorker worker = new BackgroundWorker();
            LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Working");

            worker.DoWork += (_, __) =>
            {
                foreach (var itemFL in freqList)
                {
                    BusinessDaysGenerator bdg = new BusinessDaysGenerator();
                    DateTime date = (DateTime)VehicleWorkOrderSchedule.StartDate;
                 
                    if (x > 0)//Avoid the frst item
                    {                
                        date = CalculateNextServiceDate(itemFL, date);
                    }

                    int y = 0;

                    foreach (var item in VehicleWorkOrderSchedule.ScheduledVehicle)
                    {
                        if (item.IsSelected == true)
                        {         
                            //Check if there is a work order existing for the vehicle
                            bool woExist = exFreqList.Any(z => z.Item1 == itemFL && (z.Item2.Date - date.Date).TotalDays < GetTotalDays(itemFL) && z.Item4 == item.ID);
                            if(woExist==false)
                            {
                                //Check if thre is a low frequency exist, if so cancel
                                var data = exFreqList.FirstOrDefault(z => z.Item1 < itemFL && z.Item2.Date == date.Date && z.Item4 == item.ID);
                                if(data != null)
                                {
                                    //Workorders to cancel
                                    cancelOrders.Add(data.Item3);
                                }
                                scheduledVehicleDetailsList.Add(new ScheduledVehicleDetails() 
                                                                    { 
                                                                        FirstServiceDate = date,                                                               
                                                                        Vehicle = new Vehicle() 
                                                                        { 
                                                                            ID = item.ID,
                                                                            SerialNumber=item.SerialNumber,
                                                                            VehicleBrand=item.VehicleBrand,
                                                                            VehicleCode=item.VehicleCode,
                                                                            VehicleDescription=item.VehicleDescription
                                                                        }, 
                                                                        MaintenanceFrequency = ConvertToStringFreq(itemFL), 
                                                                        IsCompleted =false,
                                                                        CreatedDate = DateTime.Now,
                                                                        CreatedBy = userName,
                                                                        Status = VehicleWorkOrderEnum.Pending.ToString(),
                                                                        NextServiceDate = date, 
                                                                        WorkOrderType = VehicleWorkOrderTypesEnum.Maintenance.ToString(),
                                                                        Urgency = 2,
                                                                        VehicleMaintenanceInfo = GetVehicleMaintenanceInfo(itemFL)
                                                                    });
                                if (y == 5)
                                {
                                    date = bdg.SkipWeekends(date.AddDays(1));
                                    y = 0;
                                }
                                y++;
                            } 
                            else
                            {
                                unDone.Add(item.ID);
                            }
                        }                
                    }
                    x++;
                }                
                result = DBAccess.InsertNewVehicleWorkOrderCollection(scheduledVehicleDetailsList, cancelOrders);
            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();
                string res1 = string.Empty;
                string res2 = string.Empty;
                string res3 = string.Empty;

                if (scheduledVehicleDetailsList.Count > 0)
                {
                    
                    if (result.Item1 > 0)
                    {
                        res1 = result.Item1 + " Vehicle Work orders were created" + System.Environment.NewLine;
                    }
                    if (result.Item2 > 0)
                    {
                        res2 = result.Item2 + " Vehicle Work orders were upgraded" + System.Environment.NewLine;
                    }
                }

                if (unDone.Count > 0)
                {
                    res3 = unDone.Count + " Vehicle Work Orders were not created as they are already existing";
                }

                ClearData();
                Msg.Show(res1 + res2 + res3, "Vehicle Work Order Result", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
            };
            worker.RunWorkerAsync();            

        }


        //private DateTime CheckDateExist(List<DateTime> existingDates,DateTime date)
        //{
        //    BusinessDaysGenerator bdg = new BusinessDaysGenerator();
        //    bool data;
        //    do
        //    {
        //        data = existingDates.Any(z => z == date);
        //        if (data == true)
        //        {
        //            date = bdg.SkipWeekends(date);
        //            data = existingDates.Any(z => z == date);
        //        }

        //    } while (data == true);

        //    return date;
        //}

        private List<int> CrateFrequencyList()
        {
            List<int> f = new List<int>();
            if(OneMonthChecked==true)
            {
                f.Add(1);
            }
            if (SixMonthChecked == true)
            {
                f.Add(2);
            }
            if (OneYearChecked == true)
            {
                f.Add(3);
            }
            if (TwoYearsChecked == true)
            {
                f.Add(4);
            }

            return f;
        }

        private DateTime CalculateNextServiceDate(int n, DateTime date)
        {
            DateTime d = CurrentDate;
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();

            switch (n)
            {
                case 1: d = bdg.SkipWeekends(date.AddMonths(1));
                    break;
                case 2: d = bdg.SkipWeekends(date.AddMonths(6));
                    break;
                case 3: d = bdg.SkipWeekends(date.AddYears(1));
                    break;
                case 4: d = bdg.SkipWeekends(date.AddYears(2));
                    break;                
            }

            return d;
        }

        private static string ConvertToStringFreq(int n)
        {
            string x = string.Empty;
            switch (n)
            {
                case 1 : x = "1 Month";
                    break;
                case 2: x = "6 Months";
                    break;
                case 3: x = "1 Year";
                    break;
                case 4: x = "2 Years";
                    break;
                default:
                    break;
            }

            return x;
        }

        private ObservableCollection<VehicleMaintenanceInfo> GetVehicleMaintenanceInfo(int seq)
        {

            ObservableCollection<VehicleMaintenanceInfo> vmi = new ObservableCollection<VehicleMaintenanceInfo>();

            foreach (var item in vehicleMaintenanceInfo)
            {
                if (seq == 1)
                {
                    if (item.VehicleMaintenanceSequence.ID == 2)
                    {                   
                        vmi.Add(item);
                    }
                }
                else if (seq == 2)
                {
                    if (item.VehicleMaintenanceSequence.ID == 2 || item.VehicleMaintenanceSequence.ID == 3)
                    {
                        vmi.Add(item);
                    }
                }
                else if (seq == 3)
                {
                    if (item.VehicleMaintenanceSequence.ID == 2 || item.VehicleMaintenanceSequence.ID == 3 || item.VehicleMaintenanceSequence.ID == 4)
                    {
                        vmi.Add(item);
                    }
                }
                else if (seq == 4)
                {
                    if (item.VehicleMaintenanceSequence.ID == 2 || item.VehicleMaintenanceSequence.ID == 3 || item.VehicleMaintenanceSequence.ID == 4 || item.VehicleMaintenanceSequence.ID == 5)
                    {
                        vmi.Add(item);
                    }
                }
            }

            return vmi;
        }

        private int GetTotalDays(int seq)
        {
            int x = 0;

            switch (seq)
            {
                case 1: x = 30;
                    break;
                case 2: x = 180;
                    break;
                case 3: x = 365;
                    break;
                case 4: x = 730;
                    break;
                default:
                    break;
            }

            return x;
        }

        private void ClearData()
        {
            VehicleWorkOrderSchedule.StartDate = CurrentDate;
            OneMonthChecked = false;
            SixMonthChecked = false;
            OneYearChecked = false;
            TwoYearsChecked = false;
            SelectedVehicleType = 0;
            SelectedLocationID = 0;
            VehicleWorkOrderSchedule.ScheduledVehicle.Clear();
            
            SubmitBackground = "#FFDEDEDE";
            ClearBackground = "#FFDEDEDE";
            MainDesBackground = "#FFDEDEDE";
            MainDisEnabled = false;
            ClearEnabled = false;
            SubmitEnabled = false;
            TickAll = false;
        }

        private void EnDisSubmit()
        {

            if (SelectedVehicleType > 0 && SelectedLocationID > 0 && (OneMonthChecked == true || SixMonthChecked == true || OneYearChecked == true || TwoYearsChecked == true))
            {
                SubmitEnabled = true;
                SubmitBackground = "#FF787C7A";
            }
            else
            {
                SubmitBackground = "#FFDEDEDE";
                SubmitEnabled = false;
            }
        }

        private void EnDisClear()
        {
            if (SelectedVehicleType > 0 && SelectedLocationID > 0 && (OneMonthChecked == true || SixMonthChecked == true || OneYearChecked == true || TwoYearsChecked == true))
            {
                ClearEnabled = true ;
                ClearBackground = "#FF787C7A";
            }
            else
            {
                ClearBackground = "#FFDEDEDE";
                ClearEnabled = false;
            }
        }

        private void EnDisMainDesButton()
        {
            if(SelectedLocationID > 0 && SelectedVehicleType > 0)
            {
                MainDesBackground = "#FF787C7A";
                MainDisEnabled = true;
            }
            else
            {
                MainDesBackground = "#FFDEDEDE";
                MainDisEnabled = false;
            }
        }

        private void ViewMaintenanceData()
        {
            ChildWindowView myChildWindow = new ChildWindowView();
            myChildWindow.vehicleMaintDes_Closed += (r =>
            {
                if(r == 1)
                {
                    Console.WriteLine("It is closed now");
                }
                //VehicleWorkOrder.Clear();
                //LoadWorkOrders();
            });
            myChildWindow.ShowVehicleMainDes(SelectedLocationID, SelectedVehicleType, userName);
        }

        #region PUBLIC_PROPERTIES



        public int SelectedLocationID
        {
            get
            {
                return _selectedLocationID;
            }
            set
            {
                _selectedLocationID = value;
                RaisePropertyChanged(() => this.SelectedLocationID);
                EnDisSubmit();
                SelectedVehicleType = 0;
                EnDisMainDesButton();
                if(SelectedLocationID == 2)
                {
                    RadioVisibility = "Collapsed";
                }
                else
                {
                    RadioVisibility = "Visible";
                }
            }
        }

        public List<StockLocation> StockLocation
        {
            get
            {
                return _stockLocation;
            }
            set
            {
                _stockLocation = value;
                RaisePropertyChanged(() => this.StockLocation);
               
            }
        }

        public VehicleWorkOrderSchedule VehicleWorkOrderSchedule
        {
            get
            {
                return _vehicleWorkOrderSchedule;
            }
            set
            {
                _vehicleWorkOrderSchedule = value;
                RaisePropertyChanged(() => this.VehicleWorkOrderSchedule);
               
            }
        }

        public List<VehicleCategory> VehicleCategory
        {
            get
            {
                return _vehicleCategory;
            }
            set
            {
                _vehicleCategory = value;
                RaisePropertyChanged(() => this.Version);
            }
        }

        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
                RaisePropertyChanged(() => this.Version);
            }
        }

        public string SubmitBackground
        {
            get
            {
                return _submitBackground;
            }
            set
            {
                _submitBackground = value;
                RaisePropertyChanged(() => this.SubmitBackground);
            }
        }

        public string ClearBackground
        {
            get
            {
                return _clearBackground;
            }
            set
            {
                _clearBackground = value;
                RaisePropertyChanged(() => this.ClearBackground);
            }
        }

      
        public int SelectedVehicleType
        {
            get
            {
                return _selectedVehicleType;
            }
            set
            {
                _selectedVehicleType = value;
                RaisePropertyChanged(() => this.SelectedVehicleType);
                GetVehciles();
                EnDisSubmit();
                EnDisClear();
                EnDisMainDesButton();
            }
        }

        public DateTime? SelectedDate
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

        public bool TickAll
        {
            get
            {
                return _tickAll;
            }
            set
            {
                _tickAll = value;
                RaisePropertyChanged(() => this.TickAll);

                if(TickAll ==true)
                {
                    foreach (var item in VehicleWorkOrderSchedule.ScheduledVehicle)
                    {
                        item.IsSelected = true;
                    }
                }
                else
                {
                    foreach (var item in VehicleWorkOrderSchedule.ScheduledVehicle)
                    {
                        item.IsSelected = false;
                    }
                }
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
                EnDisSubmit();
                EnDisClear();
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
                EnDisSubmit();
                EnDisClear();
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
                EnDisSubmit();
                EnDisClear();
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
                EnDisSubmit();
                EnDisClear();
            }
        }
        //public int DaysBetweenVehicles
        //{
        //    get
        //    {
        //        return _daysBetweenVehicles;
        //    }
        //    set
        //    {
        //        _daysBetweenVehicles = value;
        //        RaisePropertyChanged(() => this.DaysBetweenVehicles);
        //        EnDisSubmit();
        //        EnDisClear();
        //    }
        //}

        public bool ClearEnabled
        {
            get
            {
                return _clearEnabled;
            }
            set
            {
                _clearEnabled = value;
                RaisePropertyChanged(() => this.ClearEnabled);
            }
        }

        public bool SubmitEnabled
        {
            get
            {
                return _submitEnabled;
            }
            set
            {
                _submitEnabled = value;
                RaisePropertyChanged(() => this.SubmitEnabled);
            }
        }

        public string MainDesBackground
        {
            get
            {
                return _mainDesBackground;
            }
            set
            {
                _mainDesBackground = value;
                RaisePropertyChanged(() => this.MainDesBackground);
            }
        }

        public bool MainDisEnabled
        {
            get
            {
                return _mainDisEnabled;
            }
            set
            {
                _mainDisEnabled = value;
                RaisePropertyChanged(() => this.MainDisEnabled);
            }
        }

        public string RadioVisibility
        {
            get
            {
                return _radioVisibility;
            }
            set
            {
                _radioVisibility = value;
                RaisePropertyChanged(() => this.RadioVisibility);
            }
        }
        
        
      
        #endregion

        #region COMMANDS

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand VehiclesCommand
        {
            get
            {
                return _vehiclesCommand ?? (_vehiclesCommand = new LogOutCommandHandler(() => Switcher.Switch(new VehicleMenuView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand AdminDashboardCommand
        {
            get
            {
                return _adminDashboardCommand ?? (_adminDashboardCommand = new LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand CreateWorkOrderCommand
        {
            get
            {
                return _createWorkOrderCommand ?? (_createWorkOrderCommand = new LogOutCommandHandler(() => CreateWorkOrder(), canExecute));
            }
        }

        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new LogOutCommandHandler(() => ClearData(), canExecute));
            }
        }

        public ICommand ViewMaintenanceCommand
        {
            get
            {
                return _viewMaintenanceCommand ?? (_viewMaintenanceCommand = new LogOutCommandHandler(() => ViewMaintenanceData(), canExecute));
            }
        }
        
        

        #endregion
    }
}
