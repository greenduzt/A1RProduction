using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.CuringProducts;
using A1QSystem.View.Production.Grading;
using A1QSystem.View.Production.Mixing;
using A1QSystem.View.Production.Peeling;
using A1QSystem.View.Production.ReRolling;
using A1QSystem.View.Production.SlitPeel;
using A1QSystem.View.Production.Slitting;
using A1QSystem.View.Production.WeeklyScheduleFull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Dashboard
{
    public class WorkStationsViewModel : ViewModelBase
    {
        private ICommand _navHomeCommand;
        private ICommand _gradingCommand;
        private ICommand _mixingCommand;
        private ICommand _peelingCommand;
        private ICommand _slittingCommand;
        private ICommand _curingCommand;
        private ICommand _reRollingCommand;
        private ICommand _weeklyScheduleCommand;
        private string userName;
        private string state;
        private List<UserPrivilages> privilages;
        private string _version;
        private bool canExecute;
        private List<MetaData> metaData;

        public WorkStationsViewModel(string UserName, string State, List<UserPrivilages> uPriv, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = uPriv;
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
                return _navHomeCommand ?? (_navHomeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, privilages, metaData)), canExecute));
            }
        }


        public ICommand GradingCommand
        {
            get
            {
                return _gradingCommand ?? (_gradingCommand = new LogOutCommandHandler(() => Switcher.Switch(new GradingScheduleView(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand MixingCommand
        {
            get
            {
                return _mixingCommand ?? (_mixingCommand = new LogOutCommandHandler(() => Switcher.Switch(new MixingScheduleView(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand PeelingCommand
        {
            get
            {
                return _peelingCommand ?? (_peelingCommand = new LogOutCommandHandler(() => Switcher.Switch(new PeelingOrdersView(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand SlittingCommand
        {
            get
            {
                return _slittingCommand ?? (_slittingCommand = new LogOutCommandHandler(() => Switcher.Switch(new SlittingMenuView(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand CuringCommand
        {
            get
            {
                return _curingCommand ?? (_curingCommand = new LogOutCommandHandler(() => Switcher.Switch(new CuringView(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand ReRollingCommand
        {
            get
            {
                return _reRollingCommand ?? (_reRollingCommand = new LogOutCommandHandler(() => Switcher.Switch(new ReRollingOrdersView(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand WeeklyScheduleCommand
        {
            get
            {
                return _weeklyScheduleCommand ?? (_weeklyScheduleCommand = new LogOutCommandHandler(() => Switcher.Switch(new WeeklyScheduleFullView(userName, state, privilages, metaData)), canExecute));
            }
        }

        
        
    }
}

