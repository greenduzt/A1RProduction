using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Users;
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
    public class CompleteRepairVehicleWorkOrderViewModel : ViewModelBase
    {
        private ObservableCollection<VehicleWorkDescription> _vehicleWorkDescription;
        private ObservableCollection<VehicleRepairDescription> _vehicleRepairDescriptions;
        private VehicleWorkOrder _vehicleWorkOrder;
        public event Action<int> Closed;
        private bool canExecute;
        private string _selectedMechanic;
        private bool _tickAll;
        private string userCompleted;
        private List<UserPosition> _userPositions;
        private DelegateCommand _closeCommand;
        private ICommand _completeCommand;

        public CompleteRepairVehicleWorkOrderViewModel(VehicleWorkOrder vwo, string uN)
        {
            UserPositions = new List<UserPosition>();
            VehicleWorkOrder = vwo;
            userCompleted = uN;
            _closeCommand = new DelegateCommand(CloseForm);
            canExecute = true;
            SelectedMechanic = "Select";
            LoadUserPositions();

            ObservableCollection<VehicleRepairDescription> vehicleRepairDescriptionList = DBAccess.GetVehicleRepairDescriptionByID2(VehicleWorkOrder.VehicleWorkOrderID);
            ObservableCollection<VehicleParts> vehiclePartsList = DBAccess.GetVehiclePartsDescriptionByID(vehicleRepairDescriptionList);
            Int32 id = 0;
            foreach (var item in vehicleRepairDescriptionList)
            {
                id = item.VehicleWorkDescriptionID;
                break;
            }

            VehicleWorkDescription = new ObservableCollection<VehicleWorkDescription>();
            //VehicleWorkDescription = DBAccess.GetVehicleWorkDescriptionForRepair(id);

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

            VehicleRepairDescriptions = vehicleRepairDescriptionList;
            //VehicleRepairDescriptions.Add(new VehicleRepairDescription() { StrSequenceNumber = "Select" });
        }

        private void CompleteOrder()
        {

            if (SelectedMechanic == "Select")
            {
                Msg.Show("Please select your name", "Select Mechanic Name", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else
            {
                bool itemNotCompleted = false;
                foreach (var item in VehicleWorkDescription)
                {
                    foreach (var items in item.VehicleRepairDescription)
                    {
                        if(items.IsCompleted == false)
                        {
                            itemNotCompleted = true;
                        }
                    }
                }

                if(itemNotCompleted == true)
                {
                    Msg.Show("Some task(s) were not done" + System.Environment.NewLine + System.Environment.NewLine + "Please complete all the task(s) to complete this work order", "Incomplete Tasks", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }
                else
                {
                    var data = UserPositions.SingleOrDefault(x=>x.FullName == SelectedMechanic);
                    if (data != null)
                    {
                        VehicleWorkOrder vwo = new VehicleWorkOrder();
                        vwo.VehicleWorkOrderID = VehicleWorkOrder.VehicleWorkOrderID;
                        vwo.User = new User() { ID = data.User.ID };
                        vwo.CreatedBy = userCompleted;
                        vwo.Vehicle = new Vehicle() { ID = VehicleWorkOrder.Vehicle.ID };
                        vwo.User = new User() { ID = data.User.ID };
                        vwo.IsCompleted = false;
                        vwo.CreatedDate = DateTime.Now;
                        vwo.CreatedBy = userCompleted;
                        vwo.CompletedDate = DateTime.Now;
                        vwo.CompletedBy = userCompleted;
                        vwo.Status = "Completed";
                        int res = DBAccess.RepairVehicleWorkOrderCompleted(vwo, VehicleWorkDescription);
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
            UserPositions.Add(new UserPosition() { FullName = "Select" });
        }

        private void CloseForm()
        {
            if (Closed != null)
            {               
                Closed(1);
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

                if(TickAll)
                {
                    foreach (var item in VehicleWorkDescription)
                    {
                        foreach (var itemz in item.VehicleRepairDescription)
                        {
                            itemz.IsCompleted = true;
                        }
                       
                    }
                }
                else
                {
                    foreach (var item in VehicleWorkDescription)
                    {
                        foreach (var itemz in item.VehicleRepairDescription)
                        {
                            itemz.IsCompleted = false;
                        }
                    }
                }
            }
        }        

        public ICommand CompleteCommand
        {
            get
            {
                return _completeCommand ?? (_completeCommand = new A1QSystem.Commands.LogOutCommandHandler(() => CompleteOrder(), canExecute));
            }
        }
        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }
    }
}
