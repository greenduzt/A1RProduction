using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.AdminDashboard;
using A1QSystem.View.Machine;
using A1QSystem.View.Machine.MachineWorkOrders;
using A1QSystem.View.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Machine
{
    public class MachinesMenuViewModel : ViewModelBase
    {
        private string userName;
        private string state;
        private List<UserPrivilages> userPrivilages;
        private bool canExecute;
        private List<MetaData> metaData;
        private string _version;

        private ICommand _homeCommand;
        private ICommand _adminDashboardCommand;
        private ICommand _maintenanceCommand;
        private ICommand _scheduleMachineWorkOrderCommand;
        private ICommand _addNewMachineCommand;
        private ICommand _updateMachineCommand;
        private ICommand _newProviderCommand;

        public MachinesMenuViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            userPrivilages = up;
            canExecute = true;
            metaData = md;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
        }

        private void ShowAddNewMachine()
        {
            ChildWindowView myChildWindow = new ChildWindowView();
            myChildWindow.ShowAddNewMachine();
        }

        private void ShowUpdateMachine()
        {
            ChildWindowView myChildWindow = new ChildWindowView();
            myChildWindow.ShowUpdateMachine();
        }

        private void ShowProvider()
        {
            ChildWindowView myChildWindow = new ChildWindowView();
            //myChildWindow.providerScreenClosed += (r =>
            //{
            //    if (r == 1)
            //    {
            //        //Console.WriteLine("It is closed now");
            //    }
            //});
            myChildWindow.ShowProvider();
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

        #region COMMANDS

        
        public ICommand NewProviderCommand
        {
            get
            {
                return _newProviderCommand ?? (_newProviderCommand = new LogOutCommandHandler(() => ShowProvider(), canExecute));
            }
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand AdminDashboardCommand
        {
            get
            {
                return _adminDashboardCommand ?? (_adminDashboardCommand = new LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand MaintenanceCommand
        {
            get
            {
                return _maintenanceCommand ?? (_maintenanceCommand = new LogOutCommandHandler(() => Switcher.Switch(new MaintenanceMenuView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand ScheduleMachineWorkOrderCommand
        {
            get
            {
                return _scheduleMachineWorkOrderCommand ?? (_scheduleMachineWorkOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new ScehduleMachineWorkOrdersView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand AddNewMachineCommand
        {
            get
            {
                return _addNewMachineCommand ?? (_addNewMachineCommand = new LogOutCommandHandler(() => ShowAddNewMachine(), canExecute));
            }
        }

        public ICommand UpdateMachineCommand
        {
            get
            {
                return _updateMachineCommand ?? (_updateMachineCommand = new LogOutCommandHandler(() => ShowUpdateMachine(), canExecute));
            }
        }

        
        

        #endregion
    }
}
