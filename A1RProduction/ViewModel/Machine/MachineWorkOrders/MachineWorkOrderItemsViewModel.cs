using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using Microsoft.Practices.Prism.Commands;
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
    public class MachineWorkOrderItemsViewModel : ViewModelBase
    {
        private ObservableCollection<MachineWorkDescription> _machineWorkDescription;
        private ObservableCollection<MachineRepairDescription> _machineRepairDescription;
        private List<MachineSpecialRequiremants> _machineSpecialRequiremants;
        private MachineWorkOrder _machineWorkOrder;
        private string _selectedItemCode;
        private string _selectedItemCodePart;
        private string _repairDescription;
        private string _selectedRepairNo;
        private string _partCode;
        private string _addPartButtonBackGround;
        private string _addRepairButtonBackground;
        private string _selectedRepairNoRemove;
        private string _repairVisibility;
        private string _repairDesMargin;
        private string _repDesTextMargin;
        private string _addRepairBtnMargin;
        private string _repairNoBinding;
        private string _repairNoTextBinding;
        private string _partCodeMargin;
        private string _partCodeMarginText;
        private string _addPartBtnMargin;
        private bool _addRepairEnabled;
        private bool _addPartEnabled;
        private bool _workItemCodeEnabled;
        private string _specialReqVisibility;
        private bool canExecute;
        public event Action<int> Closed;
        private DelegateCommand _closeCommand;
        private ICommand _addToRepairCommand;
        private ICommand _addToPartCommand;
        private ICommand _updateCommand;

        public MachineWorkOrderItemsViewModel(MachineWorkOrder mwo)
        {
            MachineWorkOrder = mwo;
            MachineWorkDescription = new ObservableCollection<MachineWorkDescription>();
            MachineRepairDescription = new ObservableCollection<MachineRepairDescription>();
            MachineSpecialRequiremants = new List<Model.Machine.MachineSpecialRequiremants>();
            SelectedItemCode = "Select";
            SelectedItemCodePart = "Select";
            SpecialReqVisibility = "Collapsed";
            canExecute = true;
            AddRepairEnabled = false;
            MachineSpecialRequiremants = DBAccess.GetMachineSpecialReqByMachId(MachineWorkOrder.Machine.MachineID);
            int x = 1;
            foreach (var item in MachineSpecialRequiremants)
            {
                item.ID = x;
                x++;
            }

            if (MachineSpecialRequiremants.Count > 0)
            {
                SpecialReqVisibility = "Visible";
            }
            else
            {
                SpecialReqVisibility = "Collapsed";
            }

            if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
            {
                WorkItemCodeEnabled = true;
                RepairVisibility = "Visible";
                RepairDesMargin = "261,126,0,0";
                RepDesTextMargin = "383,127,0,0";
                AddRepairBtnMargin = "0,18,13,0";
                RepairNoBinding = "261,190,0,0";
                RepairNoTextBinding = "383,193,0,0";
                PartCodeMargin = "489,190,0,0";
                PartCodeMarginText = "559,194,0,0";
                AddPartBtnMargin = "0,22,13,0";
                MachineWorkDescription = DBAccess.GetAllMachineWorkDescriptionByID(MachineWorkOrder.WorkOrderNo);
                if (MachineWorkDescription.Count > 0)
                {
                    ObservableCollection<MachineRepairDescription> machineRepairDescriptionList = DBAccess.GetMachineRepairDescriptionByID(MachineWorkDescription);
                    ObservableCollection<MachineParts> machinePartsList = DBAccess.GetMachinePartsDescriptionByID(machineRepairDescriptionList);

                    foreach (var item in MachineWorkDescription)
                    {
                        item.MachineRepairDescription = new ObservableCollection<MachineRepairDescription>();
                        foreach (var items in machineRepairDescriptionList)
                        {
                            if (item.ID == items.MachineWorkDescriptionID)
                            {
                                item.MachineRepairDescription.Add(items);
                                items.MachineParts = new ObservableCollection<MachineParts>();
                                foreach (var itemz in machinePartsList)
                                {
                                    if (items.ID == itemz.MachineRepairID)
                                    {
                                        items.MachineParts.Add(itemz);
                                    }
                                }
                            }
                        }
                    }
                    MachineWorkDescription.Add(new MachineWorkDescription() { MachineMaintenanceInfo = new MachineMaintenanceInfo() { MachineCode = "Select" } });
                }
            }
            else if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
            {
                AddPartEnabled = false;
                WorkItemCodeEnabled = false;
                RepairVisibility = "Collapsed";
                RepairDesMargin = "10,125,0,0";
                RepDesTextMargin = "164,127,0,0";
                AddRepairBtnMargin = "0,20,230,0";
                RepairNoBinding = "10,190,0,0";
                RepairNoTextBinding = "164,193,0,0";
                PartCodeMargin = "270,190,0,0";
                PartCodeMarginText = "341,194,0,0";
                AddPartBtnMargin = "-40,22,230,0";


                ObservableCollection<MachineRepairDescription> machineRepairDescriptionList = DBAccess.GetMahcineRepairDescriptionByID2(MachineWorkOrder.WorkOrderNo);
                ObservableCollection<MachineParts> machinePartsList = DBAccess.GetMachinePartsDescriptionByID(machineRepairDescriptionList);
                Int32 id = 0;
                foreach (var item in machineRepairDescriptionList)
                {
                    id = item.MachineWorkDescriptionID;
                    break;
                }

                MachineWorkDescription = DBAccess.GetMachineWorkDescriptionForRepair(id);

                foreach (var item in machineRepairDescriptionList)
                {
                    item.MachineParts = new ObservableCollection<MachineParts>();
                    foreach (var items in machinePartsList)
                    {
                        if (item.ID == items.MachineRepairID)
                        {
                            item.MachineParts.Add(items);
                        }
                    }
                }
                if (machineRepairDescriptionList.Count > 0 && MachineWorkDescription.Count > 0)
                {
                    MachineWorkDescription[0].MachineRepairDescription = machineRepairDescriptionList;
                }
                else
                {
                    if (MachineWorkOrder.Machine.MachineID == 0)
                    {
                        MachineWorkDescription.Add(new MachineWorkDescription() { MachineMaintenanceInfo = new MachineMaintenanceInfo() { MachineDescription = MachineWorkOrder.Machine.MachineName }, MachineRepairDescription = machineRepairDescriptionList });
                    }
                    else
                    {
                        MachineWorkDescription.Add(new MachineWorkDescription() { MachineMaintenanceInfo = new MachineMaintenanceInfo() { MachineDescription = "Repair order for " + MachineWorkOrder.Machine.MachineName }, MachineRepairDescription = machineRepairDescriptionList });
                    }
                }

                MachineRepairDescription = machineRepairDescriptionList;
                MachineRepairDescription.Add(new MachineRepairDescription() { StrSequenceNumber = "Select" });
            }

            _closeCommand = new DelegateCommand(CloseForm);  
        }


        private void EnDisAddPartButton()
        {
            if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
            {
                if ((!String.IsNullOrWhiteSpace(SelectedItemCodePart) && SelectedItemCodePart != "Select") && (!String.IsNullOrWhiteSpace(SelectedRepairNo) && SelectedRepairNo != "Select") && (!String.IsNullOrWhiteSpace(PartCode)))
                {
                    AddPartEnabled = true;
                }
                else
                {
                    AddPartEnabled = false;
                }
            }
            else if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
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
        }


        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed(1);
            }
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        private void AddToRepair()
        {
            if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
            {
                foreach (var item in MachineWorkDescription)
                {
                    if (item.MachineMaintenanceInfo.MachineCode == SelectedItemCode)
                    {
                        item.MachineRepairDescription.Add(new MachineRepairDescription() { MachineWorkDescriptionID = item.ID, RepairDescription = RepairDescription, IsActive = true,MachineParts = new ObservableCollection<MachineParts>() });
                        SequencingService.SetCollectionSequence(item.MachineRepairDescription);
                        break;
                    }
                }
            }
            else if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
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

                    item.MachineRepairDescription.Add(new MachineRepairDescription() { MachineWorkDescriptionID = id, RepairDescription = RepairDescription, IsActive = true, MachineParts = new ObservableCollection<MachineParts>() });


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
            }

            SelectedItemCode = "Select";
            RepairDescription = string.Empty;
            SelectedItemCodePart = "Select";
            SelectedRepairNo = "Select";
            PartCode = String.Empty;
        }

        private void AddToPart()
        {
            foreach (var item in MachineWorkDescription)
            {
                if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
                {
                    if (item.MachineMaintenanceInfo.MachineCode == SelectedItemCodePart)
                    {
                        foreach (var items in item.MachineRepairDescription)
                        {
                            if (items.StrSequenceNumber == SelectedRepairNo)
                            {
                                
                                items.MachineParts.Add(new MachineParts() { MachineRepairID = items.ID, PartCode = PartCode });
                                // Resequence list
                                SequencingService.SetCollectionSequence(items.MachineParts);
                            }
                        }
                    }
                }
                else if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
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
            }
            SelectedItemCodePart = "Select";
            SelectedRepairNo = "Select";
            PartCode = String.Empty;
        }

        private void Update()
        {
            MachineWorkOrder repairMachineWorkOrder = new MachineWorkOrder();
            repairMachineWorkOrder.Machine = new Machines(0) { MachineID = MachineWorkOrder.Machine.MachineID };
            repairMachineWorkOrder.User = new User() { ID = MachineWorkOrder.User.ID };
            repairMachineWorkOrder.WorkOrderType = VehicleWorkOrderTypesEnum.Repair.ToString();
            repairMachineWorkOrder.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = Convert.ToInt16(MachineMaintenanceFreq.OneOff), Frequency = MachineMaintenanceFreq.OneOff.ToString() };
            repairMachineWorkOrder.FirstServiceDate = CalculateDate();
            repairMachineWorkOrder.NextServiceDate = CalculateDate();
            repairMachineWorkOrder.IsCompleted = false;
            repairMachineWorkOrder.CreatedDate = DateTime.Now;
            repairMachineWorkOrder.CreatedBy = MachineWorkOrder.User.FullName;
            repairMachineWorkOrder.CompletedDate = null;
            repairMachineWorkOrder.CompletedBy = string.Empty;
            repairMachineWorkOrder.Status = "Pending";
            repairMachineWorkOrder.Urgency = 1;
            repairMachineWorkOrder.OrderType = MachineWorkOrder.OrderType.Equals("External") ? "External" : "Internal";
            if (MachineWorkOrder.OrderType.Equals("External"))
            {
                repairMachineWorkOrder.ShowOrderDate = MachineWorkOrder.ShowOrderDate;
                repairMachineWorkOrder.MachineProvider = new MachineProvider() { ProviderID = MachineWorkOrder.MachineProvider.ProviderID };
            }

            for (int i = 0; i < MachineWorkDescription.Count; i++)
            {
                if(MachineWorkDescription[i].MachineMaintenanceInfo.MachineCode == "Select")
                {
                    MachineWorkDescription.RemoveAt(i);
                }
            }


            int x = DBAccess.UpdateMachineRepairPart(MachineWorkOrder, MachineWorkDescription, repairMachineWorkOrder);
            if(x > 0)
            {
                Msg.Show("Details have been updated successfully!", "Details Updated", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.OK);
            }

            CloseForm();           

        }

        private DateTime CalculateDate()
        {
            DateTime d = DateTime.Now.AddDays(+1);
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();

            switch ("Urgent")
            {
                case "Normal": d = bdg.SkipWeekends(DateTime.Now.AddDays(+7));
                    break;
                case "Urgent": d = DateTime.Now;
                    break;
                default:
                    d = bdg.SkipWeekends(DateTime.Now.AddDays(+1));
                    break;
            }

            return d;
        }

        #region PUBLIC_PROPERTIES

            public List<MachineSpecialRequiremants> MachineSpecialRequiremants
        {
            get
            {
                return _machineSpecialRequiremants;
            }
            set
            {
                _machineSpecialRequiremants = value;
                RaisePropertyChanged(() => this.MachineSpecialRequiremants);

            }
        }



        public string SpecialReqVisibility
        {
            get
            {
                return _specialReqVisibility;
            }
            set
            {
                _specialReqVisibility = value;
                RaisePropertyChanged(() => this.SpecialReqVisibility);

             
            }
        }
        public string SelectedItemCode
        {
            get
            {
                return _selectedItemCode;
            }
            set
            {
                _selectedItemCode = value;
                RaisePropertyChanged(() => this.SelectedItemCode);

                SelectedItemCodePart = "Select";
                SelectedRepairNo = "Select";
                PartCode = String.Empty;
                EnDisAddRepairButton();
            }
        }

        private void EnDisAddRepairButton()
        {
            if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
            {
                if ((!String.IsNullOrWhiteSpace(SelectedItemCode) && SelectedItemCode != "Select") && (!String.IsNullOrWhiteSpace(RepairDescription)))
                {
                    AddRepairEnabled = true;
                }
                else
                {
                    AddRepairEnabled = false;
                }
            }
            else if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
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
        }

        public string SelectedItemCodePart
        {
            get
            {
                return _selectedItemCodePart;
            }
            set
            {
                _selectedItemCodePart = value;
                RaisePropertyChanged(() => this.SelectedItemCodePart);

                SelectedRepairNo = "Select";
                PartCode = String.Empty;

                if (!string.IsNullOrWhiteSpace(SelectedItemCodePart) && SelectedItemCodePart != "Select")
                {
                    MachineRepairDescription.Clear();

                    foreach (var item in MachineWorkDescription)
                    {
                        if (item.MachineMaintenanceInfo.MachineCode == SelectedItemCodePart)
                        {
                            foreach (var items in item.MachineRepairDescription)
                            {
                                MachineRepairDescription.Add(new MachineRepairDescription() { SequenceNumber = items.SequenceNumber, StrSequenceNumber = items.StrSequenceNumber, RepairDescription = items.RepairDescription });
                            }
                        }
                    }

                    MachineRepairDescription.Add(new MachineRepairDescription() { StrSequenceNumber = "Select" });
                    SelectedRepairNo = "Select";
                }
                EnDisAddPartButton();
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
                PartCode = string.Empty;
                EnDisAddPartButton();
            }
        }

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

        public MachineWorkOrder MachineWorkOrder
        {
            get
            {
                return _machineWorkOrder;
            }
            set
            {
                _machineWorkOrder = value;
                RaisePropertyChanged(() => this.MachineWorkOrder);
            }
        }

        public ObservableCollection<MachineRepairDescription> MachineRepairDescription
        {
            get
            {
                return _machineRepairDescription;
            }
            set
            {
                _machineRepairDescription = value;
                RaisePropertyChanged(() => this.MachineRepairDescription);
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

        public bool WorkItemCodeEnabled
        {
            get
            {
                return _workItemCodeEnabled;
            }
            set
            {
                _workItemCodeEnabled = value;
                RaisePropertyChanged(() => this.WorkItemCodeEnabled);
            }
        }

        public string RepairVisibility
        {
            get
            {
                return _repairVisibility;
            }
            set
            {
                _repairVisibility = value;
                RaisePropertyChanged(() => this.RepairVisibility);
            }
        }

        public string RepairDesMargin
        {
            get
            {
                return _repairDesMargin;
            }
            set
            {
                _repairDesMargin = value;
                RaisePropertyChanged(() => this.RepairDesMargin);
            }
        }

        public string RepDesTextMargin
        {
            get
            {
                return _repDesTextMargin;
            }
            set
            {
                _repDesTextMargin = value;
                RaisePropertyChanged(() => this.RepDesTextMargin);
            }
        }

        public string AddRepairBtnMargin
        {
            get
            {
                return _addRepairBtnMargin;
            }
            set
            {
                _addRepairBtnMargin = value;
                RaisePropertyChanged(() => this.AddRepairBtnMargin);
            }
        }

        public string RepairNoBinding
        {
            get
            {
                return _repairNoBinding;
            }
            set
            {
                _repairNoBinding = value;
                RaisePropertyChanged(() => this.RepairNoBinding);
            }
        }

        public string RepairNoTextBinding
        {
            get
            {
                return _repairNoTextBinding;
            }
            set
            {
                _repairNoTextBinding = value;
                RaisePropertyChanged(() => this.RepairNoTextBinding);
            }
        }

        public string PartCodeMargin
        {
            get
            {
                return _partCodeMargin;
            }
            set
            {
                _partCodeMargin = value;
                RaisePropertyChanged(() => this.PartCodeMargin);
            }
        }

        public string PartCodeMarginText
        {
            get
            {
                return _partCodeMarginText;
            }
            set
            {
                _partCodeMarginText = value;
                RaisePropertyChanged(() => this.PartCodeMarginText);
            }
        }

        public string AddPartBtnMargin
        {
            get
            {
                return _addPartBtnMargin;
            }
            set
            {
                _addPartBtnMargin = value;
                RaisePropertyChanged(() => this.AddPartBtnMargin);
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

        public string SelectedRepairNoRemove
        {
            get
            {
                return _selectedRepairNoRemove;
            }
            set
            {
                _selectedRepairNoRemove = value;
                RaisePropertyChanged(() => this.SelectedRepairNoRemove);
            }
        }

        #endregion


        #region COMMANDS

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

        public ICommand UpdateCommand
        {
            get
            {
                return _updateCommand ?? (_updateCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Update(), canExecute));
            }
        }


        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }
        #endregion
    }
}
