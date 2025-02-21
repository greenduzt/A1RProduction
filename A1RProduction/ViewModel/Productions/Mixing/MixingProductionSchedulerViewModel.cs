using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Products;
using A1QSystem.Model.RawMaterials;
using A1QSystem.View;
using A1QSystem.View.Quoting;
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

namespace A1QSystem.ViewModel.Productions.Mixing
{
    public class MixingProductionSchedulerViewModel : ViewModelBase
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
        private ObservableCollection<RawProductionDetails> _orderProduction = null;
        private ObservableCollection<RawProductionDetails> _orderProduction1 = null;
        private ObservableCollection<RawProductionDetails> _orderProduction2 = null;
        private ObservableCollection<RawProductionDetails> _orderProduction3 = null;
        private ObservableCollection<RawProductionDetails> _orderProduction4 = null;
        private ObservableCollection<RawProductionDetails> _orderProduction5 = null;
        private List<MetaData> metaData;
        private int _tabSelectedIndex1;
        private int _tabSelectedIndex2;
        private int _tabSelectedIndex3;
        private int _tabSelectedIndex4;
        private int _tabSelectedIndex5;
        private bool canExecute;
        private DateTime CurrentDate;
        public int TotOrdersPending { get; set; }
        private string userName;
        private string state;
        private List<UserPrivilages> privilages;
        private string _version;

        public Dispatcher UIDispatcher { get; set; }
        public MixingProductionSchedulerNotifier mixingProdSchedularNotifier { get; set; }

        private ICommand _backCommand;
        private ICommand navHomeCommand;
        private ICommand _ordersCommand;

        public MixingProductionSchedulerViewModel(string uName, string uState, List<UserPrivilages> uPriv, Dispatcher uidispatcher, List<MetaData> md)
        {
            metaData = md;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;

            //CurrentDate = NTPServer.GetNetworkTime();
            //userName = uName;
            //state = uState;
            //privilages = uPriv;
            //metaData = md;

            //TabSelectedIndex1 = 0;
            //TabSelectedIndex2 = 0;
            //TabSelectedIndex3 = 0;
            //TabSelectedIndex4 = 0;
            //TabSelectedIndex5 = 0;

            //GenerateWorkingDays();

            //this.UIDispatcher = uidispatcher;

            //this.mixingProdSchedularNotifier = new MixingProductionSchedulerNotifier();

            //this.mixingProdSchedularNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage);
            //ObservableCollection<RawProductionDetails> opd = this.mixingProdSchedularNotifier.RegisterDependency();

          

            //this.LoadData(opd);

            //canExecute = true;
        }

        private void LoadData(ObservableCollection<RawProductionDetails> psd)
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

                    OrderProduction1 = AddToBusinessDates(psd, Convert.ToDateTime(D1));
                    OrderProduction2 = AddToBusinessDates(psd, Convert.ToDateTime(D2));
                    OrderProduction3 = AddToBusinessDates(psd, Convert.ToDateTime(D3));
                    OrderProduction4 = AddToBusinessDates(psd, Convert.ToDateTime(D4));
                    OrderProduction5 = AddToBusinessDates(psd, Convert.ToDateTime(D5));

