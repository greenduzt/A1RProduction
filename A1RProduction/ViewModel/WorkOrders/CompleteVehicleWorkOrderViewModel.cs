using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Users;
using A1QSystem.Model.Vehicles;
using A1QSystem.View;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.WorkOrders
{
    
    public class CompleteVehicleWorkOrderViewModel : ViewModelBase
    {
        private ObservableCollection<VehicleMaintenanceInfo> _vehicleMaintenanceInfo;
        private ObservableCollection<VehicleMaintenanceInfo> _safetyInspectionInfo;
        private VehicleWorkOrder _vehicleWorkOrder;
        private List<UserPosition> _userPositions;
        private string _currentOdometerTitle;
        private string _selectedMechanic;
        private long _odometer;
        //private string userCompleted;
        private string _partsOrdedVisibility;
        private bool canExecute;
        private string _odoVisibility;
        private string _gridVisibility;
        private string _completeBackgroundCol;
        private string _maintenanceDesHeader;
        private string _odoUnit;
        private int maintenanceSeqId;
        private bool _completeEnabled;
        private bool _tickAll;
        private bool _tickAll2;
        public event Action<int> Closed;
        private DelegateCommand _closeCommand;
        private ICommand _completeCommand;

        public CompleteVehicleWorkOrderViewModel(VehicleWorkOrder vwo)
        {
            VehicleWorkOrder = vwo; 
            //userCompleted = uN;
            UserPositions = new List<UserPosition>();
            VehicleMaintenanceInfo = new ObservableCollection<VehicleMaintenanceInfo>();
            CurrentOdometerTitle = "Current odometer reading";
            GridVisibility = "Collapsed";
            CompleteBackgroundCol = "#cccccc";
            CompleteEnabled = false;
            OdoUnit = GetOdometerReadingTitle(VehicleWorkOrder.Vehicle.VehicleBrand);
            _closeCommand = new DelegateCommand(CloseForm);
            canExecute = true;
            Odometer = VehicleWorkOrder.OdometerReading;
            LoadUserPositions();
            SelectedMechanic = "Select";
            
            List<int> nos = new List<int>();
            List<VehicleMaintenanceSequence> VehicleMaintenanceSequenceList = DBAccess.GetVehicleMaintenanceSequence(VehicleWorkOrder.Vehicle.VehicleCategory.ID);
            foreach (var item in VehicleMaintenanceSequenceList)
            {
                if (item.Kmhrs == 0)
                {
                    nos.Add(item.ID);
                }
            }
            SafetyInspectionInfo = DBAccess.GetMaintenanceInfoByMeterReading(nos, 1);
            OdoVisibility = VehicleWorkOrder.Vehicle.VehicleBrand == "HDK" ? "Collapsed" : "Visible";
            LoadDescriptions();
        }


        private void LoadDescriptions()
        {
            List<VehicleMaintenanceSequence> VehicleMaintenanceSequenceList = DBAccess.GetVehicleMaintenanceSequence(VehicleWorkOrder.Vehicle.VehicleCategory.ID);
            Tuple<int, List<Tuple<string, string>>> tup = null;
            VehicleWorkOrderManager vwom = new VehicleWorkOrderManager();
            tup=vwom.CreateVehicleWorkOrder(VehicleWorkOrder,Odometer);

            int msid = 0;
            msid = tup.Item1;
            maintenanceSeqId = msid;

            foreach (var item in tup.Item2)
            {
                if(item.Item1 == "MaintenanceDesHeader")
                {
                    MaintenanceDesHeader = item.Item2;
                }
            
            }
            if (msid == 0)
            {
                Msg.Show("Cannot retrieve information. Please try again later.", "Error Fetching Data", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else
            {
                List<int> nos = new List<int>();
                bool foundItem = false;
                foreach (var item in VehicleMaintenanceSequenceList)
                {
                    if (item.ID <= msid)
                    {
                        nos.Add(item.ID);
                        if (item.ID == 3)
                        {
                            foundItem = true;
                        }
                    }
                }
                if (foundItem == true)
                {
                    int value = nos[2];
                    nos.RemoveAt(2);
                    nos.Insert(1, value);

                }
                VehicleMaintenanceInfo = DBAccess.GetMaintenanceInfoByMeterReading(nos, 1);
            }
        }

        private string GetOdometerReadingTitle(string brand)
        {
            string n = string.Empty;

            if (brand == "Linde")
            {
                n = "Hours";
            }
            else
            {
                n = "Km";
            }

            return n;
        }
       
        private void CompleteOrder()
        {            
            if (SelectedMechanic == "Select")
            {
                Msg.Show("Please select your name", "Select Mechanic Name", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else if (VehicleWorkOrder.Vehicle.ID != 51 && VehicleWorkOrder.Vehicle.VehicleCode != "BUG1" && Odometer == 0)
            {
                Msg.Show("Please enter the Odometer reading", "Odometer Reading Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else if (VehicleWorkOrder.Vehicle.VehicleCode != "BUG1" && VehicleMaintenanceInfo.Count == 0)
            {
                Msg.Show("Please click on the Load Descriptions button to load maintenance descriptions and complete tasks", "Incomplete Tasks", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else
            {
                bool vehMainItemNotDone = false;
              
                if (VehicleWorkOrder.Vehicle.VehicleCode != "BUG1")
                {
                    foreach (var item in VehicleMaintenanceInfo)
                    {
                        if (item.ItemDone == false)
                        {
                            vehMainItemNotDone = true;
                            break;
                        }
                    }
                }               

                if (vehMainItemNotDone)
                {
                    string err = string.Empty;
                    if (vehMainItemNotDone)
                    {
                        err = "Some of the Vehicle Maintenance task(s) were not done";
                    }

                    Msg.Show(err + System.Environment.NewLine + System.Environment.NewLine + "Please complete the above task(s) to complete work order", "Task(s) Incomplete", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }               
                else
                {
                    var data = UserPositions.SingleOrDefault(x=>x.FullName == SelectedMechanic);
                    if(data != null)
                    {
                        //If the vehicle is Buggy
                        if(VehicleWorkOrder.Vehicle.VehicleBrand == "HDK")
                        {
                            if (VehicleMaintenanceInfo.Count == 0)
                            {
                                foreach (var item in SafetyInspectionInfo)
                                {
                                    VehicleMaintenanceInfo.Add(new VehicleMaintenanceInfo() { ID = item.ID, Code = item.Code, Description = item.Description });
                                }
                            }
                        }

                        VehicleWorkOrder vwo = new VehicleWorkOrder();
                        vwo.VehicleWorkOrderID = VehicleWorkOrder.VehicleWorkOrderID;
                        vwo.User = new User() { ID = data.User.ID };                       
                        vwo.Vehicle = new Vehicle() { ID = VehicleWorkOrder.Vehicle.ID };
                        vwo.User = new User() { ID = data.User.ID };
                        vwo.WorkOrderType = "Maintenance";
                        vwo.FirstServiceDate = VehicleWorkOrder.FirstServiceDate;
                        vwo.NextServiceDate = CalculateNextServiceDate();
                        vwo.LastOdometerReading = VehicleWorkOrder.OdometerReading;
                        vwo.VehicleMaintenanceSequence = new VehicleMaintenanceSequence() { ID = maintenanceSeqId };
                        vwo.LargestSeqID = (maintenanceSeqId >= VehicleWorkOrder.LargestSeqID ? maintenanceSeqId : VehicleWorkOrder.LargestSeqID);
                        vwo.OdometerReading = Odometer;
                        vwo.IsCompleted = false;
                        vwo.CreatedDate = DateTime.Now;
                        vwo.CreatedBy = SelectedMechanic;
                        vwo.CompletedDate = DateTime.Now;
                        vwo.CompletedBy = SelectedMechanic;
                        vwo.Status = "Pending";
                        vwo.Urgency = 2;
                        vwo.IsViewed = false;
                        vwo.ExtraNotes = VehicleWorkOrder.ExtraNotes;
                        int res = DBAccess.VehicleWorkOrderCompleted(vwo, VehicleMaintenanceInfo);
                        if (res > 0)
                        {
                            Msg.Show("Work order no " + VehicleWorkOrder.VehicleWorkOrderID + " has been succsessfully completed", "Vehicle Order Completed", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
                            CloseForm();
                        }
                    }
                }
            }
        }

        

        private void LoadUserPositions()
        {
            UserPositions = DBAccess.GetAllUserPositions("VehicleMechanic");
            UserPositions.Add(new UserPosition() { FullName = "Select"});
        }

        private DateTime? CalculateNextServiceDate()
        {
            DateTime? d = null;
            DateTime currentDate = DateTime.Now;
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();

            if (VehicleWorkOrder.Vehicle.ID == 51 || VehicleWorkOrder.Vehicle.ID == 54)//Weekly
            {
                d = bdg.SkipWeekends(currentDate.AddDays(7));
            }
            else
            {
                d = bdg.SkipWeekends(currentDate.AddMonths(1));
            }

            return d;
        }

        private int GetMaintenanceFreqByString(string str)
        {
            int num = 0;

            switch (str)
            {
                case "1 Month": num = 2;
                    break;
                case "6 Months": num = 3;
                    break;
                case "1 Year": num = 4;
                    break;
                case "2 Years": num = 5;
                    break;
                case "One Off": num = 6;
                    break;
            }
            return num;
        }

        private int GetMaintenanceFreqByStringNew(string str)
        {
            int num = 0;

            switch (str)
            {
                case "1 Month": num = 1;
                    break;
                case "6 Months": num = 2;
                    break;
                case "1 Year": num = 3;
                    break;
                case "2 Years": num = 4;
                    break;
           }
            return num;
        }

        private int GetTotalDays(string date)
        {
            int x = 0;

            switch (date)
            {
                case "1 Month" : x = 30;
                    break;
                case "6 Months" : x = 180;
                    break;
                case "1 Year" : x = 365;
                    break;
                case "2 Years" : x = 730;
                    break;
                default:
                    break;
            }

            return x;
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                int x = 1;
                Closed(x);
            }
        }

        //private void ClearFields()
        //{
        //    SelectedMechanic = "Select";
        //    CompleteBackgroundCol = "#cccccc";
        //    CompleteEnabled = false;
        //    GridVisibility = "Collapsed";
        //    VehicleMaintenanceInfo.Clear();
        //    Odometer = 0;
        //    MaintenanceDesHeader = string.Empty;
        //    TickAll = false;
        //    TickAll2 = false;
        //    foreach (var item in SafetyInspectionInfo)
        //    {
        //        item.ItemDone = false;
        //    }

        //    foreach (var item in VehicleMaintenanceInfo)
        //    {
        //        item.ItemDone = false;
        //    }
        //}

        #region PUBLIC_PROPERTIES

        public string OdoVisibility
        {
            get
            {
                return _odoVisibility;
            }
            set
            {
                _odoVisibility = value;
                RaisePropertyChanged(() => this.OdoVisibility);
            }
        }

        public string CurrentOdometerTitle
        {
            get
            {
                return _currentOdometerTitle;
            }
            set
            {
                _currentOdometerTitle = value;
                RaisePropertyChanged(() => this.CurrentOdometerTitle);
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

        public string SelectedMechanic
        {
            get
            {
                return _selectedMechanic;
            }
            set
            {
                _selectedMechanic = value;
                RaisePropertyChanged(() => this.SelectedMechanic);
            }
        }

        public long Odometer
        {
            get
            {
                return _odometer;
            }
            set
            {
                _odometer = value;
                RaisePropertyChanged(() => this.Odometer);
            }
        }

        public ObservableCollection<VehicleMaintenanceInfo> VehicleMaintenanceInfo
        {
            get
            {
                return _vehicleMaintenanceInfo;
            }
            set
            {
                _vehicleMaintenanceInfo = value;
                RaisePropertyChanged(() => this.VehicleMaintenanceInfo);
            }
        }

        public ObservableCollection<VehicleMaintenanceInfo> SafetyInspectionInfo
        {
            get
            {
                return _safetyInspectionInfo;
            }
            set
            {
                _safetyInspectionInfo = value;
                RaisePropertyChanged(() => this.SafetyInspectionInfo);
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
                    foreach (var item in VehicleMaintenanceInfo)
                    {
                       item.ItemDone = true;
                      //item.IsCompleted = true;
                           
                    }
                }
                else
                {
                    foreach (var item in VehicleMaintenanceInfo)
                    {
                        item.ItemDone = false;
                      //item.ItemRepair = false;                       
                    }
                }
            }
        }

        public string PartsOrdedVisibility
        {
            get { return _partsOrdedVisibility; }
            set
            {
                _partsOrdedVisibility = value;
                RaisePropertyChanged(() => this.PartsOrdedVisibility);
            }
        }

        public string GridVisibility
        {
            get { return _gridVisibility; }
            set
            {
                _gridVisibility = value;
                RaisePropertyChanged(() => this.GridVisibility);
            }
        }

        public string CompleteBackgroundCol
        {
            get { return _completeBackgroundCol; }
            set
            {
                _completeBackgroundCol = value;
                RaisePropertyChanged(() => this.CompleteBackgroundCol);
            }
        }

        public bool CompleteEnabled
        {
            get { return _completeEnabled; }
            set
            {
                _completeEnabled = value;
                RaisePropertyChanged(() => this.CompleteEnabled);
            }
        }
        
        

        public List<UserPosition> UserPositions
        {
            get
            {
                return _userPositions;
            }
            set
            {
                _userPositions = value;
                RaisePropertyChanged(() => this.UserPositions);
            }
        }

        public string MaintenanceDesHeader
        {
            get
            {
                return _maintenanceDesHeader;
            }
            set
            {
                _maintenanceDesHeader = value;
                RaisePropertyChanged(() => this.MaintenanceDesHeader);
            }
        }

        public string OdoUnit
        {
            get
            {
                return _odoUnit;
            }
            set
            {
                _odoUnit = value;
                RaisePropertyChanged(() => this.OdoUnit);
            }
        }

        

        public bool TickAll2
        {
            get
            {
                return _tickAll2;
            }
            set
            {
                _tickAll2 = value;
                RaisePropertyChanged(() => this.TickAll2);

                if (TickAll2 == true)
                {
                    foreach (var item in SafetyInspectionInfo)
                    {
                        item.ItemDone = true;
                        //item.IsCompleted = true;

                    }
                }
                else
                {
                    foreach (var item in SafetyInspectionInfo)
                    {
                        item.ItemDone = false;
                        //item.ItemRepair = false;                       
                    }
                }
            }
        }
        
        

    #endregion

        #region COMMANDS

        //public ICommand SubmitCommand
        //{
        //    get
        //    {
        //        return _submitCommand ?? (_submitCommand = new A1QSystem.Commands.LogOutCommandHandler(() => LoadDescriptions(), canExecute));
        //    }
        //}

        public ICommand CompleteCommand
        {
            get
            {
                return _completeCommand ?? (_completeCommand = new A1QSystem.Commands.LogOutCommandHandler(() => CompleteOrder(), canExecute));
            }
        }

        //public ICommand ClearCommand
        //{
        //    get
        //    {
        //        return _clearCommand ?? (_clearCommand = new A1QSystem.Commands.LogOutCommandHandler(() => ClearFields(), canExecute));
        //    }
        //}     
        
        

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }


        #endregion
    }
}
