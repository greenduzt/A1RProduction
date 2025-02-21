using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Production.Mixing;
using A1QSystem.Model.Production.SlitingPeeling;
using A1QSystem.Model.Products;
using A1QSystem.View;
using A1QSystem.View.Dashboard;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace A1QSystem.ViewModel.Productions.SlitPeel
{
    public class SlittingScheduleViewModel : ViewModelBase
    {
        private string _day1;
        private string _day2;
        private string _day3;
        private string _day4;
        private string _day5;
        private ListCollectionView _collDay1 = null;
        private ListCollectionView _collDay2 = null;
        private ListCollectionView _collDay3 = null;
        private ListCollectionView _collDay4 = null;
        private ListCollectionView _collDay5 = null;
        private ObservableCollection<SlittingProductionDetails> _MixingProduction = null;
        private ObservableCollection<SlittingProductionDetails> _mixingProduction1 = null;
        private ObservableCollection<SlittingProductionDetails> _mixingProduction2 = null;
        private ObservableCollection<SlittingProductionDetails> _mixingProduction3 = null;
        private ObservableCollection<SlittingProductionDetails> _mixingProduction4 = null;
        private ObservableCollection<SlittingProductionDetails> _mixingProduction5 = null;
        private int _tabSelectedIndex1;
        private int _tabSelectedIndex2;
        private int _tabSelectedIndex3;
        private int _tabSelectedIndex4;
        private int _tabSelectedIndex5;
        private bool _canExecute;
        private DateTime CurrentDate;
        public int TotOrdersPending { get; set; }
        private string userName;
        private string state;
        private List<UserPrivilages> privilages;
        private List<MetaData> metaData;

        private ICommand navHomeCommand;
        private ICommand _workStattionCommand;
        private bool canExecute;

        public Dispatcher UIDispatcher { get; set; }
        public SlittingOrdersNotifier mixingOrdersNotifier { get; set; }


        public SlittingScheduleViewModel(string UserName, string State, List<UserPrivilages> Privilages, Dispatcher uidispatcher, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            privilages = Privilages;
            canExecute = true;
            metaData = md;
            CurrentDate = NTPServer.GetNetworkTime();                      

            TabSelectedIndex1 = 0;
            TabSelectedIndex2 = 0;
            TabSelectedIndex3 = 0;
            TabSelectedIndex4 = 0;
            TabSelectedIndex5 = 0;

            GenerateWorkingDays();

            this.UIDispatcher = uidispatcher;

            this.mixingOrdersNotifier = new SlittingOrdersNotifier();

            this.mixingOrdersNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage);
            ObservableCollection<SlittingProductionDetails> opd = null;// this.mixingOrdersNotifier.RegisterDependency();           

            this.LoadData(opd);        
        }

        private void LoadData(ObservableCollection<SlittingProductionDetails> psd)
        {


            this.UIDispatcher.BeginInvoke((Action)delegate()
            {
                if (psd != null)
                {
                    string D1 = Day1.Remove(0, Day1.IndexOf(' ') + 1);
                    string D2 = Day2.Remove(0, Day2.IndexOf(' ') + 1);
                    string D3 = Day3.Remove(0, Day3.IndexOf(' ') + 1);
                    string D4 = Day4.Remove(0, Day4.IndexOf(' ') + 1);
                    string D5 = Day5.Remove(0, Day5.IndexOf(' ') + 1);

                    MixingProduction1 = AddToBusinessDates(psd, Convert.ToDateTime(D1));
                    MixingProduction2 = AddToBusinessDates(psd, Convert.ToDateTime(D2));
                    MixingProduction3 = AddToBusinessDates(psd, Convert.ToDateTime(D3));
                    MixingProduction4 = AddToBusinessDates(psd, Convert.ToDateTime(D4));
                    MixingProduction5 = AddToBusinessDates(psd, Convert.ToDateTime(D5));

                    CollDay1 = new ListCollectionView(AddToTabs(MixingProduction1, TabSelectedIndex1));
                    CollDay1.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("SlitPeel.Shift", System.ComponentModel.ListSortDirection.Ascending));
                   
                    CollDay2 = new ListCollectionView(AddToTabs(MixingProduction2, TabSelectedIndex2));
                    CollDay2.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("SlitPeel.Shift", System.ComponentModel.ListSortDirection.Ascending));

                    CollDay3 = new ListCollectionView(AddToTabs(MixingProduction3, TabSelectedIndex3));
                    CollDay3.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("SlitPeel.Shift", System.ComponentModel.ListSortDirection.Ascending));

                    CollDay4 = new ListCollectionView(AddToTabs(MixingProduction4, TabSelectedIndex4));
                    CollDay4.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("SlitPeel.Shift", System.ComponentModel.ListSortDirection.Ascending));

                    CollDay5 = new ListCollectionView(AddToTabs(MixingProduction5, TabSelectedIndex5));
                    CollDay5.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("SlitPeel.Shift", System.ComponentModel.ListSortDirection.Ascending));
                }
            });
        }

        private ObservableCollection<SlittingProductionDetails> AddToBusinessDates(ObservableCollection<SlittingProductionDetails> prodSchDetails, DateTime date)
        {
            MixingProduction = new ObservableCollection<SlittingProductionDetails>();
            prodSchDetails = new ObservableCollection<SlittingProductionDetails>(prodSchDetails.OrderBy(i => i.OrderType));
  
            foreach (var item in prodSchDetails)
            {              

                if (item.SlitPeel.OriginalBlockLogs <= item.RawStock.Qty && item.RawStock.Qty != 0)
                {

                    if (Convert.ToDateTime(item.SlitPeel.ProductionDate) == date || (Convert.ToDateTime(item.SlitPeel.ProductionDate) < date && date == Convert.ToDateTime(CurrentDate)))
                    {

                        //MixingProduction.Add(new SlittingProductionDetails()
                        //{
                        //    Product = new Product()
                        //    {
                        //        ProductID = item.Product.ProductID,
                        //        RawProductID = item.Product.RawProductID,
                        //        ProductDescription = item.Product.ProductDescription,
                        //        ProductUnit = item.Product.ProductUnit,
                        //        Thickness = item.Product.Thickness,
                        //        MaxItemsPer = item.Product.MaxItemsPer
                        //    },
                        //    Customer = new Customer()
                        //    {
                        //        CustomerId = item.Customer.CustomerId,
                        //        CompanyName = item.Customer.CompanyName
                        //    },
                        //    SlitPeel = new Model.Production.SlitPeel()
                        //    {
                        //        ProdTimeTableID = item.SlitPeel.ProdTimeTableID,
                        //        ID = item.SlitPeel.ID,
                        //        //SlitPeelID = item.SlitPeelProduction.SlitPeelID,
                        //        QtyToMake = Math.Ceiling(item.SlitPeel.QtyToMake),
                        //        OriginalBlockLogs = Math.Ceiling(item.SlitPeel.OriginalBlockLogs),
                        //        ProductionDate = item.SlitPeel.ProductionDate,
                        //        Shift = item.SlitPeel.Shift
                        //    },
                        //    ShiftName = GetShiftNameByID(item.SlitPeel.Shift),
                        //    RawStock = new Model.Stock.RawStock()
                        //    {
                        //        Qty = item.RawStock.Qty
                        //    },
                        //    //BlocksDone = BlockLogCalculator(Math.Ceiling(item.SlitPeel.QtyToMake), item.Product.MaxItemsPer),
                        //    SlittingStartTime = DateTime.Now,
                        //    salesOrderID = item.salesOrderID,
                        //    SalesOrder = item.SalesOrder,
                        //    FreightName = item.FreightName,
                        //    //ArrivalDateTime = item.FreightArrDate.ToString("dd/MM/yyyy") + " " + String.Format("{0:HH:mm:ss}", item.FreightArrTime),
                        //    //FreightArrDate = Convert.ToDateTime(item.FreightArrDate.ToString("dd/MM/yyyy")),
                        //    //FreightDateAvailable = item.FreightDateAvailable,
                        //    FreightArrTime = String.Format("{0:HH:mm:ss}", item.FreightArrTime),
                        //    FreightTimeAvailable = item.FreightTimeAvailable,
                        //    RowBackgroundColour = item.RowBackgroundColour
                        //});

                    }
                }
            } 
            return MixingProduction;
        }

        private ObservableCollection<SlittingProductionDetails> AddToTabs(ObservableCollection<SlittingProductionDetails> prodSchDetails, int TabSelectedIndex)
        {

            MixingProduction = new ObservableCollection<SlittingProductionDetails>();
            prodSchDetails = new ObservableCollection<SlittingProductionDetails>(prodSchDetails.OrderBy(i => i.OrderType));
            foreach (var item in prodSchDetails)
            {
                if (item.SlitPeel.OriginalBlockLogs <= item.RawStock.Qty && item.RawStock.Qty != 0)
                {
                    if (item.SlitPeel.Shift == TabSelectedIndex)
                    {

                        MixingProduction.Add(new SlittingProductionDetails()
                        {
                        //    Product = new Product()
                        //    {
                        //        ProductID = item.Product.ProductID,
                        //        RawProductID = item.Product.RawProductID,
                        //        ProductDescription = item.Product.ProductDescription,
                        //        ProductUnit = item.Product.ProductUnit,
                        //        Thickness = item.Product.Thickness,
                        //        MaxItemsPer = item.Product.MaxItemsPer
                        //    },
                        //    Customer = new Customer()
                        //    {
                        //        CustomerId = item.Customer.CustomerId,
                        //        CompanyName = item.Customer.CompanyName
                        //    },
                        //    SlitPeel = new Model.Production.SlitPeel()
                        //    {
                        //        ProdTimeTableID = item.SlitPeel.ProdTimeTableID,
                        //        ID = item.SlitPeel.ID,
                        //        //SlitPeelID = item.SlitPeelProduction.SlitPeelID,
                        //        QtyToMake = Math.Ceiling(item.SlitPeel.QtyToMake),
                        //        OriginalBlockLogs = Math.Ceiling(item.SlitPeel.OriginalBlockLogs),
                        //        ProductionDate = item.SlitPeel.ProductionDate,
                        //        Shift = item.SlitPeel.Shift
                        //    },
                        //    ShiftName = GetShiftNameByID(item.SlitPeel.Shift),
                        //    RawStock = new Model.Stock.RawStock()
                        //    {
                        //        Qty = item.RawStock.Qty
                        //    },
                        //    //BlocksDone = BlockLogCalculator(Math.Ceiling(item.SlitPeel.QtyToMake), item.Product.MaxItemsPer),
                        //    SlittingStartTime = DateTime.Now,
                        //    salesOrderID = item.salesOrderID,
                        //    SalesOrder = item.SalesOrder,
                        //    FreightName = item.FreightName,
                        //    //ArrivalDateTime = item.FreightArrDate.ToString("dd/MM/yyyy") + " " + String.Format("{0:HH:mm:ss}", item.FreightArrTime),
                        //    //FreightArrDate = Convert.ToDateTime(item.FreightArrDate.ToString("dd/MM/yyyy")),
                        //    //FreightDateAvailable = item.FreightDateAvailable,
                        //    FreightArrTime = String.Format("{0:HH:mm:ss}", item.FreightArrTime),
                        //    FreightTimeAvailable = item.FreightTimeAvailable,
                        //    RowBackgroundColour = item.RowBackgroundColour
                        });
                    }
                    else if (TabSelectedIndex == 0)
                    {
                        MixingProduction = prodSchDetails;
                    }
                }
            }

            return MixingProduction;
        }


        void notifier_NewMessage(object sender, SqlNotificationEventArgs e)
        {
            //this.LoadData(this.mixingOrdersNotifier.RegisterDependency());
        }

        private decimal BlockLogCalculator(decimal qty, decimal yield)
        {
            decimal res = 0;
            if (yield != 0)
            {
                res = Math.Ceiling(qty / yield);
            }
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

        private void GenerateWorkingDays()
        {
            DateTime today = DateTime.Today;

            List<BusinessDays> bs = SubtractBusinessDays(today, 5);
            for (int i = 0; i < bs.Count; i++)
            {
                if (i == 0)
                {
                    Day1 = bs[i].DayName + " " + bs[i].Day;
                }
                else if (i == 1)
                {
                    Day2 = bs[i].DayName + " " + bs[i].Day;
                }
                else if (i == 2)
                {
                    Day3 = bs[i].DayName + " " + bs[i].Day;
                }
                else if (i == 3)
                {
                    Day4 = bs[i].DayName + " " + bs[i].Day;
                }
                else if (i == 4)
                {
                    Day5 = bs[i].DayName + " " + bs[i].Day;
                }
            }
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

        #region PUBLIC PROPERTIES


        public ObservableCollection<SlittingProductionDetails> MixingProduction
        {
            get
            {
                return _MixingProduction;
            }
            set
            {
                _MixingProduction = value;
            }
        }

        public ObservableCollection<SlittingProductionDetails> MixingProduction1
        {
            get
            {
                return _mixingProduction1;
            }

            set
            {
                _mixingProduction1 = value;
                RaisePropertyChanged(() => this.MixingProduction1);
            }
        }

        public ObservableCollection<SlittingProductionDetails> MixingProduction2
        {
            get
            {
                return _mixingProduction2;
            }
            set
            {
                _mixingProduction2 = value;
                RaisePropertyChanged(() => this.MixingProduction2);
            }
        }

        public ObservableCollection<SlittingProductionDetails> MixingProduction3
        {
            get
            {
                return _mixingProduction3;
            }
            set
            {
                _mixingProduction3 = value;
                RaisePropertyChanged(() => this.MixingProduction3);
            }
        }

        public ObservableCollection<SlittingProductionDetails> MixingProduction4
        {
            get
            {
                return _mixingProduction4;
            }
            set
            {
                _mixingProduction4 = value;
                RaisePropertyChanged(() => this.MixingProduction4);
            }
        }

        public ObservableCollection<SlittingProductionDetails> MixingProduction5
        {
            get
            {
                return _mixingProduction5;
            }
            set
            {
                _mixingProduction5 = value;
                RaisePropertyChanged(() => this.MixingProduction5);
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
            set { _day1 = value; }
        }

        public string Day2
        {
            get { return _day2; }
            set { _day2 = value; }
        }

        public string Day3
        {
            get { return _day3; }
            set { _day3 = value; }
        }

        public string Day4
        {
            get { return _day4; }
            set { _day4 = value; }
        }

        public string Day5
        {
            get { return _day5; }
            set { _day5 = value; }
        }

        public int TabSelectedIndex1
        {
            get { return _tabSelectedIndex1; }
            set
            {
                _tabSelectedIndex1 = value;
                RaisePropertyChanged(() => this.TabSelectedIndex1);
                if (MixingProduction1 != null)
                {
                    CollDay1 = new ListCollectionView(AddToTabs(MixingProduction1, TabSelectedIndex1));
                    CollDay1.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("SlitPeelProduction.Shift", System.ComponentModel.ListSortDirection.Ascending));
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
                if (MixingProduction2 != null)
                {
                    CollDay2 = new ListCollectionView(AddToTabs(MixingProduction2, TabSelectedIndex2));
                    CollDay2.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("SlitPeelProduction.Shift", System.ComponentModel.ListSortDirection.Ascending));
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
                if (MixingProduction3 != null)
                {
                    CollDay3 = new ListCollectionView(AddToTabs(MixingProduction3, TabSelectedIndex3));
                    CollDay3.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("SlitPeelProduction.Shift", System.ComponentModel.ListSortDirection.Ascending));
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
                if (MixingProduction4 != null)
                {
                    CollDay4 = new ListCollectionView(AddToTabs(MixingProduction4, TabSelectedIndex4));
                    CollDay4.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("SlitPeelProduction.Shift", System.ComponentModel.ListSortDirection.Ascending));
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
                if (MixingProduction5 != null)
                {
                    CollDay5 = new ListCollectionView(AddToTabs(MixingProduction5, TabSelectedIndex5));
                    CollDay5.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("SlitPeelProduction.Shift", System.ComponentModel.ListSortDirection.Ascending));
                }
            }
        }

        #endregion

        #region COMMANDS
        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand WorkStationsCommand
        {
            get
            {
                return _workStattionCommand ?? (_workStattionCommand = new LogOutCommandHandler(() => Switcher.Switch(new WorkStationsView(userName, state, privilages, metaData)), canExecute));
            }
        }

        #endregion
    }
}