                    CollDay1 = new ListCollectionView(AddToTabs(OrderProduction1, TabSelectedIndex1));
                    CollDay1.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));

                    CollDay2 = new ListCollectionView(AddToTabs(OrderProduction2, TabSelectedIndex2));
                    CollDay2.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));

                    CollDay3 = new ListCollectionView(AddToTabs(OrderProduction3, TabSelectedIndex3));
                    CollDay3.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));

                    CollDay4 = new ListCollectionView(AddToTabs(OrderProduction4, TabSelectedIndex4));
                    CollDay4.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));

                    CollDay5 = new ListCollectionView(AddToTabs(OrderProduction5, TabSelectedIndex5));
                    CollDay5.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));

                }
            });
        }


        private ObservableCollection<RawProductionDetails> AddToBusinessDates(ObservableCollection<RawProductionDetails> prodSchDetails, DateTime date)
        {

            OrderProduction = new ObservableCollection<RawProductionDetails>();
            var grProdDet = from t in prodSchDetails
                            group t by new
                            {
                                t.RawProduct.RawProductID,
                                t.ProductionDate,
                                t.Shift
                            } into g
                            select new
                            {
                                RawProdDetailsID = g.First().RawProDetailsID,
                                RawProductID = g.First().RawProduct.RawProductID,
                                RawProductCode = g.First().RawProduct.RawProductCode,
                                Description = g.First().RawProduct.Description,
                                RawProductType = g.First().RawProduct.RawProductType,
                                QtyToMake = g.Sum(a => (Math.Ceiling(a.BlockLogQty))),
                                ProductionDate = g.First().ProductionDate,
                                Shift = g.First().Shift,
                                OriginType = g.First().OriginType

                            };

            foreach (var item in grProdDet)
            {
                if (Convert.ToDateTime(item.ProductionDate) == date || (Convert.ToDateTime(item.ProductionDate) < date && date == Convert.ToDateTime(CurrentDate)))
                {

                    OrderProduction.Add(new RawProductionDetails()
                    {
                        RawProduct = new RawProduct()
                        {
                            RawProductID = item.RawProductID,
                            RawProductCode = item.RawProductCode,
                            Description = item.Description,
                            RawProductType = item.RawProductType
                        },

                        RawProDetailsID = item.RawProdDetailsID,
                        BlockLogQty = item.QtyToMake,
                        ProductionDate = item.ProductionDate,
                        Shift = item.Shift,
                        ShiftName = GetShiftNameByID(item.Shift),
                        OriginType = item.OriginType

                    });

                }


            }

            return OrderProduction;
        }

        private ObservableCollection<RawProductionDetails> AddToTabs(ObservableCollection<RawProductionDetails> prodSchDetails, int TabSelectedIndex)
        {

            OrderProduction = new ObservableCollection<RawProductionDetails>();
            //OrderProduction1 = psd;
            var grProdDet = from t in prodSchDetails
                            group t by new
                            {
                                t.RawProduct.RawProductID,
                                t.ProductionDate,
                                t.Shift
                            } into g
                            select new
                            {
                                RawProdDetailsID = g.First().RawProDetailsID,
                                RawProductID = g.First().RawProduct.RawProductID,
                                RawProductCode = g.First().RawProduct.RawProductCode,
                                Description = g.First().RawProduct.Description,
                                RawProductType = g.First().RawProduct.RawProductType,
                                QtyToMake = g.Sum(a => (Math.Ceiling(a.BlockLogQty))),
                                ProductionDate = g.First().ProductionDate,
                                Shift = g.First().Shift,
                                OriginType = g.First().OriginType

                            };

            foreach (var item in grProdDet)
            {
                if (item.Shift == TabSelectedIndex)
                {

                    OrderProduction.Add(new RawProductionDetails()
                    {
                        RawProduct = new RawProduct()
                        {
                            RawProductID = item.RawProductID,
                            RawProductCode = item.RawProductCode,
                            Description = item.Description,
                            RawProductType = item.RawProductType
                        },
                        RawProDetailsID = item.RawProdDetailsID,
                        BlockLogQty = item.QtyToMake,
                        ProductionDate = item.ProductionDate,
                        Shift = item.Shift,
                        ShiftName = GetShiftNameByID(item.Shift),
                        OriginType = item.OriginType

                    });
                }
                else if (TabSelectedIndex == 0)
                {
                    OrderProduction = prodSchDetails;
                }
            }

            return OrderProduction;
        }


        void notifier_NewMessage(object sender, SqlNotificationEventArgs e)
        {
            this.LoadData(this.mixingProdSchedularNotifier.RegisterDependency());
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
                if (OrderProduction1 != null)
                {
                    CollDay1 = new ListCollectionView(AddToTabs(OrderProduction1, TabSelectedIndex1));
                    CollDay1.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
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
                    CollDay2 = new ListCollectionView(AddToTabs(OrderProduction2, TabSelectedIndex2));
                    CollDay2.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay2.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
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
                    CollDay3 = new ListCollectionView(AddToTabs(OrderProduction3, TabSelectedIndex3));
                    CollDay3.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay3.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
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
                    CollDay4 = new ListCollectionView(AddToTabs(OrderProduction4, TabSelectedIndex4));
                    CollDay4.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay4.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
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
                    CollDay5 = new ListCollectionView(AddToTabs(OrderProduction5, TabSelectedIndex5));
                    CollDay5.GroupDescriptions.Add(new PropertyGroupDescription("ShiftName"));
                    CollDay5.SortDescriptions.Add(new System.ComponentModel.SortDescription("Shift", System.ComponentModel.ListSortDirection.Ascending));
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

        public ICommand OrdersCommand
        {
            get
            {
                return _ordersCommand ?? (_ordersCommand = new LogOutCommandHandler(() => Switcher.Switch(new QuotingMainMenu(userName, state, privilages, metaData)), canExecute));
            }
        }

        #endregion
    }
}
