using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using A1QSystem.Model.Maintenance;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Vehicles;
using A1QSystem.View;
using A1QSystem.View.AdminDashboard;
using A1QSystem.View.Maintenance;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Maintenance
{
    public class MiscellaneousWorkOrderViewModel : ViewModelBase
    {
        private string userName;
        private string state;
        private List<UserPrivilages> userPrivilages;
        private bool canExecute;
        private List<MetaData> metaData;
        private string _version;
        private string _selectedArea;
        public MiscellaneousWorkOrder _miscellaneousWorkOrder;
        private DateTime _currentDate;
        private string _title;
        private string _workDescription;
        private DateTime _startDate;

        private ICommand _homeCommand;
        private ICommand _adminDashboardCommand;
        private ICommand _maintenanceCommand;
        private ICommand _clearCommand;
        private ICommand _createCommand;

        public MiscellaneousWorkOrderViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            userPrivilages = up;
            canExecute = true;
            metaData = md;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
            CurrentDate = DateTime.Now;
            SelectedArea = "Select";
            MiscellaneousWorkOrder = new MiscellaneousWorkOrder();
            StartDate = CurrentDate;
        }

        private void ClearFields()
        {
            SelectedArea = "Select";
            Title = string.Empty;
            WorkDescription = string.Empty;
            StartDate = CurrentDate;
        }

        private void CreateWorkOrder()
        {
            if(SelectedArea == "Select")
            {
                Msg.Show("Please select area", "Area Required", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
            }
            else if(string.IsNullOrWhiteSpace(Title))
            {
                Msg.Show("Please enter title", "Title Required", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);

            }
            else if(string.IsNullOrWhiteSpace(WorkDescription))
            {
                Msg.Show("Please enter work description", "Work Description Required", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);

            }
            else
            {
                if(SelectedArea == "Machines Area")
                {
                    MachineRepairWorkOrder MachineRepairWorkOrder = new MachineRepairWorkOrder();
                    MachineRepairWorkOrder.Machine = new Machines(0) { MachineID = 0 };
                    MachineRepairWorkOrder.User = new User() { ID = 0 };
                    MachineRepairWorkOrder.WorkOrderType = VehicleWorkOrderTypesEnum.Repair.ToString();
                    MachineRepairWorkOrder.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { Frequency = "One Off" };
                    MachineRepairWorkOrder.FirstServiceDate = StartDate;
                    MachineRepairWorkOrder.NextServiceDate = StartDate;
                    MachineRepairWorkOrder.IsCompleted = false;
                    MachineRepairWorkOrder.CreatedDate = DateTime.Now;
                    MachineRepairWorkOrder.CreatedBy = userName;
                    MachineRepairWorkOrder.Status = "Pending";
                    MachineRepairWorkOrder.Urgency = 1;
                    MachineRepairWorkOrder.MachineMaintenanceInfo = new ObservableCollection<MachineMaintenanceInfo>();
                    MachineRepairWorkOrder.MachineRepairDescription = new ObservableCollection<MachineRepairDescription>();                   

                    MachineRepairDescription mrd = new MachineRepairDescription();
                    mrd.SequenceNumber = 1;
                    mrd.RepairDescription = Title + " - " + WorkDescription;
                    mrd.PartsOrdered = false;
                    mrd.IsCompleted = false;
                    mrd.IsActive = true;

                    MachineRepairWorkOrder.MachineRepairDescription.Add(mrd);

                    Int32 woid = DBAccess.InsertNewMachineRepairWorkOrder(MachineRepairWorkOrder, 0);
                    if (woid > 0)
                    {
                        
                        MachineRepairWorkOrder.MachineRepairDescription.Clear();
                        ClearFields();
                        Msg.Show("Work order " + woid + " has been created successfull", "Work Order Created", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);

                    }
                }
                else if (SelectedArea == "Vehicle Area")
                {
                    VehicleRepairWorkOrder VehicleRepairWorkOrder = new VehicleRepairWorkOrder();
                    VehicleRepairWorkOrder.Vehicle = new Vehicle() { ID = 0 };
                    VehicleRepairWorkOrder.User = new User() { ID = 0 };
                    VehicleRepairWorkOrder.WorkOrderType = VehicleWorkOrderTypesEnum.Repair.ToString();
                    VehicleRepairWorkOrder.VehicleMaintenanceSequence = new VehicleMaintenanceSequence() { ID = 0 };
                    VehicleRepairWorkOrder.LargestSeqID = 0;
                    VehicleRepairWorkOrder.FirstServiceDate = StartDate;
                    VehicleRepairWorkOrder.NextServiceDate = StartDate;
                    VehicleRepairWorkOrder.OdometerReading = 0;
                    VehicleRepairWorkOrder.IsCompleted = false;
                    VehicleRepairWorkOrder.CreatedDate = DateTime.Now;
                    VehicleRepairWorkOrder.CreatedBy = userName;
                    VehicleRepairWorkOrder.Status = "Pending";
                    VehicleRepairWorkOrder.Urgency = 1;
                    VehicleRepairWorkOrder.VehicleMaintenanceInfo = new ObservableCollection<VehicleMaintenanceInfo>();
                    VehicleRepairWorkOrder.VehicleRepairDescription = new ObservableCollection<VehicleRepairDescription>();
                    VehicleRepairWorkOrder.IsViewed = false;

                    VehicleRepairDescription vrd = new VehicleRepairDescription();
                    vrd.SequenceNumber = 1;
                    vrd.RepairDescription = Title + " - " + WorkDescription;
                    vrd.PartsOrdered = false;
                    vrd.IsCompleted = false;
                    vrd.IsActive = true;
                    VehicleRepairWorkOrder.VehicleRepairDescription.Add(vrd);

                    Int32 woid = DBAccess.InsertNewVehicleRepairWorkOrder(VehicleRepairWorkOrder, 0);
                    if (woid > 0)
                    {
                        VehicleRepairWorkOrder.VehicleRepairDescription.Clear();
                        ClearFields();
                        Msg.Show("Work order " + woid + " has been created successfully", "Work Order Created", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);

                    }
                }
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

        public string SelectedArea
        {
            get
            {
                return _selectedArea;
            }
            set
            {
                _selectedArea = value;
                RaisePropertyChanged(() => this.SelectedArea);
            }
        }
        public MiscellaneousWorkOrder MiscellaneousWorkOrder
        {
            get
            {
                return _miscellaneousWorkOrder;
            }
            set
            {
                _miscellaneousWorkOrder = value;
                RaisePropertyChanged(() => this.MiscellaneousWorkOrder);
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

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                RaisePropertyChanged(() => this.Title);
            }
        }

        public string WorkDescription
        {
            get
            {
                return _workDescription;
            }
            set
            {
                _workDescription = value;
                RaisePropertyChanged(() => this.WorkDescription);
            }
        }
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                RaisePropertyChanged(() => this.StartDate);
            }
        }
        
        
        #region COMMANDS

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand AdminDashboardCommand
        {
            get
            {
                return _adminDashboardCommand ?? (_adminDashboardCommand = new LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand MaintenanceCommand
        {
            get
            {
                return _maintenanceCommand ?? (_maintenanceCommand = new LogOutCommandHandler(() => Switcher.Switch(new MaintenanceMenuView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }


        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new LogOutCommandHandler(() => ClearFields(), canExecute));
            }
        }

        public ICommand CreateCommand
        {
            get
            {
                return _createCommand ?? (_createCommand = new LogOutCommandHandler(() => CreateWorkOrder(), canExecute));
            }
        }
        
        

        #endregion
    }
}
