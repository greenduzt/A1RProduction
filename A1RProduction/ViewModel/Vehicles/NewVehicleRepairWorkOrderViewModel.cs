using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Vehicles;
using A1QSystem.View;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Vehicles
{
    public class NewVehicleRepairWorkOrderViewModel : ViewModelBase
    {
        public VehicleRepairDescription SelectedItem { get; set; }

        private ObservableCollection<VehicleWorkDescription> _vehicleWorkDescription;
        private ObservableCollection<VehicleRepairDescription> _vehicleRepairDescriptions;
        private VehicleRepairWorkOrder _vehicleRepairWorkOrder;
        private ObservableCollection<Vehicle> _vehicles;
        private int _selectedVehicle;
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

        public NewVehicleRepairWorkOrderViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            this.DeleteItem = new A1QSystem.Commands.DeleteNewVehicleRepairItem(this);
            metaData = md;
            userName = UserName;
            state = State;
            userPrivilages = up;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
            canExecute = true;
            DatagridEnabled = false;
            AddRepairEnabled = false;
            LoadVehicles();
            SelectedOrderType = "Urgent";
            VehicleWorkDescription = new ObservableCollection<VehicleWorkDescription>();
            //VehicleWorkDescription.Add(new VehicleWorkDescription() { Description="Repair Order"});
            VehicleRepairWorkOrder = new VehicleRepairWorkOrder();
            VehicleRepairDescriptions = new ObservableCollection<VehicleRepairDescription>();
            VehicleRepairWorkOrder.VehicleRepairDescription = new ObservableCollection<VehicleRepairDescription>();
            VehicleRepairWorkOrder.VehicleRepairDescription.CollectionChanged += OnVehicleRepairListChanged;
            this.VehicleRepairWorkOrder.VehicleRepairDescription = SequencingService.SetCollectionSequence(this.VehicleRepairWorkOrder.VehicleRepairDescription);
            VehicleRepairDescriptions.Add(new VehicleRepairDescription() { StrSequenceNumber = "Select" });
            SelectedRepairNo = "Select";
            CreateButtonEnabled = false;
            ClearEnabled = false;
            CrateBackground = "#FFDEDEDE";
            ClearBackground = "#FFDEDEDE";
            
        }

        void OnVehicleRepairListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Update item count
            this.ItemCount = this.VehicleRepairWorkOrder.VehicleRepairDescription.Count;

            // Resequence list
            SequencingService.SetCollectionSequence(this.VehicleRepairWorkOrder.VehicleRepairDescription);
            EnDisCreateButton();
        }

        private void LoadVehicles()
        {
            Vehicles = DBAccess.GetAllVehicles();
            Vehicles.Add(new Vehicle() { ID = 0, VehicleString = "Select" });
            SelectedVehicle = 0;
        }

        private void AddNewItem()
        {
            if (VehicleRepairWorkOrder.VehicleRepairDescription.Count > 0)
            {
                VehicleRepairDescription vd = VehicleRepairWorkOrder.VehicleRepairDescription.Last();
                if (!string.IsNullOrWhiteSpace(vd.RepairDescription))
                {
                    VehicleRepairWorkOrder.VehicleRepairDescription.Add(new VehicleRepairDescription() { });
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

            if (VehicleWorkDescription.Count == 0)
            {
                hasError = true;
                Msg.Show("Repair items required", "Repair Items Required", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
            }
            else if (VehicleWorkDescription[0].VehicleRepairDescription.Count == 0)
            {
                hasError = true;
                Msg.Show("Repair description required", "Repair Description Required", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
            }

            //if (VehicleWorkDescription.Count > 0)
            //{
                
               

            //    foreach (var item in VehicleRepairWorkOrder.VehicleRepairDescription)
            //    {
            //        if (string.IsNullOrWhiteSpace(item.RepairDescription))
            //        {
            //            hasError = true;
            //            Msg.Show("Item No " + item.SequenceNumber + " requires repair description", "Repair Description Required", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
            //            break;
            //        }
            //    }
            //}
            //else
            //{
            //    hasError = true;
            //    Msg.Show("Repair items required", "Repair Items Required", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
            //}

           

            if(hasError == false)
            {
                BusinessDaysGenerator bdg = new BusinessDaysGenerator();                
                //VehicleWorkOrder vwo = new VehicleWorkOrder();
                VehicleRepairWorkOrder.Vehicle = new Vehicle() { ID = SelectedVehicle };
                VehicleRepairWorkOrder.User = new User() { ID = 0 };
                VehicleRepairWorkOrder.WorkOrderType = VehicleWorkOrderTypesEnum.Repair.ToString();
                VehicleRepairWorkOrder.VehicleMaintenanceSequence = new VehicleMaintenanceSequence() { ID = 0 };
                VehicleRepairWorkOrder.LargestSeqID = 0;
                //VehicleRepairWorkOrder.MaintenanceFrequency = "One Off";
                VehicleRepairWorkOrder.FirstServiceDate = DateTime.Now;
                VehicleRepairWorkOrder.NextServiceDate = DateTime.Now;
                VehicleRepairWorkOrder.OdometerReading = 0;
                VehicleRepairWorkOrder.IsCompleted = false;
                VehicleRepairWorkOrder.CreatedDate = DateTime.Now;
                VehicleRepairWorkOrder.CreatedBy = userName;
                VehicleRepairWorkOrder.Status = "Pending";
                VehicleRepairWorkOrder.Urgency = ConvertUrgency();
                VehicleRepairWorkOrder.VehicleMaintenanceInfo = new ObservableCollection<VehicleMaintenanceInfo>();
                VehicleRepairWorkOrder.VehicleRepairDescription = VehicleWorkDescription[0].VehicleRepairDescription;
                VehicleRepairWorkOrder.IsViewed = false;
                

                Int32 woid = DBAccess.InsertNewVehicleRepairWorkOrder(VehicleRepairWorkOrder, 0);
                if(woid > 0)
                {
                    SelectedVehicle = 0;
                    SelectedOrderType = "Urgent";
                    VehicleRepairWorkOrder.VehicleRepairDescription.Clear();
                    VehicleWorkDescription.Clear();
                    Msg.Show("Vehicle repair order " + woid + " has been created successfully", "Vehicle Repair Work Order Created", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);

                }
            }


            //var item = VehicleRepairWorkOrder.VehicleRepairDescription.FirstOrDefault(i => i.PartNotOrdered == true);
            //if (item != null)
            //{
            //    item.LastName = "Smith";
            //}

        }

        //private DateTime CalculateDate()
        //{
        //    DateTime d = DateTime.Now.AddDays(+1);
        //    BusinessDaysGenerator bdg = new BusinessDaysGenerator();

        //    switch (SelectedOrderType)
        //    {
        //        case "Normal": d = bdg.SkipWeekends(DateTime.Now.AddDays(+7));
        //            break;
        //        case "Urgent": d = bdg.SkipWeekends(DateTime.Now.AddDays(+1)); ;
        //            break;
        //        default:
        //            d = bdg.SkipWeekends(DateTime.Now.AddDays(+1));
        //            break;
        //    }

        //    return d;
        //}
        
        private void ClearData()
        {
            if (Msg.Show("Are you sure you want to clear all data?", "Clearing Data", MsgBoxButtons.YesNo, MsgBoxImage.Information_Orange, MsgBoxResult.Yes) == MsgBoxResult.Yes)
            {
                RepairDescription = string.Empty;
                PartCode = string.Empty;
                SelectedRepairNo = "Select";
                SelectedVehicle = 0;
                SelectedOrderType = "Urgent";
                VehicleWorkDescription.Clear();
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
            foreach (var item in VehicleWorkDescription)
            {
                Int32 id = 0;
                foreach (var items in item.VehicleRepairDescription)
                {
                    if (items.ID > 0)
                    {
                        id = items.VehicleWorkDescriptionID;
                        break;
                    }
                }

                item.VehicleRepairDescription.Add(new VehicleRepairDescription() { VehicleWorkDescriptionID = id, RepairDescription = RepairDescription, IsActive = true });


                for (int i = 0; i < item.VehicleRepairDescription.Count; i++)
                {
                    if (String.IsNullOrWhiteSpace(item.VehicleRepairDescription[i].RepairDescription))
                    {
                        item.VehicleRepairDescription.RemoveAt(i);
                    }
                }
                SequencingService.SetCollectionSequence(item.VehicleRepairDescription);
                break;
            }
            //VehicleRepairDescriptions.Clear();
            
            VehicleRepairDescriptions = VehicleWorkDescription[0].VehicleRepairDescription;
            VehicleRepairDescriptions.Add(new VehicleRepairDescription() { StrSequenceNumber = "Select" });
            //SelectedItemCode = "Select";
            RepairDescription = string.Empty;
            //SelectedItemCodePart = "Select";
            SelectedRepairNo = "Select";
            EnDisCreateButton();
        }


        private void AddToPart()
        {
            foreach (var item in VehicleWorkDescription)
            {
                
                    foreach (var items in item.VehicleRepairDescription)
                    {
                        if (items.StrSequenceNumber == SelectedRepairNo)
                        {
                            items.Vehicleparts.Add(new VehicleParts() { VehicleRepairID = items.ID, PartCode = PartCode });
                            SequencingService.SetCollectionSequence(items.Vehicleparts);
                        }
                    }
            }
            
            //SelectedItemCodePart = "Select";
            SelectedRepairNo = "Select";
            PartCode = String.Empty;
        }

        private void EnDisCreateButton()
        {

            if(SelectedVehicle == 0 || VehicleWorkDescription.Count == 0 || VehicleWorkDescription[0].VehicleRepairDescription.Count == 0)
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
                if(SelectedVehicle == 0)
                {
                    DatagridEnabled = false;
                    RepairDescriptionEnabled = false;
                    VehiclePartEnabled = false;
                    VehicleCodeEnabled = false;
                }
                else
                {
                    var v = Vehicles.SingleOrDefault(x => x.ID == SelectedVehicle);
                   
                    VehicleWorkDescription.Clear();
                    VehicleWorkDescription.Add(new VehicleWorkDescription() { Description = "Repair order for " +  v.SerialNumber });
                    DatagridEnabled = true;
                    RepairDescriptionEnabled = true;
                    VehiclePartEnabled = true;

                    
                }

                RepairDescription = string.Empty;
                SelectedRepairNo = "Select";
                PartCode = string.Empty;
            }
        }

        public VehicleRepairWorkOrder VehicleRepairWorkOrder
        {
            get
            {
                return _vehicleRepairWorkOrder;
            }
            set
            {
                _vehicleRepairWorkOrder = value;
                RaisePropertyChanged(() => this.VehicleRepairWorkOrder);
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

        public ObservableCollection<VehicleRepairDescription> VehicleRepairDescriptions
        {
            get
            {
                return _vehicleRepairDescriptions;
            }
            set
            {
                _vehicleRepairDescriptions = value;
                RaisePropertyChanged(() => this.VehicleRepairDescriptions);
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
                    VehicleCodeEnabled = false;
                }
                else
                {
                    VehicleCodeEnabled = true;
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

        public bool VehiclePartEnabled
        {
            get
            {
                return _vehiclePartEnabled;
            }
            set
            {
                _vehiclePartEnabled = value;
                RaisePropertyChanged(() => this.VehiclePartEnabled);
            }
        }

        public bool VehicleCodeEnabled
        {
            get
            {
                return _vehicleCodeEnabled;
            }
            set
            {
                _vehicleCodeEnabled = value;
                RaisePropertyChanged(() => this.VehicleCodeEnabled);
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

        public ICommand DeleteItem { get; set; }

        #endregion
    }
}
