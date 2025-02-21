using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.AdminDashboard;
using A1QSystem.View.Maintenance;
using A1QSystem.View.VehicleWorkOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.VehicleWorkOrders
{
    public class VehiclesMenuViewModel : ViewModelBase
    {
        private string userName;
        private string state;
        private List<UserPrivilages> userPrivilages;
        private bool canExecute;
        private List<MetaData> metaData;
        private string _version;
        private ICommand _homeCommand;
        private ICommand _newVehicleWorkOrderCommand;
        private ICommand _newRoutineWorkOrderCommand;
        private ICommand _adminDashboardCommand;
        private ICommand _scheduleVehicleWorkOrderCommand;
        private ICommand _addNewVehicleCommand;
        private ICommand _updateVehicleCommand;
        private ICommand _maintenanceCommand;

        public VehiclesMenuViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            userPrivilages = up;
            canExecute = true;
            metaData = md;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
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

        private void ShowAddNewVehicle()
        {
            ChildWindowView myChildWindow = new ChildWindowView();
            myChildWindow.ShowAddNewVehicle();
        }

        private void ShowUpdateVehicle()
        {
            ChildWindowView myChildWindow = new ChildWindowView();
            myChildWindow.ShowUpdateVehicle();
        }

        #region COMMANDS

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand NewVehicleWorkOrderCommand
        {
            get
            {
                return _newVehicleWorkOrderCommand ?? (_newVehicleWorkOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new AddVehicleWorkOrderView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand NewRoutineWorkOrderCommand
        {
            get
            {
                return _newRoutineWorkOrderCommand ?? (_newRoutineWorkOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new NewRoutineVehicleWorkOrderView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand AdminDashboardCommand
        {
            get
            {
                return _adminDashboardCommand ?? (_adminDashboardCommand = new LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand ScheduleVehicleWorkOrderCommand
        {
            get
            {
                return _scheduleVehicleWorkOrderCommand ?? (_scheduleVehicleWorkOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new ScheduleVehicleWorkOrdersView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand AddNewVehicleCommand
        {
            get
            {
                return _addNewVehicleCommand ?? (_addNewVehicleCommand = new LogOutCommandHandler(() => ShowAddNewVehicle(), canExecute));
            }
        }

        public ICommand UpdateVehicleCommand
        {
            get
            {
                return _updateVehicleCommand ?? (_updateVehicleCommand = new LogOutCommandHandler(() => ShowUpdateVehicle(), canExecute));
            }
        }

        public ICommand MaintenanceCommand
        {
            get
            {
                return _maintenanceCommand ?? (_maintenanceCommand = new LogOutCommandHandler(() => Switcher.Switch(new MaintenanceMenuView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        #endregion
    }
}
