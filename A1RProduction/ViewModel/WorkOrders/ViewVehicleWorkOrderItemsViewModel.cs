using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
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

namespace A1QSystem.ViewModel.WorkOrders
{
    public class ViewVehicleWorkOrderItemsViewModel : ViewModelBase
    {
        private ObservableCollection<VehicleWorkDescription> _vehicleWorkDescription;
        private ObservableCollection<VehicleRepairDescription> _vehicleRepairDescriptions;
        private VehicleWorkOrder _vehicleWorkOrder;
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
        private bool canExecute;
        public event Action Closed;
        private DelegateCommand _closeCommand;
        private ICommand _addToRepairCommand;
        private ICommand _addToPartCommand;
        private ICommand _updateCommand;
        //private ICommand _removeRepairItemCommand;

        public ViewVehicleWorkOrderItemsViewModel(VehicleWorkOrder vwo)
        {
            VehicleWorkOrder = vwo;
            VehicleWorkDescription = new ObservableCollection<VehicleWorkDescription>();
            VehicleRepairDescriptions = new ObservableCollection<VehicleRepairDescription>();
            SelectedItemCode = "Select";
            SelectedItemCodePart = "Select";
            canExecute = true;
            AddRepairEnabled = false;
            if (VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
            {
                WorkItemCodeEnabled = true;
                RepairVisibility = "Visible";
                RepairDesMargin = "261,126,0,0";
                RepDesTextMargin = "383,127,0,0";
                AddRepairBtnMargin = "0,18,13,0";
                RepairNoBinding = "261,190,0,0";
                RepairNoTextBinding = "383,193,0,0";
                PartCodeMargin ="489,190,0,0";
                PartCodeMarginText ="559,194,0,0";
                AddPartBtnMargin ="0,22,13,0";
                VehicleWorkDescription = DBAccess.GetVehicleWorkDescriptionByID(VehicleWorkOrder.VehicleWorkOrderID);
                if(VehicleWorkDescription.Count > 0)
                {
                    ObservableCollection<VehicleRepairDescription> vehicleRepairDescriptionList = DBAccess.GetVehicleRepairDescriptionByID(VehicleWorkDescription);
                    ObservableCollection<VehicleParts> vehiclePartsList = DBAccess.GetVehiclePartsDescriptionByID(vehicleRepairDescriptionList);                    

                    foreach (var item in VehicleWorkDescription)
                    {
                        item.VehicleRepairDescription = new ObservableCollection<VehicleRepairDescription>();                        
                        foreach (var items in vehicleRepairDescriptionList)
                        {                            
                            if(item.ID == items.VehicleWorkDescriptionID)
                            {                                
                                item.VehicleRepairDescription.Add(items);
                                items.Vehicleparts = new ObservableCollection<VehicleParts>();
                                foreach (var itemz in vehiclePartsList)
                                {
                                   if(items.ID == itemz.VehicleRepairID)
                                   {
                                       items.Vehicleparts.Add(itemz);
                                   }
                                }
                            }
                        }                        
                    }
                    VehicleWorkDescription.Add(new VehicleWorkDescription() { VehicleMaintenanceInfo = new VehicleMaintenanceInfo() { Code = "Select" } });
                }               
            }
            else if (vwo.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
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
                        if(item.ID  == items.VehicleRepairID)
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

                VehicleRepairDescriptions = vehicleRepairDescriptionList;
                VehicleRepairDescriptions.Add(new VehicleRepairDescription() { StrSequenceNumber = "Select" });
            }

            _closeCommand = new DelegateCommand(CloseForm);            
        }

        private void AddToRepair()
        {
            if (VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
            {
                foreach (var item in VehicleWorkDescription)
                {
                    if(item.VehicleMaintenanceInfo.Code == SelectedItemCode)
                    {
                       item.VehicleRepairDescription.Add(new VehicleRepairDescription() { VehicleWorkDescriptionID = item.ID,RepairDescription= RepairDescription,IsActive=true});
                       SequencingService.SetCollectionSequence(item.VehicleRepairDescription);
                       break;
                    }
                }
            }
            else if (VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
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
                            if(String.IsNullOrWhiteSpace(item.VehicleRepairDescription[i].RepairDescription))
                            {
                                item.VehicleRepairDescription.RemoveAt(i);
                            }
                        }
                        SequencingService.SetCollectionSequence(item.VehicleRepairDescription);
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
            foreach (var item in VehicleWorkDescription)
            {
                if (VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
                {

                    if (item.VehicleMaintenanceInfo.Code == SelectedItemCodePart)
                    {
                        foreach (var items in item.VehicleRepairDescription)
                        {
                            if (items.StrSequenceNumber == SelectedRepairNo)
                            {
                                items.Vehicleparts.Add(new VehicleParts() { VehicleRepairID = items.ID, PartCode = PartCode });
                                // Resequence list
                                SequencingService.SetCollectionSequence(items.Vehicleparts);
                            }
                        }
                    }
                }
                else if (VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
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
            }
            SelectedItemCodePart = "Select";
            SelectedRepairNo = "Select";
            PartCode = String.Empty;
        }

        private void EnDisAddRepairButton()
        {
            if (VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
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
            else if (VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
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

        private void EnDisAddPartButton()
        {
            if (VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
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
            else if (VehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
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

        private void Update()
        {
            VehicleWorkOrder repairVehicleWorkOrder = new VehicleWorkOrder();
            repairVehicleWorkOrder.Vehicle = new Vehicle() { ID = VehicleWorkOrder.Vehicle.ID };
            repairVehicleWorkOrder.User = new User() { ID = VehicleWorkOrder.User.ID };
            repairVehicleWorkOrder.WorkOrderType = VehicleWorkOrderTypesEnum.Repair.ToString();
            repairVehicleWorkOrder.MaintenanceFrequency = "One Off";
            repairVehicleWorkOrder.FirstServiceDate = CalculateDate();
            repairVehicleWorkOrder.NextServiceDate = CalculateDate();
            repairVehicleWorkOrder.OdometerReading = 0;
            repairVehicleWorkOrder.IsCompleted = false;
            repairVehicleWorkOrder.CreatedDate = DateTime.Now;
            repairVehicleWorkOrder.CreatedBy = VehicleWorkOrder.User.FullName;
            repairVehicleWorkOrder.Status = "Pending";
            repairVehicleWorkOrder.Urgency = 1;

            //foreach (var item in VehicleWorkDescription)
            //{
            //    if(item.VehicleRepairDescription.Count > 0)
            //    {
            //        repairVehicleWorkOrder = new VehicleWorkOrder()
            //        {
            //            //BusinessDaysGenerator bdg = new BusinessDaysGenerator()
            //            Vehicle = new Vehicle() { ID = VehicleWorkOrder.Vehicle.ID },
            //            User = new User() { ID = VehicleWorkOrder.User.ID },
            //            WorkOrderType = VehicleWorkOrderTypesEnum.Repair.ToString(),
            //            MaintenanceFrequency = "One Off",
            //            FirstServiceDate = CalculateDate(),
            //            NextServiceDate = CalculateDate(),
            //            OdometerReading = string.Empty,
            //            IsCompleted = false,
            //            CreatedDate = DateTime.Now,
            //            CreatedBy = VehicleWorkOrder.User.FullName,
            //            Status = "Pending",
            //            Urgency = 1
            //        };
            //    }
            //    break;
            //}

            int x = DBAccess.UpdateRepairPart(VehicleWorkOrder, VehicleWorkDescription, repairVehicleWorkOrder);
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

        //private int ConvertUrgency()
        //{
        //    int x = 0;
        //    switch (SelectedOrderType)
        //    {
        //        case "Normal": x = 2;
        //            break;
        //        case "Urgent": x = 1;
        //            break;
        //        default:
        //            x = 2;
        //            break;
        //    }

        //    return x;
        //}

        //private void RemoveRepairItem(object parameter)
        //{
        //    //int index = VehicleWorkDescription.OrderDetails.IndexOf(parameter as OrderDetails);
        //    //if (index > -1 && index < Order.OrderDetails.Count)
        //    //{
               
        //    //}

        //    foreach (var item in VehicleWorkDescription)
        //    {
        //        int index = item.VehicleRepairDescription.IndexOf(parameter as VehicleRepairDescription);
        //        if (index > -1 && index < item.VehicleRepairDescription.Count)
        //        {

        //        }
        //    }
        //}

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        private bool CanExecute(object parameter)
        {
            return true;
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
                    VehicleRepairDescriptions.Clear();
                    
                    foreach (var item in VehicleWorkDescription)
                    {
                        if(item.VehicleMaintenanceInfo.Code == SelectedItemCodePart)
                        {
                            foreach (var items in item.VehicleRepairDescription)
	                        {
                                VehicleRepairDescriptions.Add(new VehicleRepairDescription() { SequenceNumber = items.SequenceNumber,StrSequenceNumber = items.StrSequenceNumber,RepairDescription=items.RepairDescription });
	                        }                            
                        }
                    }

                    VehicleRepairDescriptions.Add(new VehicleRepairDescription(){StrSequenceNumber="Select"});
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
                if(AddPartEnabled == true)
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

        //public ICommand RemoveRepairItemCommand
        //{
        //    get
        //    {
        //        if (_removeRepairItemCommand == null)
        //        {
        //            _removeRepairItemCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, RemoveRepairItem);
        //        }
        //        return _removeRepairItemCommand;
        //    }
        //}
        
        

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }
        #endregion
    }
}
