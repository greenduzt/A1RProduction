using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Formula;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Production.Grading;
using A1QSystem.Model.Products;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using A1QSystem.View.Dashboard;
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

namespace A1QSystem.ViewModel.Productions.Grading
{
    public class GradingScheduleViewModel : ViewModelBase
    {
        private string _day1;
        private string _day2;
        private string _day3;
        private string _day4;
        private string _day5;
        private string _day1Show;
        private string _day2Show;
        private string _day3Show;
        private string _day4Show;
        private string _day1and2;
        private string _day2and3;
        private string _day3and4;
        private string _day1and2Show;
        private string _day2and3Show;
        private string _day3and4Show;
        private string _day1and2and3Show;
        private string _day2and3and4Show;
        private string _day1and2and3;
        private string _day2and3and4;
        private string _grid1Shift;
        private string _grid2Shift;
        private string _grid3Shift;
        private string _grid4Shift;
        private string _grid1ShiftShow;
        private string _grid2ShiftShow;
        private string _grid3ShiftShow;
        private string _grid4ShiftShow;
        private string _dG1BackGround;
        private string _dG2BackGround;
        private string _dG3BackGround;
        private string _dG4BackGround;

        private string _day1and2BackCol;
        private string _day2and3BackCol;
        private string _day3and4BackCol;
        private string _day1and2and3BackCol;
        private string _day2and3and4BackCol;

        private string _day1BackCol;
        private string _day2BackCol;
        private string _day3BackCol;
        private string _day4BackCol;

        private string _grid1ShiftBackColor;
        private string _grid2ShiftBackColor;
        private string _grid3ShiftBackColor;
        private string _grid4ShiftBackColor;
        private string _dailyDataGridVisibility;
        private string _dailyDate;

        private int _tabMachineSelectedIndex1;
        private int _tabMachineSelectedIndex2;
        private int _tabMachineSelectedIndex3;
        private int _tabMachineSelectedIndex4;        

        private ListCollectionView _collDay1 = null;
        private ListCollectionView _collDay2 = null;
        private ListCollectionView _collDay3 = null;
        private ListCollectionView _collDay4 = null;
        private ListCollectionView _collDay5 = null;
        private ObservableCollection<Product> _gradeDetails;
        private ObservableCollection<GradingProductionDetails> _orderProduction = null;
        private ObservableCollection<GradingProductionDetails> _orderProduction1 = null;
        private ObservableCollection<GradingProductionDetails> _orderProduction2 = null;
        private ObservableCollection<GradingProductionDetails> _orderProduction3 = null;
        private ObservableCollection<GradingProductionDetails> _orderProduction4 = null;
        private ObservableCollection<GradingProductionDetails> _gradingProductionDetails = null;
        private ObservableCollection<GradingProductionDetails> _dailyOrderProduction = null;

        private ObservableCollection<GradingProductionDetails> _tempOrderProduction1 = null;
        private ObservableCollection<GradingProductionDetails> _tempOrderProduction2 = null;
        private ObservableCollection<GradingProductionDetails> _tempOrderProduction3 = null;
        private ObservableCollection<GradingProductionDetails> _tempOrderProduction4 = null;

        private List<FormulaOptions> formulaOptions;

        private int _index;
        private int _shift;
        private bool _canExecute;
        private bool _prevEnabled;
        private bool _nextEnabled;
        private DateTime CurrentDate;
        public int TotOrdersPending { get; set; }
        private Int32 firstGridId;
        private ChildWindowView LoadingScreen;
        private List<MetaData> metaData;
        private string userName;
        private string state;
        private List<UserPrivilages> privilages;
        private string _version;
        private string _controlVisibility;
        private List<int> gradesNoShowList;
        public Dispatcher UIDispatcher { get; set; }
        public GradingOrdersNotifier productionSchedularNotifier { get; set; }
        private System.Windows.Forms.Timer tmr = null;

        //private ICommand _backCommand;
        private ICommand navHomeCommand;
        private ICommand _workStationsCommand;
        private ICommand _gradedStockCommand;
        private ICommand _prevCommand;
        private ICommand _nextCommand;
        private ICommand _regrindStockCommand;
        private ICommand _printGradingPendingCommand;
        private ICommand _viewMixingScheduleCommand;
        private DelegateCommand<string> _dailyOrdersCommand;
        private DelegateCommand<string> _weeklyOrdersCommand;

        public GradingScheduleViewModel(string UserName, string State, List<UserPrivilages> Privilages, Dispatcher uidispatcher, List<MetaData> md)
        {
            gradesNoShowList = new List<int>();
            CurrentDate = NTPServer.GetNetworkTime();
            metaData = md;
            userName = UserName;
            state = State;
            privilages = Privilages;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;

            var data2 = metaData.SingleOrDefault(x => x.KeyName == "grades_no_show");
            if (data2 != null)
            {
                string[] noGradesArr = data2.Description.Split(',');

                foreach (var item in noGradesArr)
                {
                    gradesNoShowList.Add(Convert.ToInt16(item));
                }
            }

            _index = 0;
            _shift = 0;
            PrevEnabled = true;
            NextEnabled = true;
            _NextClicked = false;
            LoadFormulaOptions();
            Day1Show = "Hidden";
            Day2Show = "Hidden";
            Day3Show = "Hidden";
            Day4Show = "Hidden";
            Day1and2Show = "Hidden";
            Day2and3Show = "Hidden";
            Day3and4Show = "Hidden";
            Day1and2and3Show = "Hidden";
            Day2and3and4Show = "Hidden";
            Grid1ShiftShow = "Hidden";
            Grid2ShiftShow = "Hidden";
            Grid3ShiftShow = "Hidden";
            Grid4ShiftShow = "Hidden";

            DG1BackGround = "#dedede";
            DG2BackGround = "#dedede";
            DG3BackGround = "#dedede";
            DG4BackGround = "#dedede";
            Day1and2BackCol = "#dedede";
            Day2and3BackCol = "#dedede";
            Day3and4BackCol = "#dedede";
            Day1and2and3BackCol = "#dedede";
            Day2and3and4BackCol = "#dedede";
            Day1BackCol = "#dedede";
            Day2BackCol = "#dedede";
            Day3BackCol = "#dedede";
            Day4BackCol = "#dedede";
            Grid1ShiftBackColor = "#dedede";
            Grid2ShiftBackColor = "#dedede";
            Grid3ShiftBackColor = "#dedede";
            Grid4ShiftBackColor = "#dedede";

            TabMachineSelectedIndex1 = 0;
            TabMachineSelectedIndex2 = 0;
            TabMachineSelectedIndex3 = 0;
            TabMachineSelectedIndex4 = 0;            

            firstGridId = 0;
            OrderView("daily");

            GradeDetails = new ObservableCollection<Product>();
            GradeDetails = DBAccess.GetAllBlockLogsAsProds();

            StartTimer();

            this.UIDispatcher = uidispatcher;
            this.productionSchedularNotifier = new GradingOrdersNotifier();

            BackgroundWorker worker = new BackgroundWorker();
            LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Loading");

            worker.DoWork += (_, __) =>
            {               
                this.productionSchedularNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage);
                ObservableCollection<GradingProductionDetails> opd = this.productionSchedularNotifier.RegisterDependency();
                this.LoadData(opd);
            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();

            };
            worker.RunWorkerAsync();

            _canExecute = true;
        }

