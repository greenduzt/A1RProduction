using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Production.Slitting;
using A1QSystem.View;
using A1QSystem.View.Production.Slitting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Productions.Slitting
{
    public class SlittingHistoryViewModel : ViewModelBase
    {
        private string userName;
        private string state;
        private List<UserPrivilages> privilages;
        private DateTime _selectedDate;
        private string _headerResult;
        private ListCollectionView _collView = null;
        private DateTime _currentDate;
        private string _selectedMachine;
        public List<SlittingHistory> SlittingHistory { get; set; }
        private string _version;
        private List<MetaData> metaData;
        private ICommand _searchCommand;
        private ICommand _workStationsCommand;
        private ICommand _navHomeCommand;
        private bool canExecute;

        public SlittingHistoryViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = Privilages;
            metaData = md;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
            CurrentDate = DateTime.Now;
            SelectedDate = CurrentDate;                      
            canExecute = true;
            SelectedMachine = "All";
            LoadPeelingHistory(); 
        }

        private void LoadPeelingHistory()
        {
            SlittingHistory = DBAccess.GetSlittingHistory(SelectedDate, SelectedMachine);
            if (SlittingHistory.Count != 0)
            {
                HeaderResult = "Slitting history for the date " + SelectedDate.ToString("dd/MM/yyyy");
                CollView = new ListCollectionView(SlittingHistory);
                CollView.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                CollView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift.ShiftID", System.ComponentModel.ListSortDirection.Descending));
                CollView.SortDescriptions.Add(new System.ComponentModel.SortDescription("CreatedTime", System.ComponentModel.ListSortDirection.Descending));
            }
            else
            {
                HeaderResult = "Slitting history not found for the date " + SelectedDate.ToString("dd/MM/yyyy");
                SlittingHistory.Clear();
                CollView = new ListCollectionView(SlittingHistory);
            }
        }

        private void GoHome()
        {
            Switcher.Switch(new MainMenu(userName, state, privilages, metaData));
        }

        private void GoToProduction()
        {
            Switcher.Switch(new SlittingMenuView(userName, state, privilages, metaData));
        }

        #region PUBLIC_PROPERTIES

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


        public string SelectedMachine
        {
            get { return _selectedMachine; }
            set
            {
                _selectedMachine = value;
                RaisePropertyChanged(() => this.SelectedMachine);
            }
        }

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                RaisePropertyChanged(() => this.SelectedDate);
            }
        }

        public ListCollectionView CollView
        {
            get { return _collView; }
            set
            {
                _collView = value;
                RaisePropertyChanged(() => this.CollView);

            }
        }

        public string HeaderResult
        {
            get { return _headerResult; }
            set
            {
                _headerResult = value;
                RaisePropertyChanged(() => this.HeaderResult);
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

        #endregion

        #region COMMANDS
        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new A1QSystem.Commands.LogOutCommandHandler(() => LoadPeelingHistory(), canExecute));
            }
        }

        public ICommand WorkStationsCommand
        {
            get
            {
                return _workStationsCommand ?? (_workStationsCommand = new LogOutCommandHandler(() => GoToProduction(), canExecute));
            }
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return _navHomeCommand ?? (_navHomeCommand = new LogOutCommandHandler(() => GoHome(), canExecute));
            }
        }
        #endregion
    }
}
