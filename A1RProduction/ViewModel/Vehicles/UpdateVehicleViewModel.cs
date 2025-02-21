using A1QSystem.DB;
using A1QSystem.Model.Vehicles;
using A1QSystem.ViewModel.Stock;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UserNotification.ViewModel.Base;

namespace A1QSystem.ViewModel.Vehicles
{
    public class UpdateVehicleViewModel : ViewModelBase
    {
        private Vehicle _vehicle;
        private List<StockLocation> _stockLocation;
        private List<VehicleCategory> _vehicleCategory;
        private int _selectedStockId;
        private int _selectedVehicleCategoryId;
        private int _selectedVehicle;
        private bool _updateVehicleEnDis;
        private string _updateVehicleBackground;
        private bool execute;
        private bool _isYes;
        private bool _isNo;
        private ObservableCollection<Vehicle> _vehicles;
        public event Action Closed;
        private DelegateCommand _closeCommand;
        private ICommand _updateCommand;
        private ICommand _clearCommand;

        public UpdateVehicleViewModel()
        {
            Vehicle = new Vehicle();
            execute = true;
            _closeCommand = new DelegateCommand(CloseForm);
            LoadVehicles();
            LoadVehicleLocations();
            LoadVehicleCategory();
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
            if(IsYes)
            {
                Vehicle.Active = true;
            }
            else if(IsNo)
            {
                Vehicle.Active = false;
            }

            int res = DBAccess.UpdateVehicle(Vehicle);
            if(res > 0)
            {
                Msg.Show("Vehicle has been updated successfully", "Vehicle Updated", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.OK);
            }
            else
            {
                Msg.Show("There has been a problem while updating details." + System.Environment.NewLine + "Please try again later ", "Cannot Update Vehicle", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }
            CloseForm();
        }

        private void LoadVehicles()
        {
            Vehicles = DBAccess.GetAllVehiclesActiveOrNot();
            Vehicles.Add(new Vehicle() { ID = 0, VehicleString = "Select" });
            SelectedVehicle = 0;
        }

        private void LoadVehicleLocations()
        {
            StockLocation = DBAccess.GetStockLocations();
            StockLocation.Add(new StockLocation() { ID = 0, StockName = "Select" });
            SelectedStockId = 0;
        }

        private void LoadVehicleCategory()
        {
            VehicleCategory = DBAccess.GetVehicleCategory();
            VehicleCategory.Add(new VehicleCategory() { ID = 0, VehicleType = "Select" });
            SelectedVehicleCategoryId = 0;
        }

        private void ClearData()
        {
            SelectedVehicle = 0;
            SelectedStockId = 0;
            SelectedVehicleCategoryId = 0;
            IsNo = false;
            IsYes = false;
            Vehicle = null;
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

        public Vehicle Vehicle
        {
            get
            {
                return _vehicle;
            }
            set
            {
                _vehicle = value;
                RaisePropertyChanged(() => this.Vehicle);
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

        public int SelectedVehicleCategoryId
        {
            get
            {
                return _selectedVehicleCategoryId;
            }
            set
            {
                _selectedVehicleCategoryId = value;
                RaisePropertyChanged(() => this.SelectedVehicleCategoryId);
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

        public List<VehicleCategory> VehicleCategory
        {
            get
            {
                return _vehicleCategory;
            }
            set
            {
                _vehicleCategory = value;
                RaisePropertyChanged(() => this.VehicleCategory);
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
               
                if(SelectedVehicle > 0)
                {
                    UpdateVehicleBackground = "#FF787C7A";
                    UpdateVehicleEnDis = true;

                    Vehicle = DBAccess.GetVehiclesByVehicleID(SelectedVehicle);
                    SelectedStockId = Vehicle.StockLocation.ID;
                    SelectedVehicleCategoryId = Vehicle.VehicleCategory.ID;

                    if(Vehicle.Active ==true)
                    {
                        IsYes = true;
                        IsNo = false ;
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

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
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

        
    }
}
