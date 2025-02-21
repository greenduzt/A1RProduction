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
    public class ForkLiftMenuViewModel : CommonBase
    {
        private string _userName;
        private string _state;
        private List<UserPrivilages> _privilages;
        private string _btnForkLiftWorkOrder;
        private string _btnAddForklift;
        private string _btnUpdateForkLift;
        private List<MetaData> metaData;
        private ICommand _backCommand;
        private ICommand _forkLiftWorkOrderCommand;
        private ICommand _addForkLiftCommand;
        private ICommand _updateForkLiftCommand;
        private ICommand navHomeCommand;
        private ICommand navMaintenanceCommand;

        private bool _canExecute;

        public ForkLiftMenuViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            _userName = UserName;
            _state = State;
            _privilages = Privilages;
            metaData = md;
            _canExecute = true;

            //for (int i = 0; i < Privilages.Count; i++)
            //{
            //    BtnForkLiftWorkOrder = Privilages[i].ForkLiftWorkOrder;
            //    if (BtnForkLiftWorkOrder == "visible")
            //    {
            //        _btnForkLiftWorkOrder = "visible";
            //    }
            //    else
            //    {
            //        _btnForkLiftWorkOrder = "hidden";
            //    }
            //    BtnAddForklift = Privilages[i].AddForkLift;
            //    if (BtnAddForklift == "visible")
            //    {
            //        _btnAddForklift = "visible";
            //    }
            //    else
            //    {
            //        _btnAddForklift = "hidden";
            //    }
            //    BtnUpdateForkLift = Privilages[i].UpdateForkLift;
            //    if (BtnUpdateForkLift == "visible")
            //    {
            //        _btnUpdateForkLift = "visible";
            //    }
            //    else
            //    {
            //        _btnUpdateForkLift = "hidden";
            //    }


            //}
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
            }
        }

       
        public string BtnForkLiftWorkOrder
        {
            get { return _btnForkLiftWorkOrder; }
            set
            {
                _btnForkLiftWorkOrder = value;
                RaisePropertyChanged("BtnForkLiftWorkOrder");
            }
        }

        public string BtnAddForklift
        {
            get { return _btnAddForklift; }
            set
            {
                _btnAddForklift = value;
                RaisePropertyChanged("BtnAddForklift");
            }
        }

       
        public string BtnUpdateForkLift
        {
            get { return _btnUpdateForkLift; }
            set
            {
                _btnUpdateForkLift = value;
                RaisePropertyChanged("BtnUpdateForkLift");
            }
        }

        #region Commands

        public ICommand ForkLiftWorkOrderCommand
        {
            get
            {
                return null;// _forkLiftWorkOrderCommand ?? (_forkLiftWorkOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new ForkLiftMaintenanceView(_userName, _state, _privilages)), _canExecute));
            }
        }
        public ICommand AddForkLiftCommand
        {
            get
            {
                return _addForkLiftCommand ?? (_addForkLiftCommand = new LogOutCommandHandler(() => Switcher.Switch(new MaintenanceMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
        public ICommand UpdateForkLiftCommand
        {
            get
            {
                return _updateForkLiftCommand ?? (_updateForkLiftCommand = new LogOutCommandHandler(() => Switcher.Switch(new MaintenanceMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new LogOutCommandHandler(() => Switcher.Switch(new MaintenanceMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
      
        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
        public ICommand NavMaintenanceCommand
        {
            get
            {
                return navMaintenanceCommand ?? (navMaintenanceCommand = new LogOutCommandHandler(() => Switcher.Switch(new MaintenanceMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        #endregion
    }
}
