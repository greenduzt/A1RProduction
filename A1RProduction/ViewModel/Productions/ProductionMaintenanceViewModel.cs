using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.AdminDashboard;
using A1QSystem.View.Production;
using A1QSystem.View.Production.Mixing;
using A1QSystem.View.Production.Slitting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Productions
{
    public class ProductionMaintenanceViewModel : ViewModelBase
    {
        private string userName;
        private string state;
        private List<UserPrivilages> userPrivilages;
        private bool canExecute;
        private List<MetaData> metaData;
        private string _version;
        private ICommand _homeCommand;
        private ICommand _adminDashboardCommand;
        private ICommand _gradingSchedulerCommand;
        private ICommand _mixingSchedulerCommand;
        private ICommand _slittingSchedulerCommand;

        public ProductionMaintenanceViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
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
                return _homeCommand ?? (_homeCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand AdminDashboardCommand
        {
            get
            {
                return _adminDashboardCommand ?? (_adminDashboardCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand GradingSchedulerCommand
        {
            get
            {
                return _gradingSchedulerCommand ?? (_gradingSchedulerCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new ProductionSchedulerView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand MixingSchedulerCommand
        {
            get
            {
                return _mixingSchedulerCommand ?? (_mixingSchedulerCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new MixingProductionSchedulerView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand SlittingSchedulerCommand
        {
            get
            {
                return _slittingSchedulerCommand ?? (_slittingSchedulerCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new SlittingSchedulerView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        
    }
}
