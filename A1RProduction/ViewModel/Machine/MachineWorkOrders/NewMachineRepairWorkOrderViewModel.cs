using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Machine.MachineWorkOrders
{

    public class NewMachineRepairWorkOrderViewModel : ViewModelBase
    {
        public MachineRepairDescription SelectedItem { get; set; }

        private ObservableCollection<MachineWorkDescription> _machineWorkDescription;
        private ObservableCollection<MachineRepairDescription> _machineRepairDescriptions;
        private MachineRepairWorkOrder _machineRepairWorkOrder;
        private ObservableCollection<Machines> _machines;
        private int _selectedMachine;
        private int _itemCount;
        private string userName;
        private string state;
        private string _selectedOrderType;
        private string _repairDescription;
        private string _addRepairButtonBackground;
        private string _selectedRepairNo;
        private string _partCode;
        private string _addPartButtonBackGround;
        private string _crateBackground;
        private bool _datagridEnabled;
        private bool _addRepairEnabled;
        private bool _addPartEnabled;
        private bool _repairDescriptionEnabled;
        private bool _vehiclePartEnabled;
        private bool _vehicleCodeEnabled;
        private bool _createButtonEnabled;
        private bool _clearEnabled;
        private string _clearBackground;

        private List<UserPrivilages> userPrivilages;
        private List<MetaData> metaData;
        private bool canExecute;
        private string _version;
        private ICommand _homeCommand;
        private ICommand _addItemCommand;
        private ICommand _completeCommand;
        private ICommand _backCommand;
        private ICommand _clearCommand;
        private ICommand _addToRepairCommand;
        private ICommand _addToPartCommand;
        private ICommand _miscellaniousCommand;

        public NewMachineRepairWorkOrderViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            this.DeleteItem = new A1QSystem.Commands.DeleteMachineRepairItem(this);
            metaData = md;
            userName = UserName;
            state = State;
            userPrivilages = up;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
            canExecute = true;
            DatagridEnabled = false;
            AddRepairEnabled = false;
            LoadMachines();
            SelectedOrderType = "Urgent";
            MachineWorkDescription = new ObservableCollection<MachineWorkDescription>();
            MachineRepairWorkOrder = new MachineRepairWorkOrder();
            MachineRepairDescription = new ObservableCollection<MachineRepairDescription>();
            MachineRepairWorkOrder.MachineRepairDescription = new ObservableCollection<MachineRepairDescription>();
            MachineRepairWorkOrder.MachineRepairDescription.CollectionChanged += OnVehicleRepairListChanged;
            this.MachineRepairWorkOrder.MachineRepairDescription = SequencingService.SetCollectionSequence(this.MachineRepairWorkOrder.MachineRepairDescription);
            MachineRepairDescription.Add(new MachineRepairDescription() { StrSequenceNumber = "Select" });
            SelectedRepairNo = "Select";
            CreateButtonEnabled = false;
            ClearEnabled = false;
            CrateBackground = "#FFDEDEDE";
            ClearBackground = "#FFDEDEDE";
        }

        void OnVehicleRepairListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Update item count
            this.ItemCount = this.MachineRepairWorkOrder.MachineRepairDescription.Count;

            // Resequence list
            SequencingService.SetCollectionSequence(this.MachineRepairWorkOrder.MachineRepairDescription);
            EnDisCreateButton();
        }

        private void LoadMachines()
        {
            Machines = DBAccess.GetMachinesByLocation(1);
            Machines.Add(new Machines(0) { MachineID = 0, MachineString = "Select" });
            SelectedMachine = 0;
        }


        private void AddNewItem()
        {
            if (MachineRepairWorkOrder.MachineRepairDescription.Count > 0)
            {
                MachineRepairDescription vd = MachineRepairWorkOrder.MachineRepairDescription.Last();
                if (!string.IsNullOrWhiteSpace(vd.RepairDescription))
                {
                    MachineRepairWorkOrder.MachineRepairDescription.Add(new MachineRepairDescription() { });
                }
                else
                {
                    Msg.Show("Please enter repair description for Item No " + vd.SequenceNumber, "", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
                }
            }
        }

        private void CreateRepairWorkOrder()
        {
            bool hasError = false;

            if (MachineWorkDescription.Count == 0)
            {
                hasError = true;
                Msg.Show("Repair items required", "Repair Items Required", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
            }
            else if (MachineWorkDescription[0].MachineRepairDescription.Count == 0)
            {
                hasError = true;
                Msg.Show("Repair description required", "Repair Description Required", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
            }

            if (hasError == false)
            {
                BusinessDaysGenerator bdg = new BusinessDaysGenerator();
                //VehicleWorkOrder vwo = new VehicleWorkOrder();
                MachineRepairWorkOrder.Machine = new Machines(0) { MachineID = SelectedMachine };
                MachineRepairWorkOrder.User = new User() { ID = 0 };
                MachineRepairWorkOrder.WorkOrderType = VehicleWorkOrderTypesEnum.Repair.ToString();
                MachineRepairWorkOrder.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { Frequency = "One Off" };
                MachineRepairWorkOrder.FirstServiceDate = DateTime.Now;
                MachineRepairWorkOrder.NextServiceDate = DateTime.Now;
                MachineRepairWorkOrder.IsCompleted = false;
                MachineRepairWorkOrder.CreatedDate = DateTime.Now;
                MachineRepairWorkOrder.CreatedBy = userName;
                MachineRepairWorkOrder.Status = "Pending";
                MachineRepairWorkOrder.Urgency = ConvertUrgency();
                MachineRepairWorkOrder.MachineMaintenanceInfo = new ObservableCollection<MachineMaintenanceInfo>();
                MachineRepairWorkOrder.MachineRepairDescription = MachineWorkDescription[0].MachineRepairDescription;
                MachineRepairWorkOrder.OrderType = "Internal";
                MachineRepairWorkOrder.ShowOrderDate = DateTime.Now;

                Int32 woid = DBAccess.InsertNewMachineRepairWorkOrder(MachineRepairWorkOrder, 0);
                if (woid > 0)
                {
                    SelectedMachine = 0;
                    SelectedOrderType = "Urgent";
                    MachineRepairWorkOrder.MachineRepairDescription.Clear();
                    MachineWorkDescription.Clear();
                    Msg.Show("Machine repair order " + woid + " has been created successfull", "Machine Repair Work Order Created", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);

                }
            }


            //var item = VehicleRepairWorkOrder.VehicleRepairDescription.FirstOrDefault(i => i.PartNotOrdered == true);
            //if (item != null)
            //{
            //    item.LastName = "Smith";
            //}

        }

        private void ClearData()
        {
            if (Msg.Show("Are you sure you want to clear all data?", "Clearing Data", MsgBoxButtons.YesNo, MsgBoxImage.Information_Orange, MsgBoxResult.Yes) == MsgBoxResult.Yes)
            {
                RepairDescription = string.Empty;
                PartCode = string.Empty;
                SelectedRepairNo = "Select";
                SelectedMachine = 0;
                SelectedOrderType = "Urgent";
                MachineWorkDescription.Clear();
                EnDisCreateButton();
            }
        }

        private int ConvertUrgency()
        {
            int x = 0;
            switch (SelectedOrderType)
            {
                case "Normal": x = 2;
                    break;
                case "Urgent": x = 1;
                    break;
                default:
                    x = 2;
                    break;
            }
            return x;
        }

        private void EnDisAddRepairButton()
        {
            if (!String.IsNullOrWhiteSpace(RepairDescription))
            {
                AddRepairEnabled = true;
            }
            else
            {
                AddRepairEnabled = false;
            }
        }

        private void EnDisAddPartButton()
        {

            if ((SelectedRepairNo != "Select") && (!String.IsNullOrWhiteSpace(PartCode)))
            {
                AddPartEnabled = true;
            }
            else
            {
                AddPartEnabled = false;
            }
        }

        private void AddToRepair()
        {
            foreach (var item in MachineWorkDescription)
            {
                Int32 id = 0;
                foreach (var items in item.MachineRepairDescription)
                {
                    if (items.ID > 0)
                    {
                        id = items.MachineWorkDescriptionID;
                        break;
                    }
                }

                item.MachineRepairDescription.Add(new MachineRepairDescription() { MachineWorkDescriptionID = id, RepairDescription = RepairDescription, IsActive = true });


                for (int i = 0; i < item.MachineRepairDescription.Count; i++)
                {
                    if (String.IsNullOrWhiteSpace(item.MachineRepairDescription[i].RepairDescription))
                    {
                        item.MachineRepairDescription.RemoveAt(i);
                    }
                }
                SequencingService.SetCollectionSequence(item.MachineRepairDescription);
                break;
            }
            //VehicleRepairDescriptions.Clear();

            MachineRepairDescription = MachineWorkDescription[0].MachineRepairDescription;
            MachineRepairDescription.Add(new MachineRepairDescription() { StrSequenceNumber = "Select" });
            //SelectedItemCode = "Select";
            RepairDescription = string.Empty;
            //SelectedItemCodePart = "Select";
            SelectedRepairNo = "Select";
            EnDisCreateButton();
        }

        private void AddToPart()
        {
            foreach (var item in MachineWorkDescription)
            {

                foreach (var items in item.MachineRepairDescription)
                {
                    if (items.StrSequenceNumber == SelectedRepairNo)
                    {
                        items.MachineParts.Add(new MachineParts() { MachineRepairID = items.ID, PartCode = PartCode });
                        SequencingService.SetCollectionSequence(items.MachineParts);
                    }
                }
            }

            //SelectedItemCodePart = "Select";
            SelectedRepairNo = "Select";
            PartCode = String.Empty;
        }

        private void EnDisCreateButton()
        {

            if (SelectedMachine == 0 || MachineWorkDescription.Count == 0 || MachineWorkDescription[0].MachineRepairDescription.Count == 0)
            {
                ClearEnabled = false;
                CreateButtonEnabled = false;
                CrateBackground = "#FFDEDEDE";
                ClearBackground = "#FFDEDEDE";
            }
            else
            {
                ClearEnabled = true;
                CreateButtonEnabled = true;
                CrateBackground = "#FF787C7A";
                ClearBackground = "#FF787C7A";
            }
        }

        #region PUBLIC_PROPERTIES

        public ObservableCollection<MachineWorkDescription> MachineWorkDescription
        {
            get
            {
                return _machineWorkDescription;
            }
            set
            {
                _machineWorkDescription = value;
                RaisePropertyChanged(() => this.MachineWorkDescription);
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

        public bool DatagridEnabled
        {
            get
            {
                return _datagridEnabled;
            }
            set
            {
                _datagridEnabled = value;
                RaisePropertyChanged(() => this.DatagridEnabled);
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
                if (SelectedMachine == 0)
                {
                    DatagridEnabled = false;
                    RepairDescriptionEnabled = false;
                    MachinePartEnabled = false;
                    MachineCodeEnabled = false;
                    if (MachineWorkDescription != null)
                    {
                        MachineWorkDescription.Clear();
                    }
                }
                else
                {
                    var v = Machines.SingleOrDefault(x => x.MachineID == SelectedMachine);

                    MachineWorkDescription.Clear();
                    MachineWorkDescription.Add(new MachineWorkDescription() { MachineMaintenanceInfo = new MachineMaintenanceInfo() { MachineDescription = "Repair order for " + v.MachineName,WorkItemVisible ="Visible" }, });
                    DatagridEnabled = true;
                    RepairDescriptionEnabled = true;
                    MachinePartEnabled = true;
                }

                RepairDescription = string.Empty;
                SelectedRepairNo = "Select";
                PartCode = string.Empty;
            }
        }

        public MachineRepairWorkOrder MachineRepairWorkOrder
        {
            get
            {
                return _machineRepairWorkOrder;
            }
            set
            {
                _machineRepairWorkOrder = value;
                RaisePropertyChanged(() => this.MachineRepairWorkOrder);
            }
        }

        public int ItemCount
        {
            get { return _itemCount; }

            set
            {
                _itemCount = value;
                base.RaisePropertyChanged(() => this.ItemCount);
            }
        }

        public string SelectedOrderType
        {
            get
            {
                return _selectedOrderType;
            }
            set
            {
                _selectedOrderType = value;
                RaisePropertyChanged(() => this.SelectedOrderType);
            }
        }


        public string RepairDescription
        {
            get
            {
                return _repairDescription;
            }
            set
            {
                _repairDescription = value;
                RaisePropertyChanged(() => this.RepairDescription);
                EnDisAddRepairButton();
            }
        }

        public bool AddRepairEnabled
        {
            get
            {
                return _addRepairEnabled;
            }
            set
            {
                _addRepairEnabled = value;
                RaisePropertyChanged(() => this.AddRepairEnabled);
                if (AddRepairEnabled == true)
                {
                    AddRepairButtonBackground = "#FF787C7A";
                }
                else
                {
                    AddRepairButtonBackground = "#FFDEDEDE";
                }
            }
        }

        public string AddRepairButtonBackground
        {
            get
            {
                return _addRepairButtonBackground;
            }
            set
            {
                _addRepairButtonBackground = value;
                RaisePropertyChanged(() => this.AddRepairButtonBackground);
            }
        }

        public ObservableCollection<MachineRepairDescription> MachineRepairDescription
        {
            get
            {
                return _machineRepairDescriptions;
            }
            set
            {
                _machineRepairDescriptions = value;
                RaisePropertyChanged(() => this.MachineRepairDescription);
            }
        }

        public string SelectedRepairNo
        {
            get
            {
                return _selectedRepairNo;
            }
            set
            {
                _selectedRepairNo = value;
                RaisePropertyChanged(() => this.SelectedRepairNo);

                if (SelectedRepairNo == "Select")
                {
                    MachineCodeEnabled = false;
                }
                else
                {
                    MachineCodeEnabled = true;
                }
                PartCode = string.Empty;
                EnDisAddPartButton();
            }
        }

        public string PartCode
        {
            get
            {
                return _partCode;
            }
            set
            {
                _partCode = value;
                RaisePropertyChanged(() => this.PartCode);
                EnDisAddPartButton();
            }
        }

        public bool AddPartEnabled
        {
            get
            {
                return _addPartEnabled;
            }
            set
            {
                _addPartEnabled = value;
                RaisePropertyChanged(() => this.AddPartEnabled);
                if (AddPartEnabled == true)
                {
                    AddPartButtonBackGround = "#FF787C7A";
                }
                else
                {
                    AddPartButtonBackGround = "#FFDEDEDE";
                }
            }
        }

        public string AddPartButtonBackGround
        {
            get
            {
                return _addPartButtonBackGround;
            }
            set
            {
                _addPartButtonBackGround = value;
                RaisePropertyChanged(() => this.AddPartButtonBackGround);
            }
        }

        public bool RepairDescriptionEnabled
        {
            get
            {
                return _repairDescriptionEnabled;
            }
            set
            {
                _repairDescriptionEnabled = value;
                RaisePropertyChanged(() => this.RepairDescriptionEnabled);
            }
        }

        public bool MachinePartEnabled
        {
            get
            {
                return _vehiclePartEnabled;
            }
            set
            {
                _vehiclePartEnabled = value;
                RaisePropertyChanged(() => this.MachinePartEnabled);
            }
        }

        public bool MachineCodeEnabled
        {
            get
            {
                return _vehicleCodeEnabled;
            }
            set
            {
                _vehicleCodeEnabled = value;
                RaisePropertyChanged(() => this.MachineCodeEnabled);
            }
        }

        public string CrateBackground
        {
            get
            {
                return _crateBackground;
            }
            set
            {
                _crateBackground = value;
                RaisePropertyChanged(() => this.CrateBackground);
            }
        }

        public bool CreateButtonEnabled
        {
            get
            {
                return _createButtonEnabled;
            }
            set
            {
                _createButtonEnabled = value;
                RaisePropertyChanged(() => this.CreateButtonEnabled);
            }
        }

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




        #endregion

        #region COMMANDS

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand AddItemCommand
        {
            get
            {
                return _addItemCommand ?? (_addItemCommand = new A1QSystem.Commands.LogOutCommandHandler(() => AddNewItem(), canExecute));
            }
        }

        public ICommand CompleteCommand
        {
            get
            {
                return _completeCommand ?? (_completeCommand = new A1QSystem.Commands.LogOutCommandHandler(() => CreateRepairWorkOrder(), canExecute));
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new A1QSystem.Commands.LogOutCommandHandler(() => ClearData(), canExecute));
            }
        }

        public ICommand AddToRepairCommand
        {
            get
            {
                return _addToRepairCommand ?? (_addToRepairCommand = new A1QSystem.Commands.LogOutCommandHandler(() => AddToRepair(), canExecute));
            }
        }

        public ICommand AddToPartCommand
        {
            get
            {
                return _addToPartCommand ?? (_addToPartCommand = new A1QSystem.Commands.LogOutCommandHandler(() => AddToPart(), canExecute));
            }
        }

        public ICommand MiscellaniousCommand
        {
            get
            {
                return _miscellaniousCommand ?? (_miscellaniousCommand = new A1QSystem.Commands.LogOutCommandHandler(() => AddToPart(), canExecute));
            }
        }

        public ICommand DeleteItem { get; set; }

        #endregion
    }
}
