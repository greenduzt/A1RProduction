using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.Dashboard;
using A1QSystem.View.Production.Slitting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Productions.Slitting
{
    public class SlittingMenuViewModel : ViewModelBase
    {
        private ICommand _navHomeCommand;
        private ICommand _workStationsCommand;
        private ICommand _flatBedCommand;
        private ICommand _carouselCommand;
        private ICommand _slittingHistoryCommand;
        private List<MetaData> metaData;
        private string userName;
        private string state;
        private List<UserPrivilages> privilages;
        private bool canExecute;
        private string _version;

        public SlittingMenuViewModel(string UserName, string State, List<UserPrivilages> uPriv, List<MetaData> md)
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

        public ICommand WorkStationsCommand
        {
            get
            {
                return _workStationsCommand ?? (_workStationsCommand = new LogOutCommandHandler(() => Switcher.Switch(new WorkStationsView(userName, state, privilages, metaData)), canExecute));
            }
        }



        public ICommand FlatBedCommand
        {
            get
            {
                return _flatBedCommand ?? (_flatBedCommand = new LogOutCommandHandler(() => Switcher.Switch(new FlatBenchSlitterView(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand CarouselCommand
        {
            get
            {
                return _carouselCommand ?? (_carouselCommand = new LogOutCommandHandler(() => Switcher.Switch(new CarouselSlitterView(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand SlittingHistoryCommand
        {
            get
            {
                return _slittingHistoryCommand ?? (_slittingHistoryCommand = new LogOutCommandHandler(() => Switcher.Switch(new SlittingHistoryView(userName, state, privilages, metaData)), canExecute));
            }
        }

        
    }
}
