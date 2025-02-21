using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
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

namespace A1QSystem.ViewModel.VehicleWorkOrders
{
    public class InnerVehicleRepairWorkOrderViewModel : ViewModelBase
    {
        public VehicleRepairDescription SelectedItem { get; set; }

        private Int32 vehicleRepairWorkOrderId;
        private VehicleRepairWorkOrder _vehicleRepairWorkOrder;
        private VehicleWorkDescription _vehicleWorkDescription;
        private string _selectedOrderType;
        private bool canExecute;
        private int _itemCount;
        private string userName;
        public event Action<Int32> Closed;
        private ChildWindowView myChildWindow;
        private DelegateCommand _closeCommand;
        private ICommand _completeCommand;
        private ICommand _addItemCommand;

        public InnerVehicleRepairWorkOrderViewModel(VehicleWorkDescription vwd, string un)
        {
            this.DeleteItem = new A1QSystem.Commands.DeleteIVehicleRepairItemCommand(this);

            VehicleWorkDescription = vwd;
            userName = un;
            
            VehicleRepairWorkOrder = new VehicleRepairWorkOrder();
            VehicleRepairWorkOrder.VehicleRepairDescription = new ObservableCollection<VehicleRepairDescription>();
            _closeCommand = new DelegateCommand(CloseForm); 
            canExecute = true;
            SelectedOrderType = "Normal";
            VehicleRepairWorkOrder.VehicleRepairDescription.CollectionChanged += OnGroceryListChanged;
            // Initialize list index
            this.VehicleRepairWorkOrder.VehicleRepairDescription = SequencingService.SetCollectionSequence(this.VehicleRepairWorkOrder.VehicleRepairDescription);
           
        }

        void OnGroceryListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Update item count
            this.ItemCount = this.VehicleRepairWorkOrder.VehicleRepairDescription.Count;

            // Resequence list
            SequencingService.SetCollectionSequence(this.VehicleRepairWorkOrder.VehicleRepairDescription);
        }

        private void CreateRepairWorkOrder()
        {
            var itemToRemove = VehicleRepairWorkOrder.VehicleRepairDescription.Where(x => String.IsNullOrWhiteSpace(x.RepairDescription)).ToList();
            foreach (var item in itemToRemove)
            {
                VehicleRepairWorkOrder.VehicleRepairDescription.Remove(item);
            }

            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            VehicleRepairWorkOrder.VehicleWorkDescriptionID = VehicleWorkDescription.ID;
            VehicleRepairWorkOrder.Vehicle = new Vehicle() { ID = VehicleWorkDescription.VehicleWorkOrder.Vehicle.ID };
            VehicleRepairWorkOrder.User = new User() { ID = 0 };
            VehicleRepairWorkOrder.WorkOrderType = VehicleWorkOrderTypesEnum.Repair.ToString();
            VehicleRepairWorkOrder.MaintenanceFrequency = "One Off";
            VehicleRepairWorkOrder.FirstServiceDate = CalculateDate();
            VehicleRepairWorkOrder.NextServiceDate = CalculateDate();
            VehicleRepairWorkOrder.OdometerReading = 0;
            VehicleRepairWorkOrder.IsCompleted = false;
            VehicleRepairWorkOrder.CreatedDate = DateTime.Now;
            VehicleRepairWorkOrder.CreatedBy = userName;
            VehicleRepairWorkOrder.Status = "Pending";
            VehicleRepairWorkOrder.Urgency = ConvertUrgency();


            //foreach (var item in VehicleRepairWorkOrder.VehicleRepairDescription)
            //{
            //    vwo.VehicleMaintenanceInfo.Add(new VehicleMaintenanceInfo() { ID = item.SequenceNumber, VehicleMaintenanceSequence = new VehicleMaintenanceSequence() { ID = 0 }, Description = item.RepairDescription, Active = true });
            //}

            vehicleRepairWorkOrderId = DBAccess.InsertNewVehicleRepairWorkOrder(VehicleRepairWorkOrder, VehicleWorkDescription.ID);
            if (vehicleRepairWorkOrderId > 0)
            {
                Msg.Show("Repair work order no " + vehicleRepairWorkOrderId + " has been successfully created!!!", "Repair Work Order Created", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
                
                CloseForm();
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

        private DateTime CalculateDate()
        {
            DateTime d = DateTime.Now.AddDays(+1);
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();

            switch (SelectedOrderType)
            {
                case "Normal": d = bdg.SkipWeekends(DateTime.Now.AddDays(+7));
                    break;
                case "Urgent": d = bdg.SkipWeekends(DateTime.Now.AddDays(+1)); ;
                    break;
                default:
                    d = bdg.SkipWeekends(DateTime.Now.AddDays(+1));
                    break;
            }

            return d;
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed(vehicleRepairWorkOrderId);
                myChildWindow = new ChildWindowView();
                myChildWindow.ShowCompleteVehicleWorkOrder(VehicleWorkDescription.VehicleWorkOrder);
            }
        }

        private void AddNewItem()
        {
            if (VehicleRepairWorkOrder.VehicleRepairDescription.Count > 0)
            {
                VehicleRepairDescription vd = VehicleRepairWorkOrder.VehicleRepairDescription.Last();
                if (!string.IsNullOrWhiteSpace(vd.RepairDescription))
                {
                    VehicleRepairWorkOrder.VehicleRepairDescription.Add(new VehicleRepairDescription() { RepairDescription = string.Empty });
                }
                else
                {
                    Msg.Show("Please enter repair description for Item No " + vd.SequenceNumber, "", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
                }
            }
        }
        #region PUBLIC_PROPERTIES

        public int ItemCount
        {
            get { return _itemCount; }

            set
            {
                _itemCount = value;
                base.RaisePropertyChanged(() => this.ItemCount);
            }
        }

        public VehicleWorkDescription VehicleWorkDescription
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

        #endregion

        #region COMMANDS

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ICommand CompleteCommand
        {
            get
            {
                return _completeCommand ?? (_completeCommand = new A1QSystem.Commands.LogOutCommandHandler(() => CreateRepairWorkOrder(), canExecute));
            }
        }

        public ICommand AddItemCommand
        {
            get
            {
                return _addItemCommand ?? (_addItemCommand = new A1QSystem.Commands.LogOutCommandHandler(() => AddNewItem(), canExecute));
            }
        }

        public ICommand DeleteItem { get; set; }

        #endregion
    }
}
