
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Grading;
using A1QSystem.Model.Production.Mixing;
using A1QSystem.Model.Products;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using A1QSystem.PDFGeneration;
using MsgBox;
using System.ComponentModel;
using A1QSystem.View;

namespace A1QSystem.ViewModel.Productions.Mixing
{
    public class WeeklyScheduleViewModel : ViewModelBase
    {
        private string _day1;
        private string _day2;
        private string _day3;
        private string _day4;
        private string _day5;
        private string _day6;
        private string _day7;
        private ListCollectionView _collDay1 = null;
        private ListCollectionView _collDay2 = null;
        private ListCollectionView _collDay3 = null;
        private ListCollectionView _collDay4 = null;
        private ListCollectionView _collDay5 = null;
        private ListCollectionView _collDay6 = null;
        private ListCollectionView _collDay7 = null;
        private ObservableCollection<WeeklySchedule> _weeklySchedule1 = null;
        private ObservableCollection<WeeklySchedule> _weeklySchedule2 = null;
        private ObservableCollection<WeeklySchedule> _weeklySchedule3 = null;
        private ObservableCollection<WeeklySchedule> _weeklySchedule4 = null;
        private ObservableCollection<WeeklySchedule> _weeklySchedule5 = null;
        private ObservableCollection<WeeklySchedule> _weeklySchedule6 = null;
        private ObservableCollection<WeeklySchedule> _weeklySchedule7 = null;

        private ObservableCollection<GradingWeeklySchedule> _gradingWeeklySchedule = null;
        private ObservableCollection<MixingWeeklySchedule> _mixingWeeklySchedule = null;

        private List<DateTime> dates;
        public MixingWeeklyScheduleNotifier mixingWeeklyScheduleNotifier { get; set; }
        public GradingWeeklyScheduleNotifier gradingWeeklyScheduleNotifier { get; set; }
        public event Action Closed;
        private DelegateCommand _closeCommand;
        private ICommand _userControl_LoadedCommand;
        private ICommand _printCommand;

        public WeeklyScheduleViewModel()
        {
            _closeCommand = new DelegateCommand(CloseForm);
        }

        private void UserControlLoaded()
        {
            LoadDays();
            WeeklySchedule1 = new ObservableCollection<WeeklySchedule>();
            WeeklySchedule2 = new ObservableCollection<WeeklySchedule>();
            WeeklySchedule3 = new ObservableCollection<WeeklySchedule>();
            WeeklySchedule4 = new ObservableCollection<WeeklySchedule>();
            WeeklySchedule5 = new ObservableCollection<WeeklySchedule>();
            WeeklySchedule6 = new ObservableCollection<WeeklySchedule>();
            WeeklySchedule7 = new ObservableCollection<WeeklySchedule>();

            GradingWeeklySchedule = new ObservableCollection<GradingWeeklySchedule>();
            MixingWeeklySchedule = new ObservableCollection<MixingWeeklySchedule>();

            this.gradingWeeklyScheduleNotifier = new GradingWeeklyScheduleNotifier();
            this.gradingWeeklyScheduleNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage1);
            LoadGradingData(gradingWeeklyScheduleNotifier.RegisterDependency());

