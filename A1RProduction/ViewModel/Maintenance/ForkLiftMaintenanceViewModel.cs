using A1QSystem.Commands;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Maintenance
{
    public class ForkLiftMaintenanceViewModel
    {
        private string _userName;
        private string _state;
        private List<UserPrivilages> _privilages;
        private List<MetaData> metaData;
        private ICommand _backCommand;
        private bool _canExecute;

        public ForkLiftMaintenanceViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            _userName = UserName;
            _state = State;
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
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new LogOutCommandHandler(() => Switcher.Switch(new ForkLiftMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
       
    }
}
