using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.ReRoll;
using A1QSystem.Model.Shifts;
using A1QSystem.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Productions.ReRolling
{
    public class ReRollingHistoryViewModel : ViewModelBase
    {
        private string userName;
        private string state;
        private List<UserPrivilages> privilages;
        private DateTime _selectedDate;
        private string _headerResult;
        private int _selectedShift;
        private ListCollectionView _collView = null;
        private int _currentShift;
        private DateTime _currentDate;
        private List<Shift> _shiftList;
        private ChildWindowView LoadingScreen;
        public List<ReRollingHistory> ReRollingHistory { get; set; }
        //public List<ProductionHistory> PrintingHistory { get; set; }
        private List<MetaData> metaData;
        private string _version;

        private ICommand _searchCommand;
        private ICommand _printCommand;
        private bool canExecute;

        public ReRollingHistoryViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = Privilages;
            metaData = md;
            CurrentDate = DateTime.Now;
            SelectedDate = CurrentDate;       
            LoadreRollingHistory();
            LoadShiftList();
            canExecute = true;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
        }


        private void LoadreRollingHistory()
        {
            ReRollingHistory = DBAccess.GetReRollingHistory(SelectedDate);
            if (ReRollingHistory.Count != 0)
            {
                HeaderResult = "Re-Rolled history for the date " + SelectedDate.ToString("dd/MM/yyyy");
                CollView = new ListCollectionView(ReRollingHistory);
                CollView.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                CollView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift.ShiftID", System.ComponentModel.ListSortDirection.Descending));
                CollView.SortDescriptions.Add(new System.ComponentModel.SortDescription("CreatedTime", System.ComponentModel.ListSortDirection.Descending));
            }
            else
            {
                HeaderResult = "Re-Rolled history not found for the date " + SelectedDate.ToString("dd/MM/yyyy");
                ReRollingHistory.Clear();
                CollView = new ListCollectionView(ReRollingHistory);
                //Msg.Show("No information found for the date " + SelectedDate.ToString("dd/MM/yyyy"), "Grading History Not Found", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
        }

        private void LoadShiftList()
        {
            List<Shift> ShiftDetails = new List<Shift>();

            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                ShiftDetails.Add(new Shift() { ShiftID = 1, ShiftName = "Day", StartTime = new TimeSpan(5, 30, 0), EndTime = new TimeSpan(11, 29, 58) });
                ShiftDetails.Add(new Shift() { ShiftID = 2, ShiftName = "AfterNoon", StartTime = new TimeSpan(11, 30, 0), EndTime = new TimeSpan(17, 14, 58) });
                ShiftDetails.Add(new Shift() { ShiftID = 3, ShiftName = "Night", StartTime = new TimeSpan(17, 15, 0), EndTime = new TimeSpan(4, 29, 58) });
            }
            else
            {
                ShiftDetails = DBAccess.GetAllShifts();
            }

            foreach (var item in ShiftDetails)
            {
                bool isShift = TimeBetween(DateTime.Now, item.StartTime, item.EndTime);

                if (isShift == true)
                {
                    _currentShift = item.ShiftID;
                }
            }
            List<ProductionTimeTable> ptt = DBAccess.GetProductionTimeTableByID(1, SelectedDate);

            List<Shift> NewShiftList = new List<Shift>();

            foreach (var item in ptt)
            {
                if (item.IsMachineActive == true)
                {
                    if (item.IsDayShiftActive == true && 1 <= _currentShift && item.ProductionDate.Date == CurrentDate.Date)
                    {
                        NewShiftList.Add(new Shift() { ShiftID = 1, ShiftName = "Day" });
                    }
                    else if (item.IsDayShiftActive == true && item.ProductionDate.Date != CurrentDate.Date)
                    {
                        NewShiftList.Add(new Shift() { ShiftID = 1, ShiftName = "Day" });
                    }
                    if (item.IsEveningShiftActive == true && 2 <= _currentShift && item.ProductionDate.Date == CurrentDate.Date)
                    {
                        NewShiftList.Add(new Shift() { ShiftID = 2, ShiftName = "Evening" });
                    }
                    else if (item.IsEveningShiftActive == true && item.ProductionDate.Date != CurrentDate.Date)
                    {
                        NewShiftList.Add(new Shift() { ShiftID = 2, ShiftName = "Evening" });
                    }
                    if (item.IsNightShiftActive == true && 3 <= _currentShift && item.ProductionDate.Date == CurrentDate.Date)
                    {
                        NewShiftList.Add(new Shift() { ShiftID = 3, ShiftName = "Night" });
                    }
                    else if (item.IsNightShiftActive == true && item.ProductionDate.Date != CurrentDate.Date)
                    {
                        NewShiftList.Add(new Shift() { ShiftID = 3, ShiftName = "Night" });
                    }
                }
            }

            ShiftList = new List<Shift>();
            ShiftList = NewShiftList;
            foreach (var item in ShiftList)
            {
                if (item.ShiftID == _currentShift)
                {
                    SelectedShift = _currentShift;
                }
                else
                {
                    SelectedShift = item.ShiftID;
                }
            }

        }

        bool TimeBetween(DateTime datetime, TimeSpan start, TimeSpan end)
        {
            // convert datetime to a TimeSpan
            TimeSpan now = datetime.TimeOfDay;
            // see if start comes before end
            if (start < end)
                return start <= now && now <= end;
            // start is after end, so do the inverse comparison
            return !(end < now && now < start);
        }

        //private void PrintReRollingHistory()
        //{

        //}


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
                LoadShiftList();
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

        public int SelectedShift
        {
            get
            {
                return _selectedShift;
            }
            set
            {
                _selectedShift = value;
                RaisePropertyChanged(() => this.SelectedShift);
            }
        }

        public List<Shift> ShiftList
        {
            get
            {
                return _shiftList;
            }
            set
            {
                _shiftList = value;
                RaisePropertyChanged(() => this.ShiftList);
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
                return _searchCommand ?? (_searchCommand = new LogOutCommandHandler(() => LoadreRollingHistory(), canExecute));
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
