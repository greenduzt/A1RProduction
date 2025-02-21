using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using A1QSystem.ViewModel.Stock;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DelegateCommand = A1QSystem.Commands.DelegateCommand;

namespace A1QSystem.ViewModel.Machine
{
    public class UpdateMachineViewModel : ViewModelBase
    {
        private Machines _machine;
        private List<StockLocation> _stockLocation;
        private int _selectedStockId;
        private int _selectedMachine;
        private int _selectedMachineGroupID;
        private bool _updateVehicleEnDis;
        private string _updateVehicleBackground;
        private string _machineDesHeaderMargin;
        private string _machineDesTextMargin;
        private string _activeHeader;
        private string _yesMargin;
        private string _noMargin;
        private string _newGroupTextVisibility;
        private string _newMachineGroup;
        private List<MachineGroup> _machineGroup;
        private bool execute;
        private bool _isYes;
        private bool _isNo;
        private ObservableCollection<Machines> _machines;
        public event Action Closed;
        private ICommand _closeCommand;
        private ICommand _updateCommand;
        private ICommand _clearCommand;
        private ICommand _closeNewMachineCommand;
        private ICommand _selectionChangedCommand;

        public UpdateMachineViewModel()
        {
            MachineDesHeaderMargin = "9,220,11,0";
            MachineDesTextMargin = "9,220,22,0";
            ActiveHeader = "0,315,11,0";
            YesMargin = "8,320,0,0";
            NoMargin = "58,320,0,0";
            NewGroupTextVisibility = "Collapsed";
            NewMachineGroup = string.Empty;
            Machine = new Machines(0);
            execute = true;
            //_closeCommand = new DelegateCommand(CloseForm);
            LoadVehicles();
            LoadVehicleLocations();
            LoadMachineGroups();
        }

        private void LoadMachineGroups()
        {
            MachineGroup = DBAccess.GetMachineGroups();
            MachineGroup.Insert(0, new MachineGroup() { GroupID = 100, GroupName = "Select" });
            MachineGroup.Add(new MachineGroup() { GroupID = 101, GroupName = "New Group" });
            SelectedMachineGroupID = 100;
        }

