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

namespace A1QSystem.ViewModel.Productions.Grading
{
    public class GradingHistoryViewModel : ViewModelBase
    {
        private string userName;
        private string state;
        private List<UserPrivilages> privilages;
        private List<MetaData> metaData;
        private DateTime _selectedDate;
        private string _headerResult;
        private int _selectedShift;
        private ListCollectionView _collView = null;
        private int _currentShift;
        private DateTime _currentDate;
        private List<Shift> _shiftList;
        private ChildWindowView LoadingScreen;
        public List<ProductionHistory> ProductionHistory { get; set; }
        public List<ProductionHistory> PrintingHistory { get; set; }
        private string _version;
        private ICommand _searchCommand;
        private ICommand _printCommand;
        private bool canExecute;


        public GradingHistoryViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = Privilages;
            metaData = md;
            CurrentDate = DateTime.Now;
            SelectedDate = CurrentDate;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;

            LoadGradingHistory();
            canExecute = true;
            LoadShiftList();
        }

        private void LoadShiftList()
        {       

            ShiftList = DBAccess.GetAllShifts();
            ShiftList.Add(new  Shift(){ShiftID=0,ShiftName="Select"});
            SelectedShift = 0;
            
        }

        private void LoadGradingHistory()
        {
            ProductionHistory = DBAccess.GetGradingHistory(SelectedDate,"GradingCompleted");
            if (ProductionHistory.Count != 0)
            {
                HeaderResult = "Grading history for the date " + SelectedDate.ToString("dd/MM/yyyy");
                CollView = new ListCollectionView(ProductionHistory);
                CollView.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
            }
            else
            {
                HeaderResult = "Grading history not found for the date " + SelectedDate.ToString("dd/MM/yyyy");
                ProductionHistory.Clear();
                CollView = new ListCollectionView(ProductionHistory);
                //Msg.Show("No information found for the date " + SelectedDate.ToString("dd/MM/yyyy"), "Grading History Not Found", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
        }

        private void PrintGradingHistory()
        {            
            LoadGradingHistory();
            if (SelectedShift != 0)
            {
                if(ShiftList.Count != 0)
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
                        PrintingHistory.Add(new ProductionHistory() { RawProduct = new RawProduct() { RawProductID = data.RawProduct.RawProductID, RawProductCode = data.RawProduct.RawProductCode, Description = data.RawProduct.Description, RawProductType = data.RawProduct.RawProductType }, Qty = item.Count });

                    }

                    var r = from x in ProductionHistory
                            where x.Shift == SelectedShift && x.Status == "Returned"
                            select new { RawProductID = x.RawProduct.RawProductID };

                    Tuple<List<GradingCompleted>, List<GradedStockHistory>> tuple = DBAccess.GetGradingGradedHistory(SelectedDate, SelectedShift);
                    List<ShreddingHistory> ShreddingHistory = DBAccess.GetShredingHistory(_selectedDate, SelectedShift);

                    if (PrintingHistory.Count != 0 || tuple.Item2.Count != 0 || ShreddingHistory.Count != 0)
                    {
                        Exception exception = null;
                        BackgroundWorker worker = new BackgroundWorker();
                        LoadingScreen = new ChildWindowView();
                        LoadingScreen.ShowWaitingScreen("Processing");

                        worker.DoWork += (_, __) =>
                        {

                            PrintProductionPDF pppdf = new PrintProductionPDF("Grading", PrintingHistory, GetShiftByID(SelectedShift), tuple.Item1, tuple.Item2,  "Blocks/Logs Graded", _selectedDate, ShreddingHistory);
                            exception=pppdf.CreateProductionPDF();

                        };

                        worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                        {
                            LoadingScreen.CloseWaitingScreen();
                            if(exception != null)
                            {
                                Msg.Show("A problem has occured while the work order is prining. Please try again later." + System.Environment.NewLine + exception, "Printing Error", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);

                            }
                        };
                        worker.RunWorkerAsync();
                    }
                    else
                    {
                        LoadGradingHistory();
                        Msg.Show("No information found for this shift. Please select a different shift", "Data Not Available", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                    }
                   
                }
                else
                {
                    LoadGradingHistory();
                    Msg.Show("No information found for this date. Please select a different date", "Data Not Available", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                }
            }
            else
            {
                LoadGradingHistory();
                Msg.Show("Please select shift", "Select Shift", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }

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
                return _searchCommand ?? (_searchCommand = new LogOutCommandHandler(() => LoadGradingHistory(), canExecute));
            }
        }

        public ICommand PrintCommand
        {
            get
            {
                return _printCommand ?? (_printCommand = new LogOutCommandHandler(() => PrintGradingHistory(), canExecute));
            }
        }
        

        #endregion

    }
}

