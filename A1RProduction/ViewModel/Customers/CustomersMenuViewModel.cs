using A1QSystem.Commands;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel
{
    public class CustomersMenuViewModel : BaseViewModel
    {
        private string _userName;
        private string _state;
        private List<UserPrivilages> _privilages;
        private string _btnAddCustomer;
        private string _btnUpdateCustomer;
        private List<MetaData> metaData;
        private ICommand _addCustomerCommand;
        private ICommand _updateDeleteCustomerCommand;
        private ICommand _backCommand;
        private ICommand navHomeCommand;
        private bool _canExecute;

        public CustomersMenuViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            _userName = UserName;
            _state = State;
            _privilages = Privilages;
            metaData = md;
            _canExecute = true;

            //for (int i = 0; i < _privilages.Count; i++)
            //{
            //    BtnAddCustomer = _privilages[i].AddCustomer;
            //    if (BtnAddCustomer == "visible")
            //    {
            //        _btnAddCustomer = "visible";
            //    }
            //    else
            //    {
            //        _btnAddCustomer = "hidden";
            //    }
            //    BtnUpdateCustomer = _privilages[i].UpdateCustomer;
            //    if (BtnUpdateCustomer == "visible")
            //    {
            //        _btnUpdateCustomer = "visible";
            //    }
            //    else
            //    {
            //        _btnUpdateCustomer = "hidden";
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

        public string BtnAddCustomer
        {
            get { return _btnAddCustomer; }
            set
            {
                _btnAddCustomer = value;
                RaisePropertyChanged("BtnAddCustomer");
            }
        }
        public string BtnUpdateCustomer
        {
            get { return _btnUpdateCustomer; }
            set
            {
                _btnUpdateCustomer = value;
                RaisePropertyChanged("BtnUpdateCustomer");
            }
        }
       

        #region Commands
        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
        public ICommand AddCustomerCommand
        {
            get
            {
                return _addCustomerCommand ?? (_addCustomerCommand = new LogOutCommandHandler(() => Switcher.Switch(new AddCustomerMainView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand UpdateDeleteCustomerCommand
        {
            get
            {
                return _updateDeleteCustomerCommand ?? (_updateDeleteCustomerCommand = new LogOutCommandHandler(() => Switcher.Switch(new UpdateDeleteCustomer(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

       
        #endregion

    }
}
