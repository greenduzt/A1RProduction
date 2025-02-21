using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using A1QSystem.ViewModel.Stock;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Machine
{
    public class AddNewMachineViewModel : ViewModelBase
    {
        private List<StockLocation> _stockLocation;
        private List<MachineGroup> _machineGroup;
        private int _selectedStockId;
        private int _selectedMachineGroupID;
        private string _machineName;
        private string _machineType;
        private string _machineDescription;
        private string _addNewVehicleBackground;
        private string _machineDesHeaderMargin;
        private string _machineDesTextMargin;
        private string _newMachineGroup;
        private string _newGroupTextVisibility;
        public event Action Closed;
        private DelegateCommand _closeCommand;
        private ICommand _crateCommand;
        private ICommand _closeNewMachineCommand;
        private ICommand _selectionChangedCommand;
        private bool execute;
        private bool _vehicleEnDis;

        public AddNewMachineViewModel()
        {
            _closeCommand = new DelegateCommand(CloseForm);
            execute = true;
            StockLocation = new List<StockLocation>();
            LoadVehicleLocations();
            LoadMachineGroups();
            AddNewVehicleBackground = "#FFDEDEDE";
            VehicleEnDis = false;
            MachineDesHeaderMargin = "0,157,1,0";
            MachineDesTextMargin = "10,163,61,0";
            NewMachineGroup = string.Empty;
            NewGroupTextVisibility = "Collapsed";
        }

        private void LoadVehicleLocations()
        {
            StockLocation = DBAccess.GetStockLocations();
            StockLocation.Add(new StockLocation() { ID = 0, StockName = "Select" });
            SelectedStockId = 0;
            
        }

        private void LoadMachineGroups()
        {
            MachineGroup= DBAccess.GetMachineGroups();
            MachineGroup.Insert(0,new MachineGroup() { GroupID = 100, GroupName = "Select" });
            MachineGroup.Add(new MachineGroup() { GroupID = 101, GroupName = "New Group" });
            SelectedMachineGroupID = 100;
        }

        private void SelectionChanged()
        {
            if (SelectedMachineGroupID != 101)
            {
                CloseNewGroup();
            }
        }
        private void AddVehicle()
        {
            if(SelectedStockId == 0)
            {
                Msg.Show("Please select machine location", "Machine Location Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }
            else if (SelectedMachineGroupID == 100)
            {
                Msg.Show("Please select machine group", "Machine Group Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }
            else if(String.IsNullOrWhiteSpace(MachineName))
            {
                Msg.Show("Please enter machine name", "Machine Name Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }
            else if (String.IsNullOrWhiteSpace(MachineType))
            {
                Msg.Show("Please enter machine type", "Machine Type Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }
            else if (String.IsNullOrWhiteSpace(MachineDescription))
            {
                Msg.Show("Please enter machine description", "Machine Description Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }
            else if(SelectedMachineGroupID == 101 && string.IsNullOrWhiteSpace(NewMachineGroup))
            {
                Msg.Show("Please enter new machine group name", "Machine Group Name Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }
            else
            {            

                List<int> nos = new List<int>();
                ObservableCollection<Machines> ScheduledVehicle = DBAccess.GetMachinesByLocation(SelectedStockId);
                
                Machines m = new Machines(0);
                m.StockLocation = new StockLocation() { ID = SelectedStockId };
                m.MachineGroup = new MachineGroup() { GroupID = SelectedMachineGroupID};
                m.MachineName = MachineName;
                m.MachineType = MachineType;
                m.MachineDescription = MachineDescription;
                m.IsActive = true;
                int res =  DBAccess.InsertNewMachine(m, NewMachineGroup);
                if (res > 0)
                {
                    Clear();
                    CloseForm();
                    Msg.Show("Machine has been added successfully.", "New Machine Added", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.OK);
                }
                else
                {
                    Msg.Show("There has been an issue and the Machine did not create", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
                }
            }
        }

        private void Clear()
        {
            SelectedStockId = 0;
            SelectedMachineGroupID = 100;
            MachineName = string.Empty;
            MachineType = string.Empty;
            MachineDescription = string.Empty;
        }

        private void EnDisCreateButton()
        {
            if (SelectedStockId == 0 || SelectedMachineGroupID == 100 || String.IsNullOrWhiteSpace(MachineName) || String.IsNullOrWhiteSpace(MachineType) || String.IsNullOrWhiteSpace(MachineDescription))
            {
                AddNewVehicleBackground = "#FFDEDEDE";
                VehicleEnDis = false;
            }
            else
            {
                AddNewVehicleBackground = "#FF787C7A";
                VehicleEnDis = true;
            }
        }

        private void AddNewGroup()
        {
            NewGroupTextVisibility = "Visible";
            MachineDesHeaderMargin = "0,186,1,0";
            MachineDesTextMargin = "10,191,61,0";
        }

        private void CloseNewGroup()
        {
            NewGroupTextVisibility = "Collapsed";
            MachineDesHeaderMargin = "0,157,1,0";
            MachineDesTextMargin = "10,163,61,0";
            //SelectedMachineGroupID = 100;
            NewMachineGroup = String.Empty;
        }

        private void CloseBtnNewGroup()
        {
            CloseNewGroup();
            SelectedMachineGroupID = 100;
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        #region PUBLIC_PROPERTIES

       
        public string NewGroupTextVisibility
        {
            get
            {
                return _newGroupTextVisibility;
            }
            set
            {
                _newGroupTextVisibility = value;
                RaisePropertyChanged(() => this.NewGroupTextVisibility);
            }
        }

        public string NewMachineGroup
        {
            get
            {
                return _newMachineGroup;
            }
            set
            {
                _newMachineGroup = value;
                RaisePropertyChanged(() => this.NewMachineGroup);
            }
        }
        public string MachineDesHeaderMargin
        {
            get
            {
                return _machineDesHeaderMargin;
            }
            set
            {
                _machineDesHeaderMargin = value;
                RaisePropertyChanged(() => this.MachineDesHeaderMargin);
            }
        }

        public string MachineDesTextMargin
        {
            get
            {
                return _machineDesTextMargin;
            }
            set
            {
                _machineDesTextMargin = value;
                RaisePropertyChanged(() => this.MachineDesTextMargin);
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

        


        public int SelectedStockId
        {
            get
            {
                return _selectedStockId;
            }
            set
            {
                _selectedStockId = value;
                RaisePropertyChanged(() => this.SelectedStockId);
                EnDisCreateButton();
            }
        }

       

        public string MachineName
        {
            get
            {
                return _machineName;
            }
            set
            {
                _machineName = value;
                RaisePropertyChanged(() => this.MachineName);
                EnDisCreateButton();
            }
        }

        public string MachineType
        {
            get
            {
                return _machineType;
            }
            set
            {
                _machineType = value;
                RaisePropertyChanged(() => this.MachineType);
                EnDisCreateButton();
            }
        }
        public string MachineDescription
        {
            get
            {
                return _machineDescription;
            }
            set
            {
                _machineDescription = value;
                RaisePropertyChanged(() => this.MachineDescription);
                EnDisCreateButton();
            }
        }

        public string AddNewVehicleBackground
        {
            get
            {
                return _addNewVehicleBackground;
            }
            set
            {
                _addNewVehicleBackground = value;
                RaisePropertyChanged(() => this.AddNewVehicleBackground);
            }
        }

        public bool VehicleEnDis
        {
            get
            {
                return _vehicleEnDis;
            }
            set
            {
                _vehicleEnDis = value;
                RaisePropertyChanged(() => this.VehicleEnDis);
            }
        }

        public List<MachineGroup> MachineGroup
        {
            get
            {
                return _machineGroup;
            }
            set
            {
                _machineGroup = value;
                RaisePropertyChanged(() => this.MachineGroup);
            }
        }

        public int SelectedMachineGroupID
        {
            get
            {
                return _selectedMachineGroupID
;
            }
            set
            {
                _selectedMachineGroupID = value;
                RaisePropertyChanged(() => this.SelectedMachineGroupID);
                if(SelectedMachineGroupID == 101)
                {
                    AddNewGroup();
                }
                
                EnDisCreateButton();
            }
        }
        #endregion
        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ICommand CrateCommand
        {
            get
            {
                return _crateCommand ?? (_crateCommand = new A1QSystem.Commands.LogOutCommandHandler(() => AddVehicle(), execute));
            }
        }

        public ICommand SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ?? (_selectionChangedCommand = new A1QSystem.Commands.LogOutCommandHandler(() => SelectionChanged(), execute));
            }
        }

        public ICommand CloseNewMachineCommand
        {
            get
            {
                return _closeNewMachineCommand ?? (_closeNewMachineCommand = new A1QSystem.Commands.LogOutCommandHandler(() => CloseBtnNewGroup(), execute));
            }
        }
        

    }
}
