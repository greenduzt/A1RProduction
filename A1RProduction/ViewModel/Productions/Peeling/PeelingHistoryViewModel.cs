using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Production.Peeling;
using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Input;
using System.Linq;

namespace A1QSystem.ViewModel.Productions.Peeling
{
    public class PeelingHistoryViewModel : ViewModelBase
    {
        private string userName;
        private string state;
        private List<UserPrivilages> privilages;
        private DateTime _selectedDate;
        private string _headerResult;
        private ListCollectionView _collView = null;
        private DateTime _currentDate;
        private string _version;
        private List<MetaData> metaData;

        public List<PeelingHistory> PeelingHistory { get; set; }
       
        private ICommand _searchCommand;
        private bool canExecute;

        public PeelingHistoryViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = Privilages;
            metaData = md;
            CurrentDate = DateTime.Now;
            SelectedDate = CurrentDate;
            LoadPeelingHistory();           
            canExecute = true;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
        }

        private void LoadPeelingHistory()
        {
            PeelingHistory = DBAccess.GetPeelingHistory(SelectedDate);
            if (PeelingHistory.Count != 0)
            {
                HeaderResult = "Peeling history for the date " + SelectedDate.ToString("dd/MM/yyyy");
                CollView = new ListCollectionView(PeelingHistory);
                CollView.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                CollView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift.ShiftID", System.ComponentModel.ListSortDirection.Descending));
                CollView.SortDescriptions.Add(new System.ComponentModel.SortDescription("CreatedTime", System.ComponentModel.ListSortDirection.Descending));
            }
            else
            {
                HeaderResult = "Peeling history not found for the date " + SelectedDate.ToString("dd/MM/yyyy");
                PeelingHistory.Clear();
                CollView = new ListCollectionView(PeelingHistory);
            }
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

        //public ICommand PrintCommand
        //{
        //    get
        //    {
        //        return _printCommand ?? (_printCommand = new LogOutCommandHandler(() => PrintReRollingHistory(), canExecute));
        //    }
        //}


        #endregion
    }
}
