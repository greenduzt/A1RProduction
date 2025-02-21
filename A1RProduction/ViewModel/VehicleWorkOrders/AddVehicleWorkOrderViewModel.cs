using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Vehicles;
using A1QSystem.View;
using A1QSystem.View.AdminDashboard;
using A1QSystem.View.VehicleWorkOrders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace A1QSystem.ViewModel.VehicleWorkOrders
{
    public class AddVehicleWorkOrderViewModel : ViewModelBase
    {
        //private ObservableCollection<Vehicle> _vehicles;
        private ObservableCollection<VehicleWorkOrder> _vehicleWorkOrder;
        private ICollectionView _itemsView;
        private string _searchString;
        private string userName;
        private string state;
        private List<UserPrivilages> userPrivilages;
        private bool execute;
        private ChildWindowView myChildWindow;
        private List<MetaData> metaData;
        private string _version;

        private ICommand _doubleClickCommand;
        private ICommand _addWorkOrder;
        //private ICommand _viewEditCommand;
        private ICommand _homeCommand;
        private ICommand _vehiclesCommand;
        private ICommand _adminDashboardCommand;

        public AddVehicleWorkOrderViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            userPrivilages = up;
            execute = true;
            metaData = md;
            //Vehicles = new ObservableCollection<Vehicle>();
            VehicleWorkOrder = new ObservableCollection<VehicleWorkOrder>();
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
            //LoadVehicles();
            LoadWorkOrders();
            _itemsView = CollectionViewSource.GetDefaultView(VehicleWorkOrder);
            _itemsView.Filter = x => Filter(x as VehicleWorkOrder);
        }

        //private void LoadVehicles()
        //{
        //    Vehicles = DBAccess.GetAllVehicles();
        //}

        private void LoadWorkOrders()
        {
            VehicleWorkOrder = DBAccess.GetVehicleWorkOrders();
            foreach (var item in VehicleWorkOrder)
            {
                DateTime dumDate = (DateTime)item.NextServiceDate;
                double days = Math.Ceiling((dumDate - DateTime.Now).TotalDays);
                if (days < 0)
                {
                    item.DaysToComplete = Math.Abs(days) + " Day" + CheckNumber(days) + " Late";
                    item.DaysToCompleteBackgroundCol = "#FFC33333";
                    item.DaysToCompleteForeGroundCol = "White";
                }
                else if (days > 0)
                {
                    item.DaysToComplete = days + " Day" + CheckNumber(days) + " To Go";
                    item.DaysToCompleteBackgroundCol = "#009933";
                    item.DaysToCompleteForeGroundCol = "White";
                }
                else if (days == 0)
                {
                    item.DaysToComplete = "Service Today";
                    item.DaysToCompleteBackgroundCol = "#0424c1";
                    item.DaysToCompleteForeGroundCol = "White";
                }

                if (item.Urgency == 1)
                {
                    item.UrgencyBackgroundCol = "#FFC33333";
                    item.UrgencyForeGroundCol = "White";
                }
                else if (item.Urgency == 2)
                {
                    item.UrgencyBackgroundCol = "White";
                    item.UrgencyForeGroundCol = "Black";
                }
              
            }
        }

        private bool Filter(VehicleWorkOrder model)
        {
            var searchstring = (SearchString ?? string.Empty).ToLower();           

            return model != null &&
                 ((model.Vehicle.SerialNumber ?? string.Empty).ToLower().Contains(searchstring) ||
                 ((model.Vehicle.VehicleDescription ?? string.Empty).ToLower().Contains(searchstring) ||
                 ((model.Vehicle.VehicleCode).ToString() ?? string.Empty).ToLower().Contains(searchstring) ||
                 (model.WorkOrderType ?? string.Empty).ToLower().Contains(searchstring) ||
                 (model.NextServiceDate ?? null).ToString().ToLower().Contains(searchstring) ||
                 (model.Vehicle.VehicleBrand ?? string.Empty).ToLower().Contains(searchstring)));
        }
        private void ShowAddWorkOrderPopUp()
        {
            myChildWindow = new ChildWindowView();
            myChildWindow.newVehicleWorkOrder_Closed += ( r =>
            {
                VehicleWorkOrder.Clear();
                LoadWorkOrders();
            });
            myChildWindow.ShowNewVehicleWorkOrder(userName);
        }

        private string CheckNumber(double num)
        {
            string str = string.Empty;

            if ((num > 1) || (num < -1))
            {
                str = "s";
            }

            return str;
        }

        private void OpenUpdateWorkOrderWindow(Object parameter)
        {
            int index = VehicleWorkOrder.IndexOf(parameter as VehicleWorkOrder);
            if (index >= 0)
            {
                myChildWindow = new ChildWindowView();
                myChildWindow.updateVehicleWorkOrder_Closed += (r =>
                {
                    VehicleWorkOrder.Clear();
                    LoadWorkOrders();
                });
                myChildWindow.ShowUpdateVehicleWorkOrder(VehicleWorkOrder[index],userName);
            }
        }


        private bool CanExecute(object parameter)
        {
            return true;
        }

        public ICollectionView ItemsView
        {
            get { return _itemsView; }
        }   

        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                RaisePropertyChanged(() => this.SearchString);
                ItemsView.Refresh();

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

        public ObservableCollection<VehicleWorkOrder> VehicleWorkOrder
        {
            get { return _vehicleWorkOrder; }
            set
            {
                _vehicleWorkOrder = value;
                RaisePropertyChanged(() => this.VehicleWorkOrder);
            }
        }

        //public ObservableCollection<Vehicle> Vehicles
        //{
        //    get { return _vehicles; }
        //    set
        //    {
        //        _vehicles = value;
        //        RaisePropertyChanged(() => this.Vehicles);               
        //    }
        //}


        public ICommand AddWorkOrder
        {
            get
            {
                return _addWorkOrder ?? (_addWorkOrder = new A1QSystem.Commands.LogOutCommandHandler(() => ShowAddWorkOrderPopUp(), execute));
            }
        }

        //public ICommand ViewEditCommand
        //{
        //    get
        //    {
        //        return _viewEditCommand ?? (_viewEditCommand = new A1QSystem.Commands.LogOutCommandHandler(() => ShowAddWorkOrderPopUp(), execute));
        //    }
        //}

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, userPrivilages, metaData)), execute));
            }
        }

        public ICommand VehiclesCommand
        {
            get
            {
                return _vehiclesCommand ?? (_vehiclesCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new VehicleMenuView(userName, state, userPrivilages, metaData)), execute));
            }
        }

        public ICommand DoubleClickCommand
        {
            get
            {
                if (_doubleClickCommand == null)
                {
                    _doubleClickCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, OpenUpdateWorkOrderWindow);
                }
                return _doubleClickCommand;
            }
        }

        public ICommand AdminDashboardCommand
        {
            get
            {
                return _adminDashboardCommand ?? (_adminDashboardCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(userName, state, userPrivilages, metaData)), execute));
            }
        }
        
    }
}
