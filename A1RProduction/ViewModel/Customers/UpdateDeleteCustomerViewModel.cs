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

namespace A1QSystem.ViewModel.Customers
{
    public class UpdateDeleteCustomerViewModel : BaseViewModel
    {
        private string _userName;
        private string _state;
        private List<UserPrivilages> _privilages;
        private List<MetaData> metaData;
        private ICommand _backCommand;
        private ICommand navHomeCommand;
        private ICommand navCustomerCommand;
        private bool _canExecute;

        public UpdateDeleteCustomerViewModel(string UserName, string Password, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            _userName = UserName;
            _state = Password;
            _privilages = Privilages;
            metaData = md;
            _canExecute = true;
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged("UserName");
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new LogOutCommandHandler(() => Switcher.Switch(new CustomersMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
        public ICommand NavCustomerCommand
        {
            get
            {
                return navCustomerCommand ?? (navCustomerCommand = new LogOutCommandHandler(() => Switcher.Switch(new CustomersMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
    }
}
