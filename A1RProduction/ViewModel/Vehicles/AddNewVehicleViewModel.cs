using A1QSystem.Core;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Vehicles
{
    public class AddNewVehicleViewModel : ViewModelBase
    {
        private List<StockLocation> _stockLocation;
        private List<VehicleCategory> _vehicleCategory;
        private int _selectedStockId;
        private int _selectedVehicleCategoryId;
        private string _serialNumber;
        private string _brand;
        private string _description;
        private string _addNewVehicleBackground;
        public event Action Closed;
        private DelegateCommand _closeCommand;
        private ICommand _crateCommand;
        private bool execute;
        private bool _vehicleEnDis;

        public AddNewVehicleViewModel()
        {
            _closeCommand = new DelegateCommand(CloseForm);
            execute = true;
            StockLocation = new List<StockLocation>();
            LoadVehicleLocations();
            LoadVehicleCategory();
            AddNewVehicleBackground = "#FFDEDEDE";
            VehicleEnDis = false;
        }

        private void LoadVehicleLocations()
        {
            StockLocation = DBAccess.GetStockLocations();
            StockLocation.Add(new StockLocation() { ID= 0, StockName = "Select"});
            SelectedStockId = 0;
        }

        private void LoadVehicleCategory()
        {
            VehicleCategory =DBAccess.GetVehicleCategory();
            VehicleCategory.Add(new VehicleCategory() { ID = 0,VehicleType="Select"});
            SelectedVehicleCategoryId = 0;
        }

        private void AddVehicle()
        {
            List<int> nos = new List<int>();
            ObservableCollection<ScheduledVehicle> ScheduledVehicle = DBAccess.GetAllVehiclesByCategoryID(SelectedVehicleCategoryId);

            foreach (var item in ScheduledVehicle)
            {
                nos.Add(Convert.ToInt16(Regex.Replace(item.VehicleCode, "[^0-9]+", string.Empty)));
            }

            nos.Sort();
            int last = nos.Last();
            last = last + 1;
           
           

            Vehicle veh = new Vehicle();
            veh.StockLocation = new StockLocation() { ID = SelectedStockId };
            veh.VehicleCategory = new VehicleCategory() { ID = SelectedVehicleCategoryId };
            veh.VehicleCode = GetCode()+last;
            veh.SerialNumber = SerialNumber;
            veh.VehicleBrand = Brand;
            veh.VehicleDescription = Description;
            veh.Active = true;
            int res = DBAccess.InsertNewVehicle(veh);
            if (res > 0)
            {
                Clear();
                CloseForm();
                Msg.Show("Vehicle has been added successfully.", "New Vehicle Added", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.OK);
            }
        }

        private string GetCode()
        {
            string code = string.Empty;
            switch (SelectedVehicleCategoryId)
            {
                case 1: code = "FL";
                    break;
                case 2: code = "TRU";
                    break;
                case 3: code = "UTE";
                    break;
                case 4: code = "CAR";
                    break;
                case 5: code = "SW";
                    break;
                case 6: code = "BUG";
                    break;
                case 7: code = "CAD";
                    break;
                default:
                    break;
            }

            return code;
        }
        private void Clear()
        {
            SelectedStockId = 0;
            SelectedVehicleCategoryId = 0;
            SerialNumber = string.Empty;
            Brand = string.Empty;
            Description = string.Empty;
        }

        private void EnDisCreateButton()
        {
            if (SelectedStockId == 0 || SelectedVehicleCategoryId == 0 || String.IsNullOrWhiteSpace(SerialNumber) || String.IsNullOrWhiteSpace(Brand) || String.IsNullOrWhiteSpace(Description))
            {
                AddNewVehicleBackground = "#FFDEDEDE";
                VehicleEnDis = false;
            }
            else
            {
                AddNewVehicleBackground = "#FF787C7A";
                VehicleEnDis = true;
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
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
                EnDisCreateButton();
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
                EnDisCreateButton();
            }
        }

        public string SerialNumber
        {
            get
            {
                return _serialNumber;
            }
            set
            {
                _serialNumber = value;
                RaisePropertyChanged(() => this.SerialNumber);
                EnDisCreateButton();
            }
        }

        public string Brand
        {
            get
            {
                return _brand;
            }
            set
            {
                _brand = value;
                RaisePropertyChanged(() => this.Brand);
                EnDisCreateButton();
            }
        }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                RaisePropertyChanged(() => this.Description);
                EnDisCreateButton();
            }
        }

        public string AddNewVehicleBackground
        {
            get
            {
                return _addNewVehicleBackground;
            }
            set
            {
                _addNewVehicleBackground = value;
                RaisePropertyChanged(() => this.AddNewVehicleBackground);
            }
        }

        public bool VehicleEnDis
        {
            get
            {
                return _vehicleEnDis;
            }
            set
            {
                _vehicleEnDis = value;
                RaisePropertyChanged(() => this.VehicleEnDis);
            }
        }
        
        

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ICommand CrateCommand
        {
            get
            {
                return _crateCommand ?? (_crateCommand = new A1QSystem.Commands.LogOutCommandHandler(() => AddVehicle(), execute));
            }
        }

        
    }
}
