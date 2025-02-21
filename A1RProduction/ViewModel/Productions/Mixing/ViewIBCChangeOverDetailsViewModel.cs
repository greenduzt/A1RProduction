using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Production.Mixing;
using A1QSystem.View;
using A1QSystem.View.AdminDashboard;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Productions.Mixing
{
    public class ViewIBCChangeOverDetailsViewModel : ViewModelBase
    {
        private ObservableCollection<IBCChangeOver> _iBCChangeOver;
        private DateTime _currentDate;
        private DateTime _selectedDate;
        private string userName;
        private string state;
        private List<UserPrivilages> userPrivilages;
        private bool canExecute;
        private List<MetaData> metaData;
        private string _version;
        private bool _isDateSearch;
        private bool _isViewAllDates;
        private ICommand _homeCommand;
        private ICommand _adminDashboardCommand;

        public ViewIBCChangeOverDetailsViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            userPrivilages = up;
            canExecute = true;
            metaData = md;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
            CurrentDate = DateTime.Now;
            //IsViewAllDates = true;
            LoadIBCChangeOverDetails();
        }


        private void LoadIBCChangeOverDetails()
        {
            IBCChangeOver = new ObservableCollection<IBCChangeOver>();
            IBCChangeOver = DBAccess.GetIBCChangeOverDetails();
        }


        public ObservableCollection<IBCChangeOver> IBCChangeOver
        {
            get
            {
                return _iBCChangeOver;
            }
            set
            {
                _iBCChangeOver = value;
                RaisePropertyChanged(() => this.IBCChangeOver);
            }
        }

        public bool IsDateSearch
        {
            get
            {
                return _isDateSearch;
            }
            set
            {
                _isDateSearch = value;
                RaisePropertyChanged(() => this.IsDateSearch);
            }
        }

        public bool IsViewAllDates
        {
            get
            {
                return _isViewAllDates;
            }
            set
            {
                _isViewAllDates = value;
                RaisePropertyChanged(() => this.IsViewAllDates);
            }
        }

        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                RaisePropertyChanged(() => this.SelectedDate);
            }
        }

        public DateTime CurrentDate
        {
            get
            {
                return _currentDate;
            }
            set
            {
                _currentDate = value;
                RaisePropertyChanged(() => this.CurrentDate);
            }
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
    }
}