        private void LoadFormulaOptions()
        {
            formulaOptions = new List<FormulaOptions>();
            formulaOptions = DBAccess.GetFormulaOptions();
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

        private void LoadData(ObservableCollection<GradingProductionDetails> psd)
        {
            EnDisPrevButton();
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();

            this.UIDispatcher.BeginInvoke((Action)delegate()
            {
                if (psd != null)
                {
                    string D1 = string.Empty;
                    string D2 = string.Empty;
                    string D3 = string.Empty;
                    string D4 = string.Empty;
                    string D5 = string.Empty;
                    DateTime currentDate = DateTime.Now.Date;
                    OrderProduction1 = new ObservableCollection<GradingProductionDetails>();
                    OrderProduction2 = new ObservableCollection<GradingProductionDetails>();
                    OrderProduction3 = new ObservableCollection<GradingProductionDetails>();
                    OrderProduction4 = new ObservableCollection<GradingProductionDetails>();
                    DailyOrderProduction = new ObservableCollection<GradingProductionDetails>();
                    List<GradingProductionDetails> TempDailyOrderProduction = new List<GradingProductionDetails>();    

                    //Make sure that the dates are sorted
                    psd = new ObservableCollection<GradingProductionDetails>(psd.OrderBy(i => i.OrderType).ThenBy(i => i.PDate.Date).ThenBy(i => i.Shift).ThenBy(i => Convert.ToDateTime(i.RequiredDate).Date).ThenBy(i => i.MixingDate).ThenByDescending(t => t.MixingShift));

                    _gradingProductionDetails = psd;                  


                    for (int i = _index; i < psd.Count; i++)
                    {
                        bool exe = false;
                        //Get groupid;
                        var gid = formulaOptions.SingleOrDefault(x=>x.RawProduct.RawProductID== psd[i].RawProduct.RawProductID);
                        if (gid != null)
                        {
                            psd[i].FormulaOptions = new List<FormulaOptions>();
                            foreach (var item in formulaOptions)
                            {
                                if (gid.GroupID == item.GroupID)
                                {
                                    if (psd[i].RawProduct.RawProductID == item.RawProduct.RawProductID)
                                    {
                                        psd[i].FormulaOptions.Add(new FormulaOptions() { GradingSchedulingID = psd[i].GradingSchedulingID,Formula=item.Formula, RawProduct=item.RawProduct,Checked = true });
                                    }
                                    else
                                    {
                                        psd[i].FormulaOptions.Add(new FormulaOptions() { GradingSchedulingID = psd[i].GradingSchedulingID, Formula = item.Formula, RawProduct = item.RawProduct, Checked = false });
                                    }                                                   
                                }                               
                            }
                            psd[i].FormulaOptionVisibility = psd[i].FormulaOptions.Count == 0 ? "Collapsed" : "Visible";
                        }
                        else
                        {
                            psd[i].FormulaOptionVisibility = "Collapsed";
                        }
                           
                        if (OrderProduction1.Count != 0)
                        {
                            exe = OrderProduction1.Any(x => x.ProductionDate == psd[i].ProductionDate && x.Shift == psd[i].Shift);
                        }

                        string gs = string.Empty;

                        if (GradeDetails != null && GradeDetails.Count > 0 && gradesNoShowList != null && gradesNoShowList.Count > 0)
                        {
                            var d = GradeDetails.SingleOrDefault(x => x.RawProduct.RawProductID == psd[i].RawProduct.RawProductID);
                            if (d != null)
                            {
                                bool hasVal = gradesNoShowList.Any(x => x == psd[i].RawProduct.RawProductID);

                                if (hasVal == false)
                                {
                                    //Grade1
                                    psd[i].GradeString = d.RawProduct.Formula.FormulaName1 + " " + string.Format("{0:G29}", (d.RawProduct.Formula.GradingWeight1 * Math.Ceiling(psd[i].BlockLogQty))) + "Kg";
                                    if (d.RawProduct.Formula.ProductCapacity2 > 0)
                                    {
                                        //Grade2
                                        psd[i].GradeString += System.Environment.NewLine + d.RawProduct.Formula.FormulaName2 + " " + string.Format("{0:G29}", (d.RawProduct.Formula.GradingWeight2 * Math.Ceiling(psd[i].BlockLogQty))) + "Kg";
                                    }
                                }
                            }
                        }


                        if (OrderProduction1.Count == 0 || exe == true)
                        {
                            OrderProduction1.Add(new GradingProductionDetails()
                            {
                                RawProduct = new RawProduct() { RawProductID = psd[i].RawProduct.RawProductID, RawProductCode = psd[i].RawProduct.RawProductCode, Description = psd[i].RawProduct.Description, RawProductType = psd[i].RawProduct.RawProductType },
                                Customer = new Customer() { CompanyName = psd[i].Customer.CompanyName },
                                PDate = psd[i].PDate,
                                ProdTimeTableID = psd[i].ProdTimeTableID,
                                RequiredDate = Regex.Match(psd[i].RequiredDate, "^[^ ]+").Value,
                                GradeString = psd[i].GradeString,
                                MixingDate = psd[i].MixingDate,
                                MixingShift = psd[i].MixingShift,
                                SalesOrderId = psd[i].SalesOrderId,
                                RawProDetailsID = psd[i].RawProDetailsID,
                                BlockLogQty = Math.Ceiling(psd[i].BlockLogQty),
                                ProductionDate = psd[i].ProductionDate,
                                Shift = psd[i].Shift,
                                ShiftName = GetShiftNameByID(psd[i].Shift),
                                GradingFormula = psd[i].GradingFormula,
                                SalesOrder = psd[i].SalesOrder,
                                RowBackgroundColour = psd[i].RowBackgroundColour,
                                OrderType = psd[i].OrderType,
                                Notes = psd[i].Notes,
                                GradingActive = psd[i].GradingActive,
                                PrintCounter = psd[i].PrintCounter,
                                GradingSchedulingID = psd[i].GradingSchedulingID,
                                IsReqDateVisible = psd[i].ReqDateSelected == true ? "Visible" : "Collapsed",
                                ItemBackgroundColour = bdg.ConvertDayToColour(Convert.ToDateTime(psd[i].RequiredDate)),
                                FormulaOptions = psd[i].FormulaOptions,
                                FormulaOptionVisibility = psd[i].FormulaOptionVisibility,
                                RawProductMachine = new RawProductMachine() {GradingMachineID=psd[i].RawProductMachine.GradingMachineID }
                            });
                            TempOrderProduction1=OrderProduction1;
                            firstGridId = psd[i].ProdTimeTableID;

                            goto exit;
                        }

                        if (OrderProduction2.Count != 0)
                        {
                            exe = OrderProduction2.Any(x => x.ProductionDate == psd[i].ProductionDate && x.Shift == psd[i].Shift);
                        }

                        if (OrderProduction2.Count == 0 || exe == true)
                        {
                            OrderProduction2.Add(new GradingProductionDetails()
                            {
                                RawProduct = new RawProduct() { RawProductID = psd[i].RawProduct.RawProductID, RawProductCode = psd[i].RawProduct.RawProductCode, Description = psd[i].RawProduct.Description, RawProductType = psd[i].RawProduct.RawProductType },
                                Customer = new Customer() { CompanyName = psd[i].Customer.CompanyName },
                                PDate = psd[i].PDate,
                                ProdTimeTableID = psd[i].ProdTimeTableID,
                                RequiredDate = Regex.Match(psd[i].RequiredDate, "^[^ ]+").Value,
                                MixingDate = psd[i].MixingDate,
                                MixingShift = psd[i].MixingShift,
                                SalesOrderId = psd[i].SalesOrderId,
                                RawProDetailsID = psd[i].RawProDetailsID,
                                BlockLogQty = Math.Ceiling(psd[i].BlockLogQty),
                                ProductionDate = psd[i].ProductionDate,
                                Shift = psd[i].Shift,
                                ShiftName = GetShiftNameByID(psd[i].Shift),
                                GradingFormula = psd[i].GradingFormula,
                                SalesOrder = psd[i].SalesOrder,
                                RowBackgroundColour = psd[i].RowBackgroundColour,
                                OrderType = psd[i].OrderType,
                                Notes = psd[i].Notes,
                                GradingActive = psd[i].GradingActive,
                                PrintCounter = psd[i].PrintCounter,
                                GradingSchedulingID = psd[i].GradingSchedulingID,
                                IsReqDateVisible = psd[i].ReqDateSelected == true ? "Visible" : "Collapsed",
                                ItemBackgroundColour = bdg.ConvertDayToColour(Convert.ToDateTime(psd[i].RequiredDate)),
                                FormulaOptions = psd[i].FormulaOptions,
                                FormulaOptionVisibility = psd[i].FormulaOptionVisibility,
                                RawProductMachine = new RawProductMachine() { GradingMachineID = psd[i].RawProductMachine.GradingMachineID }
                            });
                            TempOrderProduction2 = OrderProduction2;
                            goto exit;
                        }

                        if (OrderProduction3.Count != 0)
                        {
                            exe = OrderProduction3.Any(x => x.ProductionDate == psd[i].ProductionDate && x.Shift == psd[i].Shift);
                        }

                        if (OrderProduction3.Count == 0 || exe == true)
                        {
                            OrderProduction3.Add(new GradingProductionDetails()
                            {
                                RawProduct = new RawProduct() { RawProductID = psd[i].RawProduct.RawProductID, RawProductCode = psd[i].RawProduct.RawProductCode, Description = psd[i].RawProduct.Description, RawProductType = psd[i].RawProduct.RawProductType },
                                Customer = new Customer() { CompanyName = psd[i].Customer.CompanyName },
                                PDate = psd[i].PDate,
                                ProdTimeTableID = psd[i].ProdTimeTableID,
                                RequiredDate = Regex.Match(psd[i].RequiredDate, "^[^ ]+").Value,
                                MixingDate = psd[i].MixingDate,
                                MixingShift = psd[i].MixingShift,
                                SalesOrderId = psd[i].SalesOrderId,
                                RawProDetailsID = psd[i].RawProDetailsID,
                                BlockLogQty = Math.Ceiling(psd[i].BlockLogQty),
                                ProductionDate = psd[i].ProductionDate,
                                Shift = psd[i].Shift,
                                ShiftName = GetShiftNameByID(psd[i].Shift),
                                GradingFormula = psd[i].GradingFormula,
                                SalesOrder = psd[i].SalesOrder,
                                RowBackgroundColour = psd[i].RowBackgroundColour,
                                OrderType = psd[i].OrderType,
                                Notes = psd[i].Notes,
                                GradingActive = psd[i].GradingActive,
                                PrintCounter = psd[i].PrintCounter,
                                GradingSchedulingID = psd[i].GradingSchedulingID,
                                IsReqDateVisible = psd[i].ReqDateSelected == true ? "Visible" : "Collapsed",
                                ItemBackgroundColour = bdg.ConvertDayToColour(Convert.ToDateTime(psd[i].RequiredDate)),
                                FormulaOptions = psd[i].FormulaOptions,
                                FormulaOptionVisibility = psd[i].FormulaOptionVisibility,
                                RawProductMachine = new RawProductMachine() { GradingMachineID = psd[i].RawProductMachine.GradingMachineID }
                            });
                            TempOrderProduction3 = OrderProduction3;
                            goto exit;
                        }

                        if (OrderProduction4.Count != 0)
                        {
                            exe = OrderProduction4.Any(x => x.ProductionDate == psd[i].ProductionDate && x.Shift == psd[i].Shift);
                        }

                        if (OrderProduction4.Count == 0 || exe == true)
                        {
                            OrderProduction4.Add(new GradingProductionDetails()
                            {
                                RawProduct = new RawProduct() { RawProductID = psd[i].RawProduct.RawProductID, RawProductCode = psd[i].RawProduct.RawProductCode, Description = psd[i].RawProduct.Description, RawProductType = psd[i].RawProduct.RawProductType },
                                Customer = new Customer() { CompanyName = psd[i].Customer.CompanyName },
                                PDate = psd[i].PDate,
                                ProdTimeTableID = psd[i].ProdTimeTableID,
                                RequiredDate = Regex.Match(psd[i].RequiredDate, "^[^ ]+").Value,
                                MixingDate = psd[i].MixingDate,
                                MixingShift = psd[i].MixingShift,
                                SalesOrderId = psd[i].SalesOrderId,
                                RawProDetailsID = psd[i].RawProDetailsID,
                                BlockLogQty = Math.Ceiling(psd[i].BlockLogQty),
                                ProductionDate = psd[i].ProductionDate,
                                Shift = psd[i].Shift,
                                ShiftName = GetShiftNameByID(psd[i].Shift),
                                GradingFormula = psd[i].GradingFormula,
                                SalesOrder = psd[i].SalesOrder,
                                RowBackgroundColour = psd[i].RowBackgroundColour,
                                OrderType = psd[i].OrderType,
                                Notes = psd[i].Notes,
                                GradingActive = psd[i].GradingActive,
                                PrintCounter = psd[i].PrintCounter,
                                GradingSchedulingID = psd[i].GradingSchedulingID,
                                IsReqDateVisible = psd[i].ReqDateSelected == true ? "Visible" : "Collapsed",
                                ItemBackgroundColour = bdg.ConvertDayToColour(Convert.ToDateTime(psd[i].RequiredDate)),
                                FormulaOptions = psd[i].FormulaOptions,
                                FormulaOptionVisibility = psd[i].FormulaOptionVisibility,
                                RawProductMachine = new RawProductMachine() { GradingMachineID = psd[i].RawProductMachine.GradingMachineID }
                            });

                            TempOrderProduction4 = OrderProduction4;
                            goto exit;
                        }
                    exit: ;

                    

                    TempDailyOrderProduction.Add(new GradingProductionDetails()
                        {
                            RawProduct = new RawProduct() 
                            { 
                                RawProductID = psd[i].RawProduct.RawProductID, 
                                RawProductCode = psd[i].RawProduct.RawProductCode,
                                Description = psd[i].RawProduct.Description + " X " + Math.Ceiling(psd[i].BlockLogQty) + " " + psd[i].RawProduct.RawProductType + AddS(Math.Ceiling(psd[i].BlockLogQty)), 
                                RawProductType = psd[i].RawProduct.RawProductType 
                            },
                            Customer = new Customer() { CompanyName = psd[i].Customer.CompanyName },
                            GradeString = psd[i].GradeString,
                            PDate = psd[i].PDate,
                            ProdTimeTableID = psd[i].ProdTimeTableID,
                            RequiredDate = Regex.Match(psd[i].RequiredDate, "^[^ ]+").Value,
                            MixingDate = psd[i].MixingDate,
                            MixingShift = psd[i].MixingShift,
                            SalesOrderId = psd[i].SalesOrderId,
                            RawProDetailsID = psd[i].RawProDetailsID,
                            BlockLogQty = Math.Ceiling(psd[i].BlockLogQty),
                            ProductionDate = psd[i].ProductionDate,
                            Shift = psd[i].Shift,
                            ShiftName = GetShiftNameByID(psd[i].Shift),
                            GradingFormula = psd[i].GradingFormula,
                            SalesOrder = psd[i].SalesOrder,
                            RowBackgroundColour = psd[i].RowBackgroundColour,
                            OrderType = psd[i].OrderType,
                            Notes = psd[i].Notes,
                            GradingActive = psd[i].GradingActive,
                            PrintCounter = psd[i].PrintCounter,
                            GradingSchedulingID = psd[i].GradingSchedulingID,
                            IsReqDateVisible = psd[i].ReqDateSelected == true ? "Visible" : "Collapsed",
                            ItemBackgroundColour = bdg.ConvertDayToColour(Convert.ToDateTime(psd[i].RequiredDate)),
                            FormulaOptions = psd[i].FormulaOptions,
                            FormulaOptionVisibility = psd[i].FormulaOptionVisibility,
                            RawProductMachine = new RawProductMachine() { GradingMachineID = psd[i].RawProductMachine.GradingMachineID }
                        });
                    }

                    var result = TempDailyOrderProduction.Distinct(new GradingItemEqualityComparer());
                    DailyOrderProduction = new ObservableCollection<GradingProductionDetails>(result);

                    foreach (var itemOP1 in OrderProduction1)
                    {
                        Grid1ShiftShow = "Visible";
                        Day1Show = "Visible";
                        Day1 = itemOP1.PDate.DayOfWeek + " " + itemOP1.PDate.ToString("dd/MM/yyyy");
                        Grid1Shift = GetShiftName(itemOP1.Shift);
                        Grid1ShiftBackColor = GridColorBDay(Convert.ToString(itemOP1.PDate.DayOfWeek), itemOP1.Shift);
                        Grid2ShiftShow = "Visible";
                        Grid3ShiftShow = "Visible";
                        Grid4ShiftShow = "Visible";
                        Day1and2Show = "Hidden";
                        Day1and2and3Show = "Hidden";
                        Day1and2 = string.Empty;
                        Day1and2and3 = string.Empty;
                        Grid2Shift = string.Empty;
                        Grid3Shift = string.Empty;
                        Grid4Shift = string.Empty;

                        Day1BackCol = GetHeadingColByDay(Convert.ToString(itemOP1.PDate.DayOfWeek));
                        DG1BackGround = GridColorBDay(Convert.ToString(itemOP1.PDate.DayOfWeek), itemOP1.Shift);

                        foreach (var itemOP2 in OrderProduction2)
                        {
                            Grid2ShiftShow = "Visible";
                            Day2Show = "Visible";
                            Grid3ShiftShow = "Visible";
                            Grid4ShiftShow = "Visible";
                            Day2 = itemOP2.PDate.DayOfWeek + " " + itemOP2.PDate.ToString("dd/MM/yyyy");
                            Grid2Shift = GetShiftName(itemOP2.Shift);
                            Day1and2and3Show = "Hidden";
                            Day1and2Show = "Hidden";
                            Day2and3Show = "Hidden";
                            Day2and3and4Show = "Hidden";
                            Day1and2 = string.Empty;
                            Day2and3 = string.Empty;
                            Day2and3and4 = string.Empty;
                            Day1and2and3 = string.Empty;
                            Grid3Shift = string.Empty;
                            Grid4Shift = string.Empty;

                            Grid2ShiftBackColor = GridColorBDay(Convert.ToString(itemOP2.PDate.DayOfWeek), itemOP2.Shift);
                            Day2BackCol = GetHeadingColByDay(Convert.ToString(itemOP2.PDate.DayOfWeek));
                            DG2BackGround = GridColorBDay(Convert.ToString(itemOP2.PDate.DayOfWeek), itemOP2.Shift);

                            if (itemOP1.ProductionDate == itemOP2.ProductionDate)
                            {
                                Day1and2Show = "Visible";
                                Grid3ShiftShow = "Visible";
                                Grid4ShiftShow = "Visible";
                                Grid1ShiftShow = "Visible";
                                Grid2ShiftShow = "Visible";
                                Day1and2 = itemOP1.PDate.DayOfWeek + " " + itemOP1.PDate.ToString("dd/MM/yyyy");
                                Grid1Shift = GetShiftName(itemOP1.Shift);
                                Grid2Shift = GetShiftName(itemOP2.Shift);
                                Grid1ShiftBackColor = GridColorBDay(Convert.ToString(itemOP1.PDate.DayOfWeek), itemOP1.Shift);
                                Grid2ShiftBackColor = GridColorBDay(Convert.ToString(itemOP2.PDate.DayOfWeek), itemOP2.Shift);
                                Day1Show = "Hidden";
                                Day2Show = "Hidden";
                                Day2and3Show = "Hidden";
                                Day1and2and3Show = "Hidden";
                                Day2and3and4Show = "Hidden";
                                Day1 = string.Empty;
                                Day2 = string.Empty;
                                Day2and3 = string.Empty;
                                Day1and2and3 = string.Empty;
                                Day2and3and4 = string.Empty;
                                Grid3Shift = string.Empty;
                                Grid4Shift = string.Empty;

                                Day1and2BackCol = GetHeadingColByDay(Convert.ToString(itemOP2.PDate.DayOfWeek));
                                DG1BackGround = GridColorBDay(Convert.ToString(itemOP1.PDate.DayOfWeek), itemOP1.Shift);
                                DG2BackGround = GridColorBDay(Convert.ToString(itemOP2.PDate.DayOfWeek), itemOP2.Shift);
                            }


                            foreach (var itemOP3 in OrderProduction3)
                            {
                                Day3Show = "Visible";
                                Grid3ShiftShow = "Visible";
                                Day3 = itemOP3.PDate.DayOfWeek + " " + itemOP3.PDate.ToString("dd/MM/yyyy");
                                Grid3Shift = GetShiftName(itemOP3.Shift);
                                Day2and3Show = "Hidden";
                                Day1and2and3Show = "Hidden";
                                Day2and3and4Show = "Hidden";
                                Day3and4Show = "Hidden";
                                Grid4ShiftShow = "Hidden";
                                Day1and2and3 = string.Empty;
                                Day2and3and4 = string.Empty;
                                Day2and3 = string.Empty;
                                Day3and4 = string.Empty;
                                Grid4Shift = string.Empty;

                                Grid3ShiftBackColor = GridColorBDay(Convert.ToString(itemOP3.PDate.DayOfWeek), itemOP3.Shift);
                                Day3BackCol = GetHeadingColByDay(Convert.ToString(itemOP3.PDate.DayOfWeek));
                                DG3BackGround = GridColorBDay(Convert.ToString(itemOP3.PDate.DayOfWeek), itemOP3.Shift);

                                if (itemOP1.ProductionDate == itemOP2.ProductionDate && itemOP2.ProductionDate == itemOP3.ProductionDate)
                                {
                                    Grid1ShiftShow = "Visible";
                                    Grid2ShiftShow = "Visible";
                                    Grid3ShiftShow = "Visible";
                                    Day1and2and3Show = "Visible";
                                    Day1and2and3 = itemOP2.PDate.DayOfWeek + " " + itemOP2.PDate.ToString("dd/MM/yyyy");
                                    Grid1Shift = GetShiftName(itemOP1.Shift);
                                    Grid2Shift = GetShiftName(itemOP2.Shift);
                                    Grid3Shift = GetShiftName(itemOP3.Shift);
                                    Grid1ShiftBackColor = GridColorBDay(Convert.ToString(itemOP1.PDate.DayOfWeek), itemOP1.Shift);
                                    Grid2ShiftBackColor = GridColorBDay(Convert.ToString(itemOP2.PDate.DayOfWeek), itemOP2.Shift);
                                    Grid3ShiftBackColor = GridColorBDay(Convert.ToString(itemOP3.PDate.DayOfWeek), itemOP3.Shift);
                                    Day1Show = "Hidden";
                                    Day2Show = "Hidden";
                                    Day3Show = "Hidden";
                                    Day1and2Show = "Hidden";
                                    Day2and3Show = "Hidden";
                                    Day3and4Show = "Hidden";
                                    Day2and3and4Show = "Hidden";
                                    Grid4ShiftShow = "Hidden";
                                    Day1 = string.Empty;
                                    Day2 = string.Empty;
                                    Day3 = string.Empty;
                                    Day1and2 = string.Empty;
                                    Day2and3 = string.Empty;
                                    Day3and4 = string.Empty;
                                    Day2and3and4 = string.Empty;
                                    Grid4Shift = string.Empty;

                                    Day1and2and3BackCol = GetHeadingColByDay(Convert.ToString(itemOP3.PDate.DayOfWeek));
                                    DG1BackGround = GridColorBDay(Convert.ToString(itemOP1.PDate.DayOfWeek), itemOP1.Shift);
                                    DG2BackGround = GridColorBDay(Convert.ToString(itemOP2.PDate.DayOfWeek), itemOP2.Shift);
                                    DG3BackGround = GridColorBDay(Convert.ToString(itemOP3.PDate.DayOfWeek), itemOP3.Shift);

                                }
                                else if (itemOP2.ProductionDate == itemOP3.ProductionDate)
                                {
                                    Grid2ShiftShow = "Visible";
                                    Grid3ShiftShow = "Visible";
                                    Day2and3Show = "Visible";
                                    Day2and3 = itemOP3.PDate.DayOfWeek + " " + itemOP3.PDate.ToString("dd/MM/yyyy");
                                    Grid2Shift = GetShiftName(itemOP2.Shift);
                                    Grid3Shift = GetShiftName(itemOP3.Shift);
                                    Day2Show = "Hidden";
                                    Day3Show = "Hidden";
                                    Day1and2 = "Hidden";
                                    Day3and4 = "Hidden";
                                    Day1and2and3Show = "Hidden";
                                    Day2and3and4Show = "Hidden";
                                    Grid4ShiftShow = "Hidden";
                                    Day2 = string.Empty;
                                    Day3 = string.Empty;
                                    Day1and2 = string.Empty;
                                    Day3and4 = string.Empty;
                                    Day1and2and3 = string.Empty;
                                    Day2and3and4 = string.Empty;
                                    Grid4Shift = string.Empty;

                                    Day2and3BackCol = GetHeadingColByDay(Convert.ToString(itemOP3.PDate.DayOfWeek));
                                    Day2BackCol = "#4a4a4a";
                                    Day3BackCol = "#4a4a4a";

                                    Grid2ShiftBackColor = GridColorBDay(Convert.ToString(itemOP2.PDate.DayOfWeek), itemOP2.Shift);
                                    Grid3ShiftBackColor = GridColorBDay(Convert.ToString(itemOP3.PDate.DayOfWeek), itemOP3.Shift);
                                    DG2BackGround = GridColorBDay(Convert.ToString(itemOP2.PDate.DayOfWeek), itemOP2.Shift);
                                    DG3BackGround = GridColorBDay(Convert.ToString(itemOP3.PDate.DayOfWeek), itemOP3.Shift);
                                }

                                foreach (var itemOP4 in OrderProduction4)
                                {
                                    Grid4ShiftShow = "Visible";
                                    Day4Show = "Visible";
                                    Day4 = itemOP4.PDate.DayOfWeek + " " + itemOP4.PDate.ToString("dd/MM/yyyy");
                                    Grid4Shift = GetShiftName(itemOP4.Shift);
                                    Day3and4 = "Hidden";
                                    Day2and3and4Show = "Hidden";
                                    Day2and3and4 = string.Empty;
                                    Day3and4 = string.Empty;

                                    Grid4ShiftBackColor = GridColorBDay(Convert.ToString(itemOP4.PDate.DayOfWeek), itemOP4.Shift);
                                    Day4BackCol = GetHeadingColByDay(Convert.ToString(itemOP4.PDate.DayOfWeek));
                                    DG4BackGround = GridColorBDay(Convert.ToString(itemOP4.PDate.DayOfWeek), itemOP4.Shift);

                                    if (itemOP2.ProductionDate == itemOP3.ProductionDate && itemOP3.ProductionDate == itemOP4.ProductionDate)
                                    {
                                        Grid2ShiftShow = "Visible";
                                        Grid3ShiftShow = "Visible";
                                        Grid4ShiftShow = "Visible";
                                        Day2and3and4Show = "Visible";
                                        Day2and3and4 = itemOP2.PDate.DayOfWeek + " " + itemOP2.PDate.ToString("dd/MM/yyyy");
                                        Grid2Shift = GetShiftName(itemOP2.Shift);
                                        Grid3Shift = GetShiftName(itemOP3.Shift);
                                        Grid4Shift = GetShiftName(itemOP4.Shift);
                                        Day2Show = "Hidden";
                                        Day3Show = "Hidden";
                                        Day4Show = "Hidden";
                                        Day2and3Show = "Hidden";
                                        Day3and4Show = "Hidden";
                                        Day1and2and3Show = "Hidden";
                                        Day2 = string.Empty;
                                        Day3 = string.Empty;
                                        Day4 = string.Empty;
                                        Day2and3 = "Hidden";
                                        Day3and4 = "Hidden";
                                        Day1and2and3 = "Hidden";
                                        Day2and3and4BackCol = GetHeadingColByDay(Convert.ToString(itemOP4.PDate.DayOfWeek));
                                        Day2BackCol = "#626262";
                                        Day3BackCol = "#626262";
                                        Day4BackCol = "#626262";

                                        Grid2ShiftBackColor = GridColorBDay(Convert.ToString(itemOP2.PDate.DayOfWeek), itemOP2.Shift);
                                        Grid3ShiftBackColor = GridColorBDay(Convert.ToString(itemOP3.PDate.DayOfWeek), itemOP3.Shift);
                                        Grid4ShiftBackColor = GridColorBDay(Convert.ToString(itemOP4.PDate.DayOfWeek), itemOP4.Shift);
                                        DG2BackGround = GridColorBDay(Convert.ToString(itemOP2.PDate.DayOfWeek), itemOP2.Shift);
                                        DG3BackGround = GridColorBDay(Convert.ToString(itemOP3.PDate.DayOfWeek), itemOP3.Shift);
                                        DG4BackGround = GridColorBDay(Convert.ToString(itemOP4.PDate.DayOfWeek), itemOP4.Shift);
                                    }
                                    else if (itemOP3.ProductionDate == itemOP4.ProductionDate)
                                    {
                                        Day3and4Show = "Visible";
                                        Grid3ShiftShow = "Visible";
                                        Grid4ShiftShow = "Visible";
                                        Day3and4 = itemOP3.PDate.DayOfWeek + " " + itemOP3.PDate.ToString("dd/MM/yyyy");
                                        Grid3Shift = GetShiftName(itemOP3.Shift);
                                        Grid4Shift = GetShiftName(itemOP4.Shift);
                                        Day3Show = "Hidden";
                                        Day4Show = "Hidden";
                                        Day2and3Show = "Hidden";
                                        Day2and3and4Show = "Hidden";
                                        Day3 = string.Empty;
                                        Day4 = string.Empty;
                                        Day2and3 = string.Empty;
                                        Day2and3and4 = string.Empty;

                                        Day3and4BackCol = GetHeadingColByDay(Convert.ToString(itemOP4.PDate.DayOfWeek));
                                        DG3BackGround = GridColorBDay(Convert.ToString(itemOP3.PDate.DayOfWeek), itemOP3.Shift);
                                        DG4BackGround = GridColorBDay(Convert.ToString(itemOP4.PDate.DayOfWeek), itemOP4.Shift);
                                    }
                                }
                            }
                        }
                    }

                    if (OrderProduction1 != null)
                    {
                        if (OrderProduction1.Count == 0)
                        {
                            Grid1ShiftShow = "Hidden";
                            Day1Show = "Hidden";
                            Day1and2Show = "Hidden";
                            Day1and2and3Show = "Hidden";
                            Grid1Shift = string.Empty;
                            Day1 = string.Empty;
                            Grid1ShiftBackColor = "#d8dfe5";
                            DG1BackGround = "#d8dfe5";
                            Day1and2BackCol = "#d8dfe5";
                            Day1and2and3BackCol = "#d8dfe5";
                        }
                    }
                    if (OrderProduction2 != null)
                    {
                        if (OrderProduction2.Count == 0)
                        {
                            Grid2ShiftShow = "Hidden";
                            Day2Show = "Hidden";
                            Day1and2Show = "Hidden";
                            Day2and3Show = "Hidden";
                            Day1and2and3Show = "Hidden";
                            Day2and3and4Show = "Hidden";
                            Grid2Shift = string.Empty;
                            Day2 = string.Empty;
                            Grid2ShiftBackColor = "#d8dfe5";
                            DG2BackGround = "#d8dfe5";
                            Day1and2BackCol = "#d8dfe5";
                            Day2and3BackCol = "#d8dfe5";
                            Day1and2and3BackCol = "#d8dfe5";
                            Day2and3and4BackCol = "#d8dfe5";
                        }
                    }
                    if (OrderProduction3 != null)
                    {
                        if (OrderProduction3.Count == 0)
                        {
                            Grid3ShiftShow = "Hidden";
                            Day3Show = "Hidden";
                            Day2and3Show = "Hidden";
                            Day3and4Show = "Hidden";
                            Day1and2and3Show = "Hidden";
                            Day2and3and4Show = "Hidden";
                            Grid3Shift = string.Empty;
                            Day3 = string.Empty;
                            Grid3ShiftBackColor = "#d8dfe5";
                            DG3BackGround = "#d8dfe5";
                            Day2and3BackCol = "#d8dfe5";
                            Day3and4BackCol = "#d8dfe5";
                            Day1and2and3BackCol = "#d8dfe5";
                            Day2and3and4BackCol = "#d8dfe5";
                        }
                    }
                    if (OrderProduction4 != null)
                    {
                        if (OrderProduction4.Count == 0)
                        {
                            Grid4ShiftShow = "Hidden";
                            Day4Show = "Hidden";
                            Day3and4Show = "Hidden";
                            Day2and3and4Show = "Hidden";
                            Grid4Shift = string.Empty;
                            Day4 = string.Empty;
                            Grid4ShiftBackColor = "#d8dfe5";
                            DG4BackGround = "#d8dfe5";
                            Day3and4BackCol = "#d8dfe5";
                            Day2and3and4BackCol = "#d8dfe5";
                        }
                    }                   
                }                                                                         
            });            
        }

        private string AddS(decimal x)
        {
            string s = string.Empty;
            if (x > 1)
            {
                s = "s";
            }
            return s;
        }

        private string GetShiftName(int s)
        {
            string sName = string.Empty;

            if (s == 1)
            {
                sName = " Day Shift";
            }
            else if (s == 2)
            {
                sName = " Afternoon Shift";
            }
            else if (s == 3)
            {
                sName = " Night Shift";
            }

            return sName;
        }

        private string GetHeadingColByDay(string day)
        {
            string col = string.Empty;
            switch (day)
            {
                case "Monday": col = "#d1e1fc"; break;
                case "Tuesday": col = "#f1d7cc"; break;
                case "Wednesday": col = "#cbd6d6"; break;
                case "Thursday": col = "#faf1aa"; break;
                case "Friday": col = "#ead9ca"; break;
                default: col = "#B67943"; break;
            }
            return col;
        }

        private string GridColorBDay(string day, int s)
        {
            string col = string.Empty;
            switch (day)
            {
                case "Monday":
                    if (s == 1)
                    {
                        col = "#d1e1fc";
                    }
                    else if (s == 2)
                    {
                        col = "#e0ebfd";
                    }
                    else if (s == 3)
                    {
                        col = "#eff5fe";
                    }
                    break;
                case "Tuesday":
                    if (s == 1)
                    {
                        col = "#f1d7cc";
                    }
                    else if (s == 2)
                    {
                        col = "#f4e1d9";
                    }
                    else if (s == 3)
                    {
                        col = "#f8ebe5";
                    }
                    break;
                case "Wednesday":
                    if (s == 1)
                    {
                        col = "#cbd6d6";
                    }
                    else if (s == 2)
                    {
                        col = "#d8e0e0";
                    }
                    else if (s == 3)
                    {
                        col = "#e5eaea";
                    }
                    break;
                case "Thursday":
                    if (s == 1)
                    {
                        col = "#faf1aa";
                    }
                    else if (s == 2)
                    {
                        col = "#fbf4bf";
                    }
                    else if (s == 3)
                    {
                        col = "#fcf8d4";
                    }
                    break;
                case "Friday":
                    if (s == 1)
                    {
                        col = "#ead9ca";
                    }
                    else if (s == 2)
                    {
                        col = "#efe2d7";
                    }
                    else if (s == 3)
                    {
                        col = "#f4ece4";
                    }
                    break;
                default: col = "#B67943"; break;
            }
            return col;
        }  


        void notifier_NewMessage(object sender, SqlNotificationEventArgs e)
        {
            // _index = 0;
            this.LoadData(this.productionSchedularNotifier.RegisterDependency());
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


        private void AddToGradedStock()
        {
            var childWindow = new ChildWindowView();
            //childWindow.addToGradedStock_Closed += (r =>
            //{

            //});
            childWindow.ShowAddGradedStock();
        }

        private void AddToRegrindStock()
        {
            var childWindow = new ChildWindowView();
            //childWindow.addToGradedStock_Closed += (r =>
            //{

            //});
            childWindow.ShowAddRegrindStock();
        }
               

        private void PrevDate()
        {
            bool re = DBAccess.GetSystemParameter("ChangingShiftForGrading");
            if (re == true)
            {
                Msg.Show("Orders are bieng shifted at the moment. Please try again in 5 minutes ", "Orders Shifting", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                 BackgroundWorker worker = new BackgroundWorker();
                LoadingScreen = new ChildWindowView();
                LoadingScreen.ShowWaitingScreen("Loading");

                worker.DoWork += (_, __) =>
                {
                    if (OrderProduction1 != null)
                    {
                        int n = 0;
                        int noOdItems = _gradingProductionDetails.Count();
                        DateTime d = DateTime.Now;

                        if (_NextClicked == true)
                        {
                            _NextClicked = false;
                        }
                        while (n == 0)
                        {
                            foreach (var itemOP in _gradingProductionDetails)
                            {
                                n = _gradingProductionDetails.Count(x => x.PDate.Date == _prodDate.Date && x.Shift == _shift);
                                _shift--;
                                if (_shift <= 0)
                                {
                                    _shift = 3;

                                }
                                if (_shift == 3)
                                {
                                    _prodDate = _prodDate.AddDays(-1);
                                    if (_prodDate <= d)
                                    {
                                        _prodDate = d;
                                    }
                                }
                                break;
                            }
                        }
                        _index -= n;

                        if (_index < 0)
                        {
                            _index = 0;
                        }

                        LoadData(_gradingProductionDetails);
                        EnDisPrevButton();
                        EnDisnextButton(noOdItems);
                    }
                };

                worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    LoadingScreen.CloseWaitingScreen();

                };
                worker.RunWorkerAsync();
            }
        }

        private DateTime _prodDate;
        private bool _NextClicked;
        private void NextDate()
        {

            bool re = DBAccess.GetSystemParameter("ChangingShiftForGrading");
            if (re == true)
            {
                Msg.Show("Orders are bieng shifted at the moment. Please try again in 5 minutes ", "Orders Shifting", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                BackgroundWorker worker = new BackgroundWorker();
                LoadingScreen = new ChildWindowView();
                LoadingScreen.ShowWaitingScreen("Loading");

                worker.DoWork += (_, __) =>
                {

                    if (OrderProduction1 != null)
                    {
                        int noOfItems = _gradingProductionDetails.Count();
                        int n = OrderProduction1.Count;
                        int n1 = OrderProduction2.Count;

                        if (_index < noOfItems && n1 > 0)
                        {
                            foreach (var item in OrderProduction1)
                            {
                                _prodDate = item.PDate;
                                _shift = item.Shift;
                                break;
                            }

                            _index += n;

                            _NextClicked = true;
                            LoadData(_gradingProductionDetails);
                            EnDisnextButton(noOfItems);
                        }
                    }
                };

                worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                {
                    LoadingScreen.CloseWaitingScreen();

                };
                worker.RunWorkerAsync();
            }

        }

        private void EnDisPrevButton()
        {
            if (_index == 0)
            {
                PrevEnabled = false;
            }
            else
            {
                PrevEnabled = true;
            }
        }

        private void EnDisnextButton(int noOdItems)
        {
            noOdItems -= 1;
            if (_index == noOdItems)
            {
                NextEnabled = false;
            }
            else
            {
                NextEnabled = true;
            }
        }

        private void GoHome()
        {
            productionSchedularNotifier.Dispose();
            Switcher.Switch(new MainMenu(userName, state, privilages, metaData));
        }

        private void GoToProduction()
        {
            productionSchedularNotifier.Dispose();
            Switcher.Switch(new WorkStationsView(userName, state, privilages, metaData));
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

        private int GetMachineIdByTabIndex(int m)
        {
            int x = 0;
            if (m == 1)
            {
                x = 1;
            }
            else if (m == 2)
            {
                x = 7;
            }
            else if (m == 0)
            {
                x = 0;
            }
            return x;
        }

        private void OpenMixingSchedule()
        {
            var childWindow = new ChildWindowView();
            childWindow.ShowMixingWeeklySchedule();
        }

        private void OrderView(string s)
        {
            if(s == "daily")
            {
                ControlVisibility = "Collapsed";
                DailyDataGridVisibility = "Visible";
                if (this.productionSchedularNotifier != null)
                {
                    _index = 0;
                    this.LoadData(this.productionSchedularNotifier.RegisterDependency());
                }
            }
            else if(s == "weekly")
            {
                ControlVisibility = "Visible";
                DailyDataGridVisibility = "Collapsed";
            }
        }

        #region PUBLIC PROPERTIES


        public string DailyDataGridVisibility
        {
            get
            {
                return _dailyDataGridVisibility;
            }
            set
            {
                _dailyDataGridVisibility = value;
                RaisePropertyChanged(() => this.DailyDataGridVisibility);
            }
        }

        public string ControlVisibility
        {
            get
            {
                return _controlVisibility;
            }
            set
            {
                _controlVisibility = value;
                RaisePropertyChanged(() => this.ControlVisibility);
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

        public ObservableCollection<GradingProductionDetails> OrderProduction
        {
            get
            {
                return _orderProduction;
            }
            set
            {
                _orderProduction = value;
            }
        }

        public ObservableCollection<GradingProductionDetails> OrderProduction1
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

        public ObservableCollection<GradingProductionDetails> DailyOrderProduction
        {
            get
            {
                return _dailyOrderProduction;
            }

            set
            {
                _dailyOrderProduction = value;
                RaisePropertyChanged(() => this.DailyOrderProduction);
            }
        }



        public ObservableCollection<GradingProductionDetails> OrderProduction2
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

        public ObservableCollection<GradingProductionDetails> OrderProduction3
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

        public ObservableCollection<GradingProductionDetails> OrderProduction4
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

        public ObservableCollection<GradingProductionDetails> TempOrderProduction1
        {
            get
            {
                return _tempOrderProduction1;
            }

            set
            {
                _tempOrderProduction1 = value;
                RaisePropertyChanged(() => this.TempOrderProduction1);
            }
        }

        public ObservableCollection<GradingProductionDetails> TempOrderProduction2
        {
            get
            {
                return _tempOrderProduction2;
            }

            set
            {
                _tempOrderProduction2 = value;
                RaisePropertyChanged(() => this.TempOrderProduction2);
            }
        }

        public ObservableCollection<GradingProductionDetails> TempOrderProduction3
        {
            get
            {
                return _tempOrderProduction3;
            }

            set
            {
                _tempOrderProduction3 = value;
                RaisePropertyChanged(() => this.TempOrderProduction3);
            }
        }

        public ObservableCollection<GradingProductionDetails> TempOrderProduction4
        {
            get
            {
                return _tempOrderProduction4;
            }

            set
            {
                _tempOrderProduction4 = value;
                RaisePropertyChanged(() => this.TempOrderProduction4);
            }
        }

        //public ObservableCollection<GradingProductionDetails> OrderProduction5
        //{
        //    get
        //    {
        //        return _orderProduction5;
        //    }
        //    set
        //    {
        //        _orderProduction5 = value;
        //        RaisePropertyChanged(() => this.OrderProduction5);
        //    }
        //}

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

        public string Day1and2
        {
            get { return _day1and2; }
            set { _day1and2 = value; RaisePropertyChanged(() => this.Day1and2); }
        }

        public string Day2and3
        {
            get { return _day2and3; }
            set { _day2and3 = value; RaisePropertyChanged(() => this.Day2and3); }
        }

        public string Day3and4
        {
            get { return _day3and4; }
            set { _day3and4 = value; RaisePropertyChanged(() => this.Day3and4); }
        }


        public string Day1Show
        {
            get { return _day1Show; }
            set { _day1Show = value; RaisePropertyChanged(() => this.Day1Show); }
        }

        public string Day2Show
        {
            get { return _day2Show; }
            set { _day2Show = value; RaisePropertyChanged(() => this.Day2Show); }
        }

        public string Day3Show
        {
            get { return _day3Show; }
            set { _day3Show = value; RaisePropertyChanged(() => this.Day3Show); }
        }

        public string Day4Show
        {
            get { return _day4Show; }
            set { _day4Show = value; RaisePropertyChanged(() => this.Day4Show); }
        }


        public string Day1and2Show
        {
            get { return _day1and2Show; }
            set { _day1and2Show = value; RaisePropertyChanged(() => this.Day1and2Show); }
        }

        public string Day2and3Show
        {
            get { return _day2and3Show; }
            set { _day2and3Show = value; RaisePropertyChanged(() => this.Day2and3Show); }
        }

        public string Day3and4Show
        {
            get { return _day3and4Show; }
            set { _day3and4Show = value; RaisePropertyChanged(() => this.Day3and4Show); }
        }



        public string Day1and2and3Show
        {
            get { return _day1and2and3Show; }
            set { _day1and2and3Show = value; RaisePropertyChanged(() => this.Day1and2and3Show); }
        }

        public string Day2and3and4Show
        {
            get { return _day2and3and4Show; }
            set { _day2and3and4Show = value; RaisePropertyChanged(() => this.Day2and3and4Show); }
        }

        public string Day1and2and3
        {
            get { return _day1and2and3; }
            set { _day1and2and3 = value; RaisePropertyChanged(() => this.Day1and2and3); }
        }

        public string Day2and3and4
        {
            get { return _day2and3and4; }
            set { _day2and3and4 = value; RaisePropertyChanged(() => this.Day2and3and4); }
        }



        public string Grid1Shift
        {
            get { return _grid1Shift; }
            set { _grid1Shift = value; RaisePropertyChanged(() => this.Grid1Shift); }
        }

        public string Grid2Shift
        {
            get { return _grid2Shift; }
            set { _grid2Shift = value; RaisePropertyChanged(() => this.Grid2Shift); }
        }

        public string Grid3Shift
        {
            get { return _grid3Shift; }
            set { _grid3Shift = value; RaisePropertyChanged(() => this.Grid3Shift); }
        }

        public string Grid4Shift
        {
            get { return _grid4Shift; }
            set { _grid4Shift = value; RaisePropertyChanged(() => this.Grid4Shift); }
        }

        public string Grid1ShiftShow
        {
            get { return _grid1ShiftShow; }
            set { _grid1ShiftShow = value; RaisePropertyChanged(() => this.Grid1ShiftShow); }
        }

        public string Grid2ShiftShow
        {
            get { return _grid2ShiftShow; }
            set { _grid2ShiftShow = value; RaisePropertyChanged(() => this.Grid2ShiftShow); }
        }

        public string Grid3ShiftShow
        {
            get { return _grid3ShiftShow; }
            set { _grid3ShiftShow = value; RaisePropertyChanged(() => this.Grid3ShiftShow); }
        }

        public string Grid4ShiftShow
        {
            get { return _grid4ShiftShow; }
            set { _grid4ShiftShow = value; RaisePropertyChanged(() => this.Grid4ShiftShow); }
        }

        public string DG1BackGround
        {
            get { return _dG1BackGround; }
            set { _dG1BackGround = value; RaisePropertyChanged(() => this.DG1BackGround); }
        }

        public string DG2BackGround
        {
            get { return _dG2BackGround; }
            set { _dG2BackGround = value; RaisePropertyChanged(() => this.DG2BackGround); }
        }

        public string DG3BackGround
        {
            get { return _dG3BackGround; }
            set { _dG3BackGround = value; RaisePropertyChanged(() => this.DG3BackGround); }
        }

        public string DG4BackGround
        {
            get { return _dG4BackGround; }
            set { _dG4BackGround = value; RaisePropertyChanged(() => this.DG4BackGround); }
        }



        public string Day1BackCol
        {
            get { return _day1BackCol; }
            set { _day1BackCol = value; RaisePropertyChanged(() => this.Day1BackCol); }
        }

        public string Day2BackCol
        {
            get { return _day2BackCol; }
            set { _day2BackCol = value; RaisePropertyChanged(() => this.Day2BackCol); }
        }

        public string Day3BackCol
        {
            get { return _day3BackCol; }
            set { _day3BackCol = value; RaisePropertyChanged(() => this.Day3BackCol); }
        }

        public string Day4BackCol
        {
            get { return _day4BackCol; }
            set { _day4BackCol = value; RaisePropertyChanged(() => this.Day4BackCol); }
        }




        public string Day1and2BackCol
        {
            get { return _day1and2BackCol; }
            set { _day1and2BackCol = value; RaisePropertyChanged(() => this.Day1and2BackCol); }
        }

        public string Day2and3BackCol
        {
            get { return _day2and3BackCol; }
            set { _day2and3BackCol = value; RaisePropertyChanged(() => this.Day2and3BackCol); }
        }

        public string Day3and4BackCol
        {
            get { return _day3and4BackCol; }
            set { _day3and4BackCol = value; RaisePropertyChanged(() => this.Day3and4BackCol); }
        }

        public string Day1and2and3BackCol
        {
            get { return _day1and2and3BackCol; }
            set { _day1and2and3BackCol = value; RaisePropertyChanged(() => this.Day1and2and3BackCol); }
        }
        public string Day2and3and4BackCol
        {
            get { return _day2and3and4BackCol; }
            set { _day2and3and4BackCol = value; RaisePropertyChanged(() => this.Day2and3and4BackCol); }
        }





        public string Grid1ShiftBackColor
        {
            get { return _grid1ShiftBackColor; }
            set { _grid1ShiftBackColor = value; RaisePropertyChanged(() => this.Grid1ShiftBackColor); }
        }

        public string Grid2ShiftBackColor
        {
            get { return _grid2ShiftBackColor; }
            set { _grid2ShiftBackColor = value; RaisePropertyChanged(() => this.Grid2ShiftBackColor); }
        }

        public string Grid3ShiftBackColor
        {
            get { return _grid3ShiftBackColor; }
            set { _grid3ShiftBackColor = value; RaisePropertyChanged(() => this.Grid3ShiftBackColor); }
        }

        public string Grid4ShiftBackColor
        {
            get { return _grid4ShiftBackColor; }
            set { _grid4ShiftBackColor = value; RaisePropertyChanged(() => this.Grid4ShiftBackColor); }
        }

        public bool PrevEnabled
        {
            get { return _prevEnabled; }
            set { _prevEnabled = value; RaisePropertyChanged(() => this.PrevEnabled); }
        }

        public bool NextEnabled
        {
            get { return _nextEnabled; }
            set { _nextEnabled = value; RaisePropertyChanged(() => this.NextEnabled); }
        }

        private ObservableCollection<GradingProductionDetails> LoadDataForTabClick(ObservableCollection<GradingProductionDetails> gpd,int machineId)
        {
            ObservableCollection<GradingProductionDetails> newGpd = new ObservableCollection<GradingProductionDetails>();

            foreach (var item in gpd)
            {
                if (machineId == 0)
                {
                    newGpd.Add(item);
                }
                else if (item.RawProductMachine != null && item.RawProductMachine.GradingMachineID == machineId)
                {
                    newGpd.Add(item);
                }
            }           
            return newGpd;
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
                    OrderProduction1.Clear();

                    OrderProduction1 = LoadDataForTabClick(TempOrderProduction1, GetMachineIdByTabIndex(TabMachineSelectedIndex1));

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
                    OrderProduction2.Clear();

                    OrderProduction2 = LoadDataForTabClick(TempOrderProduction2, GetMachineIdByTabIndex(TabMachineSelectedIndex2));  
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
                    OrderProduction3.Clear();

                    OrderProduction3 = LoadDataForTabClick(TempOrderProduction3, GetMachineIdByTabIndex(TabMachineSelectedIndex3));  
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
                    OrderProduction4.Clear();

                    OrderProduction4 = LoadDataForTabClick(TempOrderProduction4, GetMachineIdByTabIndex(TabMachineSelectedIndex4));  
                }
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


        public ObservableCollection<Product> GradeDetails
        {
            get { return _gradeDetails; }
            set
            {
                _gradeDetails = value;
                RaisePropertyChanged(() => this.GradeDetails);
            }
        }  

        
        

        #endregion

        #region COMMANDS
        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => GoHome(), _canExecute));
            }
        }

        public ICommand WorkStationsCommand
        {
            get
            {
                return _workStationsCommand ?? (_workStationsCommand = new LogOutCommandHandler(() => GoToProduction(), _canExecute));
            }
        }

        public ICommand GradedStockCommand
        {
            get
            {
                return _gradedStockCommand ?? (_gradedStockCommand = new LogOutCommandHandler(() => AddToGradedStock(), _canExecute));
            }
        }

        public ICommand RegrindStockCommand
        {
            get
            {
                return _regrindStockCommand ?? (_regrindStockCommand = new LogOutCommandHandler(() => AddToRegrindStock(), _canExecute));
            }
        }

        

        public ICommand PrevCommand
        {
            get
            {
                return _prevCommand ?? (_prevCommand = new LogOutCommandHandler(() => PrevDate(), _canExecute));
            }
        }

        public ICommand NextCommand
        {
            get
            {
                return _nextCommand ?? (_nextCommand = new LogOutCommandHandler(() => NextDate(), NextEnabled));
            }
        }
    
        public ICommand PrintGradingPendingCommand
        {
            get
            {
                return _printGradingPendingCommand ?? (_printGradingPendingCommand = new A1QSystem.Commands.LogOutCommandHandler(PrintGradingPending, _canExecute));
            }
        }

        public ICommand ViewMixingScheduleCommand
        {
            get
            {
                return _viewMixingScheduleCommand ?? (_viewMixingScheduleCommand = new A1QSystem.Commands.LogOutCommandHandler(OpenMixingSchedule, _canExecute));
            }
        }

        private static bool CanButtonPress(string button)
        {
            return true;
        }

        public ICommand DailyOrdersCommand
        {
            get
            {
                if (_dailyOrdersCommand == null)
                {
                    _dailyOrdersCommand = new DelegateCommand<string>(OrderView, CanButtonPress);
                }
                return _dailyOrdersCommand;
            }
        }

        public ICommand WeeklyOrdersCommand
        {
            get
            {
                if (_weeklyOrdersCommand == null)
                {
                    _weeklyOrdersCommand = new DelegateCommand<string>(OrderView, CanButtonPress);
                }
                return _weeklyOrdersCommand;
            }
        }        

        #endregion
    }
}