            this.mixingWeeklyScheduleNotifier = new MixingWeeklyScheduleNotifier();
            this.mixingWeeklyScheduleNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage2);
            LoadMixingData(mixingWeeklyScheduleNotifier.RegisterDependency());
        }

        private void LoadDays()
        {
            //var days = DayOfWeek.Monday - DateTime.UtcNow.DayOfWeek;
            //var startDate = DateTime.UtcNow.AddDays(days);
            var startDate = DateTime.Now;
            dates = new List<DateTime>();

            for (var i = 0; i < 7; i++)
            {
                dates.Add(startDate.Date);
                startDate = startDate.AddDays(1);
            }

            for (var i = 0; i < dates.Count; i++)
            {
                if(i == 0)
                {
                    Day1 = dates[i].DayOfWeek + " " + dates[i].Date.ToString("dd/MM/yyyy"); 
                }
                else if (i == 1)
                {
                    Day2 = dates[i].DayOfWeek + " " + dates[i].Date.ToString("dd/MM/yyyy"); 
                }
                else if (i == 2)
                {
                    Day3 = dates[i].DayOfWeek + " " + dates[i].Date.ToString("dd/MM/yyyy");  
                }
                else if (i == 3)
                {
                    Day4 = dates[i].DayOfWeek + " " + dates[i].Date.ToString("dd/MM/yyyy"); 
                }
                else if (i == 4)
                {
                    Day5 = dates[i].DayOfWeek + " " + dates[i].Date.ToString("dd/MM/yyyy"); 
                }
                else if (i == 5)
                {
                    Day6 = dates[i].DayOfWeek + " " + dates[i].Date.ToString("dd/MM/yyyy"); 
                }
                else if (i == 6)
                {
                    Day7 = dates[i].DayOfWeek + " " + dates[i].Date.ToString("dd/MM/yyyy"); 
                }
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        void notifier_NewMessage1(object sender, SqlNotificationEventArgs e)
        {

            this.LoadGradingData(this.gradingWeeklyScheduleNotifier.RegisterDependency());
        }

        void notifier_NewMessage2(object sender, SqlNotificationEventArgs e)
        {

            this.LoadMixingData(this.mixingWeeklyScheduleNotifier.RegisterDependency());
        }

        private void LoadGradingData(ObservableCollection<GradingWeeklySchedule> psd)
        {           
            if (psd != null)
            {
                GradingWeeklySchedule = psd;
                ConstructWeeklySchedule();
            }            
        }

        private void ConstructWeeklySchedule()
        {
            List<WeeklySchedule> tempWeeklySchedule = new List<WeeklySchedule>();

            List<ProductionHistory> ph1 = DBAccess.GetGradingMixingCompletedOrders(GradingWeeklySchedule);

            //Grading Orders
            foreach (var item in GradingWeeklySchedule)
            {
                if (ph1 != null && ph1.Count > 0)
                {
                    var data = ph1.SingleOrDefault(x => x.RawProduct.RawProductID == item.RawProduct.RawProductID && x.SalesOrder.ID == item.OrderNo);
                    if (data != null)
                    {
                        item.MixingCompleted = Convert.ToInt16(data.Qty);
                    }
                }

                //Grading orders less than today's date
                if (item.MixingDate.Date < DateTime.Now.Date)
                {
                    //Add to morning mixing shift, regardless of which mixing shift because they are yesterday's orders
                    tempWeeklySchedule = PreviousDateGradingOrders(tempWeeklySchedule, item);
                }
                //Orders in today's date or greater than today's date
                else if (item.MixingDate.Date >= DateTime.Now.Date)
                {
                    tempWeeklySchedule = ProcessTodayAndFutureGradingOrders(tempWeeklySchedule, item);
                }
            }


            if (MixingWeeklySchedule.Count > 0)
            {
                List<ProductionHistory> ph2 = DBAccess.GetMixingCompletedOrders(MixingWeeklySchedule);

                //Mixing Orders
                foreach (var item in MixingWeeklySchedule)
                {
                    bool has = MixingWeeklySchedule.Any(x=>x.RawProduct.RawProductID == item.RawProduct.RawProductID && x.OrderNo == item.OrderNo && x.TotMixedBlockLogs > 0);

                    if (ph2 != null && ph2.Count > 0 && has == false)
                    {
                        bool exists = GradingWeeklySchedule.Any(x => x.RawProduct.RawProductID == item.RawProduct.RawProductID && x.OrderNo == item.OrderNo && x.MixingCompleted > 0);
                        if (exists == false)
                        {
                            var data2 = ph2.SingleOrDefault(x => x.RawProduct.RawProductID == item.RawProduct.RawProductID && x.SalesOrder.ID == item.OrderNo);
                            if (data2 != null)
                            {
                                item.TotMixedBlockLogs = Convert.ToInt16(data2.Qty);
                            }
                        }
                    }

                    //Mixing orders less than today's date
                    if (item.MixingDate.Date < DateTime.Now.Date)
                    {
                        tempWeeklySchedule = PreviousDateMixingOrders(tempWeeklySchedule, item);
                    }
                    //Orders in today's date or greater than today's date
                    else if (item.MixingDate.Date >= DateTime.Now.Date)
                    {
                        tempWeeklySchedule = ProcessTodayAndFutureMixingOrders(tempWeeklySchedule, item);
                    }
                }
            }

            if (tempWeeklySchedule != null && dates != null)
            {
                WeeklySchedule1 = new ObservableCollection<WeeklySchedule>();
                WeeklySchedule2 = new ObservableCollection<WeeklySchedule>();
                WeeklySchedule3 = new ObservableCollection<WeeklySchedule>();
                WeeklySchedule4 = new ObservableCollection<WeeklySchedule>();
                WeeklySchedule5 = new ObservableCollection<WeeklySchedule>();
                WeeklySchedule6 = new ObservableCollection<WeeklySchedule>();
                WeeklySchedule7 = new ObservableCollection<WeeklySchedule>();

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    for (int i = 0; i < dates.Count; i++)
                    {
                        List<ProductionHistory> productionHistory = DBAccess.GetMixingCompletedByDate(dates[i].Date);
                        foreach (var item in tempWeeklySchedule)
                        {
                            if ((dates[i].Date == DateTime.Now.Date && item.MixingDate.Date <= dates[i].Date) || (dates[i].Date == item.MixingDate.Date))
                            {
                                item.BottomRowString = "TOT : " + item.TotBlockLogs + " | G : " + item.GradingBlockLogs + " | M : " + item.MixingBlockLogs + " | C : " + item.MixingCompleted;
                                switch (i)
                                {
                                    case 0:
                                        WeeklySchedule1.Add(item);//Monday
                                        break;
                                    case 1:
                                        WeeklySchedule2.Add(item);//Tuesday
                                        break;
                                    case 2:
                                        WeeklySchedule3.Add(item);//Wed
                                        break;
                                    case 3:
                                        WeeklySchedule4.Add(item);//Thus
                                        break;
                                    case 4:
                                        WeeklySchedule5.Add(item);//Fri
                                        break;
                                    case 5:
                                        WeeklySchedule6.Add(item);//Sat
                                        break;
                                    case 6:
                                        WeeklySchedule7.Add(item);//Sun
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    CollDay1 = new ListCollectionView(WeeklySchedule1);
                    CollDay1.GroupDescriptions.Add(new PropertyGroupDescription("MixingShift"));
                    CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("MixingShift", System.ComponentModel.ListSortDirection.Descending));

                    CollDay2 = new ListCollectionView(WeeklySchedule2);
                    CollDay2.GroupDescriptions.Add(new PropertyGroupDescription("MixingShift"));
                    CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("MixingShift", System.ComponentModel.ListSortDirection.Descending));

                    CollDay3 = new ListCollectionView(WeeklySchedule3);
                    CollDay3.GroupDescriptions.Add(new PropertyGroupDescription("MixingShift"));
                    CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("MixingShift", System.ComponentModel.ListSortDirection.Descending));

                    CollDay4 = new ListCollectionView(WeeklySchedule4);
                    CollDay4.GroupDescriptions.Add(new PropertyGroupDescription("MixingShift"));
                    CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("MixingShift", System.ComponentModel.ListSortDirection.Descending));

                    CollDay5 = new ListCollectionView(WeeklySchedule5);
                    CollDay5.GroupDescriptions.Add(new PropertyGroupDescription("MixingShift"));
                    CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("MixingShift", System.ComponentModel.ListSortDirection.Descending));

                    CollDay6 = new ListCollectionView(WeeklySchedule6);
                    CollDay6.GroupDescriptions.Add(new PropertyGroupDescription("MixingShift"));
                    CollDay6.SortDescriptions.Add(new System.ComponentModel.SortDescription("MixingShift", System.ComponentModel.ListSortDirection.Descending));

                    CollDay7 = new ListCollectionView(WeeklySchedule7);
                    CollDay7.GroupDescriptions.Add(new PropertyGroupDescription("MixingShift"));
                    CollDay7.SortDescriptions.Add(new System.ComponentModel.SortDescription("MixingShift", System.ComponentModel.ListSortDirection.Descending));

                });
            }
        }

        private List<WeeklySchedule> CheckWeeklyScheduleByGradingOrder(List<WeeklySchedule> tempWeeklySchedule, GradingWeeklySchedule item)
        {
            string mshift = item.MixingShift;
            DateTime mdate = item.MixingDate;
            if (item.MixingDate.Date < DateTime.Now.Date && item.MixingShift.Equals("Arvo"))
            {
                mshift = "Morning";
                mdate = DateTime.Now.Date;
            }

            tempWeeklySchedule.Add(new WeeklySchedule()
            {
                RawProduct = new RawProduct()
                {
                    RawProductID = item.RawProduct.RawProductID,
                    Description = item.RawProduct.Description
                },
                TotBlockLogs = item.GradingBlockLogs + item.MixingCompleted,
                GradingBlockLogs = item.GradingBlockLogs,
                MixingDate = mdate,
                MixingShift = mshift,
                Comments = item.Comments,
                IsCommentsVisible = item.IsCommentsVisible,
                MixingCompleted = item.MixingCompleted
            });            

            return tempWeeklySchedule;
        }

        private List<WeeklySchedule> CheckWeeklyScheduleByMixingOrder(List<WeeklySchedule> tempWeeklySchedule, MixingWeeklySchedule item)
        {
            string mshift = item.MixingShift;
            DateTime mdate = item.MixingDate;
            if (item.MixingDate.Date < DateTime.Now.Date)
            {
                mshift = "Morning";
                mdate = DateTime.Now.Date;
            }

            tempWeeklySchedule.Add(new WeeklySchedule()
            {
                RawProduct = new RawProduct()
                {
                    RawProductID = item.RawProduct.RawProductID,
                    Description = item.RawProduct.Description
                },
                TotBlockLogs =+ item.MixingBlockLogs + item.TotMixedBlockLogs,
                MixingBlockLogs = item.MixingBlockLogs,
                MixingDate = mdate,
                MixingShift = mshift,
                Comments = item.Comments,
                IsCommentsVisible = item.IsCommentsVisible,
                MixingCompleted =+ item.TotMixedBlockLogs
            });

            return tempWeeklySchedule;
        }

        private List<WeeklySchedule> PreviousDateGradingOrders(List<WeeklySchedule> tempWeeklySchedule,GradingWeeklySchedule item)
        {
            if (tempWeeklySchedule.Count == 0)
            {
                tempWeeklySchedule = CheckWeeklyScheduleByGradingOrder(tempWeeklySchedule, item);
            }
            else
            {
                var data = tempWeeklySchedule.FirstOrDefault(x=>x.RawProduct.RawProductID == item.RawProduct.RawProductID && x.MixingDate.Date <= item.MixingDate.Date);
                if(data != null)
                {
                    data.MixingShift = "Morning";
                    data.GradingBlockLogs += item.GradingBlockLogs;
                    data.TotBlockLogs = data.TotBlockLogs + item.GradingBlockLogs + item.MixingCompleted;
                    data.Comments = item.Comments;
                    data.IsCommentsVisible = item.IsCommentsVisible;
                    data.MixingCompleted += item.MixingCompleted;
                }
                else
                {
                    tempWeeklySchedule = CheckWeeklyScheduleByGradingOrder(tempWeeklySchedule, item);
                }
            }

            return tempWeeklySchedule;
        }

        //Mixing orders less than today's date
        private List<WeeklySchedule> PreviousDateMixingOrders(List<WeeklySchedule> tempWeeklySchedule, MixingWeeklySchedule item)
        {
            if (tempWeeklySchedule.Count == 0)
            {
                tempWeeklySchedule = CheckWeeklyScheduleByMixingOrder(tempWeeklySchedule, item);
            }
            else
            {
                //If the collection has todays morning order for previous mixing order
                var data = tempWeeklySchedule.FirstOrDefault(x => x.RawProduct.RawProductID == item.RawProduct.RawProductID && x.MixingDate.Date <= DateTime.Now.Date && item.MixingDate.Date <= DateTime.Now.Date && x.MixingShift.Equals("Morning"));
                if(data != null)
                {                   
                    data.MixingBlockLogs += item.MixingBlockLogs;
                    data.TotBlockLogs = data.TotBlockLogs + item.MixingBlockLogs + item.TotMixedBlockLogs;
                    data.MixingCompleted += item.TotMixedBlockLogs;
                }
                else
                {
                    //If no todays morning grading orders, make a new one
                    tempWeeklySchedule = CheckWeeklyScheduleByMixingOrder(tempWeeklySchedule, item);
                }
            }

            return tempWeeklySchedule;
        }

        private List<WeeklySchedule> ProcessTodayAndFutureGradingOrders(List<WeeklySchedule> tempWeeklySchedule,GradingWeeklySchedule item)
        {
            if (tempWeeklySchedule.Count == 0)
            {
                tempWeeklySchedule = CheckWeeklyScheduleByGradingOrder(tempWeeklySchedule, item);
            }
            else
            {
                var data = tempWeeklySchedule.FirstOrDefault(x => x.RawProduct.RawProductID == item.RawProduct.RawProductID &&
                                                             x.MixingShift.Equals(item.MixingShift) &&        
                                                             ((item.MixingDate.Date == DateTime.Now.Date && x.MixingDate.Date <= item.MixingDate.Date) ||
                                                              (item.MixingDate.Date > DateTime.Now.Date && x.MixingDate.Date == item.MixingDate.Date))); 
                                                                                                                                 
                if (data != null)
                {
                    data.MixingShift = item.MixingShift;
                    data.GradingBlockLogs += item.GradingBlockLogs;
                    data.TotBlockLogs = data.TotBlockLogs + item.GradingBlockLogs + item.MixingCompleted;
                    data.Comments = item.Comments;
                    data.IsCommentsVisible = item.IsCommentsVisible;
                    data.MixingCompleted += item.MixingCompleted;
                }
                else
                {
                    //Add to WeeklySchedule
                    tempWeeklySchedule = CheckWeeklyScheduleByGradingOrder(tempWeeklySchedule, item);
                }
            }

            return tempWeeklySchedule;
        }

        private List<WeeklySchedule> ProcessTodayAndFutureMixingOrders(List<WeeklySchedule> tempWeeklySchedule, MixingWeeklySchedule item)
        {
            if (tempWeeklySchedule.Count == 0)
            {
                tempWeeklySchedule = CheckWeeklyScheduleByMixingOrder(tempWeeklySchedule, item);
            }
            else
            {
                var data = tempWeeklySchedule.FirstOrDefault(x => x.RawProduct.RawProductID == item.RawProduct.RawProductID &&
                                                             x.MixingShift.Equals(item.MixingShift) &&
                                                             ((item.MixingDate.Date == DateTime.Now.Date && x.MixingDate.Date <= item.MixingDate.Date) ||
                                                              (item.MixingDate.Date > DateTime.Now.Date && x.MixingDate.Date == item.MixingDate.Date)));

                if (data != null)
                {
                    data.MixingShift = item.MixingShift;
                    data.MixingBlockLogs += item.MixingBlockLogs;
                    data.TotBlockLogs = data.TotBlockLogs + item.MixingBlockLogs + item.TotMixedBlockLogs;
                    data.Comments = item.Comments;
                    data.IsCommentsVisible = item.IsCommentsVisible;
                    data.MixingCompleted += item.TotMixedBlockLogs;
                }
                else
                {
                    //Add to WeeklySchedule
                    tempWeeklySchedule = CheckWeeklyScheduleByMixingOrder(tempWeeklySchedule, item);
                }
            }

            return tempWeeklySchedule;
        }

        

        private void LoadMixingData(ObservableCollection<MixingWeeklySchedule> psd)
        {
            if (psd != null)
            {
                MixingWeeklySchedule = psd;

                ConstructWeeklySchedule();                
            }
        }    
   
        private void PrintWeeklySchedule()
        {
            Exception exception = null;
            BackgroundWorker worker = new BackgroundWorker();
            ChildWindowView LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Printing");

            worker.DoWork += (_, __) =>
            {
                PrintWeeklySchedulePDF pw = new PrintWeeklySchedulePDF(new List<WeeklySchedule>(WeeklySchedule1), new List<WeeklySchedule>(WeeklySchedule2), new List<WeeklySchedule>(WeeklySchedule3), new List<WeeklySchedule>(WeeklySchedule4), new List<WeeklySchedule>(WeeklySchedule5), Day1, Day2, Day3, Day4, Day5);
                exception = pw.createWorkOrderPDF();
            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();
                if (exception != null)
                {
                    Msg.Show("Could not print the document " + System.Environment.NewLine + exception.ToString(), "Printing Failed", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }
            };
            worker.RunWorkerAsync();
        }

        #region Public_Properties

        public string Day1
        {
            get
            {
                return _day1;
            }
            set
            {
                _day1 = value;
                RaisePropertyChanged(() => this.Day1);
            }
        }

        public string Day2
        {
            get
            {
                return _day2;
            }
            set
            {
                _day2 = value;
                RaisePropertyChanged(() => this.Day2);
            }
        }

        public string Day3
        {
            get
            {
                return _day3;
            }
            set
            {
                _day3 = value;
                RaisePropertyChanged(() => this.Day3);
            }
        }

        public string Day4
        {
            get
            {
                return _day4;
            }
            set
            {
                _day4 = value;
                RaisePropertyChanged(() => this.Day4);
            }
        }

        public string Day5
        {
            get
            {
                return _day5;
            }
            set
            {
                _day5 = value;
                RaisePropertyChanged(() => this.Day5);
            }
        }

        public string Day6
        {
            get
            {
                return _day6;
            }
            set
            {
                _day6 = value;
                RaisePropertyChanged(() => this.Day6);
            }
        }

        public string Day7
        {
            get
            {
                return _day7;
            }
            set
            {
                _day7 = value;
                RaisePropertyChanged(() => this.Day7);
            }
        }

        public ListCollectionView CollDay1
        {
            get { return _collDay1; }
            set
            {
                _collDay1 = value;
                RaisePropertyChanged(() => this.CollDay1);

            }
        }

        public ListCollectionView CollDay2
        {
            get { return _collDay2; }
            set
            {
                _collDay2 = value;
                RaisePropertyChanged(() => this.CollDay2);
            }
        }

        public ListCollectionView CollDay3
        {
            get { return _collDay3; }
            set
            {
                _collDay3 = value;
                RaisePropertyChanged(() => this.CollDay3);
            }
        }

        public ListCollectionView CollDay4
        {
            get { return _collDay4; }
            set
            {
                _collDay4 = value;
                RaisePropertyChanged(() => this.CollDay4);
            }
        }
        public ListCollectionView CollDay5
        {
            get { return _collDay5; }
            set
            {
                _collDay5 = value;
                RaisePropertyChanged(() => this.CollDay5);
            }
        }

        public ListCollectionView CollDay6
        {
            get { return _collDay6; }
            set
            {
                _collDay6 = value;
                RaisePropertyChanged(() => this.CollDay6);
            }
        }

        public ListCollectionView CollDay7
        {
            get { return _collDay7; }
            set
            {
                _collDay7 = value;
                RaisePropertyChanged(() => this.CollDay7);
            }
        }

        public ObservableCollection<WeeklySchedule> WeeklySchedule1
        {
            get { return _weeklySchedule1; }
            set
            {
                _weeklySchedule1 = value;
                RaisePropertyChanged(() => this.WeeklySchedule1);
            }
        }

        public ObservableCollection<WeeklySchedule> WeeklySchedule2
        {
            get { return _weeklySchedule2; }
            set
            {
                _weeklySchedule2 = value;
                RaisePropertyChanged(() => this.WeeklySchedule2);
            }
        }

        public ObservableCollection<WeeklySchedule> WeeklySchedule3
        {
            get { return _weeklySchedule3; }
            set
            {
                _weeklySchedule3 = value;
                RaisePropertyChanged(() => this.WeeklySchedule3);
            }
        }

        public ObservableCollection<WeeklySchedule> WeeklySchedule4
        {
            get { return _weeklySchedule4; }
            set
            {
                _weeklySchedule4 = value;
                RaisePropertyChanged(() => this.WeeklySchedule4);
            }
        }

        public ObservableCollection<WeeklySchedule> WeeklySchedule5
        {
            get { return _weeklySchedule5; }
            set
            {
                _weeklySchedule5 = value;
                RaisePropertyChanged(() => this.WeeklySchedule5);
            }
        }

        public ObservableCollection<WeeklySchedule> WeeklySchedule6
        {
            get { return _weeklySchedule6; }
            set
            {
                _weeklySchedule6 = value;
                RaisePropertyChanged(() => this.WeeklySchedule6);
            }
        }

        public ObservableCollection<WeeklySchedule> WeeklySchedule7
        {
            get { return _weeklySchedule7; }
            set
            {
                _weeklySchedule7 = value;
                RaisePropertyChanged(() => this.WeeklySchedule7);
            }
        }

        public ObservableCollection<GradingWeeklySchedule> GradingWeeklySchedule
        {
            get { return _gradingWeeklySchedule; }
            set
            {
                _gradingWeeklySchedule = value;
                RaisePropertyChanged(() => this.GradingWeeklySchedule);
            }
        }

        public ObservableCollection<MixingWeeklySchedule> MixingWeeklySchedule
        {
            get { return _mixingWeeklySchedule; }
            set
            {
                _mixingWeeklySchedule = value;
                RaisePropertyChanged(() => this.MixingWeeklySchedule);
            }
        }
        

        #endregion

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }      

        
        public ICommand UserControl_LoadedCommand
        {
            get
            {
                return _userControl_LoadedCommand ?? (_userControl_LoadedCommand = new A1QSystem.Commands.LogOutCommandHandler(UserControlLoaded, true));
            }
        }

        public ICommand PrintCommand
        {
            get
            {
                return _printCommand ?? (_printCommand = new A1QSystem.Commands.LogOutCommandHandler(PrintWeeklySchedule, true));
            }
        }
        
    }
}
