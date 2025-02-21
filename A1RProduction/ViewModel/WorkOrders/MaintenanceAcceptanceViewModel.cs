
using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Users;
using A1QSystem.Model.Vehicles;
using A1QSystem.View;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace A1QSystem.ViewModel.WorkOrders
{
    public class MaintenanceAcceptanceViewModel : ViewModelBase
    {
        private string whichBttnClicked;
        private List<UserPosition> _userPositions;
        private VehicleWorkOrder vehicleWorkOrder;
        private string _odometerReading;
        private string _selectedMechanic;
        private UserPosition _selectedUserPosition;
        private bool canExecute;
        public bool isSubmiited;
        public event Action<MaintenanceAcceptanceViewModel> Closed;
        private DelegateCommand _closeCommand;
        private ICommand _submitCommand;

        public MaintenanceAcceptanceViewModel(VehicleWorkOrder vwo)
        {
            UserPositions = new List<UserPosition>();
            _closeCommand = new DelegateCommand(CloseForm);
            canExecute = true;
            LoadUserPositions();
            isSubmiited = false;
            vehicleWorkOrder = vwo;
            if (!string.IsNullOrWhiteSpace(vehicleWorkOrder.User.FullName))
            {
                SelectedMechanic = vehicleWorkOrder.User.FullName;
            }
            else
            {
                SelectedMechanic = "Select";
            }
            //OdometerReading = vehicleWorkOrder.OdometerReading;
        }

        private void LoadUserPositions()
        {
            UserPositions = DBAccess.GetAllUserPositions("VehicleMechanic");
            UserPositions.Add(new UserPosition() { FullName = "Select" });
        }

        private void SubmitDetails()
        {
            if(SelectedMechanic == "Select")
            {
                 Msg.Show("Please select mechanic's name", "Select Mechanic Name", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else if (String.IsNullOrEmpty(OdometerReading))
            {
                Msg.Show("Please enter the Odometer reading", "Odometer Reading Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else
            {
                SelectedUserPosition = UserPositions.Single(s => s.FullName == SelectedMechanic);

                isSubmiited = true;
                CloseForm();
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Exception exception = null;
                BackgroundWorker worker = new BackgroundWorker();
                ChildWindowView LoadingScreen;
                LoadingScreen = new ChildWindowView();
                LoadingScreen.ShowWaitingScreen("Printing");

                worker.DoWork += (_, __) =>
                {

                    if (SelectedUserPosition != null || SelectedMechanic != "Select" && isSubmiited)
                    {

                        vehicleWorkOrder.User = new User() { ID = SelectedUserPosition.User.ID, FullName = SelectedMechanic };
                        //vehicleWorkOrder.OdometerReading = OdometerReading;
                        int res = DBAccess.UpdateVehicleWorkOrderUserOdometer(vehicleWorkOrder);
                        if (res > 0)
                        {
                            ObservableCollection<VehicleWorkDescription> vehicleWorkDescription = new ObservableCollection<VehicleWorkDescription>();

                            if (vehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
                            {
                                vehicleWorkDescription = DBAccess.GetVehicleWorkDescriptionByID(vehicleWorkOrder.VehicleWorkOrderID);
                                if (vehicleWorkDescription.Count > 0)
                                {
                                    ObservableCollection<VehicleRepairDescription> vehicleRepairDescriptionList = DBAccess.GetVehicleRepairDescriptionByID(vehicleWorkDescription);
                                    ObservableCollection<VehicleParts> vehiclePartsList = DBAccess.GetVehiclePartsDescriptionByID(vehicleRepairDescriptionList);

                                    foreach (var item in vehicleWorkDescription)
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

                                //vehicleWorkDescription = DBAccess.GetVehicleWorkDescriptionByIDForMaintenance(vehicleWorkOrder.VehicleWorkOrderID);
                            }
                            else if (vehicleWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
                            {
                                ObservableCollection<VehicleRepairDescription> vehicleRepairDescriptionList = DBAccess.GetVehicleRepairDescriptionByID2(vehicleWorkOrder.VehicleWorkOrderID);
                                ObservableCollection<VehicleParts> vehiclePartsList = DBAccess.GetVehiclePartsDescriptionByID(vehicleRepairDescriptionList);
                                Int32 id = 0;
                                foreach (var item in vehicleRepairDescriptionList)
                                {
                                    id = item.VehicleWorkDescriptionID;
                                    break;
                                }

                                vehicleWorkDescription = DBAccess.GetVehicleWorkDescriptionForRepair(id);

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
                                if (vehicleRepairDescriptionList.Count > 0 && vehicleWorkDescription.Count > 0)
                                {
                                    vehicleWorkDescription[0].VehicleRepairDescription = vehicleRepairDescriptionList;
                                }
                                else
                                {
                                    vehicleWorkDescription.Add(new VehicleWorkDescription() { VehicleRepairDescription = vehicleRepairDescriptionList });
                                }
                            }
                            //VehicleWorkOrderPDF vwopdf = new VehicleWorkOrderPDF(vehicleWorkOrder, vehicleWorkDescription);
                            //exception = vwopdf.createWorkOrderPDF();
                        }
                    }

                    Closed(this);

                };

                worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    LoadingScreen.CloseWaitingScreen();
                    if (exception != null)
                    {
                        //DBAccess.InsertErrorLog(DateTime.Now + " Printing label error " + res.ToString());
                        //Msg.Show("A problem has occured while work order is prining. Please try again later.", "Printing Error", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                        Msg.Show("A problem has occured while the work order is prining. Please try again later." + System.Environment.NewLine + exception, "Printing Error", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                    }

                };
                worker.RunWorkerAsync();
            }
        }

        public string OdometerReading
        {
            get { return _odometerReading; }
            set
            {
                _odometerReading = value;
                RaisePropertyChanged(() => this.OdometerReading);
            }
        }

        public string SelectedMechanic
        {
            get { return _selectedMechanic; }
            set
            {
                _selectedMechanic = value;
                RaisePropertyChanged(() => this.SelectedMechanic);
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

        public UserPosition SelectedUserPosition
        {
            get
            {
                return _selectedUserPosition;
            }
            set
            {
                _selectedUserPosition = value;
                RaisePropertyChanged(() => this.SelectedUserPosition);
            }
        }

        

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ICommand SubmitCommand
        {
            get
            {
                return _submitCommand ?? (_submitCommand = new A1QSystem.Commands.LogOutCommandHandler(() =>SubmitDetails(), canExecute));
            }
        }
    }
}
