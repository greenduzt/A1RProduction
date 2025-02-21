using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model.Vehicles;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.VehicleWorkOrders
{
    public class UpdateVehicleWorkOrderViewModel : ViewModelBase
    {
        private VehicleWorkOrder _vehicleWorkOrder;
        private ObservableCollection<VehicleWorkDescription> _vehicleWorkDescription;
        private DateTime _currentDate;
        private string _vehicleType;
        private string _updateContent;
        private string userName;
        private string _partsOrdedVisibility;
        private string _radioVisibility;
        private int maintenanceFrequency;
        private int initalMaintenanceFrequency;
        private DateTime _selectedDate;
        private bool _oneMonthChecked;
        private bool _sixMonthChecked;
        private bool _oneYearChecked;
        private bool _twoYearsChecked;
        private bool _updatedChecked;
        private bool _oneMonthEnabled;
        private bool _sixMonthsEnabled;
        private bool _oneYearEnabled;
        private bool _twoYearsEnabled;
        private bool execute;

        public event Action<int> Closed;
        private DelegateCommand _closeCommand;
        private ICommand _updateWorkOrderCommand;
        private ICommand _deleteCommand;

        public UpdateVehicleWorkOrderViewModel(VehicleWorkOrder vwo, string un)
        {
            VehicleWorkOrder = new VehicleWorkOrder(); 
            VehicleWorkOrder = vwo;
            userName = un;
            CurrentDate = DateTime.Now;
            VehicleWorkDescription = new ObservableCollection<VehicleWorkDescription>();
            initalMaintenanceFrequency = GetMaintenanceFreqId(VehicleWorkOrder.MaintenanceFrequency);
            SelectedDate = Convert.ToDateTime(VehicleWorkOrder.NextServiceDate);
            UpdateContent = VehicleWorkOrder.MaintenanceFrequency.ToLower() + "(Previous work description)" + System.Environment.NewLine + "Created Date : " + VehicleWorkOrder.CreatedDate.ToString("dd/MM/yyyy");
            VehicleType = VehicleWorkOrder.Vehicle.VehicleCategory.VehicleType;

            if (VehicleWorkOrder.Vehicle.StockLocation.ID == 2)
            {
                RadioVisibility = "Collapsed";
            }
            else
            {
                RadioVisibility = "Visible";
            }

            _closeCommand = new DelegateCommand(CloseForm); 
            UpdatedChecked = true;

            if (VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
            {
                PartsOrdedVisibility = "Collapsed";
                OneMonthEnabled = true;
                SixMonthsEnabled = true;
                OneYearEnabled = true;
                TwoYearsEnabled = true;
            }
            else if (VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
            {
                PartsOrdedVisibility = "Visible";
                OneMonthEnabled = false;
                SixMonthsEnabled = false;
                OneYearEnabled = false;
                TwoYearsEnabled = false;
            }
            execute = true;
        }

        private void Update()
        {
            int res = 0;
            VehicleWorkOrder.MaintenanceFrequency = GetMaintenanceFreq(maintenanceFrequency);
            //VehicleWorkOrder.NextServiceDate = CalculateNextServiceDate(maintenanceFrequency);
            VehicleWorkOrder.NextServiceDate = SelectedDate;
            //VehicleWorkOrder.FirstServiceDate = SelectedDate;

            if(VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
            {
                res = DBAccess.UpdateVehicleWorkOrder(VehicleWorkOrder,VehicleWorkDescription, userName);
            }
            else if (VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
            {
                res = DBAccess.UpdateVehicleRepairWorkOrder(VehicleWorkOrder, VehicleWorkDescription, userName);
            }

            
            if (res > 0)
            {
                Msg.Show("Vehicle work order updated!!!", "Work Order Updated", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
            }
            else
            {
                Msg.Show("Failed to update vehicle work order", "Failed To Update Work Order", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            CloseForm();
        }

        private void Delete()
        {
            if(Msg.Show("Are you sure you want to delete this work order?", "Deleting Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Information_Orange, MsgBoxResult.Yes)==MsgBoxResult.Yes)
            {
                DBAccess.DeleteVehicleWorkOrder(VehicleWorkOrder, userName);
                CloseForm();
            }
        }

    
        private void GetVehicleWorkDescriptions()
        {
            VehicleWorkDescription.Clear();            

            if(VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
            {
                VehicleWorkDescription = DBAccess.GetVehicleWorkDescriptionByID(VehicleWorkOrder.VehicleWorkOrderID);
                if (VehicleWorkDescription.Count > 0)
                {
                    ObservableCollection<VehicleRepairDescription> vehicleRepairDescriptionList = DBAccess.GetVehicleRepairDescriptionByID(VehicleWorkDescription);
                    ObservableCollection<VehicleParts> vehiclePartsList = DBAccess.GetVehiclePartsDescriptionByID(vehicleRepairDescriptionList);

                    foreach (var item in VehicleWorkDescription)
                    {
                        item.VehicleRepairDescription = new ObservableCollection<VehicleRepairDescription>();
                        foreach (var items in vehicleRepairDescriptionList)
                        {
                            if (item.ID == items.VehicleWorkDescriptionID)
                            {
                                item.VehicleRepairDescription.Add(items);
                                items.Vehicleparts = new ObservableCollection<VehicleParts>();
                                foreach (var itemz in vehiclePartsList)
                                {
                                    if (items.ID == itemz.VehicleRepairID)
                                    {
                                        items.Vehicleparts.Add(itemz);
                                    }
                                }
                            }
                        }
                    }
                }               
               //VehicleWorkDescription = DBAccess.GetVehicleWorkDescription(VehicleWorkOrder.VehicleWorkOrderID);
            }
            else if (VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
            {
                ObservableCollection<VehicleRepairDescription> vehicleRepairDescriptionList = DBAccess.GetVehicleRepairDescriptionByID2(VehicleWorkOrder.VehicleWorkOrderID);
                ObservableCollection<VehicleParts> vehiclePartsList = DBAccess.GetVehiclePartsDescriptionByID(vehicleRepairDescriptionList);
                Int32 id = 0;
                foreach (var item in vehicleRepairDescriptionList)
                {
                    id = item.VehicleWorkDescriptionID;
                    break;
                }

                VehicleWorkDescription = DBAccess.GetVehicleWorkDescriptionForRepair(id);

                foreach (var item in vehicleRepairDescriptionList)
                {
                    item.Vehicleparts = new ObservableCollection<VehicleParts>();
                    foreach (var items in vehiclePartsList)
                    {
                        if (item.ID == items.VehicleRepairID)
                        {
                            item.Vehicleparts.Add(items);
                        }
                    }
                }
                if (vehicleRepairDescriptionList.Count > 0 && VehicleWorkDescription.Count > 0)
                {
                    VehicleWorkDescription[0].VehicleRepairDescription = vehicleRepairDescriptionList;
                }
                else
                {
                    VehicleWorkDescription.Add(new VehicleWorkDescription() { Description = "Repair order for " + VehicleWorkOrder.Vehicle.SerialNumber, VehicleRepairDescription = vehicleRepairDescriptionList });
                }
                //VehicleWorkDescription = DBAccess.GetVehicleWorkDescriptionByIDForRepairNoActive(VehicleWorkOrder.VehicleWorkOrderID);
            }
        }

        private void LoadMaintenanceInfo(int num)
        {
            if (num > 0)
            {
                if (VehicleWorkOrder.Vehicle.VehicleCategory.ID > 0)
                {
                    VehicleWorkDescription.Clear();

                    if (VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
                    {
                        VehicleWorkOrder.VehicleMaintenanceInfo = DBAccess.GetMaintenanceInfoBySequence(num, VehicleWorkOrder.Vehicle.VehicleCategory.ID, VehicleWorkOrder.Vehicle.StockLocation.ID);

                        foreach (var item in VehicleWorkOrder.VehicleMaintenanceInfo)
                        {
                            VehicleWorkDescription.Add(new VehicleWorkDescription() { VehicleMaintenanceInfo = new VehicleMaintenanceInfo() {ID=item.ID, Code = item.Code }, Description = item.Description, IsActive=item.Active,VehicleMaintenanceSequenceID=item.VehicleMaintenanceSequence.ID });
                        }
                    }
                  
                    if (VehicleWorkOrder.VehicleMaintenanceInfo.Count == 0)
                    {
                        Msg.Show("No work description found for the vehicle", "Work Description Not Found", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
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
                case 6: str = "One Off";
                    break;
              
            }
            return str;
        }

        private int GetMaintenanceFreqId(string str)
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


        private DateTime? CalculateNextServiceDate(int n)
        {
            DateTime? d = null;
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();

            switch (n)
            {
                case 1: d = bdg.SkipWeekends(SelectedDate).AddDays(1);
                    break;
                case 2: d = bdg.SkipWeekends(SelectedDate).AddMonths(1);
                    break;
                case 3: d = bdg.SkipWeekends(SelectedDate).AddMonths(6);
                    break;
                case 4: d = bdg.SkipWeekends(SelectedDate).AddYears(1);
                    break;
                case 5: d = bdg.SkipWeekends(SelectedDate).AddYears(2);
                    break;
                case 6: d = SelectedDate;
                    break;
            }

            return d;
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                int r = 1;
                Closed(r);
            }
        }

        #region PUBLIC_PROPERTIES



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

        public VehicleWorkOrder VehicleWorkOrder
        {
            get { return _vehicleWorkOrder; }
            set
            {
                _vehicleWorkOrder = value;
                RaisePropertyChanged(() => this.VehicleWorkOrder);
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
                    maintenanceFrequency = 2;
                }
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
                    maintenanceFrequency = 3;
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
                    maintenanceFrequency = 4;
                }
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
                    maintenanceFrequency = 5;
                }
            }
        }

        public bool UpdatedChecked
        {
            get
            {
                return _updatedChecked;
            }
            set
            {
                _updatedChecked = value;
                RaisePropertyChanged(() => this.UpdatedChecked);
                if(UpdatedChecked == true)
                {
                    GetVehicleWorkDescriptions();
                    maintenanceFrequency = initalMaintenanceFrequency;
                    SelectedDate = Convert.ToDateTime(VehicleWorkOrder.NextServiceDate);
                }              
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

        public string UpdateContent
        {
            get
            {
                return _updateContent;
            }
            set
            {
                _updateContent = value;
                RaisePropertyChanged(() => this.UpdateContent);
            }
        }

        public bool OneMonthEnabled
        {
            get
            {
                return _oneMonthEnabled;
            }
            set
            {
                _oneMonthEnabled = value;
                RaisePropertyChanged(() => this.OneMonthEnabled);
            }
        }

        public bool SixMonthsEnabled
        {
            get
            {
                return _sixMonthsEnabled;
            }
            set
            {
                _sixMonthsEnabled = value;
                RaisePropertyChanged(() => this.SixMonthsEnabled);
            }
        }

        public bool OneYearEnabled
        {
            get
            {
                return _oneYearEnabled;
            }
            set
            {
                _oneYearEnabled = value;
                RaisePropertyChanged(() => this.OneYearEnabled);
            }
        }

        public bool TwoYearsEnabled
        {
            get
            {
                return _twoYearsEnabled;
            }
            set
            {
                _twoYearsEnabled = value;
                RaisePropertyChanged(() => this.TwoYearsEnabled);
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
        public ObservableCollection<VehicleWorkDescription> VehicleWorkDescription
        {
            get
            {
                return _vehicleWorkDescription;
            }
            set
            {
                _vehicleWorkDescription = value;
                RaisePropertyChanged(() => this.VehicleWorkDescription);
            }
        }
        

        #endregion

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ICommand UpdateWorkOrderCommand
        {
            get
            {
                return _updateWorkOrderCommand ?? (_updateWorkOrderCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Update(), execute));
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Delete(), execute));
            }
        }
    }
}
