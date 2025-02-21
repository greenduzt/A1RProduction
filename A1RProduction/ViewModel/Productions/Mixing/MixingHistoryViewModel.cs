using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Grading;
using A1QSystem.Model.Products;
using A1QSystem.Model.Shifts;
using A1QSystem.Model.Shreding;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using MsgBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Productions.Mixing
{
    public class MixingHistoryViewModel : ViewModelBase
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
        private bool canExecute;
        private bool _cancelBtnEnabled;
        private string _version;
        private List<MetaData> metaData;
        private ICommand _searchCommand;        
        private ICommand _printCommand;
        private ICommand _cancelCommand;
        private ChildWindowView LoadingScreen;
        public List<ProductionHistory> ProductionHistory { get; set; }
        public List<ProductionHistory> PrintingHistory { get; set; }

        public MixingHistoryViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = Privilages;
            metaData = md;
            CurrentDate = DateTime.Now;
            SelectedDate = CurrentDate;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
            LoadMixingHistory();
            canExecute = true;
            LoadShiftList();
        }

        private void LoadMixingHistory()
        {
            ProductionHistory = DBAccess.GetMixingGradingHistory(SelectedDate);
            if (ProductionHistory.Count != 0)
            {
                HeaderResult = "Mixing history for the date " + SelectedDate.ToString("dd/MM/yyyy");
                CollView = new ListCollectionView(ProductionHistory);
                CollView.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
            }
            else
            {
                HeaderResult = "Mixing history not found for the date " + SelectedDate.ToString("dd/MM/yyyy");
                ProductionHistory.Clear();
                CollView = new ListCollectionView(ProductionHistory);
                //Msg.Show("No information found for the date " + SelectedDate.ToString("dd/MM/yyyy"), "Grading History Not Found", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }

            foreach (var item in ProductionHistory)
            {
                
                if (item.CreatedDateTime.Date == CurrentDate.Date && item.Status == "Completed")
                {
                    item.CancelBtnEnabled = true;
                    item.CancelBackCol = "#2E8856";
                }                
                else
                {
                    item.CancelBtnEnabled = false;
                    item.CancelBackCol = "#81b799";
                }
            }
        }

        private void LoadShiftList()
        {

            ShiftList = DBAccess.GetAllShifts();
            ShiftList.Add(new Shift() { ShiftID = 0, ShiftName = "Select" });
            SelectedShift = 0;

            //List<Shift> ShiftDetails = new List<Shift>();

            //if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            //{
            //    ShiftDetails.Add(new Shift() { ShiftID = 1, ShiftName = "Day", StartTime = new TimeSpan(5, 30, 0), EndTime = new TimeSpan(11, 29, 58) });
            //    ShiftDetails.Add(new Shift() { ShiftID = 2, ShiftName = "AfterNoon", StartTime = new TimeSpan(11, 30, 0), EndTime = new TimeSpan(17, 14, 58) });
            //    ShiftDetails.Add(new Shift() { ShiftID = 3, ShiftName = "Night", StartTime = new TimeSpan(17, 15, 0), EndTime = new TimeSpan(4, 29, 58) });
            //}
            //else
            //{
            //    ShiftDetails = DBAccess.GetAllShifts();
            //}

            //foreach (var item in ShiftDetails)
            //{
            //    bool isShift = TimeBetween(DateTime.Now, item.StartTime, item.EndTime);

            //    if (isShift == true)
            //    {
            //        _currentShift = item.ShiftID;
            //    }
            //}
            //List<ProductionTimeTable> ptt = DBAccess.GetProductionTimeTableByID(1, SelectedDate);

            //List<Shift> NewShiftList = new List<Shift>();

            //foreach (var item in ptt)
            //{

            //    if (item.IsMachineActive == true)
            //    {
            //        if (item.IsDayShiftActive == true && 1 <= _currentShift && item.ProductionDate.Date == CurrentDate.Date)
            //        {
            //            NewShiftList.Add(new Shift() { ShiftID = 1, ShiftName = "Day" });
            //        }
            //        else if (item.IsDayShiftActive == true && item.ProductionDate.Date != CurrentDate.Date)
            //        {
            //            NewShiftList.Add(new Shift() { ShiftID = 1, ShiftName = "Day" });
            //        }
            //        if (item.IsEveningShiftActive == true && 2 <= _currentShift && item.ProductionDate.Date == CurrentDate.Date)
            //        {
            //            NewShiftList.Add(new Shift() { ShiftID = 2, ShiftName = "Evening" });
            //        }
            //        else if (item.IsEveningShiftActive == true && item.ProductionDate.Date != CurrentDate.Date)
            //        {
            //            NewShiftList.Add(new Shift() { ShiftID = 2, ShiftName = "Evening" });
            //        }
            //        if (item.IsNightShiftActive == true && 3 <= _currentShift && item.ProductionDate.Date == CurrentDate.Date)
            //        {
            //            NewShiftList.Add(new Shift() { ShiftID = 3, ShiftName = "Night" });
            //        }
            //        else if (item.IsNightShiftActive == true && item.ProductionDate.Date != CurrentDate.Date)
            //        {
            //            NewShiftList.Add(new Shift() { ShiftID = 3, ShiftName = "Night" });
            //        }
            //    }
            //}

            //var result = NewShiftList.Distinct(new ShiftItemEqualityComparer());
            //ShiftList = new List<Shift>();
            //foreach (var item in result)
            //{
            //    ShiftList.Add(item);
            //}
                  

            //foreach (var item in ShiftList)
            //{
            //    bool has = ShiftList.Any(x=>x.ShiftID==item.ShiftID);
            //    if (has == false)
            //    {
            //        if (item.ShiftID == _currentShift)
            //        {
            //            SelectedShift = _currentShift;
            //        }
            //        else
            //        {
            //            SelectedShift = item.ShiftID;
            //        }
            //    }
            //}
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

        private string GetShiftByID(int s)
        {
            string shift = string.Empty;
            switch (s)
            {
                case 1:
                    shift = "Day Shift";
                    break;
                case 2:
                    shift = "Afternoon Shift";
                    break;
                case 3:
                    shift = "Night Shift";
                    break;
                default:
                    shift = "Not Available";
                    break;
            }

            return shift;
        }

        private void PrintGradingHistory()
        {
            LoadMixingHistory();

            if (SelectedShift != 0)
            {
                if (ShiftList.Count != 0)
                {
                    //PrintingHistory
                    PrintingHistory = new List<ProductionHistory>();
                    var q = from x in ProductionHistory
                            where x.Shift == SelectedShift && x.Status == "Completed"
                            group x by x.RawProduct.RawProductID into g
                            let count = g.Count()
                            orderby count descending
                            select new { Name = g.Key, Count = count, ID = g.First().ID };


                    foreach (var item in q)
                    {
                        var data = ProductionHistory.FirstOrDefault(c => c.RawProduct.RawProductID == item.Name);
                        PrintingHistory.Add(new ProductionHistory() { RawProduct = new RawProduct() { RawProductCode = data.RawProduct.RawProductCode, Description = data.RawProduct.Description, RawProductType = data.RawProduct.RawProductType }, Qty = item.Count });
                    }

                    if (PrintingHistory.Count != 0)
                    {
                        BackgroundWorker worker = new BackgroundWorker();
                        LoadingScreen = new ChildWindowView();
                        LoadingScreen.ShowWaitingScreen("Processing");

                        worker.DoWork += (_, __) =>
                        {
                            List<GradingCompleted> ggCompleted = DBAccess.GetMixingGradedHistory(SelectedDate, SelectedShift);
                            //Tuple<List<GradingCompleted>, List<GradedStockHistory>>

                            
                            PrintProductionPDF pppdf = new PrintProductionPDF("Mixing", PrintingHistory, GetShiftByID(SelectedShift), ggCompleted,new List<GradedStockHistory>(),"Blocks/Logs Mixed",_selectedDate,new List<ShreddingHistory>());
                            pppdf.CreateProductionPDF();

                        };

                        worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                        {
                            LoadingScreen.CloseWaitingScreen();

                        };
                        worker.RunWorkerAsync();
                    }
                    else
                    {
                        LoadMixingHistory();
                        Msg.Show("No information found for this shift. Please select a different shift", "Data Not Available", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                    }

                }
                else
                {
                    LoadMixingHistory();
                    Msg.Show("No information found for this date. Please select a different date", "Data Not Available", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                }
            }
            else
            {
                LoadMixingHistory();
                Msg.Show("Please select shift", "Select Shift", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
        }

        private void CancelOrder(Object parameter)
        {
            int index = ProductionHistory.IndexOf(parameter as ProductionHistory);
            if (index >= 0)
            {
                if (Msg.Show("Are you sure you want to cancel 1 " + ProductionHistory[index].RawProduct.RawProductType + " of " + ProductionHistory[index].RawProduct.Description + " and return back to Mixing?", "Returning To Mixing Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    int result = DBAccess.CancelMixing(ProductionHistory[index]);
                    if(result > 0)
                    {                       
                        Msg.Show(ProductionHistory[index].RawProduct.Description + " successfully returned to mixing!", "Order Returned To Mixing", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
                        LoadMixingHistory();
                        //ProductionHistory.Clear();
                    }
                    else
                    {
                        Msg.Show("Cannot cancel this order" + System.Environment.NewLine + "Please try again", "", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                        LoadMixingHistory();
                    }
                }
            }
        }

        private bool CanExecute(object parameter)
        {
            return true;
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

        public bool CancelBtnEnabled
        {
            get
            {
                return _cancelBtnEnabled;
            }
            set
            {
                _cancelBtnEnabled = value;
                RaisePropertyChanged(() => this.CancelBtnEnabled);
            }
        }

        
        #endregion

        #region COMMANDS
        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new LogOutCommandHandler(() => LoadMixingHistory(), canExecute));
            }
        }

        public ICommand PrintCommand
        {
            get
            {
                return _printCommand ?? (_printCommand = new LogOutCommandHandler(() => PrintGradingHistory(), canExecute));
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, CancelOrder);
                }
                return _cancelCommand;
            }
        }
        

        #endregion
    }
}




