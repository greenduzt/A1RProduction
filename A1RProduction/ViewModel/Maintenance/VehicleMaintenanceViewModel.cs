using A1QSystem.Commands;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Maintenance
{
    public class VehicleMaintenanceViewModel : CommonBase
    {
        private string _userName;
        private string _state;
        private List<UserPrivilages> _privilages;
        private string _btnVehicleWorkOrder;
        private string _btnAddVehicle;
        private List<MetaData> metaData;
        private ICommand _backCommand;
        private ICommand _vehiclWorkOrderCommand;
        private bool _canExecute;
        private string _version;

        public VehicleMaintenanceViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
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

        public string BtnVehicleWorkOrder
        {
            get { return _btnVehicleWorkOrder; }
            set
            {
                _btnVehicleWorkOrder = value;
                RaisePropertyChanged("BtnVehicleWorkOrder");
            }
        }

        public string BtnAddVehicle
        {
            get { return _btnAddVehicle; }
            set
            {
                _btnAddVehicle = value;
                RaisePropertyChanged("BtnAddVehicle");
            }
        }
        #endregion

        #region Commands

        public ICommand VehicleWorkOrderCommand
        {
            get
            {
                return null;// _vehiclWorkOrderCommand ?? (_vehiclWorkOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new VehicleWorkOrder(_userName, _state, _privilages)), _canExecute));
            }
        }
        public ICommand AddVehicleCommand
        {
            get
            {
                return null;
                //return _newVehicleCommand ?? (_newVehicleCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages)), _canExecute));
            }
        }
        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new LogOutCommandHandler(() => Switcher.Switch(new MaintenanceMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
      

        //public ICommand NavHomeCommand
        //{
        //    get
        //    {
        //        return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages)), _canExecute));
        //    }
        //}
        //public ICommand NavMaintenanceCommand
        //{
        //    get
        //    {
        //        return navMaintenanceCommand ?? (navMaintenanceCommand = new LogOutCommandHandler(() => Switcher.Switch(new MaintenanceMenuView(_userName, _state, _privilages)), _canExecute));
        //    }
        //}

        #endregion
    }
}