        private void SelectionChanged()
        {
            if(SelectedMachineGroupID != 101)
            {
                CloseNewGroup();
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        private void UpdateVehicle()
        {
            if (SelectedStockId == 0)
            {
                Msg.Show("Please select machine location", "Machine Location Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }
            else if (SelectedMachineGroupID == 100)
            {
                Msg.Show("Please select machine group", "Machine Group Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }
            else if (SelectedMachineGroupID == 101 && string.IsNullOrWhiteSpace(NewMachineGroup))
            {
                Msg.Show("Please enter new machine group name", "Machine Group Name Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }
            else
            {
                if (IsYes)
                {
                    Machine.IsActive = true;
                }
                else if (IsNo)
                {
                    Machine.IsActive = false;
                }
                Machine.StockLocation.ID = SelectedStockId;
                Machine.MachineGroup = new MachineGroup() { GroupID = SelectedMachineGroupID };
                int res =  DBAccess.UpdateMachine(Machine, NewMachineGroup);
                if (res > 0)
                {
                    Msg.Show("Machine has been updated successfully", "Machine Updated", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.OK);
                }
                else
                {
                    Msg.Show("There has been a problem while updating details." + System.Environment.NewLine + "Please try again later ", "Cannot Update Vehicle", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
                }

                CloseForm();
            }
            
        }

        private void LoadVehicles()
        {
            Machines = DBAccess.GetAllMachinesActiveOrNot();
            Machines.Add(new Machines(0) { MachineID = 0, MachineString = "Select" });
            SelectedMachine = 0;
        }

        private void LoadVehicleLocations()
        {
            StockLocation = DBAccess.GetStockLocations();
            StockLocation.Add(new StockLocation() { ID = 0, StockName = "Select" });
            SelectedStockId = 0;
        }      

        private void ClearData()
        {
            SelectedMachine = 0;
            SelectedStockId = 0;
            IsNo = false;
            IsYes = false;
            Machine = null;
        }

        private void AddNewGroup()
        {
            NewGroupTextVisibility = "Visible";
            MachineDesHeaderMargin = "9,246,12,0";
            MachineDesTextMargin = "9,246,22,0";
            ActiveHeader = "9,341,11,0";
            YesMargin = "8,346,0,0";
            NoMargin = "58,346,0,0";
        }

        private void CloseNewGroup()
        {
            NewGroupTextVisibility = "Collapsed";
            MachineDesHeaderMargin = "9,220,11,0";
            MachineDesTextMargin = "9,220,22,0";
            ActiveHeader = "9,315,11,0";
            YesMargin = "8,320,0,0";
            NoMargin = "58,320,0,0";
            //SelectedMachineGroupID = 100;
            NewMachineGroup = String.Empty;
        }

        private void CloseBtnNewGroup()
        {
            CloseNewGroup();
            SelectedMachineGroupID = 100;
        }

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
        public bool IsYes
        {
            get
            {
                return _isYes;
            }
            set
            {
                _isYes = value;
                RaisePropertyChanged(() => this.IsYes);
            }
        }

        public bool IsNo
        {
            get
            {
                return _isNo;
            }
            set
            {
                _isNo = value;
                RaisePropertyChanged(() => this.IsNo);
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

        
        public int SelectedMachine
        {
            get
            {
                return _selectedMachine;
            }
            set
            {
                _selectedMachine = value;
                RaisePropertyChanged(() => this.SelectedMachine);

                if (SelectedMachine > 0)
                {
                    UpdateVehicleBackground = "#FF787C7A";
                    UpdateVehicleEnDis = true;

                    Machine = DBAccess.GetMachineByMachineID(SelectedMachine);
                    SelectedStockId = Machine.StockLocation.ID;
                    SelectedMachineGroupID = Machine.MachineGroup.GroupID;
                    if (Machine.IsActive == true)
                    {
                        IsYes = true;
                        IsNo = false;
                    }
                    else
                    {
                        IsNo = true;
                        IsYes = false;
                    }
                }
                else
                {
                    UpdateVehicleBackground = "#FFDEDEDE";
                    UpdateVehicleEnDis = false;
                }
            }
        }

        public string UpdateVehicleBackground
        {
            get
            {
                return _updateVehicleBackground;
            }
            set
            {
                _updateVehicleBackground = value;
                RaisePropertyChanged(() => this.UpdateVehicleBackground);
            }
        }

        public bool UpdateVehicleEnDis
        {
            get
            {
                return _updateVehicleEnDis;
            }
            set
            {
                _updateVehicleEnDis = value;
                RaisePropertyChanged(() => this.UpdateVehicleEnDis);
            }
        }

        public ObservableCollection<Machines> Machines
        {
            get
            {
                return _machines;
            }
            set
            {
                _machines = value;
                RaisePropertyChanged(() => this.Machines);
            }
        }

        public Machines Machine
        {
            get
            {
                return _machine;
            }
            set
            {
                _machine = value;
                RaisePropertyChanged(() => this.Machine);
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
                
                if (SelectedMachineGroupID == 101)
                {
                    AddNewGroup();
                }               
            }
        }

        public string ActiveHeader
        {
            get
            {
                return _activeHeader
;
            }
            set
            {
                _activeHeader = value;
                RaisePropertyChanged(() => this.ActiveHeader);
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

        public string YesMargin
        {
            get
            {
                return _yesMargin;
            }
            set
            {
                _yesMargin = value;
                RaisePropertyChanged(() => this.YesMargin);
            }
        }

        public string NoMargin
        {
            get
            {
                return _noMargin;
            }
            set
            {
                _noMargin = value;
                RaisePropertyChanged(() => this.NoMargin);
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
                
        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new A1QSystem.Commands.LogOutCommandHandler(() => CloseForm(), execute));
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                return _updateCommand ?? (_updateCommand = new A1QSystem.Commands.LogOutCommandHandler(() => UpdateVehicle(), execute));
            }
        }

        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new A1QSystem.Commands.LogOutCommandHandler(() => ClearData(), execute));
            }
        }

        public ICommand CloseNewMachineCommand
        {
            get
            {
                return _closeNewMachineCommand ?? (_closeNewMachineCommand = new A1QSystem.Commands.LogOutCommandHandler(() => CloseBtnNewGroup(), execute));
            }
        }

        public ICommand SelectionChangedCommand
        {
            get
            {
                return _selectionChangedCommand ?? (_selectionChangedCommand = new A1QSystem.Commands.LogOutCommandHandler(() => SelectionChanged(), execute));
            }
        }
    }
}
