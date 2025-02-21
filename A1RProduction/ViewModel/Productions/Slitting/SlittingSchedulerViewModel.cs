using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Other;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Slitting;
using A1QSystem.Model.Products;
using A1QSystem.Model.Products.Tiles;
using A1QSystem.Model.Shifts;
using A1QSystem.View;
using A1QSystem.View.AdminDashboard;
using A1QSystem.View.Production;
using A1QSystem.ViewModel.Orders;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace A1QSystem.ViewModel.Productions.Slitting
{
    public class SlittingSchedulerViewModel : ViewModelBase
    {
        private string _day1;
        private string _day2;
        private string _day3;
        private string _day4;
        private string _day5;
        private string _enDisText1;
        private string _enDisText2;
        private string _enDisText3;
        private string _enDisText4;
        private string _enDisText5;
        private string _enDisBack1;
        private string _enDisBack2;
        private string _enDisBack3;
        private string _enDisBack4;
        private string _enDisBack5;
        private ObservableCollection<ProductionTimeTable> timeTableDates;
        public SlittingProductionNotifier SlittingProductionNotifier { get; set; }
        public ProductionTimeTableNotifier productionTimeTableNotifier { get; set; }
        public Dispatcher UIDispatcher { get; set; }
        private DateTime CurrentDate;
        private string userName;
        private string state;
        private List<UserPrivilages> userPrivilages;
        private bool canExecute;
        private List<MetaData> metaData;
        private string _version;
        private int _sliiterTab1;
        private int _sliiterTab2;
        private int _sliiterTab3;
        private int _sliiterTab4;
        private int _sliiterTab5;

        private ListCollectionView _collDay1 = null;
        private ListCollectionView _collDay2 = null;
        private ListCollectionView _collDay3 = null;
        private ListCollectionView _collDay4 = null;
        private ListCollectionView _collDay5 = null;

        private ObservableCollection<SlittingOrder> _orderProduction1;
        private ObservableCollection<SlittingOrder> _orderProduction2;
        private ObservableCollection<SlittingOrder> _orderProduction3;
        private ObservableCollection<SlittingOrder> _orderProduction4;
        private ObservableCollection<SlittingOrder> _orderProduction5;
        private ObservableCollection<SlittingOrder> _orderProductionList = null;

        private int _tabSelectedIndex1;
        private int _tabSelectedIndex2;
        private int _tabSelectedIndex3;
        private int _tabSelectedIndex4;
        private int _tabSelectedIndex5;
        private ObservableCollection<SlittingOrder> _orderProduction;

        private string D1;
        private string D2;
        private string D3;
        private string D4;
        private string D5;
        public List<Shift> ShiftDetails { get; set; }
        private int _index;
        private ChildWindowView LoadingScreen;
        private bool _nextEnabled;

        private ICommand _navHomeCommand;
        private ICommand _adminDashboardCommand;
        private ICommand _productionMaintenanceCommand;
        private ICommand _prevCommand;
        private ICommand _nextCommand;
        private ICommand _enDis1;
        private ICommand _enDis2;
        private ICommand _enDis3;
        private ICommand _enDis4;
        private ICommand _enDis5;

        public SlittingSchedulerViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md, Dispatcher uidispatcher)
        {
            int curShift = 0;
            CurrentDate = NTPServer.GetNetworkTime();
            DateTime currDate = CurrentDate;
            userName = UserName;
            state = State;
            userPrivilages = up;
            canExecute = true;
            metaData = md;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;



            TabSelectedIndex1 = 0;
            TabSelectedIndex2 = 0;
            TabSelectedIndex3 = 0;
            TabSelectedIndex4 = 0;
            TabSelectedIndex5 = 0;

            SliiterTab1 = 0;
            SliiterTab2 = 1;
            SliiterTab3 = 0;
            SliiterTab4 = 0;
            SliiterTab5 = 0;

            EnDisBack1 = "#eae6e6";
            EnDisBack2 = "#eae6e6";
            EnDisBack3 = "#eae6e6";
            EnDisBack4 = "#eae6e6";
            EnDisBack5 = "#eae6e6";

            NextEnabled = true;

            BackgroundWorker worker = new BackgroundWorker();
            LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Loading");

            worker.DoWork += (_, __) =>
            {

                ShiftDetails = DBAccess.GetAllShifts();

                timeTableDates = new ObservableCollection<ProductionTimeTable>();
                timeTableDates = DBAccess.GetAllTimeTableDatesForSlitting(CurrentDate);

                //Get the current shhift
                foreach (var item in ShiftDetails)
                {
                    bool isShift = TimeBetween(DateTime.Now, item.StartTime, item.EndTime);

                    if (isShift == true)
                    {
                        curShift = item.ShiftID;
                    }
                }

                if (curShift == 3)
                {
                    CurrentDate = currDate.AddDays(-1);
                }



                this.UIDispatcher = uidispatcher;
                this.SlittingProductionNotifier = new SlittingProductionNotifier();
                this.productionTimeTableNotifier = new ProductionTimeTableNotifier();

                this.SlittingProductionNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage);
                ObservableCollection<SlittingOrder> opd = this.SlittingProductionNotifier.RegisterDependency();

                this.productionTimeTableNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage2);
                ObservableCollection<ProductionTimeTable> ptt = this.productionTimeTableNotifier.RegisterDependency();

                _index = 0;

                this.LoadData(opd);


            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();

            };
            worker.RunWorkerAsync();

        }

        void notifier_NewMessage(object sender, SqlNotificationEventArgs e)
        {
            //OrderProduction = this.productionSchedularNotifier.RegisterDependency();

            this.LoadData(this.SlittingProductionNotifier.RegisterDependency());
        }

        void notifier_NewMessage2(object sender, SqlNotificationEventArgs e)
        {

            this.LoadProductionTimeTable(this.productionTimeTableNotifier.RegisterDependency());
        }

        private void LoadProductionTimeTable(ObservableCollection<ProductionTimeTable> ptt)
        {
            this.UIDispatcher.BeginInvoke((Action)delegate()
            {
                if (ptt != null)
                {
                    LoadData(this.SlittingProductionNotifier.RegisterDependency());
                }
            });
        }

        private void LoadData(ObservableCollection<SlittingOrder> psd)
        {

            this.UIDispatcher.BeginInvoke((Action)delegate()
            {
                if (psd != null)
                {
                    int curShift = 0;
                    DateTime currDate = DateTime.Now.Date;
                    DateTime date = currDate;
                    BusinessDaysGenerator bdg = new BusinessDaysGenerator();
                    timeTableDates = DBAccess.GetAllTimeTableDatesForSlitting(CurrentDate);

                    //Get the current shhift
                    foreach (var item in ShiftDetails)
                    {
                        bool isShift = TimeBetween(DateTime.Now, item.StartTime, item.EndTime);

                        if (isShift == true)
                        {
                            curShift = item.ShiftID;
                        }
                    }

                    if (curShift == 3)
                    {
                        date = currDate.AddDays(-1);
                    }

                    //Make sure that the dates are sorted
                    psd = new ObservableCollection<SlittingOrder>(psd.OrderBy(i => i.SlittingDate));



                    foreach (var item in timeTableDates)
                    {
                        for (int i = 1; i < 4; i++)
                        {
                            bool skip = false;

                            if (date == item.ProductionDate.Date && i < curShift)
                            {
                                var itemToRemove = psd.Where(x => Convert.ToDateTime(x.SlittingDate).Date == date && x.Shift.ShiftID == i && x.Machine.MachineID == item.MachineID).ToList();
                                foreach (var items in itemToRemove)
                                {
                                    psd.Remove(items);
                                }
                                skip = true;
                            }
                            bool exist = psd.Any(c => Convert.ToDateTime(c.SlittingDate).Date == item.ProductionDate.Date && c.Shift.ShiftID == i && c.Machine.MachineID == item.MachineID);
                            if (exist == false && skip == false)
                            {
                                string btnCol = string.Empty;
                                string shiftText = string.Empty;
                                int shift = i;
                                if (item.IsDayShiftActive == true && i == 1)
                                {
                                    shift = 1;
                                    shiftText = "Enable";
                                    btnCol = "#FFCC3700";
                                }
                                else
                                {
                                    shiftText = "Disable";
                                    btnCol = "Green";
                                }

                                if (item.IsEveningShiftActive == true && i == 2)
                                {
                                    shift = 2;
                                    shiftText = "Enable";
                                    btnCol = "#FFCC3700";
                                }
                                else
                                {
                                    shiftText = "Disable";
                                    btnCol = "Green";
                                }

                                if (item.IsNightShiftActive == true && i == 3)
                                {
                                    shift = 3;
                                    shiftText = "Enable";
                                    btnCol = "#FFCC3700";
                                }
                                else
                                {
                                    shiftText = "Disable";
                                    btnCol = "Green";
                                }

                                psd.Add(new SlittingOrder()
                                {
                                    ID = 0,
                                    ProdTimetableID = 0,
                                    Qty = 0,
                                    Blocks = 0,
                                    DollarValue = 0,
                                    Shift = new Shift() { ShiftID = shift, ShiftName = GetShiftNameByID(i) },
                                    Status = string.Empty,
                                    SlittingDate = item.ProductionDate,
                                    IsNotesVisible = "Collapsed",
                                    IsExpanded = false,
                                    ItemPresenterVivibility = "Collapsed",
                                    ShiftText = shiftText,
                                    ShiftBtnBackColour = btnCol,
                                    ItemBackgroundColour = bdg.ConvertDayToColour(item.ProductionDate),
                                    BottomRowVisibility = "Collapsed",
                                    Machine = new Machines(0)
                                    {
                                        MachineID = item.MachineID,
                                        MachineName = string.Empty
                                    },
                                    Order = new Order()
                                    {
                                        OrderNo = 0,
                                        OrderType = 3,
                                        RequiredDate = item.ProductionDate,
                                        Comments = string.Empty,
                                        Customer = new Customer()
                                        {
                                            CustomerId = 0,
                                            CompanyName = string.Empty
                                        }
                                    },
                                    Product = new Product()
                                    {
                                        ProductID = 0,
                                        ProductCode = string.Empty,
                                        ProductName = string.Empty,
                                        ProductDescription = string.Empty,
                                        ProductUnit = string.Empty,
                                        UnitPrice = 0,
                                        RawProduct = new RawProduct() { Description = string.Empty },
                                        Tile = new Tile()
                                        {
                                            Thickness = 0,
                                            MaxYield = 0,
                                            UnitPrice = 0,
                                            Tile = new Tile()
                                            {
                                                Thickness = 0,
                                                MaxYield = 0,
                                                UnitPrice = 0,
                                                RawProduct = new RawProduct()
                                                {
                                                    RawProductID = 0,
                                                    RawProductType = string.Empty
                                                }
                                            }
                                        }
                                    }
                                });
                            }
                        }

                        _orderProductionList = psd;

                        int noOdItems = timeTableDates.Count();

                        if (timeTableDates.Count != 0 && noOdItems >= _index)
                        {

                            if (timeTableDates.ElementAtOrDefault(_index) != null)
                            {
                                D1 = timeTableDates[_index].ProductionDate.ToString("dd/MM/yyyy");
                                Day1 = DateTime.Parse(D1).DayOfWeek + " " + Regex.Match(D1, "^[^ ]+").Value;
                                OrderProduction1 = AddToBusinessDates(psd, Convert.ToDateTime(D1), SliiterTab1);

                                CollDay1 = new ListCollectionView(AddToTabs(OrderProduction1, TabSelectedIndex1, SliiterTab1));
                                CollDay1.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                                CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift.ShiftID", System.ComponentModel.ListSortDirection.Ascending));
                                CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.OrderType", System.ComponentModel.ListSortDirection.Ascending));

                                if (SliiterTab1 == 1)
                                {
                                    if (timeTableDates[_index].IsDayShiftActive == true || timeTableDates[_index].IsEveningShiftActive == true || timeTableDates[_index].IsNightShiftActive == true)
                                    {
                                        EnDisText1 = "Disable";
                                        EnDisBack1 = "Green";
                                    }
                                    else
                                    {
                                        EnDisText1 = "Enable";
                                        EnDisBack1 = "#FFCC3700";
                                    }
                                }
                                if (SliiterTab1 == 0)
                                {

                                    if (timeTableDates[_index + 1].IsDayShiftActive == true || timeTableDates[_index + 1].IsEveningShiftActive == true || timeTableDates[_index + 1].IsNightShiftActive == true)
                                    {
                                        EnDisText1 = "Disable";
                                        EnDisBack1 = "Green";
                                    }
                                    else
                                    {
                                        EnDisText1 = "Enable";
                                        EnDisBack1 = "#FFCC3700";
                                    }
                                }
                            }
                            else
                            {
                                Day1 = string.Empty;
                                OrderProduction1 = null;
                                CollDay1 = null;
                            }

                            if (timeTableDates.ElementAtOrDefault(_index + 2) != null)
                            {

                                D2 = timeTableDates[_index + 2].ProductionDate.ToString("dd/MM/yyyy");
                                Day2 = DateTime.Parse(D2).DayOfWeek + " " + Regex.Match(D2, "^[^ ]+").Value;

                                OrderProduction2 = AddToBusinessDates(psd, Convert.ToDateTime(D2), SliiterTab2);
                                CollDay2 = new ListCollectionView(AddToTabs(OrderProduction2, TabSelectedIndex2, SliiterTab2));
                                CollDay2.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                                CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift.ShiftID", System.ComponentModel.ListSortDirection.Ascending));
                                CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.OrderType", System.ComponentModel.ListSortDirection.Ascending));

                                if (SliiterTab2 == 1)
                                {
                                    if (timeTableDates[_index + 2].IsDayShiftActive == true || timeTableDates[_index + 2].IsEveningShiftActive == true || timeTableDates[_index + 2].IsNightShiftActive == true)
                                    {
                                        EnDisText2 = "Disable";
                                        EnDisBack2 = "Green";
                                    }
                                    else
                                    {
                                        EnDisText2 = "Enable";
                                        EnDisBack2 = "#FFCC3700";
                                    }
                                }

                                if (SliiterTab2 == 0)
                                {

                                    if (timeTableDates[_index + 3].IsDayShiftActive == true || timeTableDates[_index + 3].IsEveningShiftActive == true || timeTableDates[_index + 3].IsNightShiftActive == true)
                                    {
                                        EnDisText2 = "Disable";
                                        EnDisBack2 = "Green";
                                    }
                                    else
                                    {
                                        EnDisText2 = "Enable";
                                        EnDisBack2 = "#FFCC3700";
                                    }
                                }
                            }
                            else
                            {
                                Day2 = string.Empty;
                                OrderProduction2 = null;
                                CollDay2 = null;
                            }

                            if (timeTableDates.ElementAtOrDefault(_index + 4) != null)
                            {
                                D3 = timeTableDates[_index + 4].ProductionDate.ToString("dd/MM/yyyy");
                                Day3 = DateTime.Parse(D3).DayOfWeek + " " + Regex.Match(D3, "^[^ ]+").Value;
                                OrderProduction3 = AddToBusinessDates(psd, Convert.ToDateTime(D3), SliiterTab3);
                                CollDay3 = new ListCollectionView(AddToTabs(OrderProduction3, TabSelectedIndex3, SliiterTab3));
                                CollDay3.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                                CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift.ShiftID", System.ComponentModel.ListSortDirection.Ascending));
                                CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.OrderType", System.ComponentModel.ListSortDirection.Ascending));

                                if (SliiterTab3 == 1)
                                {
                                    if (timeTableDates[_index + 4].IsDayShiftActive == true || timeTableDates[_index + 4].IsEveningShiftActive == true || timeTableDates[_index + 4].IsNightShiftActive == true)
                                    {
                                        EnDisText3 = "Disable";
                                        EnDisBack3 = "Green";
                                    }
                                    else
                                    {
                                        EnDisText3 = "Enable";
                                        EnDisBack3 = "#FFCC3700";
                                    }

                                }

                                if (SliiterTab3 == 0)
                                {
                                    if (timeTableDates[_index + 5].IsDayShiftActive == true || timeTableDates[_index + 5].IsEveningShiftActive == true || timeTableDates[_index + 5].IsNightShiftActive == true)
                                    {
                                        EnDisText3 = "Disable";
                                        EnDisBack3 = "Green";
                                    }
                                    else
                                    {
                                        EnDisText3 = "Enable";
                                        EnDisBack3 = "#FFCC3700";
                                    }
                                }
                            }
                            else
                            {
                                Day3 = string.Empty;
                                OrderProduction3 = null;
                                CollDay3 = null;
                            }

                            if (timeTableDates.ElementAtOrDefault(_index + 6) != null)
                            {
                                D4 = timeTableDates[_index + 6].ProductionDate.ToString("dd/MM/yyyy");
                                Day4 = DateTime.Parse(D4).DayOfWeek + " " + Regex.Match(D4, "^[^ ]+").Value;
                                OrderProduction4 = AddToBusinessDates(psd, Convert.ToDateTime(D4), SliiterTab4);
                                CollDay4 = new ListCollectionView(AddToTabs(OrderProduction4, TabSelectedIndex4, SliiterTab4));
                                CollDay4.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                                CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift.ShiftID", System.ComponentModel.ListSortDirection.Ascending));
                                CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.OrderType", System.ComponentModel.ListSortDirection.Ascending));

                                if (SliiterTab4 == 1)
                                {
                                    if (timeTableDates[_index + 6].IsDayShiftActive == true || timeTableDates[_index + 6].IsEveningShiftActive == true || timeTableDates[_index + 6].IsNightShiftActive == true)
                                    {
                                        EnDisText4 = "Disable";
                                        EnDisBack4 = "Green";
                                    }
                                    else
                                    {
                                        EnDisText4 = "Enable";
                                        EnDisBack4 = "#FFCC3700";
                                    }
                                }

                                if (SliiterTab4 == 0)
                                {
                                    if (timeTableDates[_index + 7].IsDayShiftActive == true || timeTableDates[_index + 7].IsEveningShiftActive == true || timeTableDates[_index + 7].IsNightShiftActive == true)
                                    {
                                        EnDisText4 = "Disable";
                                        EnDisBack4 = "Green";
                                    }
                                    else
                                    {
                                        EnDisText4 = "Enable";
                                        EnDisBack4 = "#FFCC3700";
                                    }
                                }
                            }
                            else
                            {
                                Day4 = string.Empty;
                                OrderProduction4 = null;
                                CollDay4 = null;
                            }

                            if (timeTableDates.ElementAtOrDefault(_index + 8) != null)
                            {
                                D5 = timeTableDates[_index + 8].ProductionDate.ToString("dd/MM/yyyy");
                                Day5 = DateTime.Parse(D5).DayOfWeek + " " + Regex.Match(D5, "^[^ ]+").Value;
                                OrderProduction5 = AddToBusinessDates(psd, Convert.ToDateTime(D5), SliiterTab5);
                                CollDay5 = new ListCollectionView(AddToTabs(OrderProduction5, TabSelectedIndex5, SliiterTab5));
                                CollDay5.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                                CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift.ShiftID", System.ComponentModel.ListSortDirection.Ascending));
                                CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.OrderType", System.ComponentModel.ListSortDirection.Ascending));

                                if (SliiterTab5 == 1)
                                {
                                    if (timeTableDates[_index + 8].IsDayShiftActive == true || timeTableDates[_index + 8].IsEveningShiftActive == true || timeTableDates[_index + 8].IsNightShiftActive == true)
                                    {
                                        EnDisText5 = "Disable";
                                        EnDisBack5 = "Green";
                                    }
                                    else
                                    {
                                        EnDisText5 = "Enable";
                                        EnDisBack5 = "#FFCC3700";
                                    }
                                }

                                if (SliiterTab5 == 0)
                                {
                                    if (timeTableDates[_index + 9].IsDayShiftActive == true || timeTableDates[_index + 9].IsEveningShiftActive == true || timeTableDates[_index + 9].IsNightShiftActive == true)
                                    {
                                        EnDisText5 = "Disable";
                                        EnDisBack5 = "Green";
                                    }
                                    else
                                    {
                                        EnDisText5 = "Enable";
                                        EnDisBack5 = "#FFCC3700";
                                    }
                                }
                            }
                            else
                            {
                                Day5 = string.Empty;
                                OrderProduction5 = null;
                                CollDay5 = null;
                            }
                        }
                    }
                }
            });
        }

        private ObservableCollection<SlittingOrder> AddToBusinessDates(ObservableCollection<SlittingOrder> prodSchDetails, DateTime date, int slitter)
        {
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            OrderProduction = new ObservableCollection<SlittingOrder>();
            prodSchDetails = new ObservableCollection<SlittingOrder>(prodSchDetails.OrderBy(i => i.Order.OrderType).ThenBy(t => t.Order.OrderNo));
            int machineId = 0;

            if (slitter == 0)
            {
                machineId = 8;
            }
            else
            {
                machineId = 4;
            }

            ObservableCollection<ProductionTimeTable> tempTimeTables = new ObservableCollection<ProductionTimeTable>();

            foreach (var item in timeTableDates)
            {
                if (item.MachineID == machineId)
                    tempTimeTables.Add(item);
            }

            foreach (var item in prodSchDetails)
            {
                if (Convert.ToDateTime(item.SlittingDate) == date || Convert.ToDateTime(item.SlittingDate) == date && date == Convert.ToDateTime(CurrentDate) && item.Machine.MachineID == machineId)
                {
                    string shiftText = "Disable";
                    string btnCol = "Green";

                    foreach (var itemTTD in tempTimeTables)
                    {
                        if (itemTTD.ProductionDate.ToString("dd/MM/yyyy") == item.SlittingDate.ToString("dd/MM/yyyy") && item.Machine.MachineID == machineId)
                        {
                            if (itemTTD.IsDayShiftActive == true && item.Shift.ShiftID == 1)
                            {
                                shiftText = "Disable";
                                btnCol = "Green";
                                break;
                            }
                            else if (itemTTD.IsEveningShiftActive == true && item.Shift.ShiftID == 2)
                            {
                                shiftText = "Disable";
                                btnCol = "Green";
                                break;
                            }
                            else if (itemTTD.IsNightShiftActive == true && item.Shift.ShiftID == 3)
                            {
                                shiftText = "Disable";
                                btnCol = "Green";
                                break;
                            }
                            else
                            {
                                shiftText = "Enable";
                                btnCol = "#FFCC3700";
                            }
                        }
                    }
                    item.BottomRowVisibility = item.Order.OrderType == 2 || item.Order.OrderType == 1 ? "Visible" : "Collapsed";
                    item.ShiftText = shiftText;
                    item.ShiftBtnBackColour = btnCol;
                    item.ItemPresenterVivibility = item.ProdTimetableID == 0 ? "Collapsed" : "Visible";
                    item.Shift.ShiftName = GetShiftNameByID(item.Shift.ShiftID);
                    item.ItemBackgroundColour = bdg.ConvertDayToColour(item.SlittingDate);
                    item.IsExpanded = item.ProdTimetableID == 0 ? false : true;

                    OrderProduction.Add(item);
                }
            }

            return OrderProduction;
        }





        private Tuple<int, string> GetMachine(int slitTab)
        {
            string slitterName = "Carousel Sliiter";
            int x = 0;
            if (slitTab == 0)
            {
                slitterName = "Carousel Sliiter";
                x = 8;
            }
            else
            {
                slitterName = "Flat Bench Sliiter";
                x = 4;
            }

            return Tuple.Create(x, slitterName);
        }

        private ObservableCollection<SlittingOrder> EnableProcessOrderProd(ObservableCollection<SlittingOrder> op, DateTime date, int machineId)
        {

            foreach (var item in op)
            {
                if (Convert.ToDateTime(item.SlittingDate).Date == date.Date && item.Machine.MachineID == machineId)
                {
                    if (item.Shift.ShiftID == 1)
                    {
                        item.ShiftText = "Disable";
                        item.ShiftBtnBackColour = "Green";
                    }
                    else if (item.Shift.ShiftID == 2)
                    {
                        item.ShiftText = "Disable";
                        item.ShiftBtnBackColour = "Green";
                    }
                    else if (item.Shift.ShiftID == 3)
                    {
                        item.ShiftText = "Disable";
                        item.ShiftBtnBackColour = "Green";
                    }
                }
            }
            return op;
        }

        private ObservableCollection<SlittingOrder> DisableProcessOrderProd(ObservableCollection<SlittingOrder> op, DateTime date, int machineId)
        {
            foreach (var item in op)
            {
                if (Convert.ToDateTime(item.SlittingDate).Date == date && item.Machine.MachineID == machineId)
                {
                    if (item.Shift.ShiftID == 1)
                    {
                        item.ShiftText = "Enable";
                        item.ShiftBtnBackColour = "#FFCC3700";
                    }
                    else if (item.Shift.ShiftID == 2)
                    {
                        item.ShiftText = "Enable";
                        item.ShiftBtnBackColour = "#FFCC3700";
                    }
                    else if (item.Shift.ShiftID == 3)
                    {
                        item.ShiftText = "Enable";
                        item.ShiftBtnBackColour = "#FFCC3700";
                    }
                }
            }
            return op;
        }

        public void EnDisDay(string str)
        {

            List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
            bool has = systemParameters.Any(x => x.Value == true);
            if (has == true)
            {
                Msg.Show("System is performing some updates. Please try again in few minutes ", "System Busy", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                SlittingManager sm = new SlittingManager();

                switch (str)
                {
                    case "1":
                        Tuple<int, string> el1 = GetMachine(SliiterTab1);
                        List<int> machineIds = new List<int>();
                        machineIds.Add(el1.Item1);
                        if (EnDisText1 == "Enable")
                        {
                            if (Msg.Show("Are you sure, you want to enable this day for " + el1.Item2 + "?", "Enabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                EnDisText1 = "Disable";
                                EnDisBack1 = "Green";
                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D1), machineIds, true, true, true, true);
                                if (r > 0)
                                {
                                    OrderProduction1 = EnableProcessOrderProd(OrderProduction1, Convert.ToDateTime(D1).Date, el1.Item1);

                                    if (Msg.Show("Do you want to allocate existing orders to this day?", "Enabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                                    {
                                        List<SlittingOrder> dd = DBAccess.GetSlittingOrdersByMachine(el1.Item1);
                                        sm.BackupDeleteAdd(dd);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Msg.Show("Are you sure, you want to disable this day for " + el1.Item2 + "?", "Disabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D1), machineIds, true, false, false, false);
                                if (r > 0)
                                {
                                    List<SlittingOrder> dd = DBAccess.GetSlittingOrdersByMachine(el1.Item1);
                                    sm.BackupDeleteAdd(dd);

                                    EnDisText1 = "Enable";
                                    EnDisBack1 = "#FFCC3700";
                                    OrderProduction1 = DisableProcessOrderProd(OrderProduction1, Convert.ToDateTime(D1).Date, el1.Item1);
                                }
                            }
                        }
                        LoadData(OrderProduction1);

                        break;
                    case "2":

                        Tuple<int, string> el2 = GetMachine(SliiterTab2);
                        List<int> machineIds2 = new List<int>();
                        machineIds2.Add(el2.Item1);
                        if (EnDisText2 == "Enable")
                        {
                            if (Msg.Show("Are you sure, you want to enable this day for " + el2.Item2 + "?", "Enabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                EnDisText2 = "Disable";
                                EnDisBack2 = "Green";
                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D2), machineIds2, true, true, true, true);
                                if (r > 0)
                                {
                                    OrderProduction2 = EnableProcessOrderProd(OrderProduction2, Convert.ToDateTime(D2).Date, el2.Item1);

                                    if (Msg.Show("Do you want to allocate existing orders to this day?", "Enabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                                    {
                                        List<SlittingOrder> dd = DBAccess.GetSlittingOrdersByMachine(el2.Item1);
                                        sm.BackupDeleteAdd(dd);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Msg.Show("Are you sure, you want to disable this day for " + el2.Item2 + "?", "Disabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D2), machineIds2, true, false, false, false);
                                if (r > 0)
                                {
                                    List<SlittingOrder> dd = DBAccess.GetSlittingOrdersByMachine(el2.Item1);
                                    sm.BackupDeleteAdd(dd);

                                    EnDisText2 = "Enable";
                                    EnDisBack2 = "#FFCC3700";
                                    OrderProduction2 = DisableProcessOrderProd(OrderProduction2, Convert.ToDateTime(D2).Date, el2.Item1);
                                }
                            }
                        }
                        LoadData(OrderProduction2);

                        break;
                    case "3":

                        Tuple<int, string> el3 = GetMachine(SliiterTab3);
                        List<int> machineIds3 = new List<int>();
                        machineIds3.Add(el3.Item1);
                        if (EnDisText3 == "Enable")
                        {
                            if (Msg.Show("Are you sure, you want to enable this day for " + el3.Item2 + "?", "Enabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                EnDisText3 = "Disable";
                                EnDisBack3 = "Green";
                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D3), machineIds3, true, true, true, true);
                                if (r > 0)
                                {
                                    OrderProduction3 = EnableProcessOrderProd(OrderProduction3, Convert.ToDateTime(D3).Date, el3.Item1);

                                    if (Msg.Show("Do you want to allocate existing orders to this day?", "Enabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                                    {
                                        List<SlittingOrder> dd = DBAccess.GetSlittingOrdersByMachine(el3.Item1);
                                        sm.BackupDeleteAdd(dd);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Msg.Show("Are you sure, you want to disable this day for " + el3.Item2 + "?", "Disabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D3), machineIds3, true, false, false, false);
                                if (r > 0)
                                {
                                    List<SlittingOrder> dd = DBAccess.GetSlittingOrdersByMachine(el3.Item1);
                                    sm.BackupDeleteAdd(dd);

                                    EnDisText3 = "Enable";
                                    EnDisBack3 = "#FFCC3700";
                                    OrderProduction3 = DisableProcessOrderProd(OrderProduction3, Convert.ToDateTime(D3).Date, el3.Item1);
                                }
                            }
                        }
                        LoadData(OrderProduction3);

                        break;
                    case "4":

                        Tuple<int, string> el4 = GetMachine(SliiterTab4);
                        List<int> machineIds4 = new List<int>();
                        machineIds4.Add(el4.Item1);
                        if (EnDisText4 == "Enable")
                        {
                            if (Msg.Show("Are you sure, you want to enable this day for " + el4.Item2 + "?", "Enabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                EnDisText4 = "Disable";
                                EnDisBack4 = "Green";
                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D4), machineIds4, true, true, true, true);
                                if (r > 0)
                                {
                                    OrderProduction4 = EnableProcessOrderProd(OrderProduction4, Convert.ToDateTime(D4).Date, el4.Item1);

                                    if (Msg.Show("Do you want to allocate existing orders to this day?", "Enabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                                    {
                                        List<SlittingOrder> dd = DBAccess.GetSlittingOrdersByMachine(el4.Item1);
                                        sm.BackupDeleteAdd(dd);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Msg.Show("Are you sure, you want to disable this day for " + el4.Item2 + "?", "Disabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D4), machineIds4, true, false, false, false);
                                if (r > 0)
                                {
                                    List<SlittingOrder> dd = DBAccess.GetSlittingOrdersByMachine(el4.Item1);
                                    sm.BackupDeleteAdd(dd);

                                    EnDisText4 = "Enable";
                                    EnDisBack4 = "#FFCC3700";
                                    OrderProduction4 = DisableProcessOrderProd(OrderProduction4, Convert.ToDateTime(D4).Date, el4.Item1);
                                }
                            }
                        }
                        LoadData(OrderProduction4);

                        break;
                    case "5":

                        Tuple<int, string> el5 = GetMachine(SliiterTab5);
                        List<int> machineIds5 = new List<int>();
                        machineIds5.Add(el5.Item1);
                        if (EnDisText5 == "Enable")
                        {
                            if (Msg.Show("Are you sure, you want to enable this day for " + el5.Item2 + "?", "Enabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                EnDisText5 = "Disable";
                                EnDisBack5 = "Green";
                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D5), machineIds5, true, true, true, true);
                                if (r > 0)
                                {
                                    OrderProduction5 = EnableProcessOrderProd(OrderProduction5, Convert.ToDateTime(D5).Date, el5.Item1);

                                    if (Msg.Show("Do you want to allocate existing orders to this day?", "Enabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                                    {
                                        List<SlittingOrder> dd = DBAccess.GetSlittingOrdersByMachine(el5.Item1);
                                        sm.BackupDeleteAdd(dd);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Msg.Show("Are you sure, you want to disable this day for " + el5.Item2 + "?", "Disabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D5), machineIds5, true, false, false, false);
                                if (r > 0)
                                {
                                    List<SlittingOrder> dd = DBAccess.GetSlittingOrdersByMachine(el5.Item1);
                                    sm.BackupDeleteAdd(dd);

                                    EnDisText5 = "Enable";
                                    EnDisBack5 = "#FFCC3700";
                                    OrderProduction5 = DisableProcessOrderProd(OrderProduction5, Convert.ToDateTime(D5).Date, el5.Item1);
                                }
                            }
                        }
                        LoadData(OrderProduction5);

                        break;
                    default:

                        break;
                }

                //LoadData(_orderProductionList);
            }
        }

        private void PrevDate()
        {
            List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
            bool has = systemParameters.Any(x => x.Value == true);
            if (has == true)
            {
                Msg.Show("Orders are bieng shifted at the moment. Please try again in few minutes ", "Orders Shifting", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                _index -= 2;
                if (_index < 0)
                {
                    _index = 0;
                }
                LoadData(_orderProductionList);
            }
        }

        private void NextDate()
        {
            List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
            bool has = systemParameters.Any(x => x.Value == true);
            if (has == true)
            {
                Msg.Show("Orders are bieng shifted at the moment. Please try again in 5 minutes ", "Orders Shifting", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                BackgroundWorker worker = new BackgroundWorker();
                LoadingScreen = new ChildWindowView();
                LoadingScreen.ShowWaitingScreen("Processing " + System.Environment.NewLine + System.Environment.NewLine + System.Environment.NewLine + "Please dont close");

                worker.DoWork += (_, __) =>
                {

                    int noOdItems = timeTableDates.Count();
                    int i = _index + 11;

                    if (noOdItems >= i)
                    {
                        _index += 2;
                    }
                    else
                    {
                        //int sp1 = DBAccess.UpdateSystemParameter("AddingNewDates", true);
                        //if (sp1 > 0)
                        //{
                        NextEnabled = false;
                        Production production = new Production();
                        bool a = production.AddNewDates(Convert.ToDateTime(D5).AddDays(1), false);
                        if (a)
                        {
                            noOdItems = 0;
                            i = 0;
                            noOdItems = timeTableDates.Count();
                            i = _index + 5;

                            if (noOdItems >= i)
                            {
                                _index += 2;
                            }
                            NextEnabled = true;
                            int sp2 = DBAccess.UpdateSystemParameter("AddingNewDates", false);
                        }
                        // }
                    }
                    LoadData(_orderProductionList);
                };

                worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    LoadingScreen.CloseWaitingScreen();
                };
                worker.RunWorkerAsync();
            }
        }


        private ObservableCollection<SlittingOrder> AddToTabs(ObservableCollection<SlittingOrder> prodSchDetails, int TabSelectedIndex, int slitter)
        {
            OrderProduction = new ObservableCollection<SlittingOrder>();

            int machineId = 0;

            if (slitter == 0)
            {
                machineId = 8;
            }
            else
            {
                machineId = 4;
            }

            foreach (var item in prodSchDetails)
            {
                if (item.Shift.ShiftID == TabSelectedIndex && item.Machine.MachineID == machineId)
                {
                    OrderProduction = ProcessTabs(item, machineId);

                }
                else if (TabSelectedIndex == 0 && item.Machine.MachineID == machineId)
                {
                    OrderProduction = ProcessTabs(item, machineId);
                    //OrderProduction = prodSchDetails;
                }
            }

            OrderProduction = new ObservableCollection<SlittingOrder>(OrderProduction.OrderBy(i => i.Order.OrderType).ThenBy(t => t.Order.OrderNo));

            return OrderProduction;
        }

        private ObservableCollection<SlittingOrder> ProcessTabs(SlittingOrder so, int x)
        {
            string shiftText = string.Empty;
            string btnCol = string.Empty;
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();

            ObservableCollection<ProductionTimeTable> tempProdTimeTables = new ObservableCollection<ProductionTimeTable>();
            foreach (var item in timeTableDates)
            {
                if (item.MachineID == x)
                {
                    tempProdTimeTables.Add(item);
                }
            }

            foreach (var itemTTD in tempProdTimeTables)
            {
                if (itemTTD.ProductionDate.ToString("dd/MM/yyyy") == so.SlittingDate.ToString("dd/MM/yyyy") && itemTTD.MachineID == x)
                {
                    if (itemTTD.IsDayShiftActive == true && so.Shift.ShiftID == 1)
                    {
                        shiftText = "Disable";
                        btnCol = "Green";
                        break;
                    }

                    else if (itemTTD.IsEveningShiftActive == true && so.Shift.ShiftID == 2)
                    {
                        shiftText = "Disable";
                        btnCol = "Green";
                        break;
                    }

                    else if (itemTTD.IsNightShiftActive == true && so.Shift.ShiftID == 3)
                    {
                        shiftText = "Disable";
                        btnCol = "Green";
                        break;
                    }
                    else
                    {
                        shiftText = "Enable";
                        btnCol = "#FFCC3700";
                    }
                }
            }


            OrderProduction.Add(new SlittingOrder()
            {
                ID = so.ID,
                ProdTimetableID = so.ProdTimetableID,
                Qty = so.Qty,
                Blocks = Math.Ceiling(so.Blocks),
                DollarValue = so.DollarValue,
                Shift = new Shift() { ShiftID = so.Shift.ShiftID, ShiftName = GetShiftNameByID(so.Shift.ShiftID) },
                Status = so.Status,
                SlittingDate = so.SlittingDate,
                IsNotesVisible = so.IsNotesVisible,
                IsExpanded = so.ProdTimetableID == 0 ? false : true,
                ItemPresenterVivibility = so.ProdTimetableID == 0 ? "Collapsed" : "Visible",
                ShiftText = shiftText,
                ShiftBtnBackColour = btnCol,
                BottomRowVisibility = so.Order.OrderType == 2 || so.Order.OrderType == 1 ? "Visible" : "Collapsed",
                ItemBackgroundColour = bdg.ConvertDayToColour(so.SlittingDate),
                Machine = new Machines(0)
                {
                    MachineID = so.Machine.MachineID,
                    MachineName = so.Machine.MachineName
                },
                Order = new Order()
                {
                    OrderNo = so.Order.OrderNo,

                    Comments = so.Order.Comments,
                    OrderType = so.Order.OrderType,
                    RequiredDate = so.SlittingDate,
                    Customer = new Customer()
                    {
                        CustomerId = so.Order.Customer.CustomerId,
                        CompanyName = so.Order.Customer.CompanyName
                    }
                },
                Product = new Product()
                {
                    ProductID = so.Product.ProductID,
                    ProductCode = so.Product.ProductCode,
                    ProductName = so.Product.ProductName,
                    ProductDescription = so.Product.ProductDescription,
                    ProductUnit = so.Product.ProductUnit,
                    UnitPrice = so.Product.UnitPrice,
                    RawProduct = new RawProduct() { Description = so.Product.RawProduct.Description },
                    Tile = new Tile()
                    {
                        Thickness = Math.Ceiling(so.Product.Tile.Thickness),
                        MaxYield = so.Product.Tile.MaxYield,
                        UnitPrice = so.Product.UnitPrice,
                        RawProduct = new RawProduct()
                        {
                            RawProductID = so.Product.RawProduct == null ? 0 : so.Product.RawProduct.RawProductID,
                            RawProductType = so.Product.RawProduct == null ? "" : so.Product.RawProduct.RawProductType
                        }
                    }
                }
            });

            return OrderProduction;
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

        private string GetShiftNameByID(int shiftId)
        {

            string result = string.Empty;

            switch (shiftId)
            {

                case 1: result = "Day";
                    break;
                case 2: result = "Afternoon";
                    break;
                case 3: result = "Night";
                    break;
                default: result = "Unspecified";
                    break;
            }

            return result;
        }


        public ObservableCollection<SlittingOrder> OrderProduction1
        {
            get
            {
                return _orderProduction1;
            }

            set
            {
                _orderProduction1 = value;
                RaisePropertyChanged(() => this.OrderProduction1);
            }
        }

        public ObservableCollection<SlittingOrder> OrderProduction2
        {
            get
            {
                return _orderProduction2;
            }

            set
            {
                _orderProduction2 = value;
                RaisePropertyChanged(() => this.OrderProduction2);
            }
        }

        public ObservableCollection<SlittingOrder> OrderProduction3
        {
            get
            {
                return _orderProduction3;
            }

            set
            {
                _orderProduction3 = value;
                RaisePropertyChanged(() => this.OrderProduction3);
            }
        }

        public ObservableCollection<SlittingOrder> OrderProduction4
        {
            get
            {
                return _orderProduction4;
            }

            set
            {
                _orderProduction4 = value;
                RaisePropertyChanged(() => this.OrderProduction4);
            }
        }

        public ObservableCollection<SlittingOrder> OrderProduction5
        {
            get
            {
                return _orderProduction5;
            }

            set
            {
                _orderProduction5 = value;
                RaisePropertyChanged(() => this.OrderProduction5);
            }
        }

        public ObservableCollection<SlittingOrder> OrderProduction
        {
            get
            {
                return _orderProduction;
            }
            set
            {
                _orderProduction = value;
                RaisePropertyChanged(() => this.OrderProduction);
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

        public int SliiterTab1
        {
            get
            {
                return _sliiterTab1;
            }
            set
            {
                _sliiterTab1 = value;
                RaisePropertyChanged(() => this.SliiterTab1);
                if (OrderProduction1 != null)
                {
                    LoadData(_orderProductionList);
                }

            }
        }

        public int SliiterTab2
        {
            get
            {
                return _sliiterTab2;
            }
            set
            {
                _sliiterTab2 = value;
                RaisePropertyChanged(() => this.SliiterTab2);
                if (OrderProduction2 != null)
                {
                    LoadData(_orderProductionList);
                }
            }
        }

        public int SliiterTab3
        {
            get
            {
                return _sliiterTab3;
            }
            set
            {
                _sliiterTab3 = value;
                RaisePropertyChanged(() => this.SliiterTab3);
                if (OrderProduction3 != null)
                {
                    LoadData(_orderProductionList);
                    //CollDay3 = new ListCollectionView(AddToTabs(OrderProduction3, TabSelectedIndex3, SliiterTab3));
                    //CollDay3.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                    //CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                    //CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

                }
            }
        }

        public int SliiterTab4
        {
            get
            {
                return _sliiterTab4;
            }
            set
            {
                _sliiterTab4 = value;
                RaisePropertyChanged(() => this.SliiterTab4);
                if (OrderProduction4 != null)
                {
                    LoadData(_orderProductionList);
                    //CollDay4 = new ListCollectionView(AddToTabs(OrderProduction4, TabSelectedIndex4, SliiterTab4));
                    //CollDay4.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                    //CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                    //CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));
                }
            }
        }

        public int SliiterTab5
        {
            get
            {
                return _sliiterTab5;
            }
            set
            {
                _sliiterTab5 = value;
                RaisePropertyChanged(() => this.SliiterTab5);
                if (OrderProduction5 != null)
                {
                    LoadData(_orderProductionList);
                    //CollDay5 = new ListCollectionView(AddToTabs(OrderProduction5, TabSelectedIndex5, SliiterTab5));
                    //CollDay5.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                    //CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                    //CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

                }
            }
        }

        public int TabSelectedIndex1
        {
            get { return _tabSelectedIndex1; }
            set
            {
                _tabSelectedIndex1 = value;
                RaisePropertyChanged(() => this.TabSelectedIndex1);
                if (OrderProduction1 != null)
                {
                    CollDay1 = new ListCollectionView(AddToTabs(OrderProduction1, TabSelectedIndex1, SliiterTab1));
                    CollDay1.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                    CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift.ShiftID", System.ComponentModel.ListSortDirection.Ascending));
                    CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.OrderType", System.ComponentModel.ListSortDirection.Ascending));
                }
            }
        }

        public int TabSelectedIndex2
        {
            get { return _tabSelectedIndex2; }
            set
            {
                _tabSelectedIndex2 = value;
                RaisePropertyChanged(() => this.TabSelectedIndex2);
                if (OrderProduction2 != null)
                {
                    CollDay2 = new ListCollectionView(AddToTabs(OrderProduction2, TabSelectedIndex2, SliiterTab2));
                    CollDay2.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                    CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift.ShiftID", System.ComponentModel.ListSortDirection.Ascending));
                    CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.OrderType", System.ComponentModel.ListSortDirection.Ascending));
                }
            }
        }

        public int TabSelectedIndex3
        {
            get { return _tabSelectedIndex3; }
            set
            {
                _tabSelectedIndex3 = value;
                RaisePropertyChanged(() => this.TabSelectedIndex3);
                if (OrderProduction3 != null)
                {
                    CollDay3 = new ListCollectionView(AddToTabs(OrderProduction3, TabSelectedIndex3, SliiterTab3));
                    CollDay3.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                    CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift.ShiftID", System.ComponentModel.ListSortDirection.Ascending));
                    CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.OrderType", System.ComponentModel.ListSortDirection.Ascending));

                }
            }
        }

        public int TabSelectedIndex4
        {
            get { return _tabSelectedIndex4; }
            set
            {
                _tabSelectedIndex4 = value;
                RaisePropertyChanged(() => this.TabSelectedIndex4);
                if (OrderProduction4 != null)
                {
                    CollDay4 = new ListCollectionView(AddToTabs(OrderProduction4, TabSelectedIndex4, SliiterTab4));
                    CollDay4.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                    CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift.ShiftID", System.ComponentModel.ListSortDirection.Ascending));
                    CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.OrderType", System.ComponentModel.ListSortDirection.Ascending));
                }
            }
        }

        public int TabSelectedIndex5
        {
            get { return _tabSelectedIndex5; }
            set
            {
                _tabSelectedIndex5 = value;
                RaisePropertyChanged(() => this.TabSelectedIndex5);
                if (OrderProduction5 != null)
                {
                    CollDay5 = new ListCollectionView(AddToTabs(OrderProduction5, TabSelectedIndex5, SliiterTab5));
                    CollDay5.GroupDescriptions.Add(new PropertyGroupDescription("Shift.ShiftName"));
                    CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift.ShiftID", System.ComponentModel.ListSortDirection.Ascending));
                    CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("Order.OrderType", System.ComponentModel.ListSortDirection.Ascending));
                }
            }
        }


        public string Day1
        {
            get { return _day1; }
            set { _day1 = value; RaisePropertyChanged(() => this.Day1); }
        }

        public string Day2
        {
            get { return _day2; }
            set { _day2 = value; RaisePropertyChanged(() => this.Day2); }
        }

        public string Day3
        {
            get { return _day3; }
            set { _day3 = value; RaisePropertyChanged(() => this.Day3); }
        }

        public string Day4
        {
            get { return _day4; }
            set { _day4 = value; RaisePropertyChanged(() => this.Day4); }
        }

        public string Day5
        {
            get { return _day5; }
            set { _day5 = value; RaisePropertyChanged(() => this.Day5); }
        }

        public bool NextEnabled
        {
            get { return _nextEnabled; }
            set
            {
                _nextEnabled = value;
                RaisePropertyChanged(() => this.NextEnabled);
            }
        }

        public string EnDisText1
        {
            get { return _enDisText1; }
            set
            {
                _enDisText1 = value;
                RaisePropertyChanged(() => this.EnDisText1);
            }
        }

        public string EnDisText2
        {
            get { return _enDisText2; }
            set
            {
                _enDisText2 = value;
                RaisePropertyChanged(() => this.EnDisText2);
            }
        }

        public string EnDisText3
        {
            get { return _enDisText3; }
            set
            {
                _enDisText3 = value;
                RaisePropertyChanged(() => this.EnDisText3);
            }
        }

        public string EnDisText4
        {
            get { return _enDisText4; }
            set
            {
                _enDisText4 = value;
                RaisePropertyChanged(() => this.EnDisText4);
            }
        }

        public string EnDisText5
        {
            get { return _enDisText5; }
            set
            {
                _enDisText5 = value;
                RaisePropertyChanged(() => this.EnDisText5);
            }
        }

        public string EnDisBack1
        {
            get { return _enDisBack1; }
            set
            {
                _enDisBack1 = value;
                RaisePropertyChanged(() => this.EnDisBack1);
            }
        }

        public string EnDisBack2
        {
            get { return _enDisBack2; }
            set
            {
                _enDisBack2 = value;
                RaisePropertyChanged(() => this.EnDisBack2);
            }
        }

        public string EnDisBack3
        {
            get { return _enDisBack3; }
            set
            {
                _enDisBack3 = value;
                RaisePropertyChanged(() => this.EnDisBack3);
            }
        }

        public string EnDisBack4
        {
            get { return _enDisBack4; }
            set
            {
                _enDisBack4 = value;
                RaisePropertyChanged(() => this.EnDisBack4);
            }
        }

        public string EnDisBack5
        {
            get { return _enDisBack5; }
            set
            {
                _enDisBack5 = value;
                RaisePropertyChanged(() => this.EnDisBack5);
            }
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return _navHomeCommand ?? (_navHomeCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand AdminDashboardCommand
        {
            get
            {
                return _adminDashboardCommand ?? (_adminDashboardCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand ProductionMaintenanceCommand
        {
            get
            {
                return _productionMaintenanceCommand ?? (_productionMaintenanceCommand = new LogOutCommandHandler(() => Switcher.Switch(new ProductionMaintenanceView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand PrevCommand
        {
            get
            {
                return _prevCommand ?? (_prevCommand = new LogOutCommandHandler(() => PrevDate(), canExecute));
            }
        }

        public ICommand NextCommand
        {
            get
            {
                return _nextCommand ?? (_nextCommand = new LogOutCommandHandler(() => NextDate(), canExecute));
            }
        }

        private ICommand _enDisCommandDay;
        public ICommand EnDisCommandDay
        {
            get
            {
                if (_enDisCommandDay == null)
                {
                    _enDisCommandDay = new DelegateCommand<string>(EnDisDay, CanExecute);
                }
                return _enDisCommandDay;
            }
        }

    }
}
