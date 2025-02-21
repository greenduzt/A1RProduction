using A1QSystem.Commands;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.AdminDashboard;
using A1QSystem.View.Machine;
using A1QSystem.View.Maintenance;
using A1QSystem.View.VehicleWorkOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Maintenance
{
    public class MaintenanceMenuViewModel : CommonBase
    {
        private string _userName;
        private string _state;
        private List<UserPrivilages> _privilages;
        private string _btnForklifts;
        private string _btnVehicles;
        private List<MetaData> metaData;
        private string _version;
        private ICommand _adminDashboardCommand;
        private ICommand navHomeCommand;
        private ICommand _vehiclesCommand;
        private ICommand _maintenanceCommand;
        private ICommand _machinesCommand;
        private ICommand _miscellaniousCommand;
        private bool _canExecute;

        public MaintenanceMenuViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md) 
        {
            _userName = UserName;
            _state = State;
            _privilages = Privilages;
            metaData = md;
            _canExecute = true;

            var data = metaData.SingleOrDefault(x => x.KeyName == "version");

            Version = data.Description;
        }

        #region Public Properties

        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
                RaisePropertyChanged("Version");
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
               _userName = value;
                
            }
        }
      
        public string BtnForklifts
        {
            get { return _btnForklifts; }
            set
            {
                _btnForklifts = value;
                RaisePropertyChanged("BtnForklifts");
            }
        }

        public string BtnVehicles
        {
            get { return _btnVehicles; }
            set
            {
                _btnVehicles = value;
                RaisePropertyChanged("BtnVehicles");
            }
        }

        #endregion

        #region Commands
     
        public ICommand AdminDashboardCommand
        {
            get
            {
                return _adminDashboardCommand ?? (_adminDashboardCommand = new LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
      
        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand VehiclesCommand
        {
            get
            {
                return _vehiclesCommand ?? (_vehiclesCommand = new LogOutCommandHandler(() => Switcher.Switch(new VehicleMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand MaintenanceCommand
        {
            get
            {
                return _maintenanceCommand ?? (_maintenanceCommand = new LogOutCommandHandler(() => Switcher.Switch(new VehicleMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand MachinesCommand
        {
            get
            {
                return _machinesCommand ?? (_machinesCommand = new LogOutCommandHandler(() => Switcher.Switch(new MachinesMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand MiscellaniousCommand
        {
            get
            {
                return _miscellaniousCommand ?? (_miscellaniousCommand = new LogOutCommandHandler(() => Switcher.Switch(new MiscellaneousWorkOrderView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
        
        #endregion 
    }
}
