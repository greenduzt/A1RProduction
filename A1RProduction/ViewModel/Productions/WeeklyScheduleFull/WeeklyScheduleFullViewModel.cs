using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Grading;
using A1QSystem.Model.Production.Mixing;
using A1QSystem.Model.Products;
using A1QSystem.View.Dashboard;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Productions.WeeklyScheduleFull
{
    public class WeeklyScheduleFullViewModel : ViewModelBase
    {
        private string _day1;
        private string _day2;
        private string _day3;
        private string _day4;
        private string _day5;
        private string _dayName1;
        private string _dayName2;
        private string _dayName3;
        private string _dayName4;
        private string _dayName5;
        private string _dayName1Visibility;
        private string _dayName2Visibility;
        private string _dayName3Visibility;
        private string _dayName4Visibility;
        private string _dayName5Visibility;
        private string _dayName1And2;
        private string _dayName2And3;
        private string _dayName3And4;
        private string _dayName4And5;
        private string _dayName1And2Visibility;
        private string _dayName2And3Visibility;
        private string _dayName3And4Visibility;
        private string _dayName4And5Visibility;
        private string _weeklySchedule1Back;
        private string _weeklySchedule2Back;
        private string _weeklySchedule3Back;
        private string _weeklySchedule4Back;
        private string _weeklySchedule5Back;
        private string _dailyDate;
        private string _dayName1Background;
        private string _dayName2Background;
        private string _dayName3Background;
        private string _dayName4Background;
        private string _dayName5Background;
        private string _dayName1And2Background;
        private string _dayName2And3Background;
        private string _dayName3And4Background;
        private string _dayName4And5Background;

        private ListCollectionView _collDay1 = null;
        private ListCollectionView _collDay2 = null;
        private ListCollectionView _collDay3 = null;
        private ListCollectionView _collDay4 = null;
        private ListCollectionView _collDay5 = null;
        private ObservableCollection<WeeklySchedule> _weeklySchedule1 = null;
        private ObservableCollection<WeeklySchedule> _weeklySchedule2 = null;
        private ObservableCollection<WeeklySchedule> _weeklySchedule3 = null;
        private ObservableCollection<WeeklySchedule> _weeklySchedule4 = null;
        private ObservableCollection<WeeklySchedule> _weeklySchedule5 = null;
        private ObservableCollection<GradingWeeklySchedule> _gradingWeeklySchedule = null;
        private ObservableCollection<MixingWeeklySchedule> _mixingWeeklySchedule = null;
        private List<DateTime> dates;
        public MixingWeeklyScheduleNotifier mixingWeeklyScheduleNotifier { get; set; }
        public GradingWeeklyScheduleNotifier gradingWeeklyScheduleNotifier { get; set; }
        private System.Windows.Forms.Timer tmr = null;
        private string userName;
        private string state;
        private List<UserPrivilages> privilages;
        private string _version;
        private List<MetaData> metaData;
        private ICommand _backCommand;
        private ICommand _userControl_LoadedCommand;

        public WeeklyScheduleFullViewModel(string UserName, string State, List<UserPrivilages> uPriv, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = uPriv;
            metaData = md;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
            StartTimer();
        }

        private void UserControlLoaded()
        {
            LoadDays();
            WeeklySchedule1 = new ObservableCollection<WeeklySchedule>();
            WeeklySchedule2 = new ObservableCollection<WeeklySchedule>();
            WeeklySchedule3 = new ObservableCollection<WeeklySchedule>();
            WeeklySchedule4 = new ObservableCollection<WeeklySchedule>();
            WeeklySchedule5 = new ObservableCollection<WeeklySchedule>();
            

            GradingWeeklySchedule = new ObservableCollection<GradingWeeklySchedule>();
            MixingWeeklySchedule = new ObservableCollection<MixingWeeklySchedule>();

            this.gradingWeeklyScheduleNotifier = new GradingWeeklyScheduleNotifier();
            this.gradingWeeklyScheduleNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage1);
            LoadGradingData(gradingWeeklyScheduleNotifier.RegisterDependency());

            this.mixingWeeklyScheduleNotifier = new MixingWeeklyScheduleNotifier();
            this.mixingWeeklyScheduleNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage2);
            LoadMixingData(mixingWeeklyScheduleNotifier.RegisterDependency());
        }

        private void LoadMixingData(ObservableCollection<MixingWeeklySchedule> psd)
        {
            if (psd != null)
            {
                MixingWeeklySchedule = psd;

                ConstructWeeklySchedule();                
            }
        }   

        private void LoadDays()
        {           
            var startDate = DateTime.Now;
            dates = new List<DateTime>();

            for (var i = 0; i < 7; i++)
            {
                dates.Add(startDate.Date);
                startDate = startDate.AddDays(1);
            }           
        }

        private void StartTimer()
        {
            tmr = new System.Windows.Forms.Timer();
            tmr.Interval = 1000;
            tmr.Tick += new EventHandler(tmr_Tick);
            tmr.Enabled = true;
        }

        void tmr_Tick(object sender, EventArgs e)
        {
            DailyDate = DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm:ss tt");
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

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    //for (int i = 0; i < dates.Count; i++)
                    //{
                        //List<ProductionHistory> productionHistory = DBAccess.GetMixingCompletedByDate(dates[i].Date);
                        Day1 = string.Empty;
                        Day2 = string.Empty;
                        Day3 = string.Empty;
                        Day4 = string.Empty;
                        Day5 = string.Empty;

                        DayName1 = string.Empty;
                        DayName2 = string.Empty;
                        DayName3 = string.Empty;
                        DayName4 = string.Empty;
                        DayName5 = string.Empty;

                        DayName1And2 = string.Empty;
                        DayName2And3 = string.Empty;
                        DayName3And4 = string.Empty;
                        DayName4And5 = string.Empty;

                        DayName1Visibility = "Collapsed";
                        DayName2Visibility = "Collapsed";
                        DayName3Visibility = "Collapsed";
                        DayName4Visibility = "Collapsed";
                        DayName5Visibility = "Collapsed";

                        DayName1And2Visibility = "Collapsed";
                        DayName2And3Visibility = "Collapsed";
                        DayName3And4Visibility = "Collapsed";
                        DayName4And5Visibility = "Collapsed";

                        tempWeeklySchedule = tempWeeklySchedule.OrderBy(x=>x.MixingDate).ThenByDescending(x=>x.MixingShift).ToList();

                        foreach (var item in tempWeeklySchedule)
                        { 
                            item.BottomRowString = "TOT : " + item.TotBlockLogs + " | G : " + item.GradingBlockLogs + " | M : " + item.MixingBlockLogs + " | C : " + item.MixingCompleted;

                            if (WeeklySchedule1.Count == 0)
                            {
                                WeeklySchedule1.Add(item);
                                DayName1Visibility = "Visible";
                                DayName1 = item.MixingDate.DayOfWeek + " " + item.MixingDate.Date.ToString("dd/MM/yyyy");
                                Day1 = item.MixingShift; 
                            }
                            else
                            {
                                bool exists = false;
                                exists = WeeklySchedule1.Any(x => (x.MixingDate.Date == item.MixingDate.Date || item.MixingDate.Date <= DateTime.Now.Date) && x.MixingShift == item.MixingShift);
                                if (exists)
                                {
                                    WeeklySchedule1.Add(item);
                                    DayName1Visibility = "Visible";
                                    DayName1 = item.MixingDate.DayOfWeek + " " + item.MixingDate.Date.ToString("dd/MM/yyyy");
                                    Day1 = item.MixingShift; 
                                }
                                else if (WeeklySchedule2.Count == 0)
                                {
                                    WeeklySchedule2.Add(item);
                                    DayName2Visibility = "Visible";
                                    DayName2 = item.MixingDate.DayOfWeek + " " + item.MixingDate.Date.ToString("dd/MM/yyyy");
                                    Day2 = item.MixingShift; 
                                }
                                else
                                {
                                    exists = WeeklySchedule2.Any(x => x.MixingDate.Date == item.MixingDate.Date && x.MixingShift == item.MixingShift);
                                    if (exists)
                                    {
                                        WeeklySchedule2.Add(item);
                                        DayName2Visibility = "Visible";
                                        DayName2 = item.MixingDate.DayOfWeek + " " + item.MixingDate.Date.ToString("dd/MM/yyyy");
                                        Day2 = item.MixingShift; 
                                    }
                                    else if (WeeklySchedule3.Count == 0)
                                    {
                                        WeeklySchedule3.Add(item);
                                        DayName3Visibility = "Visible";
                                        DayName3 = item.MixingDate.DayOfWeek + " " + item.MixingDate.Date.ToString("dd/MM/yyyy");
                                        Day3 = item.MixingShift; 
                                    }
                                    else
                                    {
                                        exists = WeeklySchedule3.Any(x => x.MixingDate.Date == item.MixingDate.Date && x.MixingShift == item.MixingShift);
                                        if (exists)
                                        {
                                            WeeklySchedule3.Add(item);
                                            DayName3Visibility = "Visible";
                                            DayName3 = item.MixingDate.DayOfWeek + " " + item.MixingDate.Date.ToString("dd/MM/yyyy");
                                            Day3 = item.MixingShift; 
                                        }
                                        else if (WeeklySchedule4.Count == 0)
                                        {
                                            WeeklySchedule4.Add(item);
                                            DayName4Visibility = "Visible";
                                            DayName4 = item.MixingDate.DayOfWeek + " " + item.MixingDate.Date.ToString("dd/MM/yyyy");
                                            Day4 = item.MixingShift; 
                                        }
                                        else
                                        {
                                            exists = WeeklySchedule4.Any(x => x.MixingDate.Date == item.MixingDate.Date && x.MixingShift == item.MixingShift);
                                            if (exists)
                                            {
                                                WeeklySchedule4.Add(item);
                                                DayName4Visibility = "Visible";
                                                DayName4 = item.MixingDate.DayOfWeek + " " + item.MixingDate.Date.ToString("dd/MM/yyyy");
                                                Day4 = item.MixingShift; 
                                            }
                                            else if (WeeklySchedule5.Count == 0)
                                            {
                                                WeeklySchedule5.Add(item);
                                                DayName5Visibility = "Visible";
                                                DayName5 = item.MixingDate.DayOfWeek + " " + item.MixingDate.Date.ToString("dd/MM/yyyy");
                                                Day5 = item.MixingShift; 
                                            }
                                            else
                                            {
                                                exists = WeeklySchedule5.Any(x => x.MixingDate.Date == item.MixingDate.Date && x.MixingShift == item.MixingShift);
                                                if (exists)
                                                {
                                                    WeeklySchedule5.Add(item);
                                                    DayName5Visibility = "Visible";
                                                    DayName5 = item.MixingDate.DayOfWeek + " " + item.MixingDate.Date.ToString("dd/MM/yyyy");
                                                    Day5 = item.MixingShift; 
                                                }
                                            }
                                        }
                                    }
                                }                            
                            }
                            Console.WriteLine(item.TotBlockLogs + " " + item.MixingDate + " " + item.MixingShift);
                        }
                    //}

                         if(!string.IsNullOrWhiteSpace(DayName1))
                         {
                             WeeklySchedule1Back = "#F9BDBD";
                             DayName1Background = "#F9BDBD";

                             if(!string.IsNullOrWhiteSpace(DayName2))
                             {
                                 if(DayName1.Equals(DayName2))
                                 {
                                     DayName1And2Visibility = "Visible";
                                     DayName1Visibility = "Collapsed";
                                     DayName2Visibility = "Collapsed";
                                     DayName1And2 = DayName2;
                                     WeeklySchedule1Back = "#F9BDBD";
                                     WeeklySchedule2Back = "#F9BDBD";
                                     DayName1And2Background = "#F9BDBD";
                                 }
                                 else
                                 {
                                     DayName1And2Visibility = "Collapsed";
                                     DayName1Visibility = "Visible";
                                     DayName2Visibility = "Visible";
                                     DayName1And2 = string.Empty;
                                     WeeklySchedule1Back = "#F9BDBD";
                                     WeeklySchedule2Back = "#FBE9AA";
                                     DayName1Background = "#F9BDBD";
                                     DayName2Background = "#FBE9AA";
                                     DayName1And2Background = "";
                                 }

                                 if(!string.IsNullOrWhiteSpace(DayName3))
                                 {
                                     if (DayName2.Equals(DayName3))
                                     {
                                         DayName2And3Visibility = "Visible";
                                         DayName2Visibility = "Collapsed";
                                         DayName3Visibility = "Collapsed";
                                         DayName2And3 = DayName3;
                                         WeeklySchedule2Back = "#FBE9AA";
                                         WeeklySchedule3Back = "#FBE9AA";
                                         DayName2And3Background = "#FBE9AA";
                                     }
                                     else
                                     {
                                         DayName2And3Visibility = "Collapsed";
                                         DayName3Visibility = "Visible";
                                         DayName2And3 = string.Empty;
                                         WeeklySchedule3Back = "#B7F8A5";                                         
                                         DayName3Background = "#B7F8A5";
                                         DayName2And3Background = "";

                                         if (!DayName1.Equals(DayName2))
                                         {
                                             //Coll2
                                             DayName2Background = "#FBE9AA";
                                             WeeklySchedule2Back = "#FBE9AA";
                                         }
                                     }

                                     if (!string.IsNullOrWhiteSpace(DayName4))
                                     {
                                         if (DayName3.Equals(DayName4))
                                         {
                                             DayName3And4Visibility = "Visible";
                                             DayName3Visibility = "Collapsed";
                                             DayName4Visibility = "Collapsed";
                                             DayName3And4 = DayName4;
                                             WeeklySchedule3Back = "#B7F8A5";
                                             WeeklySchedule4Back = "#B7F8A5";
                                             DayName3And4Background = "#B7F8A5";
                                         }
                                         else
                                         {
                                             DayName3And4Visibility = "Collapsed";
                                             DayName4Visibility = "Visible";
                                             DayName3And4 = string.Empty;
                                             WeeklySchedule4Back = "#B7F8A5";
                                             DayName3And4Background = "";

                                             if (!DayName2.Equals(DayName3))
                                             {
                                                 //Coll3
                                                 DayName3Background = "#FACFAF";
                                                 WeeklySchedule3Back = "#FACFAF";
                                             }
                                             
                                             DayName4Background = "#B7F8A5";
                                         }

                                         if (!string.IsNullOrWhiteSpace(DayName5))
                                         {
                                             if (DayName4.Equals(DayName5))
                                             {
                                                 DayName4And5Visibility = "Visible";
                                                 DayName4Visibility = "Collapsed";
                                                 DayName5Visibility = "Collapsed";
                                                 DayName4And5 = DayName5;
                                                 WeeklySchedule4Back = "#C9D4FC";
                                                 WeeklySchedule5Back = "#C9D4FC";
                                                 DayName4And5Background = "#C9D4FC";
                                             }
                                             else
                                             {
                                                 DayName4And5Visibility = "Collapsed";
                                                 DayName5Visibility = "Visible";
                                                 DayName4And5 = string.Empty;
                                                 WeeklySchedule5Back = "#C9D4FC";
                                                 DayName4And5Background = "";                                                 
                                                 DayName5Background = "#C9D4FC";

                                                 if (!DayName3.Equals(DayName4))
                                                 {
                                                     //Coll4
                                                     DayName4Background = "#B7F8A5";
                                                     WeeklySchedule4Back = "#B7F8A5";
                                                 }
                                             }
                                         }
                                         else
                                         {
                                             WeeklySchedule5Back = "";
                                             DayName5Background = "";
                                         }

                                     }
                                     else
                                     {
                                         WeeklySchedule4Back = "";
                                         DayName4Background = "";
                                     }
                                 }
                                 else
                                 {
                                     WeeklySchedule3Back = "";
                                     DayName3Background = "";
                                 }
                             }
                             else
                             {
                                 WeeklySchedule2Back = "";
                                 DayName2Background = "";
                             }
                         }
                         else
                         {
                             WeeklySchedule1Back = "";
                             DayName1Background = "";
                         }                                      

                });
            }
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

        private List<WeeklySchedule> PreviousDateGradingOrders(List<WeeklySchedule> tempWeeklySchedule, GradingWeeklySchedule item)
        {
            if (tempWeeklySchedule.Count == 0)
            {
                tempWeeklySchedule = CheckWeeklyScheduleByGradingOrder(tempWeeklySchedule, item);
            }
            else
            {
                var data = tempWeeklySchedule.FirstOrDefault(x => x.RawProduct.RawProductID == item.RawProduct.RawProductID && x.MixingDate.Date <= item.MixingDate.Date);
                if (data != null)
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

        public string DayName1
        {
            get
            {
                return _dayName1;
            }
            set
            {
                _dayName1 = value;
                RaisePropertyChanged(() => this.DayName1);
            }
        }

        public string DayName2
        {
            get
            {
                return _dayName2;
            }
            set
            {
                _dayName2 = value;
                RaisePropertyChanged(() => this.DayName2);
            }
        }

        public string DayName3
        {
            get
            {
                return _dayName3;
            }
            set
            {
                _dayName3 = value;
                RaisePropertyChanged(() => this.DayName3);
            }
        }

        public string DayName4
        {
            get
            {
                return _dayName4;
            }
            set
            {
                _dayName4 = value;
                RaisePropertyChanged(() => this.DayName4);
            }
        }

        public string DayName5
        {
            get
            {
                return _dayName5;
            }
            set
            {
                _dayName5 = value;
                RaisePropertyChanged(() => this.DayName5);
            }
        }

        public string DayName1Visibility
        {
            get
            {
                return _dayName1Visibility;
            }
            set
            {
                _dayName1Visibility = value;
                RaisePropertyChanged(() => this.DayName1Visibility);
            }
        }

        public string DayName2Visibility
        {
            get
            {
                return _dayName2Visibility;
            }
            set
            {
                _dayName2Visibility = value;
                RaisePropertyChanged(() => this.DayName2Visibility);
            }
        }

        public string DayName3Visibility
        {
            get
            {
                return _dayName3Visibility;
            }
            set
            {
                _dayName3Visibility = value;
                RaisePropertyChanged(() => this.DayName3Visibility);
            }
        }

        public string DayName4Visibility
        {
            get
            {
                return _dayName4Visibility;
            }
            set
            {
                _dayName4Visibility = value;
                RaisePropertyChanged(() => this.DayName4Visibility);
            }
        }

        public string DayName5Visibility
        {
            get
            {
                return _dayName5Visibility;
            }
            set
            {
                _dayName5Visibility = value;
                RaisePropertyChanged(() => this.DayName5Visibility);
            }
        }

        public string DayName1And2
        {
            get
            {
                return _dayName1And2;
            }
            set
            {
                _dayName1And2 = value;
                RaisePropertyChanged(() => this.DayName1And2);
            }
        }

        public string DayName2And3
        {
            get
            {
                return _dayName2And3;
            }
            set
            {
                _dayName2And3 = value;
                RaisePropertyChanged(() => this.DayName2And3);
            }
        }
        
        public string DayName3And4
        {
            get
            {
                return _dayName3And4;
            }
            set
            {
                _dayName3And4 = value;
                RaisePropertyChanged(() => this.DayName3And4);
            }
        }

        public string DayName4And5
        {
            get
            {
                return _dayName4And5;
            }
            set
            {
                _dayName4And5 = value;
                RaisePropertyChanged(() => this.DayName4And5);
            }
        }

        public string DayName1And2Visibility
        {
            get
            {
                return _dayName1And2Visibility;
            }
            set
            {
                _dayName1And2Visibility = value;
                RaisePropertyChanged(() => this.DayName1And2Visibility);
            }
        }

        public string DayName2And3Visibility
        {
            get
            {
                return _dayName2And3Visibility;
            }
            set
            {
                _dayName2And3Visibility = value;
                RaisePropertyChanged(() => this.DayName2And3Visibility);
            }
        }

        public string DayName3And4Visibility
        {
            get
            {
                return _dayName3And4Visibility;
            }
            set
            {
                _dayName3And4Visibility = value;
                RaisePropertyChanged(() => this.DayName3And4Visibility);
            }
        }

        public string DayName4And5Visibility
        {
            get
            {
                return _dayName4And5Visibility;
            }
            set
            {
                _dayName4And5Visibility = value;
                RaisePropertyChanged(() => this.DayName4And5Visibility);
            }
        }

        public string WeeklySchedule1Back
        {
            get
            {
                return _weeklySchedule1Back;
            }
            set
            {
                _weeklySchedule1Back = value;
                RaisePropertyChanged(() => this.WeeklySchedule1Back);
            }
        }

        public string WeeklySchedule2Back
        {
            get
            {
                return _weeklySchedule2Back;
            }
            set
            {
                _weeklySchedule2Back = value;
                RaisePropertyChanged(() => this.WeeklySchedule2Back);
            }
        }

        public string WeeklySchedule3Back
        {
            get
            {
                return _weeklySchedule3Back;
            }
            set
            {
                _weeklySchedule3Back = value;
                RaisePropertyChanged(() => this.WeeklySchedule3Back);
            }
        }

        public string WeeklySchedule4Back
        {
            get
            {
                return _weeklySchedule4Back;
            }
            set
            {
                _weeklySchedule4Back = value;
                RaisePropertyChanged(() => this.WeeklySchedule4Back);
            }
        }

        public string WeeklySchedule5Back
        {
            get
            {
                return _weeklySchedule5Back;
            }
            set
            {
                _weeklySchedule5Back = value;
                RaisePropertyChanged(() => this.WeeklySchedule5Back);
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

        public string DailyDate
        {
            get { return _dailyDate; }
            set
            {
                _dailyDate = value;
                RaisePropertyChanged(() => this.DailyDate);
            }
        }

        public string DayName1Background
        {
            get { return _dayName1Background; }
            set
            {
                _dayName1Background = value;
                RaisePropertyChanged(() => this.DayName1Background);
            }
        }

        public string DayName2Background
        {
            get { return _dayName2Background; }
            set
            {
                _dayName2Background = value;
                RaisePropertyChanged(() => this.DayName2Background);
            }
        }

        public string DayName3Background
        {
            get { return _dayName3Background; }
            set
            {
                _dayName3Background = value;
                RaisePropertyChanged(() => this.DayName3Background);
            }
        }

        public string DayName4Background
        {
            get { return _dayName4Background; }
            set
            {
                _dayName4Background = value;
                RaisePropertyChanged(() => this.DayName4Background);
            }
        }

        public string DayName5Background
        {
            get { return _dayName5Background; }
            set
            {
                _dayName5Background = value;
                RaisePropertyChanged(() => this.DayName5Background);
            }
        }

        public string DayName1And2Background
        {
            get { return _dayName1And2Background; }
            set
            {
                _dayName1And2Background = value;
                RaisePropertyChanged(() => this.DayName1And2Background);
            }
        }

        public string DayName2And3Background
        {
            get { return _dayName2And3Background; }
            set
            {
                _dayName2And3Background = value;
                RaisePropertyChanged(() => this.DayName2And3Background);
            }
        }

        public string DayName3And4Background
        {
            get { return _dayName3And4Background; }
            set
            {
                _dayName3And4Background = value;
                RaisePropertyChanged(() => this.DayName3And4Background);
            }
        }

        public string DayName4And5Background
        {
            get { return _dayName4And5Background; }
            set
            {
                _dayName4And5Background = value;
                RaisePropertyChanged(() => this.DayName4And5Background);
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new LogOutCommandHandler(() => Switcher.Switch(new WorkStationsView(userName, state, privilages, metaData)), true));
            }
        }

        public ICommand UserControl_LoadedCommand
        {
            get
            {
                return _userControl_LoadedCommand ?? (_userControl_LoadedCommand = new A1QSystem.Commands.LogOutCommandHandler(UserControlLoaded, true));
            }
        }
        
    }
}
