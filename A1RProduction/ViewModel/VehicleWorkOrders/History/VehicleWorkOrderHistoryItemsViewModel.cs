using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model.Vehicles;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.ViewModel.VehicleWorkOrders.History
{
    public class VehicleWorkOrderHistoryItemsViewModel : ViewModelBase
    {
        private ObservableCollection<VehicleWorkDescription> _vehicleWorkDescription;
        private VehicleWorkOrderHistory _vehicleWorkOrderHistory;
        private string _partsOrdedVisibility;
        private string _repairWONoVisibility;
        private string _currentOdometerTitle;
        private Int64 _odometer;
        private string _hideOdo;
        private string _maintenanceDesHeader;
        public event Action Closed;
        private DelegateCommand _closeCommand;

        public VehicleWorkOrderHistoryItemsViewModel(VehicleWorkOrderHistory vwoh)
        {
            VehicleWorkOrderHistory = vwoh;
            VehicleWorkOrderHistory.VehicleWorkDescription = new ObservableCollection<VehicleWorkDescription>();

            if (VehicleWorkOrderHistory.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
            {
                RepairWONoVisibility = "Visible";
                PartsOrdedVisibility = "Collapsed";
                CurrentOdometerTitle = "Odometer reading in " + GetOdometerReadingTitle(VehicleWorkOrderHistory.Vehicle.VehicleBrand);
                Odometer = VehicleWorkOrderHistory.OdometerReading;
                MaintenanceDesHeader = VehicleWorkOrderHistory.MaintenanceFrequency + " Maintenance Description Schedule";

                VehicleWorkDescription = DBAccess.GetCompletedVehicleWorkDescriptionByID(VehicleWorkOrderHistory.VehicleWorkOrderID);
                //VehicleWorkDescription = new ObservableCollection<VehicleWorkDescription>();
                if (VehicleWorkDescription.Count > 0)
                {
                    ObservableCollection<VehicleRepairDescription> vehicleRepairDescriptionList = DBAccess.GetVehicleRepairDescriptionByID(VehicleWorkDescription);
                    ObservableCollection<VehicleParts> vehiclePartsList = DBAccess.GetVehiclePartsDescriptionByID(vehicleRepairDescriptionList);

                    foreach (var item in VehicleWorkDescription)
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

                if (VehicleWorkOrderHistory.Vehicle.VehicleBrand == "HDK")
                {
                    HideOdo = "Collapsed";
                }
                else
                {
                    HideOdo = "Visible";
                }

                //VehicleWorkOrderHistory.VehicleWorkOrderDetailsHistory = DBAccess.GetVehicleWorkDescriptionCompleted(VehicleWorkOrderHistory.VehicleWorkOrderID);
            }
            else if (VehicleWorkOrderHistory.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
            {
                PartsOrdedVisibility = "Visible";
                RepairWONoVisibility = "Collapsed";
                HideOdo = "Collapsed";

                ObservableCollection<VehicleRepairDescription> vehicleRepairDescriptionList = DBAccess.GetVehicleRepairDescriptionByID2(VehicleWorkOrderHistory.VehicleWorkOrderID);
                ObservableCollection<VehicleParts> vehiclePartsList = DBAccess.GetVehiclePartsDescriptionByID(vehicleRepairDescriptionList);
                Int32 id = 0;
                foreach (var item in vehicleRepairDescriptionList)
                {
                    id = item.VehicleWorkDescriptionID;
                    break;
                }

                //VehicleWorkDescription = DBAccess.GetVehicleWorkDescriptionForRepair(id);
                VehicleWorkDescription = new ObservableCollection<VehicleWorkDescription>();

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
                    VehicleWorkDescription.Add(new VehicleWorkDescription() { Description = "Repair order for " + VehicleWorkOrderHistory.Vehicle.SerialNumber, VehicleRepairDescription = vehicleRepairDescriptionList });
                }
                //VehicleWorkOrderHistory.VehicleWorkOrderDetailsHistory = DBAccess.GetVehicleWorkDescriptionRepairCompleted(VehicleWorkOrderHistory.VehicleWorkOrderID);          
            }

            _closeCommand = new DelegateCommand(CloseForm);
        }

        private string GetOdometerReadingTitle(string brand)
        {
            string n = string.Empty;

            if (brand == "Linde")
            {
                n = "Hours";
            }
            else
            {
                n = "Km";
            }

            return n;
        }

        public string MaintenanceDesHeader
        {
            get
            {
                return _maintenanceDesHeader;
            }
            set
            {
                _maintenanceDesHeader = value;
                RaisePropertyChanged(() => this.MaintenanceDesHeader);
            }
        }

        public string HideOdo
        {
            get
            {
                return _hideOdo;
            }
            set
            {
                _hideOdo = value;
                RaisePropertyChanged(() => this.HideOdo);
            }
        }

        public Int64 Odometer
        {
            get
            {
                return _odometer;
            }
            set
            {
                _odometer = value;
                RaisePropertyChanged(() => this.Odometer);
            }
        }

        public VehicleWorkOrderHistory VehicleWorkOrderHistory
        {
            get
            {
                return _vehicleWorkOrderHistory;
            }
            set
            {
                _vehicleWorkOrderHistory = value;
                RaisePropertyChanged(() => this.VehicleWorkOrderHistory);
            }
        }

        public string PartsOrdedVisibility
        {
            get { return _partsOrdedVisibility; }
            set
            {
                _partsOrdedVisibility = value;
                RaisePropertyChanged(() => this.PartsOrdedVisibility);
            }
        }

        public string RepairWONoVisibility
        {
            get { return _repairWONoVisibility; }
            set
            {
                _repairWONoVisibility = value;
                RaisePropertyChanged(() => this.RepairWONoVisibility);
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

        public string CurrentOdometerTitle
        {
            get
            {
                return _currentOdometerTitle;
            }
            set
            {
                _currentOdometerTitle = value;
                RaisePropertyChanged(() => this.CurrentOdometerTitle);
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }
    }
}
