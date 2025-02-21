using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Capacity;
using A1QSystem.Model.Formula;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Other;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Grading;
using A1QSystem.Model.Production.Mixing;
using A1QSystem.Model.Production.Model;
using A1QSystem.Model.Products;
using A1QSystem.Model.RawMaterials;
using A1QSystem.Model.Shifts;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using A1QSystem.View.AdminDashboard;
using A1QSystem.View.Orders;
using A1QSystem.View.Production;
using A1QSystem.View.Quoting;
using A1QSystem.ViewModel.Orders;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace A1QSystem.ViewModel.Productions
{
    public class ProductionSchedulerViewModel : ViewModelBase
    {
        private string _day1, _day2, _day3, _day4, _day5, _enDisText1, _enDisText2, _enDisText3, _enDisText4, _enDisText5, _enDisBack1, _enDisBack2, _enDisBack3, _enDisBack4, _enDisBack5, D1,
            D2, D3, D4, D5, _enDis1Visible, _enDis2Visible, _enDis3Visible, _enDis4Visible, _enDis5Visible, _version, userName, state, _selectedSchedulerType;

        private List<RawProductMachine> rawProductMachineList;

        private ListCollectionView _collDay1 = null;
        private ListCollectionView _collDay2 = null;
        private ListCollectionView _collDay3 = null;
        private ListCollectionView _collDay4 = null;
        private ListCollectionView _collDay5 = null;
        private ObservableCollection<RawProductionDetails> _orderProduction = null;
        private ObservableCollection<RawProductionDetails> _orderProduction1 = null;
        private ObservableCollection<RawProductionDetails> _orderProduction2 = null;
        private ObservableCollection<RawProductionDetails> _orderProduction3 = null;
        private ObservableCollection<RawProductionDetails> _orderProduction4 = null;
        private ObservableCollection<RawProductionDetails> _orderProduction5 = null;
        private ObservableCollection<RawProductionDetails> _orderProductionList = null;
        public List<Shift> ShiftDetails { get; set; }
        
        private int _index, _tabSelectedIndex1, _tabSelectedIndex2, _tabSelectedIndex3, _tabSelectedIndex4, _tabSelectedIndex5, _tabAreaSelectedIndex, _tabMachineSelectedIndex1,
            _tabMachineSelectedIndex2, _tabMachineSelectedIndex3, _tabMachineSelectedIndex4, _tabMachineSelectedIndex5, _tabMixingSelectedIndex1, _tabMixingSelectedIndex2, _tabMixingSelectedIndex3, 
            _tabMixingSelectedIndex4, _tabMixingSelectedIndex5, _tabMixingShiftSelectedIndex1, _tabMixingShiftSelectedIndex2, _tabMixingShiftSelectedIndex3, _tabMixingShiftSelectedIndex4, _tabMixingShiftSelectedIndex5;

        private ObservableCollection<MixingProductionDetails> _mixingOrders1, _mixingOrders2, _mixingOrders3, _mixingOrders4, _mixingOrders5, _mixingOrders;

        private bool canExecute, _nextEnabled;
        private DateTime CurrentDate;
        public int TotOrdersPending { get; set; }
        private ObservableCollection<ProductionTimeTable> timeTableDates;
        private List<UserPrivilages> privilages;
        private ChildWindowView LoadingScreen;
        public Dispatcher UIDispatcher { get; set; }
        public ProductionSchedularNotifier productionSchedularNotifier { get; set; }
        public MixingOrdersNotifier mixingOrdersNotifier { get; set; }
        public ProductionTimeTableNotifier productionTimeTableNotifier { get; set; }
        private List<FormulaOptions> formulaOptions;
        private List<MetaData> metaData;
        private ICommand _enDis1, _enDis2, _enDis3, _enDis4, _enDis5, navHomeCommand, _ordersCommand, _prevCommand, _nextCommand, _productionMaintenanceCommand, _adminDashboardCommand,
            _printMixingPendingCommand, _printGradingPendingCommand, _viewMixingScheduleCommand, _printMixingUnCompletedCommand;

        public ProductionSchedulerViewModel(string uName, string uState, List<UserPrivilages> uPriv, Dispatcher uidispatcher, List<MetaData> md)
        {            
            this.UIDispatcher = uidispatcher;            

            userName = uName;
            state = uState;
            privilages = uPriv;
            metaData = md;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
            TabAreaSelectedIndex = 0;
            TabSelectedIndex1 = 0;
            TabSelectedIndex2 = 0;
            TabSelectedIndex3 = 0;
            TabSelectedIndex4 = 0;
            TabSelectedIndex5 = 0;

            TabMachineSelectedIndex1 = 0;
            TabMachineSelectedIndex2 = 0;
            TabMachineSelectedIndex3 = 0;
            TabMachineSelectedIndex4 = 0;
            TabMachineSelectedIndex5 = 0;

            EnDisBack1 = "#eae6e6";
            EnDisBack2 = "#eae6e6";
            EnDisBack3 = "#eae6e6";
            EnDisBack4 = "#eae6e6";
            EnDisBack5 = "#eae6e6";

            EnDis1Visible = "Collapsed";
            EnDis2Visible = "Collapsed";
            EnDis3Visible = "Collapsed";
            EnDis4Visible = "Collapsed";
            EnDis5Visible = "Collapsed";

            NextEnabled = true;
            SelectedSchedulerType = "Select";

            rawProductMachineList = new List<RawProductMachine>();
            rawProductMachineList = DBAccess.GetAllRawProductMachines();

            LoadFormulaOptions();

            LoadGradingData();            

            canExecute = true;                    
        }

        private void LoadGradingData()
        {
            BackgroundWorker worker = new BackgroundWorker();
            LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Loading");
            int curShift = 0;
            CurrentDate = NTPServer.GetNetworkTime();
            DateTime currDate = CurrentDate;

            worker.DoWork += (_, __) =>
            {
                ShiftDetails = DBAccess.GetAllShifts();

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

                this.productionSchedularNotifier = new ProductionSchedularNotifier();
                this.productionTimeTableNotifier = new ProductionTimeTableNotifier();

                this.productionSchedularNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage);
                ObservableCollection<RawProductionDetails> opd = this.productionSchedularNotifier.RegisterDependency();

                this.productionTimeTableNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage2);
                ObservableCollection<ProductionTimeTable> ptt = this.productionTimeTableNotifier.RegisterDependency();

                OrderProduction = new ObservableCollection<RawProductionDetails>();
                OrderProduction = opd;

                _index = 0;

                this.LoadGrading(opd);
            };

            worker.RunWorkerCompleted += delegate (object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();

            };
            worker.RunWorkerAsync();
        }


        private void LoadMixingData()
        {
            //BackgroundWorker worker = new BackgroundWorker();
            //LoadingScreen = new ChildWindowView();
            //LoadingScreen.ShowWaitingScreen("Loading");
           
            //worker.DoWork += (_, __) =>
            //{             
                this.productionTimeTableNotifier = new ProductionTimeTableNotifier();
                                
                this.productionTimeTableNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage2);
                ObservableCollection<ProductionTimeTable> ptt = this.productionTimeTableNotifier.RegisterDependency();

                MixingOrders = new ObservableCollection<MixingProductionDetails>();
                this.mixingOrdersNotifier = new MixingOrdersNotifier();
                this.mixingOrdersNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_MixingMessage);
                ObservableCollection<MixingProductionDetails> mo = this.mixingOrdersNotifier.RegisterDependency();

                _index = 0;

                foreach (var item in mo)
                {
                    MixingOrders.Add((MixingProductionDetails)item.Clone());
                }

                _index = 0;
                this.LoadMixing(mo);
                                                
            //};

            //worker.RunWorkerCompleted += delegate (object s, RunWorkerCompletedEventArgs args)
            //{
            //    LoadingScreen.CloseWaitingScreen();

            //};
            //worker.RunWorkerAsync();
        }

        private void LoadFormulaOptions()
        {
            formulaOptions = new List<FormulaOptions>();
            formulaOptions = DBAccess.GetFormulaOptions();
        }

        private void PrintMixingPending()
        {           

            List<MixingOrder> mo = new List<MixingOrder>();
            mo = DBAccess.GetCurrentMixingList();

            if (mo.Count > 0)
            {
                if (Msg.Show("Would you like to print the Mixing Pending Report?", "Printing Mixing Pending", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    Exception exception = null;
                    BackgroundWorker worker = new BackgroundWorker();
                    ChildWindowView LoadingScreen = new ChildWindowView();
                    LoadingScreen.ShowWaitingScreen("Printing");

                    worker.DoWork += (_, __) =>
                    {
                        GradingCompletedMixingStatusPDF pdf = new GradingCompletedMixingStatusPDF(mo);
                        pdf.CreatePDF();
                    };

                    worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                    {
                        LoadingScreen.CloseWaitingScreen();
                        if (exception != null)
                        {
                            Msg.Show("A problem has occured while prining. Please try again later." + System.Environment.NewLine + exception, "Printing Error", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                        }
                    };
                    worker.RunWorkerAsync();
                }
            }
            else
            {
                Msg.Show("There are no items pending in mixing" , "No Items found", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
            }           
        }

        private void PrintGradingPending()
        {   
            List<GradingPending> gradingPendingList = DBAccess.GetGradingPendingList();
            if (gradingPendingList.Count > 0)
            {
                if (Msg.Show("Would you like to print the Grading Pending Report?", "Printing Grading Pending", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    Exception exception = null;
                    BackgroundWorker worker = new BackgroundWorker();
                    ChildWindowView LoadingScreen = new ChildWindowView();
                    LoadingScreen.ShowWaitingScreen("Printing");

                    worker.DoWork += (_, __) =>
                    {
                        GradingPendingPDF pdf = new GradingPendingPDF(gradingPendingList);
                        pdf.CreatePDF();
                    };

                    worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                    {
                        LoadingScreen.CloseWaitingScreen();
                        if (exception != null)
                        {
                            Msg.Show("A problem has occured while prining. Please try again later." + System.Environment.NewLine + exception, "Printing Error", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                        }
                    };
                    worker.RunWorkerAsync();
                }
            }
            else
            {
                Msg.Show("There are no items pending in grading", "No Items found", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
            }
        }

        private void OpenMixingSchedule()
        {
            var childWindow = new ChildWindowView();
            childWindow.ShowMixingWeeklySchedule();
        }

        private void PrintMixingUnfinishedOrders()
        {
           var childWindow = new ChildWindowView();
           childWindow.ShowMixingUnfinishedList();            
        }
        
        private void LoadGrading(ObservableCollection<RawProductionDetails> psd)
        {
            this.UIDispatcher.BeginInvoke((Action)delegate ()
            {
                if (psd != null)
                {
                    D1 = string.Empty;
                    D2 = string.Empty;
                    D3 = string.Empty;
                    D4 = string.Empty;
                    D5 = string.Empty;
                    int curShift = 0;
                    DateTime currDate = DateTime.Now.Date;
                    DateTime date = currDate;

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

                    timeTableDates = new ObservableCollection<ProductionTimeTable>();
                    timeTableDates = DBAccess.GetAllTimeTableDatesForObservableColl(1, CurrentDate);

                    //Make sure that the dates are sorted
                    psd = new ObservableCollection<RawProductionDetails>(psd.OrderBy(i => i.OrderRequiredDate).ThenBy(i => i.MixingDate).ThenByDescending(t => t.MixingShift));


                    foreach (var item in psd)
                    {
                        bool has = formulaOptions.Any(x => x.RawProduct.RawProductID == item.RawProduct.RawProductID);
                        if (has)
                        {
                            item.ConvertEnable = false;
                        }
                        else
                        {
                            item.ConvertEnable = true;
                        }

                        //Disable CONVERT button for REDMILL
                        //if (item.RawProduct.RawProductID == 95 || item.RawProduct.RawProductID == 97 || item.RawProduct.RawProductID == 98 || item.RawProduct.RawProductID == 99 || item.RawProduct.RawProductID == 100)
                        //{
                        //    item.ConvertEnable = false;
                        //}
                    }

                    foreach (var item in timeTableDates)
                    {
                        for (int i = 1; i < 4; i++)
                        {
                            bool skip = false;

                            if (date == item.ProductionDate.Date && i < curShift)
                            {
                                var itemToRemove = psd.Where(x => Convert.ToDateTime(x.ProductionDate).Date == date && x.Shift == i).ToList();
                                foreach (var items in itemToRemove)
                                {
                                    psd.Remove(items);
                                }
                                skip = true;
                            }

                            bool exist = psd.Any(c => Convert.ToDateTime(c.ProductionDate) == item.ProductionDate && c.Shift == i && c.RawProductMachine != null && c.RawProductMachine.GradingMachineID == item.MachineID);
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

                                //for (int c = 0; c < psd.Count; c++)
                                //{
                                //    if (psd[c].ProdTimeTableID == item.ID && psd[c].Shift == i)
                                //    {
                                //        psd[c].ProdTimeTableID = item.ID;
                                //        psd[c].ShiftName = GetShiftNameByID(i);
                                //        psd[c].ShiftBtnBackColour = btnCol;
                                //        psd[c].ShiftText = shiftText;
                                //        psd[c].SalesOrderVisibility = "Collapsed";
                                //    }
                                //}

                                //psd.Add(new RawProductionDetails()
                                //{
                                //    RawProduct = new RawProduct() { RawProductID = 0, SalesID = 0, RawProductCode = string.Empty, Description = string.Empty, RawProductType = string.Empty },
                                //    Customer = new Customer() { CompanyName = string.Empty, CustomerId = 0 },
                                //    GradingSchedulingID = 0,
                                //    ProdTimeTableID = item.ID,
                                //    SalesOrderId = 0,
                                //    RawProDetailsID = 0,
                                //    BlockLogQty = 0,
                                //    ProductionDate = item.ProductionDate.ToString("dd/MM/yyyy"),
                                //    Shift = shift,
                                //    ShiftName = GetShiftNameByID(i),
                                //    OriginType = string.Empty,
                                //    SalesOrder = string.Empty,
                                //    FreightDescription = string.Empty,
                                //    RequiredDate = string.Empty,
                                //    //MixingDate = string.Empty,
                                //    FreightDateAvailable = false,
                                //    FreightArrTime = string.Empty,
                                //    FreightTimeAvailable = false,
                                //    RowBackgroundColour = string.Empty,
                                //    OrderType = 3,
                                //    Notes = string.Empty,
                                //    ActiveOrder = false,
                                //    PrintCounter = 0,
                                //    IsReqDateVisible = "Visible",
                                //    IsExpanded = false,
                                //    ItemPresenterVivibility = "Collapsed",
                                //    ShiftBtnBackColour = btnCol,
                                //    ShiftText = shiftText,
                                //    ReqDateSelected = false,
                                //    SalesOrderVisibility = "Collapsed",
                                //    RawProductMachine = new RawProductMachine() { GradingMachineID = item.MachineID},
                                //    MachineActive=true
                                //    //OrderRequiredDate = DateTime.Now
                                //});
                            }
                        }
                    }

                    //Make sure that the dates are sorted
                    _orderProductionList = new ObservableCollection<RawProductionDetails>(psd.OrderBy(i => i.OrderRequiredDate).ThenBy(i => i.MixingDate).ThenByDescending(t => t.MixingShift));

                    int noOdItems = timeTableDates.Count();

                    if (timeTableDates.Count != 0 && noOdItems >= _index)
                    {
                        //Grid1
                        //Machine1
                        if (timeTableDates.ElementAtOrDefault(_index) != null)
                        {
                            D1 = timeTableDates[_index].ProductionDate.ToString("dd/MM/yyyy");
                            Day1 = DateTime.Parse(D1).DayOfWeek + " " + Regex.Match(D1, "^[^ ]+").Value;
                            OrderProduction1 = AddToBusinessDates(psd, Convert.ToDateTime(D1));

                            CollDay1 = new ListCollectionView(OrderProduction1);
                            CollDay1.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                            CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                            CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

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

                            TabSelectedIndex1 = 0;
                            TabMachineSelectedIndex1 = 0;
                        }
                        //Machine7
                        else if (timeTableDates.ElementAtOrDefault(_index + 1) != null)
                        {

                            D1 = timeTableDates[_index + 1].ProductionDate.ToString("dd/MM/yyyy");
                            Day1 = DateTime.Parse(D1).DayOfWeek + " " + Regex.Match(D1, "^[^ ]+").Value;
                            OrderProduction1 = AddToBusinessDates(psd, Convert.ToDateTime(D1));

                            CollDay1 = new ListCollectionView(OrderProduction1);
                            CollDay1.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                            CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                            CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

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

                            TabSelectedIndex1 = 0;
                            TabMachineSelectedIndex1 = 0;
                        }
                        else
                        {
                            //Day1 = string.Empty;
                            //OrderProduction1 = null;
                            //CollDay1 = null;

                            //OrderProduction1 = AddToBusinessDates(psd, Convert.ToDateTime(D1));

                            //CollDay1 = new ListCollectionView(OrderProduction1);
                            //CollDay1.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                            //CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                            //CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));
                        }

                        //Grid2
                        //Machine1
                        if (timeTableDates.ElementAtOrDefault(_index + 2) != null)
                        {
                            D2 = timeTableDates[_index + 2].ProductionDate.ToString("dd/MM/yyyy");
                            Day2 = DateTime.Parse(D2).DayOfWeek + " " + Regex.Match(D2, "^[^ ]+").Value;

                            OrderProduction2 = AddToBusinessDates(psd, Convert.ToDateTime(D2));
                            CollDay2 = new ListCollectionView(OrderProduction2);
                            CollDay2.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                            CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                            CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));
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

                            TabSelectedIndex2 = 0;
                            TabMachineSelectedIndex2 = 0;


                        }//Machine7
                        else if (timeTableDates.ElementAtOrDefault(_index + 3) != null)
                        {
                            D2 = timeTableDates[_index + 3].ProductionDate.ToString("dd/MM/yyyy");
                            Day2 = DateTime.Parse(D2).DayOfWeek + " " + Regex.Match(D2, "^[^ ]+").Value;

                            OrderProduction2 = AddToBusinessDates(psd, Convert.ToDateTime(D2));
                            CollDay2 = new ListCollectionView(OrderProduction2);
                            CollDay2.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                            CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                            CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));
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
                            TabSelectedIndex2 = 0;
                            TabMachineSelectedIndex2 = 0;
                        }
                        else
                        {
                            Day2 = string.Empty;
                            OrderProduction2 = null;
                            CollDay2 = null;
                        }

                        //Grid3
                        //Machine1
                        if (timeTableDates.ElementAtOrDefault(_index + 4) != null)
                        {
                            D3 = timeTableDates[_index + 4].ProductionDate.ToString("dd/MM/yyyy");
                            Day3 = DateTime.Parse(D3).DayOfWeek + " " + Regex.Match(D3, "^[^ ]+").Value;
                            OrderProduction3 = AddToBusinessDates(psd, Convert.ToDateTime(D3));
                            CollDay3 = new ListCollectionView(OrderProduction3);
                            CollDay3.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                            CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                            CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

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
                            TabSelectedIndex3 = 0;
                            TabMachineSelectedIndex3 = 0;
                        }//Machine7
                        else if (timeTableDates.ElementAtOrDefault(_index + 5) != null)
                        {
                            D3 = timeTableDates[_index + 5].ProductionDate.ToString("dd/MM/yyyy");
                            Day3 = DateTime.Parse(D3).DayOfWeek + " " + Regex.Match(D3, "^[^ ]+").Value;
                            OrderProduction3 = AddToBusinessDates(psd, Convert.ToDateTime(D3));
                            CollDay3 = new ListCollectionView(OrderProduction3);
                            CollDay3.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                            CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                            CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

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
                            TabSelectedIndex3 = 0;
                            TabMachineSelectedIndex3 = 0;
                        }
                        else
                        {
                            Day3 = string.Empty;
                            OrderProduction3 = null;
                            CollDay3 = null;
                        }

                        //Grid4
                        //Machine1
                        if (timeTableDates.ElementAtOrDefault(_index + 6) != null)
                        {
                            D4 = timeTableDates[_index + 6].ProductionDate.ToString("dd/MM/yyyy");
                            Day4 = DateTime.Parse(D4).DayOfWeek + " " + Regex.Match(D4, "^[^ ]+").Value;
                            OrderProduction4 = AddToBusinessDates(psd, Convert.ToDateTime(D4));
                            CollDay4 = new ListCollectionView(OrderProduction4);
                            CollDay4.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                            CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                            CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

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
                            TabSelectedIndex4 = 0;
                            TabMachineSelectedIndex4 = 0;
                        }//Machine7
                        else if (timeTableDates.ElementAtOrDefault(_index + 7) != null)
                        {
                            D4 = timeTableDates[_index + 7].ProductionDate.ToString("dd/MM/yyyy");
                            Day4 = DateTime.Parse(D4).DayOfWeek + " " + Regex.Match(D4, "^[^ ]+").Value;
                            OrderProduction4 = AddToBusinessDates(psd, Convert.ToDateTime(D4));
                            CollDay4 = new ListCollectionView(OrderProduction4);
                            CollDay4.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                            CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                            CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

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
                            TabSelectedIndex4 = 0;
                            TabMachineSelectedIndex4 = 0;
                        }
                        else
                        {
                            Day4 = string.Empty;
                            OrderProduction4 = null;
                            CollDay4 = null;
                        }
                        //Grid5
                        //Machine1
                        if (timeTableDates.ElementAtOrDefault(_index + 8) != null)
                        {
                            D5 = timeTableDates[_index + 8].ProductionDate.ToString("dd/MM/yyyy");
                            Day5 = DateTime.Parse(D5).DayOfWeek + " " + Regex.Match(D5, "^[^ ]+").Value;
                            OrderProduction5 = AddToBusinessDates(psd, Convert.ToDateTime(D5));
                            CollDay5 = new ListCollectionView(OrderProduction5);
                            CollDay5.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                            CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                            CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

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
                            TabSelectedIndex5 = 0;
                            TabMachineSelectedIndex5 = 0;
                        } //Machine7
                        else if (timeTableDates.ElementAtOrDefault(_index + 9) != null)
                        {
                            D5 = timeTableDates[_index + 9].ProductionDate.ToString("dd/MM/yyyy");
                            Day5 = DateTime.Parse(D5).DayOfWeek + " " + Regex.Match(D5, "^[^ ]+").Value;
                            OrderProduction5 = AddToBusinessDates(psd, Convert.ToDateTime(D5));
                            CollDay5 = new ListCollectionView(OrderProduction5);
                            CollDay5.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                            CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                            CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

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
                            TabSelectedIndex5 = 0;
                            TabMachineSelectedIndex5 = 0;
                        }
                        else
                        {
                            Day5 = string.Empty;
                            OrderProduction5 = null;
                            CollDay5 = null;
                        }
                    }
                    else
                    {
                        //Msg.Show("No orders found ", "Orders Not Found", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                    }
                }




            });
        }

        private void LoadMixing(ObservableCollection<MixingProductionDetails> mo)
        {
            this.UIDispatcher.BeginInvoke((Action)delegate ()
            {
                BackgroundWorker worker = new BackgroundWorker();
                LoadingScreen = new ChildWindowView();
                LoadingScreen.ShowWaitingScreen("Loading");

                worker.DoWork += (_, __) =>
                {

                    if (mo != null)
                {
                    D1 = string.Empty;
                    D2 = string.Empty;
                    D3 = string.Empty;
                    D4 = string.Empty;
                    D5 = string.Empty;
                    int curShift = 0;
                    DateTime currDate = DateTime.Now.Date;
                    DateTime date = currDate;

                    TabMixingSelectedIndex1 = 0;
                    TabMixingSelectedIndex2 = 0;
                    TabMixingSelectedIndex3 = 0;
                    TabMixingSelectedIndex4 = 0;
                    TabMixingSelectedIndex5 = 0;

                    MixingOrders1 = new ObservableCollection<MixingProductionDetails>();
                    MixingOrders2 = new ObservableCollection<MixingProductionDetails>();
                    MixingOrders3 = new ObservableCollection<MixingProductionDetails>();
                    MixingOrders4 = new ObservableCollection<MixingProductionDetails>();
                    MixingOrders5 = new ObservableCollection<MixingProductionDetails>();

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

                    timeTableDates = new ObservableCollection<ProductionTimeTable>();
                    timeTableDates = DBAccess.GetAllTimeTableDatesForObservableColl(CurrentDate, "Mixing");

                    //foreach (var item in timeTableDates)
                    //{
                    //    for (int i = 1; i < 4; i++)
                    //    {
                    //        bool skip = false;

                    //        bool exist = mo.Any(c => Convert.ToDateTime(c.ProductionDate).Date == item.ProductionDate.Date && c.RawProductMachine != null && c.RawProductMachine.MixingMachine.MachineID == item.MachineID);
                    //        if (exist == false && skip == false)
                    //        {
                    //            string btnCol = string.Empty;
                    //            string shiftText = string.Empty;
                    //            int shift = i;
                    //            if (item.IsDayShiftActive == true && i == 1)
                    //            {
                    //                shift = 1;
                    //                shiftText = "Enable";
                    //                btnCol = "#FFCC3700";
                    //            }
                    //            else
                    //            {
                    //                shiftText = "Disable";
                    //                btnCol = "Green";
                    //            }

                    //            if (item.IsEveningShiftActive == true && i == 2)
                    //            {
                    //                shift = 2;
                    //                shiftText = "Enable";
                    //                btnCol = "#FFCC3700";
                    //            }
                    //            else
                    //            {
                    //                shiftText = "Disable";
                    //                btnCol = "Green";
                    //            }

                    //            if (item.IsNightShiftActive == true && i == 3)
                    //            {
                    //                shift = 3;
                    //                shiftText = "Enable";
                    //                btnCol = "#FFCC3700";
                    //            }
                    //            else
                    //            {
                    //                shiftText = "Disable";
                    //                btnCol = "Green";
                    //            }
                    //        }
                    //    }
                    //}

                    //Make sure that the dates are sorted
                    MixingOrders = new ObservableCollection<MixingProductionDetails>(mo.OrderBy(i => i.MixingDate.Date).ThenBy(i => i.OrderType));
                    int noOdItems = timeTableDates.Count();

                    if (timeTableDates.Count != 0 && noOdItems >= _index)
                    {
                        //Grid1
                        if (timeTableDates.ElementAtOrDefault(_index) != null)
                        {
                            TabMixingSelectedIndex1 = 0;
                            TabMixingShiftSelectedIndex1 = 0;

                            D1 = timeTableDates[_index].ProductionDate.ToString("dd/MM/yyyy");
                            Day1 = DateTime.Parse(D1).DayOfWeek + " " + Regex.Match(D1, "^[^ ]+").Value;

                            MixingOrders1 = ConstructMixingOrders(MixingOrders, Convert.ToDateTime(D1), TabMixingSelectedIndex1, TabMixingShiftSelectedIndex1);

                            //if (timeTableDates[_index].IsDayShiftActive == true || timeTableDates[_index].IsEveningShiftActive == true || timeTableDates[_index].IsNightShiftActive == true)
                            //{
                            //    EnDisText1 = "Disable";
                            //    EnDisBack1 = "Green";
                            //}
                            //else
                            //{
                            //    EnDisText1 = "Enable";
                            //    EnDisBack1 = "#FFCC3700";
                            //}
                        }


                        //Grid2
                        if (timeTableDates.ElementAtOrDefault(_index + 2) != null)
                        {
                            TabMixingSelectedIndex2 = 0;
                            TabMixingShiftSelectedIndex2 = 0;

                            D2 = timeTableDates[_index + 2].ProductionDate.ToString("dd/MM/yyyy");
                            Day2 = DateTime.Parse(D2).DayOfWeek + " " + Regex.Match(D2, "^[^ ]+").Value;

                            MixingOrders2 = ConstructMixingOrders(MixingOrders, Convert.ToDateTime(D2), TabMixingSelectedIndex2, TabMixingShiftSelectedIndex2);

                            //if (timeTableDates[_index + 2].IsDayShiftActive == true || timeTableDates[_index + 2].IsEveningShiftActive == true || timeTableDates[_index + 2].IsNightShiftActive == true)
                            //{
                            //    EnDisText2 = "Disable";
                            //    EnDisBack2 = "Green";
                            //}
                            //else
                            //{
                            //    EnDisText2 = "Enable";
                            //    EnDisBack2 = "#FFCC3700";
                            //}
                        }


                        //Grid3
                        if (timeTableDates.ElementAtOrDefault(_index + 4) != null)
                        {
                            TabMixingSelectedIndex3 = 0;
                            TabMixingShiftSelectedIndex3 = 0;

                            D3 = timeTableDates[_index + 4].ProductionDate.ToString("dd/MM/yyyy");
                            Day3 = DateTime.Parse(D3).DayOfWeek + " " + Regex.Match(D3, "^[^ ]+").Value;

                            MixingOrders3 = ConstructMixingOrders(MixingOrders, Convert.ToDateTime(D3), TabMixingSelectedIndex3, TabMixingShiftSelectedIndex3);

                            //if (timeTableDates[_index + 4].IsDayShiftActive == true || timeTableDates[_index + 4].IsEveningShiftActive == true || timeTableDates[_index + 4].IsNightShiftActive == true)
                            //{
                            //    EnDisText3 = "Disable";
                            //    EnDisBack3 = "Green";
                            //}
                            //else
                            //{
                            //    EnDisText3 = "Enable";
                            //    EnDisBack3 = "#FFCC3700";
                            //}
                        }

                        //Grid4
                        if (timeTableDates.ElementAtOrDefault(_index + 6) != null)
                        {
                            TabMixingSelectedIndex4 = 0;
                            TabMixingShiftSelectedIndex4 = 0;

                            D4 = timeTableDates[_index + 6].ProductionDate.ToString("dd/MM/yyyy");
                            Day4 = DateTime.Parse(D4).DayOfWeek + " " + Regex.Match(D4, "^[^ ]+").Value;

                            MixingOrders4 = ConstructMixingOrders(MixingOrders, Convert.ToDateTime(D4), TabMixingSelectedIndex4, TabMixingShiftSelectedIndex4);

                            //if (timeTableDates[_index + 6].IsDayShiftActive == true || timeTableDates[_index + 6].IsEveningShiftActive == true || timeTableDates[_index + 6].IsNightShiftActive == true)
                            //{
                            //    EnDisText4 = "Disable";
                            //    EnDisBack4 = "Green";
                            //}
                            //else
                            //{
                            //    EnDisText4 = "Enable";
                            //    EnDisBack4 = "#FFCC3700";
                            //}
                        }

                        //Grid5
                        if (timeTableDates.ElementAtOrDefault(_index + 8) != null)
                        {
                            TabMixingSelectedIndex5 = 0;
                            TabMixingShiftSelectedIndex5 = 0;

                            D5 = timeTableDates[_index + 8].ProductionDate.ToString("dd/MM/yyyy");
                            Day5 = DateTime.Parse(D5).DayOfWeek + " " + Regex.Match(D5, "^[^ ]+").Value;

                            MixingOrders5 = ConstructMixingOrders(MixingOrders, Convert.ToDateTime(D5), TabMixingSelectedIndex5, TabMixingShiftSelectedIndex5);

                            //if (timeTableDates[_index + 8].IsDayShiftActive == true || timeTableDates[_index + 8].IsEveningShiftActive == true || timeTableDates[_index + 8].IsNightShiftActive == true)
                            //{
                            //    EnDisText5 = "Disable";
                            //    EnDisBack5 = "Green";
                            //}
                            //else
                            //{
                            //    EnDisText5 = "Enable";
                            //    EnDisBack5 = "#FFCC3700";
                            //}
                        }
                    }
                }


            };

            worker.RunWorkerCompleted += delegate (object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();

            };
            worker.RunWorkerAsync();



        });
        }

        private ObservableCollection<MixingProductionDetails> ConstructMixingOrders(ObservableCollection<MixingProductionDetails> mo, DateTime date, int tabIndex, int shift)
        {
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            ObservableCollection<MixingProductionDetails> constructedMixingOrders = new ObservableCollection<MixingProductionDetails>();
            int machineId = 0;
            string shiftName = string.Empty;

            if (tabIndex == 1)
            {
                machineId = 2;
            }
            else if (tabIndex == 2)
            {
                machineId = 3;
            }
            else if (tabIndex == 3)
            {
                machineId = 18;
            }

            if (shift == 0)
            {
                shiftName = "All";
            }
            else if (shift == 1)
            {
                shiftName = "Morning";
            }
            else if (shift == 2)
            {
                shiftName = "Arvo";
            }
            else if (shift == 3)
            {
                shiftName = "Night";
            }

            foreach (var item in mo)
            {
                if (item.BlockLogQty > 0 && (machineId == 0 || item.RawProductMachine.MixingMachineID == machineId) &&
                   (Convert.ToDateTime(item.MixingDate).Date == date || (Convert.ToDateTime(item.MixingDate).Date == date && date == Convert.ToDateTime(CurrentDate))) &&
                    (shiftName.Equals("All") || item.MixingShift.Equals(shiftName)))
                {
                    string shiftText = string.Empty;
                    string btnCol = string.Empty;

                    foreach (var itemTTD in timeTableDates)
                    {
                        if (itemTTD.ID == item.ProdTimeTableID)
                        {
                            if (itemTTD.IsDayShiftActive == true && item.RawProductMachine.MixingMachineID == 1)
                            {
                                shiftText = "Disable";
                                btnCol = "Green";
                                break;
                            }
                            else if (itemTTD.IsEveningShiftActive == true && item.RawProductMachine.MixingMachineID == 2)
                            {
                                shiftText = "Disable";
                                btnCol = "Green";
                                break;
                            }
                            else if (itemTTD.IsNightShiftActive == true && item.RawProductMachine.MixingMachineID == 3)
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

                    MixingProductionDetails mpd = new MixingProductionDetails();
                    mpd.ProductVisibility = item.ProductVisibility;
                    //mpd.Product = new Product()
                    //{
                    //    ProductID = item.Product.ProductID,
                    //    ProductDescription = item.Product.ProductDescription
                    //};
                    mpd.RawProduct = new RawProduct()
                    {
                        RawProductID = item.RawProduct.RawProductID,
                        RawProductCode = item.RawProduct.RawProductCode,
                        RawProductName = item.RawProduct.RawProductName,
                        Description = item.RawProduct.Description,
                        RawProductType = item.RawProduct.RawProductType
                    };
                    mpd.Customer = new Customer()
                    {
                        CompanyName = item.Customer.CompanyName
                    };
                    mpd.RawProductMachine = new RawProductMachine() { MixingMachineID = item.RawProductMachine.MixingMachineID };
                    mpd.ProductionDate = item.ProductionDate;
                    mpd.MixingCurrentCapacityID = item.MixingCurrentCapacityID;
                    mpd.ProdTimeTableID = item.ProdTimeTableID;
                    mpd.MixingTimeTableID = item.MixingTimeTableID;
                    mpd.SalesOrder = item.SalesOrder;
                    mpd.SalesOrderId = item.SalesOrderId;
                    mpd.RequiredDate = item.RequiredDate;
                    mpd.MixingShift = item.MixingShift;
                    mpd.MixingDate = item.MixingDate;
                    mpd.MixingShift = item.MixingShift;
                    mpd.Comment = item.Comment;
                    mpd.BlockLogQty = Math.Ceiling(item.BlockLogQty);
                    mpd.RawProDetailsID = item.RawProDetailsID;
                    mpd.MixingFormula = item.MixingFormula;
                    mpd.IsReturnEnabled = (item.MixingOnlyList.Select(x => x.RawProductID).Contains(item.RawProduct.RawProductID)) ? false : true;
                    mpd.OrderType = item.OrderType;
                    mpd.RowBackgroundColour = item.RowBackgroundColour;
                    mpd.Rank = item.Rank;
                    mpd.MixingActive = item.ActiveOrder;
                    mpd.IsNotesVisible = string.IsNullOrWhiteSpace(item.Comment) ? "Collapsed" : "Visibile";
                    mpd.IsReqDateVisible = item.ReqDateSelected == true ? "Visible" : "Collapsed";
                    mpd.CustomerNameVisibility = string.IsNullOrWhiteSpace(item.Customer.CompanyName) ? "Collapsed" : "Visible";
                    mpd.RawBackgroundColour = bdg.ConvertDayToColour(Convert.ToDateTime(item.MixingDate));
                    mpd.BottomRowVisibility = "Visible";
                    //mpd.BlockLogStockStr = item.Product.ProductID == 0 ? "[Block/Log Stock]" : "";
                    mpd.BlockLogStockStr = "[Block/Log Stock]";
                    constructedMixingOrders.Add(mpd);
                }
            }

            return constructedMixingOrders;
        }

        private ObservableCollection<RawProductionDetails> AddToBusinessDates(ObservableCollection<RawProductionDetails> prodSchDetails, DateTime date)
        {
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            OrderProduction = new ObservableCollection<RawProductionDetails>();
            prodSchDetails = new ObservableCollection<RawProductionDetails>(prodSchDetails.OrderBy(i => i.OrderType).ThenBy(t => t.OrderRequiredDate).ThenBy(i => i.MixingDate).ThenByDescending(t => t.MixingShift));           
          
                foreach (var item in prodSchDetails)
                {
                    
                    if (Convert.ToDateTime(item.ProductionDate) == date || (Convert.ToDateTime(item.ProductionDate) == date && date == Convert.ToDateTime(CurrentDate)))
                    {
                        string shiftText = string.Empty;
                        string btnCol = string.Empty;                        

                        foreach (var itemTTD in timeTableDates)
                        {
                            if(itemTTD.ID == item.ProdTimeTableID)
                            {                              

                                if(itemTTD.IsDayShiftActive == true && item.Shift == 1)
                                {
                                    shiftText = "Disable";
                                    btnCol = "Green";
                                    break;
                                }
                                else if (itemTTD.IsEveningShiftActive == true && item.Shift == 2)
                                {
                                    shiftText = "Disable";
                                    btnCol = "Green";
                                    break;
                                }
                                else if (itemTTD.IsNightShiftActive == true && item.Shift == 3)
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

                        
                        OrderProduction.Add(new RawProductionDetails()
                        {
                            RawProduct = new RawProduct()
                            {
                                RawProductID = item.RawProduct.RawProductID,
                                SalesID = item.RawProduct.SalesID,
                                RawProductCode = item.RawProduct.RawProductCode,
                                Description = item.RawProduct.Description,
                                RawProductType = item.RawProduct.RawProductType
                            },
                            Customer = new Customer()
                            {
                                CompanyName = item.Customer.CompanyName,
                                CustomerId = item.Customer.CustomerId
                            },
                            GradingSchedulingID = item.GradingSchedulingID,
                            ProdTimeTableID = item.ProdTimeTableID,
                            SalesOrderId = item.SalesOrderId,
                            RawProDetailsID = item.RawProDetailsID,
                            BlockLogQty = Math.Ceiling(item.BlockLogQty),
                            ProductionDate = item.ProductionDate,
                            Shift = item.Shift,
                            ShiftName = GetShiftNameByID(item.Shift),
                            OriginType = item.OriginType,
                            SalesOrder = item.SalesOrder,
                            FreightDescription = item.FreightDescription,
                            RequiredDate = item.OrderRequiredDate.ToString("dd/MM/yyyy") + " " + String.Format("{0:HH:mm:ss}", item.FreightArrTime),
                            MixingDate = item.MixingDate ,
                            MixingShift = item.MixingShift,
                            OrderRequiredDate = item.OrderRequiredDate,
                            FreightDateAvailable = item.FreightDateAvailable,
                            FreightArrTime = String.Format("{0:HH:mm:ss}", item.FreightArrTime),
                            FreightTimeAvailable = item.FreightTimeAvailable,
                            RowBackgroundColour = item.RowBackgroundColour,
                            OrderType = item.OrderType,
                            Notes = item.Notes,
                            ActiveOrder = item.ActiveOrder,
                            PrintCounter = item.PrintCounter,
                            IsReqDateVisible = item.ReqDateSelected == true ? "Visible" : "Collapsed",
                            IsExpanded = item.RawProDetailsID == 0 ? false : true,
                            ItemPresenterVivibility = item.RawProDetailsID == 0 ? "Collapsed" : "Visible",
                            ShiftText = shiftText,
                            ShiftBtnBackColour = btnCol,
                            SalesOrderVisibility = item.SalesOrderVisibility,
                            ItemBackgroundColour = bdg.ConvertDayToColour(Convert.ToDateTime(item.OrderRequiredDate)),
                            ConvertEnable = item.ConvertEnable,
                            RawProductMachine = new RawProductMachine() { GradingMachineID = item.RawProductMachine != null ? item.RawProductMachine.GradingMachineID : 0 },
                            EnDisVisibilty="Collapsed",
                            MachineActive=true
                        });
                    }                  
                }

                OrderProduction = new ObservableCollection<RawProductionDetails>(OrderProduction.OrderBy(i => i.OrderRequiredDate).ThenBy(i => i.MixingDate).ThenByDescending(t => t.MixingShift));
            
            return OrderProduction;
        }

        private ObservableCollection<RawProductionDetails> AddToTabs(ObservableCollection<RawProductionDetails> prodSchDetails, int TabSelectedIndex,int machineId)
        {       
            
            OrderProduction = new ObservableCollection<RawProductionDetails>();

            if (TabSelectedIndex == 0 && machineId == 0)
            {
                //var itemToRemove = prodSchDetails.Where(x => x.RawProduct.RawProductID==0).ToList();

                prodSchDetails = new ObservableCollection<RawProductionDetails>(prodSchDetails.OrderByDescending(i => i.RawProduct.RawProductID));

                foreach (var item in prodSchDetails)
                {                    
                    if (item.RawProduct.RawProductID != 0)
                    {
                        OrderProduction.Add(item);
                    }
                    else
                    {
                        bool has = OrderProduction.Any(x=>x.Shift==item.Shift);
                        if (has == false)
                        {
                            item.MachineActive = true;
                            OrderProduction.Add(item);
                        }
                    }                    
                }                
            }
            else if (TabSelectedIndex == 0 && machineId != 0)
            {
                foreach (var item in prodSchDetails)
                {                   

                    if(item.RawProductMachine.GradingMachineID == 0 && item.RawProduct.RawProductID==0)
                    {                        
                        OrderProduction.Add(ProcessRawProductDetails(item, machineId));
                    }
                    else  if(item.RawProductMachine.GradingMachineID == machineId)
                    {                        
                        OrderProduction.Add(ProcessRawProductDetails(item, machineId));
                    }
                }
            }
            else if(TabSelectedIndex != 0 && machineId == 0)
            {
                foreach (var item in prodSchDetails)
                {
                    if (item.Shift == TabSelectedIndex && item.RawProduct.RawProductID != 0)
                    {                        
                        OrderProduction.Add(item);
                    }                   
                }
            }
            else
            {
                foreach (var item in prodSchDetails)
                {
                    if (item.Shift == TabSelectedIndex && item.RawProductMachine != null && item.RawProductMachine.GradingMachineID == machineId)
                    {                       
                        OrderProduction.Add(ProcessRawProductDetails(item, machineId));
                    }
                    else if (TabSelectedIndex == 0 && item.RawProductMachine != null && item.RawProductMachine.GradingMachineID == machineId)
                    {                        
                        OrderProduction.Add(item);                    }
                }
            }
            OrderProduction = new ObservableCollection<RawProductionDetails>(OrderProduction.OrderBy(i => i.OrderType).ThenBy(t => t.OrderRequiredDate).ThenBy(t => t.MixingDate).ThenByDescending(t => t.MixingShift));     

            return OrderProduction;
        }

        private RawProductionDetails ProcessRawProductDetails(RawProductionDetails item, int machineId)
        {
            RawProductionDetails rpd = new RawProductionDetails();
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();

            string shiftText = string.Empty;
            string btnCol = string.Empty;

            foreach (var itemTTD in timeTableDates)
            {
                if (itemTTD.ID == item.ProdTimeTableID)
                {
                    if (itemTTD.IsDayShiftActive == true && item.Shift == 1)
                    {
                        shiftText = "Disable";
                        btnCol = "Green";
                        break;
                    }
                    else if (itemTTD.IsEveningShiftActive == true && item.Shift == 2)
                    {
                        shiftText = "Disable";
                        btnCol = "Green";
                        break;
                    }
                    else if (itemTTD.IsNightShiftActive == true && item.Shift == 3)
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



            
                rpd.RawProduct = new RawProduct()
                {
                    RawProductID = item.RawProduct.RawProductID,
                    SalesID = item.RawProduct.SalesID,
                    RawProductCode = item.RawProduct.RawProductCode,
                    Description = item.RawProduct.Description,
                    RawProductType = item.RawProduct.RawProductType
                };
                rpd.Customer = new Customer()
                {
                    CompanyName = item.Customer.CompanyName,
                    CustomerId = item.Customer.CustomerId
                };
                rpd.GradingSchedulingID = item.GradingSchedulingID;
                rpd.ProdTimeTableID = item.ProdTimeTableID;
                rpd.SalesOrderId = item.SalesOrderId;
                rpd.RawProDetailsID = item.RawProDetailsID;
                rpd.BlockLogQty = Math.Ceiling(item.BlockLogQty);
                rpd.ProductionDate = item.ProductionDate;
                rpd.Shift = item.Shift;
                rpd.ShiftName = GetShiftNameByID(item.Shift);
                rpd.OriginType = item.OriginType;
                rpd.SalesOrder = item.SalesOrder;
                rpd.FreightDescription = item.FreightDescription;
                rpd.RequiredDate = item.RequiredDate + " " + String.Format("{0:HH:mm:ss}", item.FreightArrTime);
                rpd.MixingDate = item.MixingDate ;
                rpd.MixingShift = item.MixingShift;
                rpd.FreightDateAvailable = item.FreightDateAvailable;
                rpd.FreightArrTime = String.Format("{0:HH:mm:ss}", item.FreightArrTime);
                rpd.FreightTimeAvailable = item.FreightTimeAvailable;
                rpd.RowBackgroundColour = item.RowBackgroundColour;
                rpd.OrderType = item.OrderType;
                rpd.Notes = item.Notes;
                rpd.ActiveOrder = item.ActiveOrder;
                rpd.PrintCounter = item.PrintCounter;
                rpd.IsReqDateVisible = item.IsReqDateVisible;
                rpd.IsExpanded = item.RawProDetailsID == 0 ? false : true;
                rpd.ItemPresenterVivibility = item.RawProDetailsID == 0 ? "Collapsed" : "Visible";
                rpd.ShiftText = shiftText;
                rpd.ShiftBtnBackColour = btnCol;
                rpd.SalesOrderVisibility = item.SalesOrderVisibility;
                rpd.ItemBackgroundColour = bdg.ConvertDayToColour(Convert.ToDateTime(item.OrderRequiredDate));
                rpd.ConvertEnable = item.ConvertEnable;
                rpd.RawProductMachine = new RawProductMachine() { GradingMachineID = machineId };
                rpd.MachineActive = true;

            

            return rpd;

        }

        void notifier_NewMessage(object sender, SqlNotificationEventArgs e)
        {
            //OrderProduction = this.productionSchedularNotifier.RegisterDependency();

            this.LoadGrading(this.productionSchedularNotifier.RegisterDependency());
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
                    //Console.WriteLine("Changed");

                    ObservableCollection<RawProductionDetails> rmdList = DBAccess.GetProductionSchedulerData();
                    LoadGrading(rmdList);
                }
            });
        }

        private decimal BlockLogCalculator(decimal qty, decimal yield)
        {
            decimal res = 0;

            res = Math.Ceiling(qty / yield);

            return res;
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

        public List<BusinessDays> SubtractBusinessDays(DateTime current, int days)
        {
            List<BusinessDays> WorkingDays = new List<BusinessDays>();
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            for (int x = 0; x < days; x++)
            {
                BusinessDays bs = new BusinessDays();

                bs.Day = bdg.AddBusinessDays(current, x).ToString("dd/MM/yyyy");
                bs.DayName = bdg.AddBusinessDays(current, x).DayOfWeek.ToString();
                WorkingDays.Add(bs);
            }

            return WorkingDays;
        }

        private void PrevDate()
        {
            List<SystemParameters> systemParameters= DBAccess.GetAllSystemParametersByValue(true);
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

                if (TabAreaSelectedIndex == 0)
                {
                    LoadGrading(_orderProductionList);
                }
                else if (TabAreaSelectedIndex == 1)
                {
                    LoadMixing(_mixingOrders);
                }
                else if (TabAreaSelectedIndex == 2)
                {

                }
                else if (TabAreaSelectedIndex == 3)
                {
                    //LoadSlittingData(_slittingOrderList);
                }
                else if (TabAreaSelectedIndex == 3)
                {
                    //LoadPeelingData(_peelingOrderList);
                }
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
                         int sp1 = DBAccess.UpdateSystemParameter("AddingNewDates", true);
                         if (sp1 > 0)
                         {
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
                         }                         
                     }
                     if (TabAreaSelectedIndex == 0)
                     {
                         LoadGrading(_orderProductionList);
                     }
                     else if (TabAreaSelectedIndex == 1)
                     {
                         LoadMixing(_mixingOrders);
                     }
                     else if (TabAreaSelectedIndex == 2)
                     {

                     }
                     else if (TabAreaSelectedIndex == 3)
                     {
                         //LoadSlittingData(_slittingOrderList);
                     }
                     else if (TabAreaSelectedIndex == 4)
                     {
                         //LoadPeelingData(_peelingOrderList);
                     }
                 };

                worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    LoadingScreen.CloseWaitingScreen();                                       
                };
                worker.RunWorkerAsync();
             }
        }

        private void EnDisDay1()
        {
            Tuple<int,string> ele = GetMachineInfo(TabMachineSelectedIndex1);

            if (ele.Item1 == 0)
            {
                Msg.Show("Please select Red Mill or Blue Mill to enable/disable shifts", "Select Mill", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                List<int> machineIds = new List<int>();
                machineIds.Add(ele.Item1);
                List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
                bool has = systemParameters.Any(x => x.Value == true);
                if (has == true)
                {
                    Msg.Show("System is performing some updates. Please try again in few minutes ", "System Busy", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                }
                else
                {
                    int sp1 = DBAccess.UpdateSystemParameter("DisableEnableDates", true);
                    if (sp1 > 0)
                    {
                        GradingManager gm = new GradingManager();

                        if (EnDisText1 == "Enable")
                        {
                            if (Msg.Show("Are you sure, you want to enable all shifts for " + ele.Item2 + " on " + D1 + "?", "Enabling All Shifts Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                EnDisText1 = "Disable";
                                EnDisBack1 = "Green";

                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D1), machineIds, true, true, true, true);
                                if (r > 0)
                                {

                                    foreach (var item in OrderProduction1)
                                    {
                                        if (Convert.ToDateTime(item.ProductionDate).Date == Convert.ToDateTime(D1).Date)
                                        {
                                            if (item.Shift == 1)
                                            {
                                                item.ShiftText = "Disable";
                                                item.ShiftBtnBackColour = "Green";
                                            }
                                            else if (item.Shift == 2)
                                            {
                                                item.ShiftText = "Disable";
                                                item.ShiftBtnBackColour = "Green";
                                            }
                                            else if (item.Shift == 3)
                                            {
                                                item.ShiftText = "Disable";
                                                item.ShiftBtnBackColour = "Green";
                                            }

                                            if (TabSelectedIndex1 == 0 && TabMachineSelectedIndex1 != 0)
                                            {
                                                item.EnDisVisibilty = "Visible";
                                            }
                                            else
                                            {
                                                item.EnDisVisibilty = "Collapsed";
                                            }
                                        }
                                    }
                                    if (Msg.Show("Do you want to allocate existing orders to this day?", "Enabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                                    {
                                        gm.ShiftOrders("Enable", 1, Convert.ToDateTime(D1), "DayButton");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Msg.Show("Are you sure, you want to disable all shifts for " + ele.Item2 + " on " + D1 + "?", "Disabling All Shifts Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D1), machineIds, true, false, false, false);
                                if (r > 0)
                                {
                                    gm.ShiftOrders("Disable", 0, Convert.ToDateTime(D1), "DayButton");
                                    EnDisText1 = "Enable";
                                    EnDisBack1 = "#FFCC3700";
                                    foreach (var item in OrderProduction1)
                                    {
                                        if (Convert.ToDateTime(item.ProductionDate).Date == Convert.ToDateTime(D1).Date)
                                        {
                                            if (item.Shift == 1)
                                            {
                                                item.ShiftText = "Enable";
                                                item.ShiftBtnBackColour = "#FFCC3700";
                                            }
                                            else if (item.Shift == 2)
                                            {
                                                item.ShiftText = "Enable";
                                                item.ShiftBtnBackColour = "#FFCC3700";
                                            }
                                            else if (item.Shift == 3)
                                            {
                                                item.ShiftText = "Enable";
                                                item.ShiftBtnBackColour = "#FFCC3700";
                                            }

                                            if (TabSelectedIndex1 == 0 && TabMachineSelectedIndex1 != 0)
                                            {
                                                item.EnDisVisibilty = "Visible";
                                            }
                                            else
                                            {
                                                item.EnDisVisibilty = "Collapsed";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        int sp2 = DBAccess.UpdateSystemParameter("DisableEnableDates", false);
                    }
                }
            }
        }

        private void EnDisDay2()
        {
            Tuple<int,string> ele = GetMachineInfo(TabMachineSelectedIndex2);

            if (ele.Item1 == 0)
            {
                Msg.Show("Please select Red Mill or Blue Mill to enable/disable shifts", "Select Mill", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                List<int> machineIds = new List<int>();
                machineIds.Add(ele.Item1);
                List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
                bool has = systemParameters.Any(x => x.Value == true);
                if (has == true)
                {
                    Msg.Show("System is performing some updates. Please try again in few minutes ", "System Busy", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                }
                else
                {
                    int sp1 = DBAccess.UpdateSystemParameter("DisableEnableDates", true);
                    if (sp1 > 0)
                    {
                        GradingManager gm = new GradingManager();

                        if (EnDisText2 == "Enable")
                        {
                            if (Msg.Show("Are you sure, you want to enable all shifts for " + ele.Item2 + " on " + D2 + "?", "Enabling All Shifts Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                EnDisText2 = "Disable";
                                EnDisBack2 = "Green";//Enable all the shifts  
                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D2), machineIds, true, true, true, true);
                                if (r > 0)
                                {
                                    foreach (var item in OrderProduction2)
                                    {
                                        if (Convert.ToDateTime(item.ProductionDate).Date == Convert.ToDateTime(D2).Date)
                                        {
                                            if (item.Shift == 1)
                                            {
                                                item.ShiftText = "Disable";
                                                item.ShiftBtnBackColour = "Green";
                                            }
                                            else if (item.Shift == 2)
                                            {
                                                item.ShiftText = "Disable";
                                                item.ShiftBtnBackColour = "Green";
                                            }
                                            else if (item.Shift == 3)
                                            {
                                                item.ShiftText = "Disable";
                                                item.ShiftBtnBackColour = "Green";
                                            }
                                        }
                                    }
                                    if (Msg.Show("Do you want to allocate existing orders to this day?", "Enabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                                    {
                                        gm.ShiftOrders("Enable", 1, Convert.ToDateTime(D2), "DayButton");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Msg.Show("Are you sure, you want to disable all shifts for " + ele.Item2 + " on " + D2 + "?", "Disabling All Shifts Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D2), machineIds, true, false, false, false);
                                if (r > 0)
                                {
                                    gm.ShiftOrders("Disable", 0, Convert.ToDateTime(D2), "DayButton");
                                    EnDisText2 = "Enable";
                                    EnDisBack2 = "#FFCC3700";
                                    foreach (var item in OrderProduction2)
                                    {
                                        if (Convert.ToDateTime(item.ProductionDate).Date == Convert.ToDateTime(D2).Date)
                                        {
                                            if (item.Shift == 1)
                                            {
                                                item.ShiftText = "Enable";
                                                item.ShiftBtnBackColour = "#FFCC3700";
                                            }
                                            else if (item.Shift == 2)
                                            {
                                                item.ShiftText = "Enable";
                                                item.ShiftBtnBackColour = "#FFCC3700";
                                            }
                                            else if (item.Shift == 3)
                                            {
                                                item.ShiftText = "Enable";
                                                item.ShiftBtnBackColour = "#FFCC3700";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        int sp2 = DBAccess.UpdateSystemParameter("DisableEnableDates", false);
                    }
                }
            }
        }

        private void EnDisDay3()
        {
             Tuple<int,string> ele = GetMachineInfo(TabMachineSelectedIndex3);

             if (ele.Item1 == 0)
             {
                 Msg.Show("Please select Red Mill or Blue Mill to enable/disable shifts", "Select Mill", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
             }
             else
             {
                 List<int> machineIds = new List<int>();
                 machineIds.Add(ele.Item1);
                 List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
                 bool has = systemParameters.Any(x => x.Value == true);
                 if (has == true)
                 {
                     Msg.Show("System is performing some updates. Please try again in few minutes ", "System Busy", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                 }
                 else
                 {
                     int sp1 = DBAccess.UpdateSystemParameter("DisableEnableDates", true);
                     if (sp1 > 0)
                     {
                         GradingManager gm = new GradingManager();

                         if (EnDisText3 == "Enable")
                         {
                             if (Msg.Show("Are you sure, you want to enable all shifts for " + ele.Item2 + " on " + D3 + "?", "Enabling All Shifts Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                             {
                                 EnDisText3 = "Disable";
                                 EnDisBack3 = "Green";
                                 int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D3), machineIds, true, true, true, true);
                                 if (r > 0)
                                 {
                                     foreach (var item in OrderProduction3)
                                     {
                                         if (Convert.ToDateTime(item.ProductionDate).Date == Convert.ToDateTime(D3).Date)
                                         {
                                             if (item.Shift == 1)
                                             {
                                                 item.ShiftText = "Disable";
                                                 item.ShiftBtnBackColour = "Green";
                                             }
                                             else if (item.Shift == 2)
                                             {
                                                 item.ShiftText = "Disable";
                                                 item.ShiftBtnBackColour = "Green";
                                             }
                                             else if (item.Shift == 3)
                                             {
                                                 item.ShiftText = "Disable";
                                                 item.ShiftBtnBackColour = "Green";
                                             }
                                         }
                                     }
                                     if (Msg.Show("Do you want to allocate existing orders to this day?", "Enabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                                     {
                                         gm.ShiftOrders("Enable", 1, Convert.ToDateTime(D3), "DayButton");
                                     }
                                 }
                             }
                         }
                         else
                         {
                             if (Msg.Show("Are you sure, you want to disable all shifts for " + ele.Item2 + " on " + D3 + "?", "Disabling All Shifts Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                             {
                                 int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D3), machineIds, true, false, false, false);
                                 if (r > 0)
                                 {
                                     gm.ShiftOrders("Disable", 0, Convert.ToDateTime(D3), "DayButton");
                                     EnDisText3 = "Enable";
                                     EnDisBack3 = "#FFCC3700";
                                     foreach (var item in OrderProduction3)
                                     {
                                         if (Convert.ToDateTime(item.ProductionDate).Date == Convert.ToDateTime(D3).Date)
                                         {
                                             if (item.Shift == 1)
                                             {
                                                 item.ShiftText = "Enable";
                                                 item.ShiftBtnBackColour = "#FFCC3700";
                                             }
                                             else if (item.Shift == 2)
                                             {
                                                 item.ShiftText = "Enable";
                                                 item.ShiftBtnBackColour = "#FFCC3700";
                                             }
                                             else if (item.Shift == 3)
                                             {
                                                 item.ShiftText = "Enable";
                                                 item.ShiftBtnBackColour = "#FFCC3700";
                                             }
                                         }
                                     }
                                 }
                             }
                         }
                         int sp2 = DBAccess.UpdateSystemParameter("DisableEnableDates", false);
                     }
                 }
             }
        }

        private void EnDisDay4()
        {
           Tuple<int,string> ele = GetMachineInfo(TabMachineSelectedIndex4);

            if (ele.Item1 == 0)
            {
                Msg.Show("Please select Red Mill or Blue Mill to enable/disable shifts", "Select Mill", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                List<int> machineIds = new List<int>();
                machineIds.Add(ele.Item1);
            List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
            bool has = systemParameters.Any(x => x.Value == true);
            if (has == true)
            {
                Msg.Show("System is performing some updates. Please try again in few minutes ", "System Busy", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                int sp1 = DBAccess.UpdateSystemParameter("DisableEnableDates", true);
                if (sp1 > 0)
                {
                    GradingManager gm = new GradingManager();

                    if (EnDisText4 == "Enable")
                    {
                        if (Msg.Show("Are you sure, you want to enable all shifts for " + ele.Item2 + " on " + D4 + "?", "Enabling All Shifts Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                        {
                            EnDisText4 = "Disable";
                            EnDisBack4 = "Green";
                            int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D4), machineIds, true, true, true, true);
                            if (r > 0)
                            {
                                foreach (var item in OrderProduction4)
                                {
                                    if (Convert.ToDateTime(item.ProductionDate).Date == Convert.ToDateTime(D4).Date)
                                    {
                                        if (item.Shift == 1)
                                        {
                                            item.ShiftText = "Disable";
                                            item.ShiftBtnBackColour = "Green";
                                        }
                                        else if (item.Shift == 2)
                                        {
                                            item.ShiftText = "Disable";
                                            item.ShiftBtnBackColour = "Green";
                                        }
                                        else if (item.Shift == 3)
                                        {
                                            item.ShiftText = "Disable";
                                            item.ShiftBtnBackColour = "Green";
                                        }
                                    }
                                }

                                if (Msg.Show("Do you want to allocate existing orders to this day?", "Enabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                                {
                                    gm.ShiftOrders("Enable", 1, Convert.ToDateTime(D4), "DayButton");
                                }
                            }
                        }

                    }
                    else
                    {
                        if (Msg.Show("Are you sure, you want to disable all shifts for " + ele.Item2 + " on " + D4 + "?", "Disabling All Shifts Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                        {
                            int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D4), machineIds, true, false, false, false);
                            if (r > 0)
                            {
                                foreach (var item in OrderProduction4)
                                {
                                    if (Convert.ToDateTime(item.ProductionDate).Date == Convert.ToDateTime(D4).Date)
                                    {
                                        if (item.Shift == 1)
                                        {
                                            item.ShiftText = "Enable";
                                            item.ShiftBtnBackColour = "#FFCC3700";
                                        }
                                        else if (item.Shift == 2)
                                        {
                                            item.ShiftText = "Enable";
                                            item.ShiftBtnBackColour = "#FFCC3700";
                                        }
                                        else if (item.Shift == 3)
                                        {
                                            item.ShiftText = "Enable";
                                            item.ShiftBtnBackColour = "#FFCC3700";
                                        }
                                    }
                                }
                                gm.ShiftOrders("Disable", 0, Convert.ToDateTime(D4), "DayButton");
                                EnDisText4 = "Enable";
                                EnDisBack4 = "#FFCC3700";
                            }
                        }
                    }
                    int sp2 = DBAccess.UpdateSystemParameter("DisableEnableDates", false);
                }
            }
            }
        }

        

        private void EnDisDay5()
        {
            Tuple<int,string> ele = GetMachineInfo(TabMachineSelectedIndex5);

            if (ele.Item1 == 0)
            {
                Msg.Show("Please select Red Mill or Blue Mill to enable/disable shifts", "Select Mill", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                List<int> machineIds = new List<int>();
                machineIds.Add(ele.Item1);

                List<SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
                bool has = systemParameters.Any(x => x.Value == true);
                if (has == true)
                {
                    Msg.Show("System is performing some updates. Please try again in few minutes ", "System Busy", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                }
                else
                {
                    int sp1 = DBAccess.UpdateSystemParameter("DisableEnableDates", true);
                    if (sp1 > 0)
                    {
                        GradingManager gm = new GradingManager();

                        if (EnDisText5 == "Enable")
                        {
                            if (Msg.Show("Are you sure, you want to enable all shifts for " + ele.Item2 + " on " + D5 + "?", "Enabling All Shifts Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                EnDisText5 = "Disable";
                                EnDisBack5 = "Green";

                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D5), machineIds, true, true, true, true);
                                if (r > 0)
                                {
                                    foreach (var item in OrderProduction5)
                                    {
                                        if (Convert.ToDateTime(item.ProductionDate).Date == Convert.ToDateTime(D5).Date)
                                        {
                                            if (item.Shift == 1)
                                            {
                                                item.ShiftText = "Disable";
                                                item.ShiftBtnBackColour = "Green";
                                            }
                                            else if (item.Shift == 2)
                                            {
                                                item.ShiftText = "Disable";
                                                item.ShiftBtnBackColour = "Green";
                                            }
                                            else if (item.Shift == 3)
                                            {
                                                item.ShiftText = "Disable";
                                                item.ShiftBtnBackColour = "Green";
                                            }
                                        }
                                    }
                                    if (Msg.Show("Do you want to allocate existing orders to this day?", "Enabling Day Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                                    {
                                        gm.ShiftOrders("Enable", 1, Convert.ToDateTime(D5), "DayButton");
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Msg.Show("Are you sure, you want to disable all shifts for " + ele.Item2 + " on " + D5 + "?", "Disabling All Shifts Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                            {
                                int r = DBAccess.EnableDisableShift(Convert.ToDateTime(D5), machineIds, true, false, false, false);
                                if (r > 0)
                                {
                                    foreach (var item in OrderProduction5)
                                    {
                                        if (Convert.ToDateTime(item.ProductionDate).Date == Convert.ToDateTime(D5).Date)
                                        {
                                            if (item.Shift == 1)
                                            {
                                                item.ShiftText = "Enable";
                                                item.ShiftBtnBackColour = "#FFCC3700";
                                            }
                                            else if (item.Shift == 2)
                                            {
                                                item.ShiftText = "Enable";
                                                item.ShiftBtnBackColour = "#FFCC3700";
                                            }
                                            else if (item.Shift == 3)
                                            {
                                                item.ShiftText = "Enable";
                                                item.ShiftBtnBackColour = "#FFCC3700";
                                            }
                                        }
                                    }
                                    gm.ShiftOrders("Disable", 0, Convert.ToDateTime(D5), "DayButton");
                                    EnDisText5 = "Enable";
                                    EnDisBack5 = "#FFCC3700";
                                }
                            }
                        }
                        int sp2 = DBAccess.UpdateSystemParameter("DisableEnableDates", false);
                    }
                }
            }
        }


        private Tuple<int, string> GetMachineInfo(int tabMachineIndex)
        {
            int mId = 0;
            string machineName = string.Empty;
            if (tabMachineIndex == 1)
            {
                machineName = "Blue Mill";
                mId = 1;
            }
            else if (tabMachineIndex == 2)
            {
                machineName = "Red Mill";
                mId = 7;
            }
            else
            {
                machineName = "All Mills";
            }

            return Tuple.Create(mId, machineName);
        }
        //private void EnableDisableShift()
        //{
        //    Console.WriteLine(OrderProduction4.First().RawProduct.RawProductCode);
        //}

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

        private int GetMachineIdByTabIndex(int m)
        {
            int x = 0;
            if(m==1)
            {
                x = 1;
            }
            else if(m==2)
            {
                x = 7;
            }
            else if (m == 0)
            {
                x = 0;
            }
            return x;
        }

        void notifier_MixingMessage(object sender, SqlNotificationEventArgs e)
        {
            this.LoadMixing(this.mixingOrdersNotifier.RegisterDependency());
        }

        #region Shift_Visibility_Functions
        private void VisibleCollapseGrid1()
        {
            if (TabMachineSelectedIndex1 == 1 || TabMachineSelectedIndex1 == 2)
            {
                bool enabled = timeTableDates.Any(x => x.MachineID == (TabMachineSelectedIndex1 == 1 ? 1 : 7) && x.ProductionDate == Convert.ToDateTime(D1) && ((x.IsDayShiftActive == false && x.IsEveningShiftActive == false && x.IsNightShiftActive == false) || (x.IsMachineActive == false)));
                if (enabled)
                {
                    EnDisText1 = "Enable";
                    EnDisBack1 = "#FFCC3700";
                }
                else
                {
                    EnDisText1 = "Disable";
                    EnDisBack1 = "Green";
                }
            } 

            foreach (var item in OrderProduction1)
            {
                if (item.Shift == 1 && TabSelectedIndex1 == 0 && TabMachineSelectedIndex1 != 0)
                {
                    item.EnDisVisibilty = "Visible";
                }
                else if (item.Shift == 1 && TabSelectedIndex1 == 0 && TabMachineSelectedIndex1 == 0)
                {
                    item.EnDisVisibilty = "Collapsed";
                }

                if (item.Shift == 2 && TabSelectedIndex1 == 0 && TabMachineSelectedIndex1 != 0)
                {
                    item.EnDisVisibilty = "Visible";
                }
                else if (item.Shift == 2 && TabSelectedIndex1 == 0 && TabMachineSelectedIndex1 == 0)
                {
                    item.EnDisVisibilty = "Collapsed";
                }

                if (item.Shift == 3 && TabSelectedIndex1 == 0 && TabMachineSelectedIndex1 != 0)
                {
                    item.EnDisVisibilty = "Visible";
                }
                else if (item.Shift == 3 && TabSelectedIndex1 == 0 && TabMachineSelectedIndex1 == 0)
                {
                    item.EnDisVisibilty = "Collapsed";
                }
            }
        }

        private void VisibleCollapseGrid2()
        {
            if (TabMachineSelectedIndex2 == 1 || TabMachineSelectedIndex2 == 2)
            {
                bool enabled = timeTableDates.Any(x => x.MachineID == (TabMachineSelectedIndex2 == 1 ? 1 : 7) && x.ProductionDate == Convert.ToDateTime(D2) && ((x.IsDayShiftActive == false && x.IsEveningShiftActive == false && x.IsNightShiftActive == false) || (x.IsMachineActive == false)));
                if (enabled)
                {
                    EnDisText2 = "Enable";
                    EnDisBack2 = "#FFCC3700";
                }
                else
                {
                    EnDisText2 = "Disable";
                    EnDisBack2 = "Green";
                }
            } 

            foreach (var item in OrderProduction2)
            {
                if (item.Shift == 1 && TabSelectedIndex2 == 0 && TabMachineSelectedIndex2 != 0)
                {
                    item.EnDisVisibilty = "Visible";
                }
                else if (item.Shift == 1 && TabSelectedIndex2 == 0 && TabMachineSelectedIndex2 == 0)
                {
                    item.EnDisVisibilty = "Collapsed";
                }

                if (item.Shift == 2 && TabSelectedIndex2 == 0 && TabMachineSelectedIndex2 != 0)
                {
                    item.EnDisVisibilty = "Visible";
                }
                else if (item.Shift == 2 && TabSelectedIndex2 == 0 && TabMachineSelectedIndex2 == 0)
                {
                    item.EnDisVisibilty = "Collapsed";
                }

                if (item.Shift == 3 && TabSelectedIndex2 == 0 && TabMachineSelectedIndex2 != 0)
                {
                    item.EnDisVisibilty = "Visible";
                }
                else if (item.Shift == 3 && TabSelectedIndex2 == 0 && TabMachineSelectedIndex2 == 0)
                {
                    item.EnDisVisibilty = "Collapsed";
                }
            }
        }

        private void VisibleCollapseGrid3()
        {

            if (TabMachineSelectedIndex3 == 1 || TabMachineSelectedIndex3 == 2)
            {
                bool enabled = timeTableDates.Any(x => x.MachineID == (TabMachineSelectedIndex3 == 1 ? 1 : 7) && x.ProductionDate == Convert.ToDateTime(D3) && ((x.IsDayShiftActive == false && x.IsEveningShiftActive == false && x.IsNightShiftActive == false) || (x.IsMachineActive == false)));
                if (enabled)
                {
                    EnDisText3 = "Enable";
                    EnDisBack3 = "#FFCC3700";
                }
                else
                {
                    EnDisText3 = "Disable";
                    EnDisBack3 = "Green";
                }
            } 

            foreach (var item in OrderProduction3)
            {
                if (item.Shift == 1 && TabSelectedIndex3 == 0 && TabMachineSelectedIndex3 != 0)
                {
                    item.EnDisVisibilty = "Visible";
                }
                else if (item.Shift == 1 && TabSelectedIndex3 == 0 && TabMachineSelectedIndex3 == 0)
                {
                    item.EnDisVisibilty = "Collapsed";
                }

                if (item.Shift == 2 && TabSelectedIndex3 == 0 && TabMachineSelectedIndex3 != 0)
                {
                    item.EnDisVisibilty = "Visible";
                }
                else if (item.Shift == 2 && TabSelectedIndex3 == 0 && TabMachineSelectedIndex3 == 0)
                {
                    item.EnDisVisibilty = "Collapsed";
                }

                if (item.Shift == 3 && TabSelectedIndex3 == 0 && TabMachineSelectedIndex3 != 0)
                {
                    item.EnDisVisibilty = "Visible";
                }
                else if (item.Shift == 3 && TabSelectedIndex3 == 0 && TabMachineSelectedIndex3 == 0)
                {
                    item.EnDisVisibilty = "Collapsed";
                }
            }
        }

        private void VisibleCollapseGrid4()
        {
            if (TabMachineSelectedIndex4 == 1 || TabMachineSelectedIndex4 == 2)
            {
                bool enabled = timeTableDates.Any(x => x.MachineID == (TabMachineSelectedIndex4 == 1 ? 1 : 7) && x.ProductionDate == Convert.ToDateTime(D4) && ((x.IsDayShiftActive == false && x.IsEveningShiftActive == false && x.IsNightShiftActive == false) || (x.IsMachineActive == false)));
                if (enabled)
                {
                    EnDisText4 = "Enable";
                    EnDisBack4 = "#FFCC3700";
                }
                else
                {
                    EnDisText4 = "Disable";
                    EnDisBack4 = "Green";
                }
            }       

            foreach (var item in OrderProduction4)
            {
                if (item.Shift == 1 && TabSelectedIndex4 == 0 && TabMachineSelectedIndex4 != 0)
                {
                    item.EnDisVisibilty = "Visible";
                }
                else if (item.Shift == 1 && TabSelectedIndex4 == 0 && TabMachineSelectedIndex4 == 0)
                {
                    item.EnDisVisibilty = "Collapsed";
                }

                if (item.Shift == 2 && TabSelectedIndex4 == 0 && TabMachineSelectedIndex4 != 0)
                {
                    item.EnDisVisibilty = "Visible";
                }
                else if (item.Shift == 2 && TabSelectedIndex4 == 0 && TabMachineSelectedIndex4 == 0)
                {
                    item.EnDisVisibilty = "Collapsed";
                }

                if (item.Shift == 3 && TabSelectedIndex4 == 0 && TabMachineSelectedIndex4 != 0)
                {
                    item.EnDisVisibilty = "Visible";
                }
                else if (item.Shift == 3 && TabSelectedIndex4 == 0 && TabMachineSelectedIndex4 == 0)
                {
                    item.EnDisVisibilty = "Collapsed";
                }
            }
        }

        private void VisibleCollapseGrid5()
        {
            if (TabMachineSelectedIndex5 == 1 || TabMachineSelectedIndex5 == 2)
            {                
                bool enabled = timeTableDates.Any(x => x.MachineID == (TabMachineSelectedIndex5 == 1 ? 1 : 7) && x.ProductionDate ==Convert.ToDateTime(D5) && ((x.IsDayShiftActive == false && x.IsEveningShiftActive == false && x.IsNightShiftActive == false) || (x.IsMachineActive == false)));
                if (enabled)
                {
                    EnDisText5 = "Enable";
                    EnDisBack5 = "#FFCC3700";
                }
                else
                {
                    EnDisText5 = "Disable";
                    EnDisBack5 = "Green";
                }
            }           

            foreach (var item in OrderProduction5)
            {
                

                if (item.Shift == 1 && TabSelectedIndex5 == 0 && TabMachineSelectedIndex5 != 0)
                {
                    item.EnDisVisibilty = "Visible";
                }
                else if (item.Shift == 1 && TabSelectedIndex5 == 0 && TabMachineSelectedIndex5 == 0)
                {
                    item.EnDisVisibilty = "Collapsed";
                }

                if (item.Shift == 2 && TabSelectedIndex5 == 0 && TabMachineSelectedIndex5 != 0)
                {
                    item.EnDisVisibilty = "Visible";
                }
                else if (item.Shift == 2 && TabSelectedIndex5 == 0 && TabMachineSelectedIndex5 == 0)
                {
                    item.EnDisVisibilty = "Collapsed";
                }

                if (item.Shift == 3 && TabSelectedIndex5 == 0 && TabMachineSelectedIndex5 != 0)
                {
                    item.EnDisVisibilty = "Visible";
                }
                else if (item.Shift == 3 && TabSelectedIndex5 == 0 && TabMachineSelectedIndex5 == 0)
                {
                    item.EnDisVisibilty = "Collapsed";
                }
            }
        }

        #endregion

        #region PUBLIC PROPERTIES

        public ObservableCollection<MixingProductionDetails> MixingOrders1
        {
            get
            {
                return _mixingOrders1;
            }
            set
            {
                _mixingOrders1 = value;
                RaisePropertyChanged(() => this.MixingOrders1);
            }
        }

        public ObservableCollection<MixingProductionDetails> MixingOrders2
        {
            get
            {
                return _mixingOrders2;
            }
            set
            {
                _mixingOrders2 = value;
                RaisePropertyChanged(() => this.MixingOrders2);
            }
        }

        public ObservableCollection<MixingProductionDetails> MixingOrders3
        {
            get
            {
                return _mixingOrders3;
            }
            set
            {
                _mixingOrders3 = value;
                RaisePropertyChanged(() => this.MixingOrders3);
            }
        }

        public ObservableCollection<MixingProductionDetails> MixingOrders4
        {
            get
            {
                return _mixingOrders4;
            }
            set
            {
                _mixingOrders4 = value;
                RaisePropertyChanged(() => this.MixingOrders4);
            }
        }

        public ObservableCollection<MixingProductionDetails> MixingOrders5
        {
            get
            {
                return _mixingOrders5;
            }
            set
            {
                _mixingOrders5 = value;
                RaisePropertyChanged(() => this.MixingOrders5);
            }
        }

        public ObservableCollection<MixingProductionDetails> MixingOrders
        {
            get
            {
                return _mixingOrders;
            }
            set
            {
                _mixingOrders = value;
                RaisePropertyChanged(() => this.MixingOrders);
            }
        }


        public int TabAreaSelectedIndex
        {
            get
            {
                return _tabAreaSelectedIndex;
            }
            set
            {
                _tabAreaSelectedIndex = value;
                RaisePropertyChanged(() => this.TabAreaSelectedIndex);
                if(TabAreaSelectedIndex == 0)
                {
                    LoadGradingData();
                }
                else if (TabAreaSelectedIndex == 1)
                {
                    LoadMixingData();
                }
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


        public ObservableCollection<RawProductionDetails> OrderProduction
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

        public ObservableCollection<RawProductionDetails> OrderProduction1
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

        public ObservableCollection<RawProductionDetails> OrderProduction2
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

        public ObservableCollection<RawProductionDetails> OrderProduction3
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

        public ObservableCollection<RawProductionDetails> OrderProduction4
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

        public ObservableCollection<RawProductionDetails> OrderProduction5
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


        public string SelectedSchedulerType
        {
            get { return _selectedSchedulerType; }
            set { _selectedSchedulerType = value; 
                RaisePropertyChanged(() => this.SelectedSchedulerType); 
                
                if(SelectedSchedulerType == "Grading")
                {

                }
                else if (SelectedSchedulerType == "Mixing")
                {

                }
                else if (SelectedSchedulerType == "Slitting")
                {

                }
                else if (SelectedSchedulerType == "Peeling")
                {

                }
                else if (SelectedSchedulerType == "Re-Rolling")
                {

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

                    CollDay1 = new ListCollectionView(AddToTabs(OrderProduction1, TabSelectedIndex1, GetMachineIdByTabIndex(TabMachineSelectedIndex1)));
                    CollDay1.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                    CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

                    VisibleCollapseGrid1();
                      
                }

                if (TabSelectedIndex1 == 0 && TabMachineSelectedIndex1 != 0)
                {
                    EnDis1Visible = "Visible";
                }
                else
                {
                    EnDis1Visible = "Collapsed";
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

                    CollDay2 = new ListCollectionView(AddToTabs(OrderProduction2, TabSelectedIndex2, GetMachineIdByTabIndex(TabMachineSelectedIndex2)));
                    CollDay2.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                    CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

                    VisibleCollapseGrid2();
                }

                if (TabSelectedIndex2 == 0 && TabMachineSelectedIndex2 != 0)
                {
                    EnDis2Visible = "Visible";
                }
                else
                {
                    EnDis2Visible = "Collapsed";
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
                    CollDay3 = new ListCollectionView(AddToTabs(OrderProduction3, TabSelectedIndex3, GetMachineIdByTabIndex(TabMachineSelectedIndex3)));
                    CollDay3.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                    CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

                    VisibleCollapseGrid3();
                }

                if (TabSelectedIndex3 == 0 && TabMachineSelectedIndex3 != 0)
                {
                    EnDis3Visible = "Visible";
                }
                else
                {
                    EnDis3Visible = "Collapsed";
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
                    CollDay4 = new ListCollectionView(AddToTabs(OrderProduction4, TabSelectedIndex4, GetMachineIdByTabIndex(TabMachineSelectedIndex4)));
                    CollDay4.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                    CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

                    VisibleCollapseGrid4();
                }

                if (TabSelectedIndex4 == 0 && TabMachineSelectedIndex4 != 0)
                {
                    EnDis4Visible = "Visible";
                }
                else
                {
                    EnDis4Visible = "Collapsed";
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
                    CollDay5 = new ListCollectionView(AddToTabs(OrderProduction5, TabSelectedIndex5, GetMachineIdByTabIndex(TabMachineSelectedIndex5)));
                    CollDay5.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                    CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

                    VisibleCollapseGrid5();
                }

                if (TabSelectedIndex5 == 0 && TabMachineSelectedIndex5 != 0)
                {
                    EnDis5Visible = "Visible";
                }
                else
                {
                    EnDis5Visible = "Collapsed";
                }
            }
        }

        public int TabMachineSelectedIndex1
        {
            get { return _tabMachineSelectedIndex1; }
            set
            {
                _tabMachineSelectedIndex1 = value;
                RaisePropertyChanged(() => this.TabMachineSelectedIndex1);

                if (OrderProduction1 != null)
                {
                    //foreach (var item in OrderProduction1)
                    //{
                    //    Console.WriteLine(item.RawProduct.RawProductID + " " + item.RawProductMachine.GradingMachineID);
                    //}

                    CollDay1 = new ListCollectionView(AddToTabs(OrderProduction1, TabSelectedIndex1, GetMachineIdByTabIndex(TabMachineSelectedIndex1)));
                    CollDay1.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                    CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

                    VisibleCollapseGrid1();
                }

                if (TabSelectedIndex1 == 0 && TabMachineSelectedIndex1 != 0)
                {
                    EnDis1Visible = "Visible";
                }
                else
                {
                    EnDis1Visible = "Collapsed";
                }
            }
        }

        public int TabMachineSelectedIndex2
        {
            get { return _tabMachineSelectedIndex2; }
            set
            {
                _tabMachineSelectedIndex2 = value;
                RaisePropertyChanged(() => this.TabMachineSelectedIndex2);
                if (OrderProduction2 != null)
                {
                    
                    CollDay2 = new ListCollectionView(AddToTabs(OrderProduction2, TabSelectedIndex2, GetMachineIdByTabIndex(TabMachineSelectedIndex2)));
                    CollDay2.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                    CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

                    VisibleCollapseGrid2();
                }

                if (TabSelectedIndex2 == 0 && TabMachineSelectedIndex2 != 0)
                {
                    EnDis2Visible = "Visible";
                }
                else
                {
                    EnDis2Visible = "Collapsed";
                }
            
            }
        }

        public int TabMachineSelectedIndex3
        {
            get { return _tabMachineSelectedIndex3; }
            set
            {
                _tabMachineSelectedIndex3 = value;
                RaisePropertyChanged(() => this.TabMachineSelectedIndex3);
                if (OrderProduction3 != null)
                {
                    CollDay3 = new ListCollectionView(AddToTabs(OrderProduction3, TabSelectedIndex3, GetMachineIdByTabIndex(TabMachineSelectedIndex3)));
                    CollDay3.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                    CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

                    VisibleCollapseGrid3();
                }

                if (TabSelectedIndex3 == 0 && TabMachineSelectedIndex3 != 0)
                {
                    EnDis3Visible = "Visible";
                }
                else
                {
                    EnDis3Visible = "Collapsed";
                }
            }
        }

        public int TabMachineSelectedIndex4
        {
            get { return _tabMachineSelectedIndex4; }
            set
            {
                _tabMachineSelectedIndex4 = value;
                RaisePropertyChanged(() => this.TabMachineSelectedIndex4);
                if (OrderProduction4 != null)
                {
                    CollDay4 = new ListCollectionView(AddToTabs(OrderProduction4, TabSelectedIndex4, GetMachineIdByTabIndex(TabMachineSelectedIndex4)));
                    CollDay4.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                    CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

                    VisibleCollapseGrid4();
                }

                if (TabSelectedIndex4 == 0 && TabMachineSelectedIndex4 != 0)
                {
                    EnDis4Visible = "Visible";
                }
                else
                {
                    EnDis4Visible = "Collapsed";
                }
            }
        }

        public int TabMachineSelectedIndex5
        {
            get { return _tabMachineSelectedIndex5; }
            set
            {
                _tabMachineSelectedIndex5 = value;
                RaisePropertyChanged(() => this.TabMachineSelectedIndex5);
                if (OrderProduction5 != null)
                {
                    CollDay5 = new ListCollectionView(AddToTabs(OrderProduction5, TabSelectedIndex5, GetMachineIdByTabIndex(TabMachineSelectedIndex5)));
                    CollDay5.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
                    CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("OrderType", System.ComponentModel.ListSortDirection.Ascending));

                    VisibleCollapseGrid5();
                }

                if (TabSelectedIndex5 == 0 && TabMachineSelectedIndex5 != 0)
                {
                    EnDis5Visible = "Visible";
                }
                else
                {
                    EnDis5Visible = "Collapsed";
                }
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
        public bool NextEnabled
        {
            get { return _nextEnabled; }
            set
            {
                _nextEnabled = value;
                RaisePropertyChanged(() => this.NextEnabled);
            }
        }

        public string EnDis1Visible
        {
            get { return _enDis1Visible; }
            set
            {
                _enDis1Visible = value;
                RaisePropertyChanged(() => this.EnDis1Visible);
            }
        }

        public string EnDis2Visible
        {
            get { return _enDis2Visible; }
            set
            {
                _enDis2Visible = value;
                RaisePropertyChanged(() => this.EnDis2Visible);
            }
        }

        public string EnDis3Visible
        {
            get { return _enDis3Visible; }
            set
            {
                _enDis3Visible = value;
                RaisePropertyChanged(() => this.EnDis3Visible);
            }
        }

        public string EnDis4Visible
        {
            get { return _enDis4Visible; }
            set
            {
                _enDis4Visible = value;
                RaisePropertyChanged(() => this.EnDis4Visible);
            }
        }

        public string EnDis5Visible
        {
            get { return _enDis5Visible; }
            set
            {
                _enDis5Visible = value;
                RaisePropertyChanged(() => this.EnDis5Visible);
            }
        }


        public int TabMixingSelectedIndex1
        {
            get
            {
                return _tabMixingSelectedIndex1;
            }
            set
            {
                _tabMixingSelectedIndex1 = value;
                RaisePropertyChanged(() => this.TabMixingSelectedIndex1);

                if (MixingOrders != null && MixingOrders.Count > 0 && !string.IsNullOrWhiteSpace(D1))
                {
                    MixingOrders1 = ConstructMixingOrders(MixingOrders, Convert.ToDateTime(D1), TabMixingSelectedIndex1, TabMixingShiftSelectedIndex1);
                }
            }
        }

        public int TabMixingSelectedIndex2
        {
            get
            {
                return _tabMixingSelectedIndex2;
            }
            set
            {
                _tabMixingSelectedIndex2 = value;
                RaisePropertyChanged(() => this.TabMixingSelectedIndex2);

                if (MixingOrders != null && MixingOrders.Count > 0 && !string.IsNullOrWhiteSpace(D2))
                {
                    MixingOrders2 = ConstructMixingOrders(MixingOrders, Convert.ToDateTime(D2), TabMixingSelectedIndex2, TabMixingShiftSelectedIndex2);
                }
            }
        }

        public int TabMixingSelectedIndex3
        {
            get
            {
                return _tabMixingSelectedIndex3;
            }
            set
            {
                _tabMixingSelectedIndex3 = value;
                RaisePropertyChanged(() => this.TabMixingSelectedIndex3);

                if (MixingOrders != null && MixingOrders.Count > 0 && !string.IsNullOrWhiteSpace(D3))
                {
                    MixingOrders3 = ConstructMixingOrders(MixingOrders, Convert.ToDateTime(D3), TabMixingSelectedIndex3, TabMixingShiftSelectedIndex3);
                }
            }
        }

        public int TabMixingSelectedIndex4
        {
            get
            {
                return _tabMixingSelectedIndex4;
            }
            set
            {
                _tabMixingSelectedIndex4 = value;
                RaisePropertyChanged(() => this.TabMixingSelectedIndex4);

                if (MixingOrders != null && MixingOrders.Count > 0 && !string.IsNullOrWhiteSpace(D4))
                {
                    MixingOrders4 = ConstructMixingOrders(MixingOrders, Convert.ToDateTime(D4), TabMixingSelectedIndex4, TabMixingShiftSelectedIndex4);
                }
            }
        }

        public int TabMixingSelectedIndex5
        {
            get
            {
                return _tabMixingSelectedIndex5;
            }
            set
            {
                _tabMixingSelectedIndex5 = value;
                RaisePropertyChanged(() => this.TabMixingSelectedIndex5);

                if (MixingOrders != null && MixingOrders.Count > 0 && !string.IsNullOrWhiteSpace(D5))
                {
                    MixingOrders5 = ConstructMixingOrders(MixingOrders, Convert.ToDateTime(D5), TabMixingSelectedIndex5, TabMixingShiftSelectedIndex5);
                }
            }
        }

        public int TabMixingShiftSelectedIndex1
        {
            get
            {
                return _tabMixingShiftSelectedIndex1;
            }
            set
            {
                _tabMixingShiftSelectedIndex1 = value;
                RaisePropertyChanged(() => this.TabMixingShiftSelectedIndex1);
                if (MixingOrders != null && MixingOrders.Count > 0 && !string.IsNullOrWhiteSpace(D1))
                {
                    MixingOrders1 = ConstructMixingOrders(MixingOrders, Convert.ToDateTime(D1), TabMixingSelectedIndex1, TabMixingShiftSelectedIndex1);
                }
            }
        }

        public int TabMixingShiftSelectedIndex2
        {
            get
            {
                return _tabMixingShiftSelectedIndex2;
            }
            set
            {
                _tabMixingShiftSelectedIndex2 = value;
                RaisePropertyChanged(() => this.TabMixingShiftSelectedIndex2);

                if (MixingOrders != null && MixingOrders.Count > 0 && !string.IsNullOrWhiteSpace(D2))
                {
                    MixingOrders2 = ConstructMixingOrders(MixingOrders, Convert.ToDateTime(D2), TabMixingSelectedIndex2, TabMixingShiftSelectedIndex2);
                }
            }
        }

        public int TabMixingShiftSelectedIndex3
        {
            get
            {
                return _tabMixingShiftSelectedIndex3;
            }
            set
            {
                _tabMixingShiftSelectedIndex3 = value;
                RaisePropertyChanged(() => this.TabMixingShiftSelectedIndex3);

                if (MixingOrders != null && MixingOrders.Count > 0 && !string.IsNullOrWhiteSpace(D3))
                {
                    MixingOrders3 = ConstructMixingOrders(MixingOrders, Convert.ToDateTime(D3), TabMixingSelectedIndex3, TabMixingShiftSelectedIndex3);
                }
            }
        }

        public int TabMixingShiftSelectedIndex4
        {
            get
            {
                return _tabMixingShiftSelectedIndex4;
            }
            set
            {
                _tabMixingShiftSelectedIndex4 = value;
                RaisePropertyChanged(() => this.TabMixingShiftSelectedIndex4);

                if (MixingOrders != null && MixingOrders.Count > 0 && !string.IsNullOrWhiteSpace(D4))
                {
                    MixingOrders4 = ConstructMixingOrders(MixingOrders, Convert.ToDateTime(D4), TabMixingSelectedIndex4, TabMixingShiftSelectedIndex4);
                }
            }
        }

        public int TabMixingShiftSelectedIndex5
        {
            get
            {
                return _tabMixingShiftSelectedIndex5;
            }
            set
            {
                _tabMixingShiftSelectedIndex5 = value;
                RaisePropertyChanged(() => this.TabMixingShiftSelectedIndex5);

                if (MixingOrders != null && MixingOrders.Count > 0 && !string.IsNullOrWhiteSpace(D5))
                {
                    MixingOrders5 = ConstructMixingOrders(MixingOrders, Convert.ToDateTime(D5), TabMixingSelectedIndex5, TabMixingShiftSelectedIndex5);
                }
            }
        }

        #endregion

        #region COMMANDS



        public ICommand EnDis1
        {
            get
            {
                return _enDis1 ?? (_enDis1 = new LogOutCommandHandler(() => EnDisDay1(), canExecute));
            }
        }

        public ICommand EnDis2
        {
            get
            {
                return _enDis2 ?? (_enDis2 = new LogOutCommandHandler(() => EnDisDay2(), canExecute));
            }
        }

        public ICommand EnDis3
        {
            get
            {
                return _enDis3 ?? (_enDis3 = new LogOutCommandHandler(() => EnDisDay3(), canExecute));
            }
        }
        public ICommand EnDis4
        {
            get
            {
                return _enDis4 ?? (_enDis4 = new LogOutCommandHandler(() => EnDisDay4(), canExecute));
            }
        }

        public ICommand EnDis5
        {
            get
            {
                return _enDis5 ?? (_enDis5 = new LogOutCommandHandler(() => EnDisDay5(), canExecute));
            }
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand OrdersCommand
        {
            get
            {
                return _ordersCommand ?? (_ordersCommand = new LogOutCommandHandler(() => Switcher.Switch(new QuotingMainMenu(userName, state, privilages, metaData)), canExecute));
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

        public ICommand ProductionMaintenanceCommand
        {
            get
            {
                return _productionMaintenanceCommand ?? (_productionMaintenanceCommand = new LogOutCommandHandler(() => Switcher.Switch(new ProductionMaintenanceView(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand AdminDashboardCommand
        {
            get
            {
                return _adminDashboardCommand ?? (_adminDashboardCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand PrintMixingPendingCommand
        {
            get
            {
                return _printMixingPendingCommand ?? (_printMixingPendingCommand = new A1QSystem.Commands.LogOutCommandHandler(PrintMixingPending, canExecute));
            }
        }
        public ICommand PrintGradingPendingCommand
        {
            get
            {
                return _printGradingPendingCommand ?? (_printGradingPendingCommand = new A1QSystem.Commands.LogOutCommandHandler(PrintGradingPending, canExecute));
            }
        }

        public ICommand ViewMixingScheduleCommand
        {
            get
            {
                return _viewMixingScheduleCommand ?? (_viewMixingScheduleCommand = new A1QSystem.Commands.LogOutCommandHandler(OpenMixingSchedule, canExecute));
            }
        }

        public ICommand PrintMixingUnCompletedCommand
        {
            get
            {
                return _printMixingUnCompletedCommand ?? (_printMixingUnCompletedCommand = new A1QSystem.Commands.LogOutCommandHandler(PrintMixingUnfinishedOrders, canExecute));
            }
        }

        
       
        #endregion
    }
}
