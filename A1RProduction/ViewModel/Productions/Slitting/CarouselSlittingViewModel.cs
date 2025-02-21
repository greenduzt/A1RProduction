using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Production.Slitting;
using A1QSystem.Model.Products;
using A1QSystem.Model.Products.Tiles;
using A1QSystem.Model.Shifts;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using A1QSystem.View.Dashboard;
using A1QSystem.View.Production.Slitting;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace A1QSystem.ViewModel.Productions.Slitting
{
    public class CarouselSlittingViewModel : ViewModelBase
    {
        private int _index;
        private Int32 firstGridId;
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
        private string _printAllOrdersVis1;
        private string _printAllOrdersVis2;
        private string _printAllOrdersVis3;
        private string _printAllOrdersVis4;
        //private string _grid1Visiblity1;
        //private string _grid1Visiblity2;
        //private string _grid1Visiblity3;
        //private string _grid1Visiblity4;
        private bool _prevEnabled;
        private bool _nextEnabled;
        private DateTime _prodDate;
        private bool _NextClicked;
        private int _shift;
        private string _version;
        private ObservableCollection<SlittingOrder> _slittingOrder = null;
        private ObservableCollection<SlittingOrder> _slittingOrder1 = null;
        private ObservableCollection<SlittingOrder> _slittingOrder2 = null;
        private ObservableCollection<SlittingOrder> _slittingOrder3 = null;
        private ObservableCollection<SlittingOrder> _slittingOrder4 = null;
        private List<DateTime> _datesList;
        private List<MetaData> metaData;
        public List<Shift> ShiftDetails { get; set; }
        private ChildWindowView LoadingScreen;
        private ICommand _prevCommand;
        private ICommand _nextCommand;
        private ICommand _navHomeCommand;
        private ICommand _workStationsCommand;
        private ICommand _printAll1;
        private ICommand _printAll2;
        private ICommand _printAll3;
        private ICommand _printAll4;
        private bool canExecute;
        private DateTime CurrentDate;
        private string userName;
        private string state;
        private List<UserPrivilages> privilages;

        public Dispatcher UIDispatcher { get; set; }
        public SlittingOrdersNotifier SlittingOrdersNotifier { get; set; }

        private ICommand navHomeCommand;

        public CarouselSlittingViewModel(string UserName, string State, List<UserPrivilages> Privilages, Dispatcher uidispatcher, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = Privilages;
            canExecute = true;
            _index = 0;
            _shift = 0;
            PrevEnabled = true;
            NextEnabled = true;
            _NextClicked = false;
            metaData = md;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
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
            PrintAllOrdersVis1 = "Visible";
            PrintAllOrdersVis2 = "Visible";
            PrintAllOrdersVis3 = "Visible";
            PrintAllOrdersVis4 = "Visible";
            CurrentDate = NTPServer.GetNetworkTime();
            ShiftDetails = DBAccess.GetAllShifts();
            firstGridId = 0;
            this.UIDispatcher = uidispatcher;

            this.SlittingOrdersNotifier = new SlittingOrdersNotifier();

            this.SlittingOrdersNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage);
            ObservableCollection<SlittingOrder> opd = this.SlittingOrdersNotifier.RegisterDependency(8);

            this.LoadData(opd);        
        }


        private void LoadData(ObservableCollection<SlittingOrder> psd)
        {
            EnDisPrevButton();

            this.UIDispatcher.BeginInvoke((Action)delegate()
            {
                if (psd != null)
                {

                     BackgroundWorker worker = new BackgroundWorker();
                     LoadingScreen = new ChildWindowView();
                     LoadingScreen.ShowWaitingScreen("Processing");
                 
                     worker.DoWork += (_, __) =>
                     {
                         string D1 = string.Empty;
                         string D2 = string.Empty;
                         string D3 = string.Empty;
                         string D4 = string.Empty;
                         DateTime currentDate = DateTime.Now.Date;
                         SlittingOrder1 = new ObservableCollection<SlittingOrder>();
                         SlittingOrder2 = new ObservableCollection<SlittingOrder>();
                         SlittingOrder3 = new ObservableCollection<SlittingOrder>();
                         SlittingOrder4 = new ObservableCollection<SlittingOrder>();

                         //Make sure that the dates are sorted
                         psd = new ObservableCollection<SlittingOrder>(psd.OrderBy(i => i.SlittingDate).ThenBy(i => i.Shift.ShiftID).ThenBy(i => i.Order.OrderType));
                         _slittingOrder = psd;

                         //Add all dates
                         _datesList = new List<DateTime>();
                         foreach (var item in psd)
                         {
                             bool c = _datesList.Any(x => x == item.SlittingDate);
                             if (c == false)
                             {
                                 _datesList.Add(item.SlittingDate);
                             }
                         }

                         System.Windows.Application.Current.Dispatcher.Invoke(() =>
                         {

                             for (int i = _index; i < psd.Count; i++)
                             {
                                 bool exe = false;

                                 if (SlittingOrder1.Count != 0)
                                 {
                                     exe = SlittingOrder1.Any(x => x.ProdTimetableID == psd[i].ProdTimetableID && x.Shift.ShiftID == psd[i].Shift.ShiftID);
                                 }

                                 if (SlittingOrder1.Count == 0 || exe == true)
                                 {
                                     SlittingOrder1.Add(new SlittingOrder()
                                     {
                                         ID = psd[i].ID,
                                         ProdTimetableID = psd[i].ProdTimetableID,
                                         Product = new Product()
                                         {
                                             RawProduct = new RawProduct()
                                             {
                                                 RawProductID = psd[i].Product.RawProduct.RawProductID,
                                                 RawProductType = psd[i].Product.RawProduct.RawProductType,
                                                 Description = psd[i].Product.RawProduct.Description
                                             },
                                             ProductID = psd[i].Product.ProductID,
                                             ProductCode = psd[i].Product.ProductCode,
                                             ProductName = psd[i].Product.ProductName,
                                             ProductDescription = psd[i].Product.ProductDescription,
                                             ProductUnit = psd[i].Product.ProductUnit,
                                             UnitPrice = psd[i].Product.UnitPrice,
                                             Tile = new Tile()
                                             {
                                                 Thickness = Math.Round(psd[i].Product.Tile.Thickness, 0),
                                                 MaxYield = psd[i].Product.Tile.MaxYield
                                             },

                                         },
                                         Blocks = Math.Round(psd[i].Blocks, 0),
                                         Qty = Math.Round(psd[i].Qty, 0),
                                         DollarValue = psd[i].DollarValue,
                                         Order = new Order()
                                         {
                                             OrderNo = psd[i].Order.OrderNo,
                                             SalesNo = psd[i].Order.SalesNo,
                                             OrderType = psd[i].Order.OrderType,
                                             Comments = psd[i].Order.Comments,
                                             Customer = new Customer() { CompanyName = psd[i].Order.Customer.CompanyName }
                                         },
                                         Shift = psd[i].Shift,
                                         SlittingDate = psd[i].SlittingDate,
                                         Status = psd[i].Status,
                                         RowBackgroundColour = psd[i].RowBackgroundColour,
                                         BottomRowVisibility = psd[i].Order.OrderType == 2 || psd[i].Order.OrderType == 1 ? "Visible" : "Collapsed"

                                     });

                                     firstGridId = psd[i].ProdTimetableID;

                                     goto exit;
                                 }

                                 if (SlittingOrder2.Count != 0)
                                 {
                                     exe = SlittingOrder2.Any(x => x.ProdTimetableID == psd[i].ProdTimetableID && x.Shift.ShiftID == psd[i].Shift.ShiftID);
                                 }

                                 if (SlittingOrder2.Count == 0 || exe == true)
                                 {
                                     SlittingOrder2.Add(new SlittingOrder()
                                     {
                                         ID = psd[i].ID,
                                         ProdTimetableID = psd[i].ProdTimetableID,
                                         Product = new Product()
                                         {
                                             RawProduct = new RawProduct()
                                             {
                                                 RawProductID = psd[i].Product.RawProduct.RawProductID,
                                                 RawProductType = psd[i].Product.RawProduct.RawProductType,
                                                 Description = psd[i].Product.RawProduct.Description
                                             },
                                             ProductID = psd[i].Product.ProductID,
                                             ProductCode = psd[i].Product.ProductCode,
                                             ProductName = psd[i].Product.ProductName,
                                             ProductDescription = psd[i].Product.ProductDescription,
                                             ProductUnit = psd[i].Product.ProductUnit,
                                             UnitPrice = psd[i].Product.UnitPrice,
                                             Tile = new Tile()
                                             {
                                                 Thickness = Math.Round(psd[i].Product.Tile.Thickness, 0),
                                                 MaxYield = psd[i].Product.Tile.MaxYield
                                             },
                                         },
                                         Blocks = Math.Round(psd[i].Blocks, 0),
                                         Qty = Math.Round(psd[i].Qty, 0),
                                         DollarValue = psd[i].DollarValue,
                                         Order = new Order()
                                         {
                                             OrderNo = psd[i].Order.OrderNo,
                                             SalesNo = psd[i].Order.SalesNo,
                                             OrderType = psd[i].Order.OrderType,
                                             Comments = psd[i].Order.Comments,
                                             Customer = new Customer() { CompanyName = psd[i].Order.Customer.CompanyName }
                                         },
                                         Shift = psd[i].Shift,
                                         SlittingDate = psd[i].SlittingDate,
                                         Status = psd[i].Status,
                                         RowBackgroundColour = psd[i].RowBackgroundColour,
                                         BottomRowVisibility = psd[i].Order.OrderType == 2 || psd[i].Order.OrderType == 1 ? "Visible" : "Collapsed"
                                     });

                                     firstGridId = psd[i].ProdTimetableID;

                                     goto exit;
                                 }

                                 if (SlittingOrder3.Count != 0)
                                 {
                                     exe = SlittingOrder3.Any(x => x.ProdTimetableID == psd[i].ProdTimetableID && x.Shift.ShiftID == psd[i].Shift.ShiftID);
                                 }

                                 if (SlittingOrder3.Count == 0 || exe == true)
                                 {
                                     SlittingOrder3.Add(new SlittingOrder()
                                     {
                                         ID = psd[i].ID,
                                         ProdTimetableID = psd[i].ProdTimetableID,
                                         Product = new Product()
                                         {
                                             RawProduct = new RawProduct()
                                             {
                                                 RawProductID = psd[i].Product.RawProduct.RawProductID,
                                                 RawProductType = psd[i].Product.RawProduct.RawProductType,
                                                 Description = psd[i].Product.RawProduct.Description
                                             },
                                             ProductID = psd[i].Product.ProductID,
                                             ProductCode = psd[i].Product.ProductCode,
                                             ProductName = psd[i].Product.ProductName,
                                             ProductDescription = psd[i].Product.ProductDescription,
                                             ProductUnit = psd[i].Product.ProductUnit,
                                             UnitPrice = psd[i].Product.UnitPrice,
                                             Tile = new Tile()
                                             {
                                                 Thickness = Math.Round(psd[i].Product.Tile.Thickness, 0),
                                                 MaxYield = psd[i].Product.Tile.MaxYield
                                             },
                                         },
                                         Blocks = Math.Round(psd[i].Blocks, 0),
                                         Qty = Math.Round(psd[i].Qty, 0),
                                         DollarValue = psd[i].DollarValue,
                                         Order = new Order()
                                         {
                                             OrderNo = psd[i].Order.OrderNo,
                                             SalesNo = psd[i].Order.SalesNo,
                                             OrderType = psd[i].Order.OrderType,
                                             Comments = psd[i].Order.Comments,
                                             Customer = new Customer() { CompanyName = psd[i].Order.Customer.CompanyName }
                                         },
                                         Shift = psd[i].Shift,
                                         SlittingDate = psd[i].SlittingDate,
                                         Status = psd[i].Status,
                                         RowBackgroundColour = psd[i].RowBackgroundColour,
                                         BottomRowVisibility = psd[i].Order.OrderType == 2 || psd[i].Order.OrderType == 1 ? "Visible" : "Collapsed"
                                     });

                                     firstGridId = psd[i].ProdTimetableID;

                                     goto exit;
                                 }

                                 if (SlittingOrder4.Count != 0)
                                 {
                                     exe = SlittingOrder4.Any(x => x.ProdTimetableID == psd[i].ProdTimetableID && x.Shift.ShiftID == psd[i].Shift.ShiftID);
                                 }

                                 if (SlittingOrder4.Count == 0 || exe == true)
                                 {
                                     SlittingOrder4.Add(new SlittingOrder()
                                     {
                                         ID = psd[i].ID,
                                         ProdTimetableID = psd[i].ProdTimetableID,
                                         Product = new Product()
                                         {
                                             RawProduct = new RawProduct()
                                             {
                                                 RawProductID = psd[i].Product.RawProduct.RawProductID,
                                                 RawProductType = psd[i].Product.RawProduct.RawProductType,
                                                 Description = psd[i].Product.RawProduct.Description
                                             },
                                             ProductID = psd[i].Product.ProductID,
                                             ProductCode = psd[i].Product.ProductCode,
                                             ProductName = psd[i].Product.ProductName,
                                             ProductDescription = psd[i].Product.ProductDescription,
                                             ProductUnit = psd[i].Product.ProductUnit,
                                             UnitPrice = psd[i].Product.UnitPrice,
                                             Tile = new Tile()
                                             {
                                                 Thickness = Math.Round(psd[i].Product.Tile.Thickness, 0),
                                                 MaxYield = psd[i].Product.Tile.MaxYield
                                             },
                                         },
                                         Blocks = Math.Round(psd[i].Blocks, 0),
                                         Qty = Math.Round(psd[i].Qty, 0),
                                         DollarValue = psd[i].DollarValue,
                                         Order = new Order()
                                         {
                                             OrderNo = psd[i].Order.OrderNo,
                                             SalesNo = psd[i].Order.SalesNo,
                                             OrderType = psd[i].Order.OrderType,
                                             Comments = psd[i].Order.Comments,
                                             Customer = new Customer() { CompanyName = psd[i].Order.Customer.CompanyName }
                                         },
                                         Shift = psd[i].Shift,
                                         SlittingDate = psd[i].SlittingDate,
                                         Status = psd[i].Status,
                                         RowBackgroundColour = psd[i].RowBackgroundColour,
                                         BottomRowVisibility = psd[i].Order.OrderType == 2 || psd[i].Order.OrderType == 1 ? "Visible" : "Collapsed"
                                     });

                                     firstGridId = psd[i].ProdTimetableID;

                                     goto exit;
                                 }
                             exit: ;
                             }
                         });
                         foreach (var itemOP1 in SlittingOrder1)
                         {
                             Grid1ShiftShow = "Visible";
                             Day1Show = "Visible";
                             Day1 = itemOP1.SlittingDate.DayOfWeek + " " + itemOP1.SlittingDate.ToString("dd/MM/yyyy");
                             Grid1Shift = GetShiftName(itemOP1.Shift.ShiftID);
                             Grid1ShiftBackColor = GridColorBDay(Convert.ToString(itemOP1.SlittingDate.DayOfWeek), itemOP1.Shift.ShiftID);
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
                             PrintAllOrdersVis1 = "Visible";

                             Day1BackCol = GetHeadingColByDay(Convert.ToString(itemOP1.SlittingDate.DayOfWeek));
                             DG1BackGround = GridColorBDay(Convert.ToString(itemOP1.SlittingDate.DayOfWeek), itemOP1.Shift.ShiftID);

                             foreach (var itemOP2 in SlittingOrder2)
                             {
                                 Grid2ShiftShow = "Visible";
                                 Day2Show = "Visible";
                                 Grid3ShiftShow = "Visible";
                                 Grid4ShiftShow = "Visible";
                                 Day2 = itemOP2.SlittingDate.DayOfWeek + " " + itemOP2.SlittingDate.ToString("dd/MM/yyyy");
                                 Grid2Shift = GetShiftName(itemOP2.Shift.ShiftID);
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
                                 PrintAllOrdersVis2 = "Visible";

                                 Grid2ShiftBackColor = GridColorBDay(Convert.ToString(itemOP2.SlittingDate.DayOfWeek), itemOP2.Shift.ShiftID);
                                 Day2BackCol = GetHeadingColByDay(Convert.ToString(itemOP2.SlittingDate.DayOfWeek));
                                 DG2BackGround = GridColorBDay(Convert.ToString(itemOP2.SlittingDate.DayOfWeek), itemOP2.Shift.ShiftID);

                                 if (itemOP1.ProdTimetableID == itemOP2.ProdTimetableID)
                                 {
                                     Day1and2Show = "Visible";
                                     Grid3ShiftShow = "Visible";
                                     Grid4ShiftShow = "Visible";
                                     Grid1ShiftShow = "Visible";
                                     Grid2ShiftShow = "Visible";
                                     Day1and2 = itemOP1.SlittingDate.DayOfWeek + " " + itemOP1.SlittingDate.ToString("dd/MM/yyyy");
                                     Grid1Shift = GetShiftName(itemOP1.Shift.ShiftID);
                                     Grid2Shift = GetShiftName(itemOP2.Shift.ShiftID);
                                     Grid1ShiftBackColor = GridColorBDay(Convert.ToString(itemOP1.SlittingDate.DayOfWeek), itemOP1.Shift.ShiftID);
                                     Grid2ShiftBackColor = GridColorBDay(Convert.ToString(itemOP2.SlittingDate.DayOfWeek), itemOP2.Shift.ShiftID);
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
                                     PrintAllOrdersVis1 = "Visible";
                                     PrintAllOrdersVis2 = "Visible";

                                     Day1and2BackCol = GetHeadingColByDay(Convert.ToString(itemOP2.SlittingDate.DayOfWeek));
                                     DG1BackGround = GridColorBDay(Convert.ToString(itemOP1.SlittingDate.DayOfWeek), itemOP1.Shift.ShiftID);
                                     DG2BackGround = GridColorBDay(Convert.ToString(itemOP2.SlittingDate.DayOfWeek), itemOP2.Shift.ShiftID);
                                 }
                                 foreach (var itemOP3 in SlittingOrder3)
                                 {
                                     Day3Show = "Visible";
                                     Grid3ShiftShow = "Visible";
                                     Day3 = itemOP3.SlittingDate.DayOfWeek + " " + itemOP3.SlittingDate.ToString("dd/MM/yyyy");
                                     Grid3Shift = GetShiftName(itemOP3.Shift.ShiftID);
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
                                     PrintAllOrdersVis3 = "Visible";

                                     Grid3ShiftBackColor = GridColorBDay(Convert.ToString(itemOP3.SlittingDate.DayOfWeek), itemOP3.Shift.ShiftID);
                                     Day3BackCol = GetHeadingColByDay(Convert.ToString(itemOP3.SlittingDate.DayOfWeek));
                                     DG3BackGround = GridColorBDay(Convert.ToString(itemOP3.SlittingDate.DayOfWeek), itemOP3.Shift.ShiftID);

                                     if (itemOP1.ProdTimetableID == itemOP2.ProdTimetableID && itemOP2.ProdTimetableID == itemOP3.ProdTimetableID)
                                     {
                                         Grid1ShiftShow = "Visible";
                                         Grid2ShiftShow = "Visible";
                                         Grid3ShiftShow = "Visible";
                                         Day1and2and3Show = "Visible";
                                         Day1and2and3 = itemOP2.SlittingDate.DayOfWeek + " " + itemOP2.SlittingDate.ToString("dd/MM/yyyy");
                                         Grid1Shift = GetShiftName(itemOP1.Shift.ShiftID);
                                         Grid2Shift = GetShiftName(itemOP2.Shift.ShiftID);
                                         Grid3Shift = GetShiftName(itemOP3.Shift.ShiftID);
                                         Grid1ShiftBackColor = GridColorBDay(Convert.ToString(itemOP1.SlittingDate.DayOfWeek), itemOP1.Shift.ShiftID);
                                         Grid2ShiftBackColor = GridColorBDay(Convert.ToString(itemOP2.SlittingDate.DayOfWeek), itemOP2.Shift.ShiftID);
                                         Grid3ShiftBackColor = GridColorBDay(Convert.ToString(itemOP3.SlittingDate.DayOfWeek), itemOP3.Shift.ShiftID);
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
                                         PrintAllOrdersVis1 = "Visible";
                                         PrintAllOrdersVis2 = "Visible";
                                         PrintAllOrdersVis3 = "Visible";

                                         Day1and2and3BackCol = GetHeadingColByDay(Convert.ToString(itemOP3.SlittingDate.DayOfWeek));
                                         DG1BackGround = GridColorBDay(Convert.ToString(itemOP1.SlittingDate.DayOfWeek), itemOP1.Shift.ShiftID);
                                         DG2BackGround = GridColorBDay(Convert.ToString(itemOP2.SlittingDate.DayOfWeek), itemOP2.Shift.ShiftID);
                                         DG3BackGround = GridColorBDay(Convert.ToString(itemOP3.SlittingDate.DayOfWeek), itemOP3.Shift.ShiftID);

                                     }
                                     else if (itemOP2.ProdTimetableID == itemOP3.ProdTimetableID)
                                     {
                                         Grid2ShiftShow = "Visible";
                                         Grid3ShiftShow = "Visible";
                                         Day2and3Show = "Visible";
                                         Day2and3 = itemOP3.SlittingDate.DayOfWeek + " " + itemOP3.SlittingDate.ToString("dd/MM/yyyy");
                                         Grid2Shift = GetShiftName(itemOP2.Shift.ShiftID);
                                         Grid3Shift = GetShiftName(itemOP3.Shift.ShiftID);
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
                                         PrintAllOrdersVis2 = "Visible";
                                         PrintAllOrdersVis3 = "Visible";

                                         Day2and3BackCol = GetHeadingColByDay(Convert.ToString(itemOP3.SlittingDate.DayOfWeek));
                                         Day2BackCol = "#4a4a4a";
                                         Day3BackCol = "#4a4a4a";

                                         Grid2ShiftBackColor = GridColorBDay(Convert.ToString(itemOP2.SlittingDate.DayOfWeek), itemOP2.Shift.ShiftID);
                                         Grid3ShiftBackColor = GridColorBDay(Convert.ToString(itemOP3.SlittingDate.DayOfWeek), itemOP3.Shift.ShiftID);
                                         DG2BackGround = GridColorBDay(Convert.ToString(itemOP2.SlittingDate.DayOfWeek), itemOP2.Shift.ShiftID);
                                         DG3BackGround = GridColorBDay(Convert.ToString(itemOP3.SlittingDate.DayOfWeek), itemOP3.Shift.ShiftID);
                                     }

                                     foreach (var itemOP4 in SlittingOrder4)
                                     {
                                         Grid4ShiftShow = "Visible";
                                         Day4Show = "Visible";
                                         Day4 = itemOP4.SlittingDate.DayOfWeek + " " + itemOP4.SlittingDate.ToString("dd/MM/yyyy");
                                         Grid4Shift = GetShiftName(itemOP4.Shift.ShiftID);
                                         Day3and4 = "Hidden";
                                         Day2and3and4Show = "Hidden";
                                         Day2and3and4 = string.Empty;
                                         Day3and4 = string.Empty;
                                         PrintAllOrdersVis4 = "Visible";

                                         Grid4ShiftBackColor = GridColorBDay(Convert.ToString(itemOP4.SlittingDate.DayOfWeek), itemOP4.Shift.ShiftID);
                                         Day4BackCol = GetHeadingColByDay(Convert.ToString(itemOP4.SlittingDate.DayOfWeek));
                                         DG4BackGround = GridColorBDay(Convert.ToString(itemOP4.SlittingDate.DayOfWeek), itemOP4.Shift.ShiftID);

                                         if (itemOP2.ProdTimetableID == itemOP3.ProdTimetableID && itemOP3.ProdTimetableID == itemOP4.ProdTimetableID)
                                         {
                                             Grid2ShiftShow = "Visible";
                                             Grid3ShiftShow = "Visible";
                                             Grid4ShiftShow = "Visible";
                                             Day2and3and4Show = "Visible";
                                             Day2and3and4 = itemOP2.SlittingDate.DayOfWeek + " " + itemOP2.SlittingDate.ToString("dd/MM/yyyy");
                                             Grid2Shift = GetShiftName(itemOP2.Shift.ShiftID);
                                             Grid3Shift = GetShiftName(itemOP3.Shift.ShiftID);
                                             Grid4Shift = GetShiftName(itemOP4.Shift.ShiftID);
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
                                             Day2and3and4BackCol = GetHeadingColByDay(Convert.ToString(itemOP4.SlittingDate.DayOfWeek));
                                             Day2BackCol = "#626262";
                                             Day3BackCol = "#626262";
                                             Day4BackCol = "#626262";
                                             PrintAllOrdersVis2 = "Visible";
                                             PrintAllOrdersVis3 = "Visible";
                                             PrintAllOrdersVis4 = "Visible";

                                             Grid2ShiftBackColor = GridColorBDay(Convert.ToString(itemOP2.SlittingDate.DayOfWeek), itemOP2.Shift.ShiftID);
                                             Grid3ShiftBackColor = GridColorBDay(Convert.ToString(itemOP3.SlittingDate.DayOfWeek), itemOP3.Shift.ShiftID);
                                             Grid4ShiftBackColor = GridColorBDay(Convert.ToString(itemOP4.SlittingDate.DayOfWeek), itemOP4.Shift.ShiftID);
                                             DG2BackGround = GridColorBDay(Convert.ToString(itemOP2.SlittingDate.DayOfWeek), itemOP2.Shift.ShiftID);
                                             DG3BackGround = GridColorBDay(Convert.ToString(itemOP3.SlittingDate.DayOfWeek), itemOP3.Shift.ShiftID);
                                             DG4BackGround = GridColorBDay(Convert.ToString(itemOP4.SlittingDate.DayOfWeek), itemOP4.Shift.ShiftID);
                                         }
                                         else if (itemOP3.ProdTimetableID == itemOP4.ProdTimetableID)
                                         {
                                             Day3and4Show = "Visible";
                                             Grid3ShiftShow = "Visible";
                                             Grid4ShiftShow = "Visible";
                                             Day3and4 = itemOP3.SlittingDate.DayOfWeek + " " + itemOP3.SlittingDate.ToString("dd/MM/yyyy");
                                             Grid3Shift = GetShiftName(itemOP3.Shift.ShiftID);
                                             Grid4Shift = GetShiftName(itemOP4.Shift.ShiftID);
                                             Day3Show = "Hidden";
                                             Day4Show = "Hidden";
                                             Day2and3Show = "Hidden";
                                             Day2and3and4Show = "Hidden";
                                             Day3 = string.Empty;
                                             Day4 = string.Empty;
                                             Day2and3 = string.Empty;
                                             Day2and3and4 = string.Empty;
                                             PrintAllOrdersVis3 = "Visible";
                                             PrintAllOrdersVis4 = "Visible";

                                             Day3and4BackCol = GetHeadingColByDay(Convert.ToString(itemOP4.SlittingDate.DayOfWeek));
                                             DG3BackGround = GridColorBDay(Convert.ToString(itemOP3.SlittingDate.DayOfWeek), itemOP3.Shift.ShiftID);
                                             DG4BackGround = GridColorBDay(Convert.ToString(itemOP4.SlittingDate.DayOfWeek), itemOP4.Shift.ShiftID);
                                         }
                                     }

                                 }

                             }
                         }

                         if (SlittingOrder1 != null)
                         {
                             if (SlittingOrder1.Count == 0)
                             {
                                 //Grid1Visiblity1 = "Collapsed";
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
                                 PrintAllOrdersVis1 = "Collapsed";
                             }
                             else
                             {
                                // Grid1Visiblity1 = "Visible";
                             }
                         }
                         else
                         {
                            // Grid1Visiblity1 = "Visible";
                         }
                         if (SlittingOrder2 != null)
                         {
                             if (SlittingOrder2.Count == 0)
                             {
                                // Grid1Visiblity2 = "Collapsed";
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
                                 PrintAllOrdersVis2 = "Collapsed";
                             }
                             else
                             {
                                // Grid1Visiblity2 = "Visible";
                             }
                         }
                         else
                         {
                            // Grid1Visiblity2 = "Visible";
                         }

                         if (SlittingOrder3 != null)
                         {
                             if (SlittingOrder3.Count == 0)
                             {
                                 // Grid1Visiblity3 = "Collapsed";
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
                                 PrintAllOrdersVis3 = "Collapsed";
                             }
                              else
                             {
                                // Grid1Visiblity3 = "Visible";
                             }
                         }
                         else
                         {
                           //  Grid1Visiblity3 = "Visible";
                         }
                         if (SlittingOrder4 != null)
                         {
                             if (SlittingOrder4.Count == 0)
                             {
                               //  Grid1Visiblity4 = "Collapsed";
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
                                 PrintAllOrdersVis4 = "Collapsed";
                             }
                             else
                             {
                                // Grid1Visiblity4 = "Visible";
                             }
                         }
                         else
                         {
                           //  Grid1Visiblity4 = "Visible";
                         }

                     };

                     worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                     {
                         LoadingScreen.CloseWaitingScreen();
                     };
                    worker.RunWorkerAsync();
                   
                }
             });                               
              
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
                case "Monday": col = "#4285F4"; break;
                case "Tuesday": col = "#c04c1d"; break;
                case "Wednesday": col = "#285555"; break;
                case "Thursday": col = "#c3b023"; break;
                case "Friday": col = "#B67943"; break;
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
                        col = "#679df6";
                    }
                    else if (s == 2)
                    {
                        col = "#2e5daa";
                    }
                    else if (s == 3)
                    {
                        col = "#1a3561";
                    }
                    break;
                case "Tuesday":
                    if (s == 1)
                    {
                        col = "#dc9c81";
                    }
                    else if (s == 2)
                    {
                        col = "#c04c1d";
                    }
                    else if (s == 3)
                    {
                        col = "#6f2202";
                    }
                    break;
                case "Wednesday":
                    if (s == 1)
                    {
                        col = "#7e9999";
                    }
                    else if (s == 2)
                    {
                        col = "#285555";
                    }
                    else if (s == 3)
                    {
                        col = "#142a2a";
                    }
                    break;
                case "Thursday":
                    if (s == 1)
                    {
                        col = "#f4dd2c";
                    }
                    else if (s == 2)
                    {
                        col = "#c3b023";
                    }
                    else if (s == 3)
                    {
                        col = "#92841a";
                    }
                    break;
                case "Friday":
                    if (s == 1)
                    {
                        col = "#cba17b";
                    }
                    else if (s == 2)
                    {
                        col = "#b67943";
                    }
                    else if (s == 3)
                    {
                        col = "#6d4828";
                    }
                    break;
                default: col = "#B67943"; break;
            }
            return col;
        }

        void notifier_NewMessage(object sender, SqlNotificationEventArgs e)
        {
            this.LoadData(this.SlittingOrdersNotifier.RegisterDependency(8));
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

        private void GoHome()
        {
            SlittingOrdersNotifier.Dispose();
            Switcher.Switch(new MainMenu(userName, state, privilages, metaData));
        }

        private void GoToProduction()
        {
            SlittingOrdersNotifier.Dispose();
            Switcher.Switch(new SlittingMenuView(userName, state, privilages, metaData));
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
                if (SlittingOrder1 != null)
                {
                    int n = 0;
                    int noOdItems = SlittingOrder.Count();
                    DateTime d = DateTime.Now;

                    if (_NextClicked == true)
                    {
                        _NextClicked = false;
                    }
                    while (n == 0)
                    {
                        foreach (var itemOP in SlittingOrder)
                        {
                            n = SlittingOrder.Count(x => x.SlittingDate.Date == _prodDate.Date && x.Shift.ShiftID == _shift);
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

                    LoadData(SlittingOrder);
                    EnDisPrevButton();
                    EnDisnextButton(noOdItems);
                }
            }
        }

        private void NextDate()
        {
            bool re = DBAccess.GetSystemParameter("ChangingShiftForGrading");
            if (re == true)
            {
                Msg.Show("Orders are bieng shifted at the moment. Please try again in 5 minutes ", "Orders Shifting", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                if (SlittingOrder1 != null)
                {


                    int noOdItems = SlittingOrder.Count();
                    int n = SlittingOrder1.Count;
                    int n1 = SlittingOrder2.Count;

                    if (_index < noOdItems && n1 > 0)
                    {
                        foreach (var item in SlittingOrder1)
                        {
                            _prodDate = item.SlittingDate;
                            _shift = item.Shift.ShiftID;
                            break;
                        }

                        _index += n;

                        _NextClicked = true;
                        LoadData(SlittingOrder);
                        EnDisnextButton(noOdItems);
                    }
                }
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

        private void PrintAllOrders(int x)
        {
            if (x > 0)
            {
                if (Msg.Show("Are you sure you want to print this slitting order?", "Printing Slitting Order", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    Exception exception = null;
                    BackgroundWorker worker = new BackgroundWorker();
                    ChildWindowView LoadingScreen = new ChildWindowView();
                    LoadingScreen.ShowWaitingScreen("Printing");

                    worker.DoWork += (_, __) =>
                    {

                        int curShift = 0;
                        PrintAllSlittingOrdersPDF pasopdf = null;
                        //Get current shift
                        foreach (var item in ShiftDetails)
                        {
                            bool isShift = TimeBetween(DateTime.Now, item.StartTime, item.EndTime);

                            if (isShift == true)
                            {
                                curShift = item.ShiftID;
                            }
                        }

                        switch (x)
                        {
                            case 1: pasopdf = new PrintAllSlittingOrdersPDF(SlittingOrder1, GetShiftName(curShift), "CAROUSEL");
                                exception=pasopdf.CreatePDF();
                                break;
                            case 2: pasopdf = new PrintAllSlittingOrdersPDF(SlittingOrder2, GetShiftName(curShift), "CAROUSEL");
                                exception = pasopdf.CreatePDF();
                                break;
                            case 3: pasopdf = new PrintAllSlittingOrdersPDF(SlittingOrder3, GetShiftName(curShift), "CAROUSEL");
                                exception = pasopdf.CreatePDF();
                                break;
                            case 4: pasopdf = new PrintAllSlittingOrdersPDF(SlittingOrder4, GetShiftName(curShift), "CAROUSEL");
                                exception = pasopdf.CreatePDF();
                                break;
                            default:
                                break;
                        }
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
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }


        //private void CompleteSlitting(Object parameter)
        //{
        //    int index = 0;
        //    var childWindow = new ChildWindowView();
            
        //    if(parameter.ToString() == "1")
        //    {
        //        index = SlittingOrder1.IndexOf(parameter as SlittingOrder);
        //        childWindow.ShowSlittingConfirmationView(SlittingOrder1[index]);
        //    }
        //    else  if(parameter.ToString() == "2")
        //    {
        //        index = SlittingOrder2.IndexOf(parameter as SlittingOrder);
        //        childWindow.ShowSlittingConfirmationView(SlittingOrder2[index]);
        //    }
        //    else if (parameter.ToString() == "3")
        //    {
        //        index = SlittingOrder3.IndexOf(parameter as SlittingOrder);
        //        childWindow.ShowSlittingConfirmationView(SlittingOrder3[index]);
        //    }
        //    else if (parameter.ToString() == "4")
        //    {
        //        index = SlittingOrder4.IndexOf(parameter as SlittingOrder);
        //        childWindow.ShowSlittingConfirmationView(SlittingOrder4[index]);
        //    }           
            
        //}  


        #region PUBLIC PROPERTIES

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

        //public string Grid1Visiblity1
        //{
        //    get { return _grid1Visiblity1; }
        //    set { _grid1Visiblity1 = value; RaisePropertyChanged(() => this.Grid1Visiblity1); }
        //}

        //public string Grid1Visiblity2
        //{
        //    get { return _grid1Visiblity2; }
        //    set { _grid1Visiblity2 = value; RaisePropertyChanged(() => this.Grid1Visiblity2); }
        //}

        //public string Grid1Visiblity3
        //{
        //    get { return _grid1Visiblity3; }
        //    set { _grid1Visiblity3 = value; RaisePropertyChanged(() => this.Grid1Visiblity3); }
        //}

        //public string Grid1Visiblity4
        //{
        //    get { return _grid1Visiblity4; }
        //    set { _grid1Visiblity4 = value; RaisePropertyChanged(() => this.Grid1Visiblity4); }
        //}

        public string PrintAllOrdersVis1
        {
            get { return _printAllOrdersVis1; }
            set { _printAllOrdersVis1 = value; RaisePropertyChanged(() => this.PrintAllOrdersVis1); }
        }

        public string PrintAllOrdersVis2
        {
            get { return _printAllOrdersVis2; }
            set { _printAllOrdersVis2 = value; RaisePropertyChanged(() => this.PrintAllOrdersVis2); }
        }

        public string PrintAllOrdersVis3
        {
            get { return _printAllOrdersVis3; }
            set { _printAllOrdersVis3 = value; RaisePropertyChanged(() => this.PrintAllOrdersVis3); }
        }

        public string PrintAllOrdersVis4
        {
            get { return _printAllOrdersVis4; }
            set { _printAllOrdersVis4 = value; RaisePropertyChanged(() => this.PrintAllOrdersVis4); }
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

        public ObservableCollection<SlittingOrder> SlittingOrder
        {
            get
            {
                return _slittingOrder;
            }
            set
            {
                _slittingOrder = value;
            }
        }

        public ObservableCollection<SlittingOrder> SlittingOrder1
        {
            get
            {
                return _slittingOrder1;
            }

            set
            {
                _slittingOrder1 = value;
                RaisePropertyChanged(() => this.SlittingOrder1);
            }
        }

        public ObservableCollection<SlittingOrder> SlittingOrder2
        {
            get
            {
                return _slittingOrder2;
            }
            set
            {
                _slittingOrder2 = value;
                RaisePropertyChanged(() => this.SlittingOrder2);
            }
        }

        public ObservableCollection<SlittingOrder> SlittingOrder3
        {
            get
            {
                return _slittingOrder3;
            }
            set
            {
                _slittingOrder3 = value;
                RaisePropertyChanged(() => this.SlittingOrder3);
            }
        }

        public ObservableCollection<SlittingOrder> SlittingOrder4
        {
            get
            {
                return _slittingOrder4;
            }
            set
            {
                _slittingOrder4 = value;
                RaisePropertyChanged(() => this.SlittingOrder4);
            }
        }
     

      
       

        #endregion

        #region COMMANDS

        //public ICommand CompleteCommand
        //{
        //    get
        //    {
        //        if (_completeCommand == null)
        //        {
        //            _completeCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, CompleteSlitting);
        //        }
        //        return _completeCommand;
        //    }
        //}
     
        public ICommand NavHomeCommand
        {
            get
            {
                return _navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => GoHome(), canExecute));
            }
        }

        public ICommand WorkStationsCommand
        {
            get
            {
                return _workStationsCommand ?? (_workStationsCommand = new LogOutCommandHandler(() => GoToProduction(), canExecute));
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
                return _nextCommand ?? (_nextCommand = new LogOutCommandHandler(() => NextDate(), NextEnabled));
            }
        }

        public ICommand PrintAll1
        {
            get
            {
                return _printAll1 ?? (_printAll1 = new LogOutCommandHandler(() => PrintAllOrders(1), NextEnabled));
            }
        }

        public ICommand PrintAll2
        {
            get
            {
                return _printAll2 ?? (_printAll2 = new LogOutCommandHandler(() => PrintAllOrders(2), NextEnabled));
            }
        }

        public ICommand PrintAll3
        {
            get
            {
                return _printAll3 ?? (_printAll3 = new LogOutCommandHandler(() => PrintAllOrders(3), NextEnabled));
            }
        }

        public ICommand PrintAll4
        {
            get
            {
                return _printAll4 ?? (_printAll4 = new LogOutCommandHandler(() => PrintAllOrders(4), NextEnabled));
            }
        }

        
        #endregion

    }
}
