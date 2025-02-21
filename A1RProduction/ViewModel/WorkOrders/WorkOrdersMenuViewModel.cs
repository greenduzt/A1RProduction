using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.WorkOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.WorkOrders
{
    public class WorkOrdersMenuViewModel : ViewModelBase
    {
        private string userName;
        private string state;
        private List<UserPrivilages> userPrivilages;
        private bool canExecute;
        private List<MetaData> metaData;
        private string _version;
        private ICommand _homeCommand;
        private ICommand _vehicleWorkOrderCommand;

        public WorkOrdersMenuViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
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


        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand VehicleWorkOrderCommand
        {
            get
            {
                return _vehicleWorkOrderCommand ?? (_vehicleWorkOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new VehicleWorkOrdersView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }
    }
}
