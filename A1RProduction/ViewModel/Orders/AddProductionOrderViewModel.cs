﻿using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Capacity;
using A1QSystem.Model.Formula;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Grading;
using A1QSystem.Model.Products;
using A1QSystem.Model.Shifts;
using A1QSystem.Model.Stock;
using A1QSystem.View;
using A1QSystem.View.Quoting;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using System.Diagnostics;
using A1QSystem.View.Loading;
using A1QSystem.Model.Products.Tiles;
using A1QSystem.Model.Products.Rolls;
using A1QSystem.Model.DeliveryDetails;
using A1QSystem.Model.Other;
using A1QSystem.View.Orders;
using System.Windows;
using System.Windows.Threading;
using A1QSystem.View.AdminDashboard;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Shreding;
using System.Data.SqlClient;
using System.Windows.Data;
using A1QSystem.Model.Production.Mixing;
using A1QSystem.Model.RawMaterials;
using iTextSharp.text.pdf.parser.clipper;

namespace A1QSystem.ViewModel.Orders
{
    public class ShiftDetails
    {
        public int ProTimeTableID { get; set; }
        public int GradingID { get; set; }
        public int shift { get; set; }
        public DateTime ProdDate { get; set; }
        public decimal CurrentCapacity { get; set; }
        public decimal TotCapacityKg { get; set; }
        public decimal GradedKg { get; set; }
        public List<GradedStock> GradedStock { get; set; }
    }
    public class DateShift
    {
        public DateTime ProdDate { get; set; }
        public List<ShiftDetails> ShiftList { get; set; }
    }

    public class AddProductionOrderViewModel : ViewModelBase
    {
        public event Action Closed;
        private Microsoft.Practices.Prism.Commands.DelegateCommand _closeCommand;
        private ObservableCollection<ProductionTotals> _productionTotals;
        private ObservableCollection<ProductionKgs> _productionKgs;
        public ObservableCollection<Product> Product { get; set; }
        public ObservableCollection<Product> ProductBlockLogs { get; set; }
        public ObservableCollection<Product> ProductFinished { get; set; }
        public ObservableCollection<ProductionDetails> ProductionDetails { get; set; }
        public ObservableCollection<Formulas> FormulaColl { get; set; }
        public ObservableCollection<ProductCapacity> CapacityLimitations { get; set; }
        public ObservableCollection<CurrentCapacity> CurrentCapacities { get; set; }
        public List<ProductionTimeTable> ProductionTimeTable { get; set; }
        public List<ProductionTimeTable> shiftList { get; set; }
        public List<MixingOnly> MixingOnlyList { get; set; }
        public List<ProductionTimeTable> NewDateList { get; set; }
        public ObservableCollection<OrderTypes> OrderTypeList { get; set; }
        public List<GradedStock> GradedStock { get; set; }
        public List<ShredStock> ShredStock { get; set; }
        private List<ProductionTimeTable> MixingTimeTableDetails;

        private ObservableCollection<ProductionHistory> _productionHistory;
        
        public List<Shift> ShiftDetails { get; set; }
        //private string _notes;
        private string _gradingComments;

        private ListCollectionView _listCollection;
        public DateTime CurrentDate { get; set; }
        public DateTime CurrentTime { get; set; }
        public DateTime ProductionDate { get; set; }
        public DateTime localDate { get; set; }
        public DateTime SysGen7Days { get; set; }
        public List<Machines> MachinesList { get; set; }
        private Order _order;
        private DateTime _mixingDate;
        private DateTime _gradingDate;
        private DateTime _freightArrivTime;
        //private BindingList<OrderDetails> _rawMaterialDetails;
        private ObservableCollection<Customer> _customerList;
        private ObservableCollection<Freight> _freightList;
        private string _selectedCustomer;
        private int _selectedFreight;       
        private string _version;        
        private string _selectedOrderType;
        private string _salesNo;
        private string _selectedOrderPriority;
        private string _mixingComments;
        private string _slittingComments;
        private string _peelingComments;
        private string _reRollingComments;
        private string prevOrderType;
        private bool _productionDateAvailable;
        private bool _isProductionDateAvailable;
        private bool _busyIndicator;
        private bool _freightTimeEnabled;
        private bool _freightTimeAvailable;
        private bool _isUrgentOrder;
        private bool _isEmergencyOrder;
        private bool _salesNoEnabled;
        private bool _customerEnabled;
        private bool _freightEnabled;
        private bool _productionStartdateEnabled;
        private bool _commentsEnabled;
        private bool _orderPriorityEnabled;
        private bool _dataGridEnabled;
        private bool _addCustomerEnabled;
        private string _orderTypeSelLbl;
        private string selectedShiftType;
        private List<int> gradesNoShowList;
        private ICommand _command;
        private ICommand _clearFields;
        private ICommand _addOrder;
        private ICommand _backCommand;
        private ICommand _homeCommand;
        private ICommand _ordersCommand;
        private ICommand _gradedStockCommand;
        private ICommand _adminDashboardCommand;
        private ICommand _selectionChangedCommand;
        private ICommand _addNewRowCommand;
        private ICommand _gradingFormulaCommand;
        private ICommand _mixingFormulaCommand;
        private ICommand _qtyChangedCommand;
        private ICommand _chiftChangedCommand;
        private ICommand _refreshGrdid;
        private ICommand _clearMixingOrderCommand;

        private Microsoft.Practices.Prism.Commands.DelegateCommand _addCustomerCommand;
        private Microsoft.Practices.Prism.Commands.DelegateCommand _addFreightCommand;
        private List<MetaData> metaData;
        private bool canExecute;
        public bool _arvoChecked;
        public bool _morningChecked;
        private List<UserPrivilages> privilages;
        private string userName;
        private string state;
        private string _gradingFormulaBtnVisibility;
        private string _mixingFormulaBtnVisibility;
        private ChildWindowView LoadingScreen;

        public GradingMixingNotifier gradingMixingNotifier { get; set; }

        public AddProductionOrderViewModel(string UserName, string State, List<UserPrivilages> UserPrivilages, List<MetaData> md)
        {
            gradesNoShowList = new List<int>();
            CurrentDate = Convert.ToDateTime(NTPServer.GetNetworkTime().ToString("dd/MM/yyyy"));
            MixingDate = CurrentDate;
            GradingDate = CurrentDate;
            ProductionDate = CurrentDate;
            CurrentTime.Date.AddHours(00).AddMinutes(00);
            FreightArrivTime = CurrentTime;
            FreightTimeEnabled = false;
            FreightTimeAvailable = false;
            IsEmergencyOrder = false;
            IsUrgentOrder = false;
            userName = UserName;
            state = State;
            privilages = UserPrivilages;
            SelectedFreight = 56;
            ProductionDateAvailable = false;
            IsProductionDateAvailable = false;
            SalesNoEnabled = false;
            CustomerEnabled = false;
            FreightEnabled = false;
            ProductionStartdateEnabled = false;
            CommentsEnabled = false;
            OrderPriorityEnabled = false;
            DataGridEnabled = false;
            AddCustomerEnabled = false;
            OrderTypeSelLbl = "Visible";
            selectedShiftType = string.Empty;
            ArvoChecked = false;
            MorningChecked = false;
            GradingFormulaBtnVisibility = "Collapsed";
            MixingFormulaBtnVisibility = "Collapsed";
            CustomerList = new ObservableCollection<Customer>();
            metaData = md;

            var data2 = metaData.SingleOrDefault(x => x.KeyName == "grades_no_show");
            if (data2 != null)
            {
                string[] noGradesArr = data2.Description.Split(',');

                foreach (var item in noGradesArr)
                {
                    gradesNoShowList.Add(Convert.ToInt16(item));
                }
            }

            LoadProducts();
            LoadBlockLogs();
            LoadCustomers();
            LoadFreights();
            LoadMixingOnlyProducts();
            LoadOrderTypes();
            LoadGradedStock();
            LoadMachines();

            Order = new Order();
            Order.OrderDetails = new ObservableCollection<OrderDetails>();
            FormulaColl = new ObservableCollection<Formulas>();
            CapacityLimitations = new ObservableCollection<ProductCapacity>();
            CurrentCapacities = new ObservableCollection<CurrentCapacity>();
            ProductionTimeTable = new List<ProductionTimeTable>();
            NewDateList = new List<ProductionTimeTable>();
            MixingTimeTableDetails = new List<ProductionTimeTable>();

            FormulaColl = DBAccess.GetFormulas();
            LoadShiftDetails();

            _addCustomerCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(ShowAddCustomer);
            _addFreightCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(ShowAddFreightWindow);
            _closeCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(CloseForm);
            canExecute = true;

            SelectedOrderType = "Normal";
            SelectedCustomer = "A1Rubber";
            SelectedFreight = 56;
            //SalesNo = "Stock Filling";
            SelectedOrderPriority = "Block/Log Stock";
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");

            Version = data.Description;

            AddNewRow();
            CalculateProductionTotals();
            GetMixingUncompleted();
        }

        private void GetMixingUncompleted()
        {
            ProductionHistory = new ObservableCollection<ProductionHistory>();
            ProductionHistory = new ObservableCollection<ProductionHistory>(DBAccess.GetMixingUnCompletedList(DateTime.Now, true));

        }

        private void LoadOrderTypes()
        {
            ObservableCollection<OrderTypes> lot = new ObservableCollection<OrderTypes>();
            lot.Add(new OrderTypes() { OrderTypeID = 1, OrderType = "Urgent" });
            lot.Add(new OrderTypes() { OrderTypeID = 3, OrderType = "Normal" });
            //lot.Add(new OrderTypes() {OrderTypeID=3, OrderType = "Normal" });
            OrderTypeList = lot;
        }

        private void LoadProducts()
        {
            ProductFinished = DBAccess.GetAllProds();
        }

        private void LoadBlockLogs()
        {
            ProductBlockLogs = DBAccess.GetAllBlockLogsAsProds();
        }
        private void LoadFreights()
        {
            //CustomerList = DBAccess.GetCustomerData();   
            CustomerList.Add(new Customer() { CustomerId = 1560, CompanyName = "A1Rubber", FirstName = "A1Rubber", LastName = "A1Rubber", Telephone = "000000000", Mobile = "0000000000", Email = "a1rubber@a1rubber.com", Address = "26 Binary Str Yatala", City = "Yatala", State = "QLD", PostCode = "3977" });

            //CustomerList.Add(new Customer() { CustomerId = 0, CompanyName = "Select", FirstName = "A1Rubber", LastName = "A1Rubber", Telephone = "000000000", Mobile = "0000000000", Email = "a1rubber@a1rubber.com", Address = "26 Binary Str Yatala", City = "Yatala", State = "QLD", PostCode = "3977" });
        }

        private void LoadCustomers()
        {
            //FreightList = DBAccess.GetFreightData();      
            FreightList = new ObservableCollection<Freight>();
            FreightList.Add(new Freight() { Id = 56, FreightName = "No Freight", FreightUnit = "PLT", FreightPrice = 0, FreightDescription = "No Freight" });
        }

        private void LoadGradedStock()
        {
            GradedStock = DBAccess.GetGradedStock();
        }

        private void LoadShredStock()
        {
            ShredStock = DBAccess.GetShredStock();
        }

        private void LoadMixingOnlyProducts()
        {
            MixingOnlyList = DBAccess.GetMixingOnly();
        }

        private void LoadShiftDetails()
        {
            ShiftDetails = DBAccess.GetAllShifts();
        }

        private void LoadMachines()
        {
            MachinesList = DBAccess.GetNumberOfMachines();
        }

        private void CalculateProductionTotals()
        {
            ProductionTotals = new ObservableCollection<ProductionTotals>();
            ProductionKgs = new ObservableCollection<ProductionKgs>();
           
            if(Order != null && Order.OrderDetails != null)
            {               
                
                foreach (var item in Order.OrderDetails)
                {
                    if (item.Product != null)
                    {
                        var data = ProductionTotals.FirstOrDefault(x=> Convert.ToDateTime(x.Date) == item.MixingDate && x.Type.Equals(item.Product.ProductUnit));
                        if(data != null)
                        {
                            if (item.IsMorningShift)
                            {
                                data.GradingNewMorning = data.GradingNewMorning + item.Quantity;
                            }
                            else if (item.IsArvoShift)
                            {
                                data.GradingNewArvo = data.GradingNewArvo + item.Quantity;
                            }
                            else
                            {
                                data.GradingNewNight = data.GradingNewNight + item.Quantity;
                            }

                            data.Total = data.GradingNewMorning + data.GradingNewArvo + data.GradingNewNight;// +data.GradingMorningQty + data.GradingArvoQty + data.GradingNightQty + data.MixingMorningQty + data.MixingArvoQty + data.MixingNightQty;
                        }
                        else
                        {
                            decimal m = 0, a = 0, n = 0;
                            if (item.IsMorningShift)
                            {
                               m = item.Quantity;
                            }
                            else if (item.IsArvoShift)
                            {
                               a = item.Quantity;
                            }
                            else
                            {
                               n = item.Quantity;
                            }

                            ProductionTotals.Add(new ProductionTotals() { Date = item.MixingDate.Date, GradingNewMorning = m, GradingNewArvo = a, GradingNewNight = n, Type = item.Product.Type, Total = item.Quantity });                        
                        }

                        //Production Kg
                        if(gradesNoShowList != null && gradesNoShowList.Count > 0)
                        {
                            bool hasVal = gradesNoShowList.Any(x => x == item.Product.RawProduct.RawProductID);

                            if (hasVal == false)
                            {                                                        
                                var dataKg1 = ProductionKgs.FirstOrDefault(x => Convert.ToDateTime(x.Date.Date) == item.MixingDate.Date && x.GradedStock.ID.Equals(item.Product.RawProduct.Formula.ProductCapacity1));
                                if (dataKg1 != null)
                                {
                                    if (item.IsMorningShift)
                                    {
                                        dataKg1.GradingMorningKg = dataKg1.GradingMorningKg + (item.Quantity * item.Product.RawProduct.Formula.GradingWeight1);
                                    }
                                    else if (item.IsArvoShift)
                                    {
                                        dataKg1.GradingArvoKg = dataKg1.GradingArvoKg + (item.Quantity * item.Product.RawProduct.Formula.GradingWeight1);
                                    }
                                    else
                                    {
                                        dataKg1.GradingNightKg = dataKg1.GradingNightKg + (item.Quantity * item.Product.RawProduct.Formula.GradingWeight1);
                                    }

                                    dataKg1.Total = dataKg1.GradingMorningKg + dataKg1.GradingArvoKg + dataKg1.GradingNightKg;

                                    var dataKg2 = ProductionKgs.FirstOrDefault(x => Convert.ToDateTime(x.Date.Date) == item.MixingDate.Date && x.GradedStock.ID.Equals(item.Product.RawProduct.Formula.ProductCapacity2));
                                    if (dataKg2 != null)
                                    {
                                        if (item.IsMorningShift)
                                        {
                                            dataKg2.GradingMorningKg = dataKg2.GradingMorningKg + (item.Quantity * item.Product.RawProduct.Formula.GradingWeight2);
                                        }
                                        else if (item.IsArvoShift)
                                        {
                                            dataKg2.GradingArvoKg = dataKg2.GradingArvoKg + (item.Quantity * item.Product.RawProduct.Formula.GradingWeight2);
                                        }
                                        else
                                        {
                                            dataKg2.GradingNightKg = dataKg2.GradingNightKg + (item.Quantity * item.Product.RawProduct.Formula.GradingWeight2);
                                        }

                                        dataKg2.Total = dataKg2.GradingMorningKg + dataKg2.GradingArvoKg + dataKg2.GradingNightKg;
                                    }
                                }
                                else
                                {
                                    decimal m1 = 0, m2 = 0, a1 = 0, a2 = 0, n1 = 0, n2 = 0;
                                    if (item.IsMorningShift)
                                    {
                                        m1 = item.Quantity * item.Product.RawProduct.Formula.GradingWeight1;
                                        if (item.Product.RawProduct.Formula.GradingWeight2 > 0)
                                        {
                                            m2 = item.Quantity * item.Product.RawProduct.Formula.GradingWeight2;
                                        }
                                
                                    }
                                    else if (item.IsArvoShift)
                                    {
                                        a1 = item.Quantity * item.Product.RawProduct.Formula.GradingWeight1;
                                        if (item.Product.RawProduct.Formula.GradingWeight2 > 0)
                                        {
                                            a2 = item.Quantity * item.Product.RawProduct.Formula.GradingWeight2;
                                        }
                                    }
                                    else
                                    {
                                        n1 = item.Quantity * item.Product.RawProduct.Formula.GradingWeight1;
                                        if (item.Product.RawProduct.Formula.GradingWeight2 > 0)
                                        {
                                            n2 = item.Quantity * item.Product.RawProduct.Formula.GradingWeight2;
                                        }
                                    }
                            
                                    ProductionKgs.Add(new ProductionKgs() 
                                    { 
                                        Date = item.MixingDate.Date, 
                                        GradedStock = new GradedStock() 
                                        { 
                                            ID =  item.Product.RawProduct.Formula.ProductCapacity1, 
                                            GradeName = item.Product.RawProduct.Formula.FormulaName1, 
                                            Qty = item.Product.RawProduct.Formula.GradingWeight1 
                                        },
                                        GradingMorningKg = m1,
                                        GradingArvoKg = a1,
                                        GradingNightKg = n1,
                                        Total = m1 + a1 + n1
                                    });
                        
                                    if(item.Product.RawProduct.Formula.GradingWeight2 > 0)
                                    {
                                        var dataKg2 = ProductionKgs.FirstOrDefault(x => Convert.ToDateTime(x.Date.Date) == item.MixingDate.Date && x.GradedStock.ID.Equals(item.Product.RawProduct.Formula.ProductCapacity2));
                                        if (dataKg2 != null)
                                        {
                                            if (item.IsMorningShift)
                                            {
                                                dataKg2.GradingMorningKg = dataKg2.GradingMorningKg + (item.Quantity * item.Product.RawProduct.Formula.GradingWeight2);
                                            }
                                            else if (item.IsArvoShift)
                                            {
                                                dataKg2.GradingArvoKg = dataKg2.GradingArvoKg + (item.Quantity * item.Product.RawProduct.Formula.GradingWeight2);
                                            }
                                            else
                                            {
                                                dataKg2.GradingNightKg = dataKg2.GradingNightKg + (item.Quantity * item.Product.RawProduct.Formula.GradingWeight2);
                                            }

                                            dataKg2.Total = dataKg2.GradingMorningKg + dataKg2.GradingArvoKg + dataKg2.GradingNightKg;
                                        }
                                        else
                                        {

                                            ProductionKgs.Add(new ProductionKgs()
                                            {
                                                Date = item.MixingDate.Date,
                                                GradedStock = new GradedStock()
                                                {
                                                    ID = item.Product.RawProduct.Formula.ProductCapacity2,
                                                    GradeName = item.Product.RawProduct.Formula.FormulaName2,
                                                    Qty = item.Product.RawProduct.Formula.GradingWeight2
                                                },
                                                GradingMorningKg = m2,
                                                GradingArvoKg = a2,
                                                GradingNightKg = n2,
                                                Total = m2 + a2 + n2
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //ListCollection = new ListCollectionView(ProductionKgs);
                //ListCollection.GroupDescriptions.Add(new PropertyGroupDescription("Date"));
            }

            var tempList = ProductionTotals.OrderBy(x=>x.Date.Date);

            tempList.ToList().ForEach(q =>
            {
                ProductionTotals.Remove(q);
                ProductionTotals.Add(q);
            });
        }

        #region PUBLIC PROPERTIES


        public string SelectedOrderPriority
        {
            get
            {
                return _selectedOrderPriority;
            }
            set
            {
                prevOrderType = _selectedOrderPriority;
                if (value == _selectedOrderPriority)
                    return;

                _selectedOrderPriority = value;
                RaisePropertyChanged(() => this.SelectedOrderPriority);
                if (SelectedOrderPriority != "Select")
                {
                    OrderTypeSelLbl = "Collapsed";
                    SalesNoEnabled = true;
                    CustomerEnabled = true;
                    FreightEnabled = true;
                    ProductionStartdateEnabled = true;
                    CommentsEnabled = true;
                    OrderPriorityEnabled = true;
                    DataGridEnabled = true;
                    AddCustomerEnabled = true;

                    if (Order.OrderDetails.Count > 0)
                    {
                        if (Msg.Show("Changing the Order Type will clear the existing order details?" + System.Environment.NewLine + "Do you want to change the Order Type?", "Order Type Changing Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                        {
                            //Order.OrderDetails.Clear();

                            if (SelectedOrderPriority == "Finished Goods Stock")
                            {
                                Product = ProductFinished;
                                SalesNo = string.Empty;
                                SelectedCustomer = "A1Rubber";
                            }
                            else if (SelectedOrderPriority == "Block/Log Stock")
                            {
                                Product = ProductBlockLogs;
                                SalesNo = "Stock Filling";
                                SelectedCustomer = "A1Rubber";
                            }
                        }
                        else
                        {
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                Debug.WriteLine("Dispatcher BeginInvoke " + "Setting CurrentPersonCancellable."
                             );
                                _selectedOrderPriority = prevOrderType;
                                RaisePropertyChanged(() => this.SelectedOrderPriority);
                            }),
                                                                            DispatcherPriority.ContextIdle,
                                                                             null
                                                                            );

                            // Exit early. 
                            return;
                        }

                    }
                    else
                    {
                        if (SelectedOrderPriority == "Finished Goods Stock")
                        {
                            Product = ProductFinished;
                            SalesNo = string.Empty;
                            SelectedCustomer = "A1Rubber";
                        }
                        else if (SelectedOrderPriority == "Block/Log Stock")
                        {
                            Product = ProductBlockLogs;
                            SalesNo = "Stock Filling";
                            SelectedCustomer = "A1Rubber";
                        }
                        RaisePropertyChanged(() => this.SelectedOrderPriority);
                    }
                }
                else
                {
                    OrderTypeSelLbl = "Visible";
                    SalesNoEnabled = false;
                    CustomerEnabled = false;
                    FreightEnabled = false;
                    ProductionStartdateEnabled = false;
                    CommentsEnabled = false;
                    OrderPriorityEnabled = false;
                    MixingDate = CurrentDate;
                    GradingDate = CurrentDate;
                    IsProductionDateAvailable = false;
                    GradingComments = string.Empty;
                    SelectedOrderType = "Normal";
                    SalesNo = string.Empty;
                    Order.OrderDetails.Clear();
                    DataGridEnabled = false;
                    AddCustomerEnabled = false;
                }
            }
        }
        private void ClearOrderTypeInfo()
        {
            SalesNoEnabled = false;
            CustomerEnabled = false;
            FreightEnabled = false;
            ProductionStartdateEnabled = false;
            CommentsEnabled = false;
            OrderPriorityEnabled = false;
            MixingDate = CurrentDate;
            GradingDate = CurrentDate;
            IsProductionDateAvailable = false;
            GradingComments = string.Empty;
            SelectedOrderType = "Normal";
            DataGridEnabled = false;
            Order.OrderDetails.Clear();
        }

        private void ProductChanged(object parameter)
        {
            int index = Order.OrderDetails.IndexOf(parameter as OrderDetails);
            if (index > -1 && index < Order.OrderDetails.Count)
            {
                for (int i = 0; i < Order.OrderDetails.Count; i++)
                {
                    if (i > 0)
                    {
                        if (i == index)
                        {
                            int x = i - 1;

                            Order.OrderDetails[i].MixingDate = Order.OrderDetails[x].MixingDate;
                        }
                    }
                }

                CalculateProductionTotals();
            }

        }

        void productChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //this.ItemCount = this.Order.OrderDetails.Count;

            // Resequence list
            //SequencingService.SetCollectionSequence(this.Order.OrderDetails);
            
        }



        public ObservableCollection<ProductionKgs> ProductionKgs
        {
            get
            {
                return _productionKgs;
            }
            set
            {
                _productionKgs = value;
                RaisePropertyChanged(() => this.ProductionKgs);

            }
        }

        public bool AddCustomerEnabled
        {
            get
            {
                return _addCustomerEnabled;
            }
            set
            {
                _addCustomerEnabled = value;
                RaisePropertyChanged(() => this.AddCustomerEnabled);

            }
        }
        public string OrderTypeSelLbl
        {
            get
            {
                return _orderTypeSelLbl;
            }
            set
            {
                _orderTypeSelLbl = value;
                RaisePropertyChanged(() => this.OrderTypeSelLbl);
            }
        }

        public ObservableCollection<ProductionTotals> ProductionTotals
        {
            get
            {
                return _productionTotals;
            }
            set
            {
                _productionTotals = value;
                RaisePropertyChanged(() => this.ProductionTotals);
            }
        }

        public string SelectedOrderType
        {
            get
            {
                return _selectedOrderType;
            }
            set
            {
                _selectedOrderType = value;
                RaisePropertyChanged(() => this.SelectedOrderType);

            }
        }

        public Order Order
        {
            get { return _order; }
            set
            {
                _order = value;
                if (Order.OrderDetails != null)
                {
                    //Order.OrderDetails.ListChanged += (o, e) => RaisePropertyChanged(() => this.DiscountedTotal);
                }
                RaisePropertyChanged(() => this.Order);
            }
        }

        public bool MorningChecked
        {
            get { return _morningChecked; }
            set
            {
                _morningChecked = value;
               
                RaisePropertyChanged(() => this.MorningChecked);
                if (MorningChecked)
                {
                    selectedShiftType = "Morning";
                }
            }
        }

        public bool ArvoChecked
        {
            get { return _arvoChecked; }
            set
            {
                _arvoChecked = value;

                RaisePropertyChanged(() => this.ArvoChecked);
                if(ArvoChecked)
                {
                    selectedShiftType = "Arvo";
                }
            }
        }

        public ObservableCollection<ProductionHistory> ProductionHistory
        {
            get { return _productionHistory; }
            set
            {
                _productionHistory = value;
                RaisePropertyChanged(() => this.ProductionHistory);

            }
        }

        public string GradingComments
        {
            get { return _gradingComments; }
            set
            {
                _gradingComments = value;
                RaisePropertyChanged(() => this.GradingComments);
                
                if(!string.IsNullOrWhiteSpace(GradingComments))
                {
                    MixingComments = GradingComments;
                }
            }
        }

        public string MixingComments
        {
            get { return _mixingComments; }
            set
            {
                _mixingComments = value;
                RaisePropertyChanged(() => this.MixingComments);
            }
        }

        public string SlittingComments
        {
            get { return _slittingComments; }
            set
            {
                _slittingComments = value;
                RaisePropertyChanged(() => this.SlittingComments);
            }
        }

        public string PeelingComments
        {
            get { return _peelingComments; }
            set
            {
                _peelingComments = value;
                RaisePropertyChanged(() => this.PeelingComments);
            }
        }

        public string ReRollingComments
        {
            get { return _reRollingComments; }
            set
            {
                _reRollingComments = value;
                RaisePropertyChanged(() => this.ReRollingComments);
            }
        }
        //public string Notes
        //{
        //    get { return _notes; }
        //    set
        //    {
        //        _notes = value;
        //        RaisePropertyChanged(() => this.Notes);
        //    }
        //}


        public bool BusyIndicator
        {
            get { return _busyIndicator; }
            set
            {
                _busyIndicator = value;
                RaisePropertyChanged(() => this.BusyIndicator);
            }
        }
        public string SalesNo
        {
            get { return _salesNo; }
            set
            {
                _salesNo = value;
                RaisePropertyChanged(() => this.SalesNo);
            }
        }

        public string SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                RaisePropertyChanged(() => this.SelectedCustomer);
                if (SelectedCustomer != null)
                {
                    if (SelectedCustomer == "A1Rubber Stock")
                    {
                        SelectedFreight = 56;
                    }
                    else
                    {
                        SelectedFreight = 56;
                    }
                }
            }
        }

        public int SelectedFreight
        {
            get { return _selectedFreight; }
            set
            {
                _selectedFreight = value;
                RaisePropertyChanged(() => this.SelectedFreight);

                if (SelectedFreight == 56)
                {
                    FreightTimeAvailable = true;
                }
                else
                {
                    FreightTimeAvailable = false;
                }
            }
        }

        public DateTime FreightArrivTime
        {
            get { return _freightArrivTime; }
            set
            {
                _freightArrivTime = value;
                RaisePropertyChanged(() => this.FreightArrivTime);
            }
        }

        public ObservableCollection<Customer> CustomerList
        {
            get
            {
                return _customerList;
            }
            set
            {
                _customerList = value;

                RaisePropertyChanged(() => this.CustomerList);
            }
        }

        public ObservableCollection<Freight> FreightList
        {
            get
            {
                return _freightList;
            }
            set
            {
                _freightList = value;

                RaisePropertyChanged(() => this.FreightList);
            }
        }

        private decimal _discountedTotal;
        public decimal DiscountedTotal
        {
            get
            {
                _discountedTotal = Order.OrderDetails.Sum(x => x.Quantity);
                return _discountedTotal;
            }
            set
            {
                _discountedTotal = value;
                RaisePropertyChanged(() => this.DiscountedTotal);                
            }
        }

        private OrderDetails _selectedOrder;
        public OrderDetails SelectedOrder
        {
            get
            {
                return _selectedOrder;
            }
            set
            {
                _selectedOrder = value;
                RaisePropertyChanged(() => this.SelectedOrder);

                if (SelectedOrder != null)
                {
                   
                    //for (int i = 0; i < Order.OrderDetails.Count; i++)
                    //{
                    //    if(Order.OrderDetails[i])
                    //}
                }
            }
        }

        

        public DateTime MixingDate
        {
            get { return _mixingDate; }
            set
            {
                _mixingDate = value;
                RaisePropertyChanged(() => this.MixingDate);
            }
        }

        public DateTime GradingDate
        {
            get { return _gradingDate; }
            set
            {
                _gradingDate = value;
                RaisePropertyChanged(() => this.GradingDate);
            }
        }


        public bool FreightTimeAvailable
        {
            get
            {
                return _freightTimeAvailable;
            }
            set
            {
                _freightTimeAvailable = value;
                RaisePropertyChanged(() => this.FreightTimeAvailable);

                if (FreightTimeAvailable == true)
                {
                    FreightTimeEnabled = false;
                }
                else
                {
                    FreightTimeEnabled = true;
                }
            }
        }

        public bool IsProductionDateAvailable
        {
            get
            {
                return _isProductionDateAvailable;
            }
            set
            {
                _isProductionDateAvailable = value;
                RaisePropertyChanged(() => this.IsProductionDateAvailable);

                if (IsProductionDateAvailable == true)
                {
                    ProductionDateAvailable = true;
                }
                else
                {
                    ProductionDateAvailable = false;
                    MixingDate = CurrentDate;
                    GradingDate = CurrentDate;
                    ArvoChecked = false;
                    MorningChecked = false;
                    selectedShiftType = string.Empty;
                }

            }
        }

        public bool FreightTimeEnabled
        {
            get
            {
                return _freightTimeEnabled;
            }
            set
            {
                _freightTimeEnabled = value;
                RaisePropertyChanged(() => this.FreightTimeEnabled);
            }
        }

        public bool ProductionDateAvailable
        {
            get
            {
                return _productionDateAvailable;
            }
            set
            {
                _productionDateAvailable = value;
                RaisePropertyChanged(() => this.ProductionDateAvailable);
            }
        }




        public bool IsUrgentOrder
        {
            get
            {
                return _isUrgentOrder;
            }
            set
            {
                _isUrgentOrder = value;
                RaisePropertyChanged(() => this.IsUrgentOrder);
                if (IsUrgentOrder == true)
                {
                    IsEmergencyOrder = false;
                }
            }
        }

        public bool IsEmergencyOrder
        {
            get
            {
                return _isEmergencyOrder;
            }
            set
            {
                _isEmergencyOrder = value;
                RaisePropertyChanged(() => this.IsEmergencyOrder);
                if (IsEmergencyOrder == true)
                {
                    IsUrgentOrder = false;
                }
            }
        }

        public bool SalesNoEnabled
        {
            get
            {
                return _salesNoEnabled;
            }
            set
            {
                _salesNoEnabled = value;
                RaisePropertyChanged(() => this.SalesNoEnabled);
            }
        }

        public bool CustomerEnabled
        {
            get
            {
                return _customerEnabled;
            }
            set
            {
                _customerEnabled = value;
                RaisePropertyChanged(() => this.CustomerEnabled);
            }
        }

        public bool FreightEnabled
        {
            get
            {
                return _freightEnabled;
            }
            set
            {
                _freightEnabled = value;
                RaisePropertyChanged(() => this.FreightEnabled);
            }
        }

        public bool ProductionStartdateEnabled
        {
            get
            {
                return _productionStartdateEnabled;
            }
            set
            {
                _productionStartdateEnabled = value;
                RaisePropertyChanged(() => this.ProductionStartdateEnabled);
            }
        }

        public bool CommentsEnabled
        {
            get
            {
                return _commentsEnabled;
            }
            set
            {
                _commentsEnabled = value;
                RaisePropertyChanged(() => this.CommentsEnabled);
            }
        }

        public bool OrderPriorityEnabled
        {
            get
            {
                return _orderPriorityEnabled;
            }
            set
            {
                _orderPriorityEnabled = value;
                RaisePropertyChanged(() => this.OrderPriorityEnabled);
            }
        }

        public bool DataGridEnabled
        {
            get
            {
                return _dataGridEnabled;
            }
            set
            {
                _dataGridEnabled = value;
                RaisePropertyChanged(() => this.DataGridEnabled);
            }
        }

        public string GradingFormulaBtnVisibility
        {
            get
            {
                return _gradingFormulaBtnVisibility;
            }
            set
            {
                _gradingFormulaBtnVisibility = value;
                RaisePropertyChanged(() => this.GradingFormulaBtnVisibility);
            }
        }

        public string MixingFormulaBtnVisibility
        {
            get
            {
                return _mixingFormulaBtnVisibility;
            }
            set
            {
                _mixingFormulaBtnVisibility = value;
                RaisePropertyChanged(() => this.MixingFormulaBtnVisibility);
            }
        }
        public ListCollectionView ListCollection
        {
            get { return _listCollection; }
            set
            {
                _listCollection = value;
                RaisePropertyChanged(() => this.ListCollection);

            }
        }

        #endregion



        private bool CanExecute(object parameter)
        {
            return true;
        }

        private void Execute(object parameter)
        {
            int index = Order.OrderDetails.IndexOf(parameter as OrderDetails);
            if (index > -1 && index < Order.OrderDetails.Count)
            {
              Order.OrderDetails.RemoveAt(index);
              CalculateProductionTotals();                
            }           
        }
               

        private void AddNewRow()
        {
            if(Order != null && Order.OrderDetails != null)
            {
                bool has = Order.OrderDetails.Any(x=> x.Product == null &&  x.Quantity == 0);
                if (has == false)
                {
                    OrderDetails od = null;
                    if (Order.OrderDetails.Count > 0)
                    {
                        od = Order.OrderDetails[Order.OrderDetails.Count - 1];
                    }
                    
                    Order.OrderDetails.Add(new OrderDetails() { MixingDate = od == null ? DateTime.Now.Date : od.MixingDate });                    
                }
            }
        }

        private void Clear()
        {
            MixingDate = CurrentDate;
            GradingDate = CurrentDate;
            Order.OrderDetails.Clear();
            //Order.OrderDetails = new BindingList<OrderDetails>();
            SalesNo = string.Empty;
            SelectedCustomer = "A1Rubber";
            SelectedFreight = 56;
            FreightArrivTime = CurrentTime;
            CurrentCapacities.Clear();
            IsEmergencyOrder = false;
            IsUrgentOrder = false;            
            //Notes = string.Empty;
            GradingComments = string.Empty;
            MixingComments = string.Empty;
            SlittingComments = string.Empty;
            PeelingComments = string.Empty;
            ReRollingComments = string.Empty;
            ProductionDateAvailable = false;
            IsProductionDateAvailable = false;
            ProductionStartdateEnabled = false;
            CommentsEnabled = false;
            OrderPriorityEnabled = false;
            SelectedOrderPriority = "Block/Log Stock";
            AddCustomerEnabled = false;
            selectedShiftType = string.Empty;
            ArvoChecked = false;
            MorningChecked = false;
            AddNewRow();
        }


        /***Function to desides wheather to add or substract days****/
        private DateTime AddSubstractDays(DateTime date)
        {
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            DateTime newDate = new DateTime(1900, 01, 01);

            if (date <= SysGen7Days && date > CurrentDate)
            {
                newDate = bdg.SubstractBusinessDays(date, 1);
            }
            if (date <= CurrentDate)
            {
                if (SysGen7Days <= CurrentDate)
                {
                    newDate = bdg.AddBusinessDays(CurrentDate, 1);
                }
                else
                {
                    date = SysGen7Days;
                    newDate = bdg.AddBusinessDays(date, 1);
                }
            }
            if (date > SysGen7Days)
            {
                newDate = bdg.AddBusinessDays(date, 1);
            }

            return newDate;
        }



        private decimal CalNoOfBlocks(decimal amount, decimal gradingWeight)
        {
            decimal blk = 0;

            blk = decimal.Floor(amount / gradingWeight);

            return blk;
        }

        public bool CheckDateAvailable(DateTime cDate)
        {
            bool res = false;
            Production prod = new Production();
            ProductionTimeTable = DBAccess.GetProductionTimeTableByID(1, cDate);//Get Production TimeTable
            if (ProductionTimeTable.Count == 0)
            {
                res = prod.AddNewDates(cDate, false);
            }
            else
            {
                res = true;
                Console.WriteLine("date available in the current list");
            }
            return res;
        }


        private void CloseWaitingScreen()
        {
            ChildWindowManager.Instance.CloseChildWindow();
        }

        private Tuple<int, string> PreCheckRegrindingProduct()
        {
            decimal kg = 0;
            string resString = string.Empty;
            string rawProd = string.Empty;
            int error = 0;
            bool hasVal = false;
            decimal tot = 0;
            decimal logCount = 0;
            List<ProductMeterage> prodMeterage = DBAccess.GetProductMeterage();
            //var data = (OrderDetails)null;

            //remove rows which doesn't have a product
            for (int i = 0; i < Order.OrderDetails.Count; i++)
            {
                if (Order.OrderDetails[i].Product == null)
                {
                    Order.OrderDetails.RemoveAt(i--);
                }
            }

            hasVal = Order.OrderDetails.Any(c => c.Product.RawProduct.RawProductID == 12 || c.Product.RawProduct.RawProductID == 73 || c.Product.RawProduct.RawProductID == 74 || c.Product.RawProduct.RawProductID == 71);
            if (hasVal == true)
            {
                LoadShredStock();
                var ssData = ShredStock.Single(c => c.Shred.ID == 6);//Regrinding

                resString = "Insufficient regrind!" + System.Environment.NewLine + "Regrind available - " + ssData.Qty.ToString("G29") + "kg" + System.Environment.NewLine + System.Environment.NewLine;

                foreach (var item in Order.OrderDetails)
                {
                    kg = 0;
                    List<Formulas> fList = DBAccess.GetFormulaDetailsByRawProdID(item.Product.RawProduct.RawProductID);
                    if (item.Product.RawProduct.RawProductID == 12 || item.Product.RawProduct.RawProductID == 71 || item.Product.RawProduct.RawProductID == 73 || item.Product.RawProduct.RawProductID == 74)
                    {
                        if (item.Product.Type == "Bulk" || item.Product.Type == "Log")
                        {
                            kg = item.Quantity * fList[0].GradingWeight1;
                            tot += kg;
                            resString += "-" + item.Product.ProductDescription + " requires " + kg.ToString("G29") + "kg (" + item.Quantity + " logs)" + System.Environment.NewLine;
                            logCount += item.Quantity;
                        }
                        else if (item.Product.Type == "Standard")
                        {
                            var prodM = prodMeterage.Single(x => x.MouldSize == item.Product.Width && x.MouldType == item.Product.MouldType && x.Thickness == item.Product.Tile.Thickness);
                            decimal maxRollsPerLog = Math.Floor(prodM.ExpectedYield / item.Product.Tile.MaxYield);
                            decimal noOfLogsReq = Math.Ceiling(item.Quantity / maxRollsPerLog);
                            kg = noOfLogsReq * fList[0].GradingWeight1;
                            tot += kg;
                            logCount += noOfLogsReq;
                            resString += "-" + item.Product.ProductDescription + " requires " + kg.ToString("G29") + "kg (" + noOfLogsReq + " logs)" + System.Environment.NewLine;
                        }
                    }
                }

                resString += System.Environment.NewLine + "Total of " + tot.ToString("G29") + "kg (" + logCount.ToString("G29") + " logs)" + " requires to complete this order";

                //Calculation
                if (tot > ssData.Qty)
                {
                    error = 1;
                }
                else if (ssData.Qty == 0)
                {
                    error = 1;
                }

            }

            return Tuple.Create(error, resString);
        }

        private void RemoveRawZero()
        {
            var itemToRemove = Order.OrderDetails.Where(x => x.Quantity == 0 && x.Product == null).ToList();
            foreach (var item in itemToRemove)
            {
                Order.OrderDetails.Remove(item);
            }
        }

        private Tuple<string, bool> CheckZeroQty()
        {
            bool b = false;
            string s = string.Empty;
            //b = Order.OrderDetails.Any(item => item.Quantity == 0);

            var data = Order.OrderDetails.FirstOrDefault(c => c.Quantity == 0);

            if (data != null)
            {
                s = data.Product.ProductDescription;
                b = true;
            }
            return Tuple.Create(s, b);
        }

        private bool CheckProductExist()
        {
            bool ex = false;

            ex = Order.OrderDetails.Any(c => c.Quantity != 0 && c.Product == null);


            return ex;
        }

        private void AddOrderProduction()
        {
            int result = 0;
            Int32 cusId = 0;
            Tuple<int, string> regrindingProd = null;
            List<A1QSystem.Model.Other.SystemParameters> systemParameters = DBAccess.GetAllSystemParametersByValue(true);
            bool has = systemParameters.Any(x => x.Value == true);
            if (has == true)
            {
                Msg.Show("Orders are bieng shifted at the moment. Please try again in few minutes ", "Orders Shifting", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
            }
            else
            {
                int sp1 = DBAccess.UpdateSystemParameter("AddingOrder", true);
                if (sp1 > 0)
                {
                    RemoveRawZero();//remove rows which has zeros
                    Tuple<string, bool> qtyAvailable = CheckZeroQty();//Check for qty zeros

                    if (CheckProductExist() == true)
                    {
                        Msg.Show("Please select product", "Select Product", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                    }
                    else
                    {
                        //var y = Order.OrderDetails.GroupBy(n => n.Product.ProductCode).Any(c => c.Count() > 1);
                        bool y = false;

                        //Copy
                        List<OrderDetails> odList = new List<OrderDetails>();
                        foreach (var items in Order.OrderDetails)
                        {
                            OrderDetails od = new OrderDetails();
                            od.IsArvoShift = items.IsArvoShift;
                            od.IsMorningShift = items.IsMorningShift;
                            od.MixingComment = items.MixingComment;
                            od.MixingDate = items.MixingDate;
                            od.Product = new Product();
                            od.Product.RawProduct = new RawProduct();
                            od.Product.RawProduct.RawProductID = items.Product.RawProduct.RawProductID;
                            od.Product.RawProduct.Description = items.Product.RawProduct.Description;
                            od.Quantity = items.Quantity;
                            odList.Add(od);
                        }

                        List<OrderDetails> newList = odList.Distinct(new OrderDetailsComparer()).ToList();

                        if (newList.Count != Order.OrderDetails.Count)
                        {
                            y = true;
                        }                        

                        if (y == true)
                        {
                            Msg.Show("Duplicate products with same mixing date or mixing shift exist. Please change the mixing date or mixing shift ", "Duplicate Products Exist", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                        }
                        else
                        {
                            if (SelectedOrderPriority == "Select")
                            {
                                Msg.Show("Please select Order Type", "Select Order Type", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                            }                           
                            else if (qtyAvailable.Item2)
                            {
                                Msg.Show("Please enter QTY for the product " + qtyAvailable.Item1, "Qty Required", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                            }
                            else if (Order.OrderDetails.Count == 0)
                            {
                                AddNewRow();
                                Msg.Show("Order details required", "Enter Order", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                            }
                            else if (Order.OrderDetails.Count == 1 && (Order.OrderDetails == null || Order.OrderDetails[0].Quantity == 0 || Order.OrderDetails[0].Product == null))
                            {
                                AddNewRow();
                                Msg.Show("Order details required", "Enter Order", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                            }
                            else
                            {       

                                bool validate = CheckCapacity(Order.OrderDetails);
                                if (validate)
                                {
                                    BackgroundWorker worker = new BackgroundWorker();
                                    LoadingScreen = new ChildWindowView();
                                    LoadingScreen.ShowWaitingScreen("Processing");

                                    worker.DoWork += (_, __) =>
                                    {
                                        regrindingProd = PreCheckRegrindingProduct();//Check graded stock for regrinding

                                        if (regrindingProd.Item1 == 1)
                                        {
                                            result = 1;
                                        }
                                        else
                                        {
                                            var data = CustomerList.SingleOrDefault(c => c.CompanyName == SelectedCustomer);
                                            if (data == null)
                                            {
                                                cusId = AddCustomer();
                                            }
                                            else
                                            {
                                                cusId = data.CustomerId;
                                            }

                                            OrderManager om = new OrderManager(this);
                                            Order.OrderType = ConvertOrderTypeToInt();
                                            Order.OrderPriority = ConvertOrderPriorityToInt();
                                            Order.RequiredDate = GradingDate;//Grading date
                                            Order.MixingDate = MixingDate;//Mixing date
                                            Order.SalesNo = SalesNo;
                                            Order.Comments = GradingComments;
                                            Order.MixingComments = MixingComments;
                                            Order.SlittingComments = SlittingComments;
                                            Order.PeelingComments = PeelingComments;
                                            Order.ReRollingComments = ReRollingComments;
                                            Order.DeliveryDetails = new List<Delivery>() { new Delivery() { FreightID = SelectedFreight } };
                                            Order.IsRequiredDateSelected = IsProductionDateAvailable;
                                            Order.OrderCreatedDate = DateTime.Now.Date;
                                            Order.Customer = new Customer() { CustomerId = cusId };
                                            Order.MixingShift = selectedShiftType;
                                            result = om.ProcessOrder(Order);
                                        }
                                    };
                                    worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                                    {
                                        LoadingScreen.CloseWaitingScreen();

                                        if (result == 1)
                                        {
                                            Msg.Show(regrindingProd.Item2, "Re Adjust Quantity", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                                        }
                                        else if (result == 2)
                                        {
                                            Msg.Show("The Graded Stock is not available to complete " + regrindingProd.Item2, "Order cannot be completed", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                                        }
                                        else if (result == 3)
                                        {
                                            Msg.Show("Cannot generate order id!!!", "Order ID Generation Failed", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                                        }
                                        else if (result == 4)
                                        {
                                            Msg.Show("There has been a problem separating grading and mixing orders", "Problem Has Occured", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                                        }
                                        else if (result == 5)
                                        {
                                            Msg.Show("Cannot add to orders", "Adding To Orders Failed", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                                        }
                                        else if (result == 10)
                                        {
                                            Clear();
                                            Msg.Show("Order submitted successfully ", "Order Created", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
                                        }
                                        else
                                        {
                                            Msg.Show("Something went wrong and the order wasn't submitted successfully ", "Order Submition Failed", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                                        }
                                    };
                                    worker.RunWorkerAsync();
                                }
                            }
                        }
                    }
                }
                int sp2 = DBAccess.UpdateSystemParameter("AddingOrder", false);
            }
        }


        public Int32 AddCustomer()
        {
            Int32 id = 0;

            Customer newCustomer = new Customer();
            newCustomer.CompanyName = SelectedCustomer;
            newCustomer.FirstName = "Unknown";
            newCustomer.LastName = "Unknown";
            newCustomer.Telephone = "0000000000";
            newCustomer.Mobile = "0000000000";
            newCustomer.Email = "Unknown";
            newCustomer.Address = "Unknown";
            newCustomer.City = "Unknown";
            newCustomer.State = "Unknown";
            newCustomer.PostCode = "0000";

            id = DBAccess.InsertCustomerDetails(newCustomer);

            return id;
        }

        private string GetShiftNameById(int x)
        {
            string shiftName = string.Empty;

            switch (x)
            {
                case 1: shiftName = "Day";
                    break;
                case 2: shiftName = "Evening";
                    break;
                case 3: shiftName = "Night";
                    break;
                default:
                    break;
            }

            return shiftName;
        }





        private bool CheckWeekEndAllocation(DateTime date)
        {
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            bool allocate = false;
            bool isNotWeekend = bdg.CheckWeekEnd(date);


            if (isNotWeekend == false)
            {
                if (Msg.Show("Do you want to allocate this order to " + date.ToString("dd/MM/yyyy") + "(" + date.DayOfWeek + ") ?", "Order Allocation Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    List<int> machineIds = new List<int>();
                    machineIds.Add(1);
                    machineIds.Add(7);
                    int res = DBAccess.EnableDisableShift(date, machineIds, true, true, true, true);
                    if (res > 0)
                    {
                        allocate = true;
                    }
                    else
                    {
                        DBAccess.InsertErrorLog(DateTime.Now + " Did not update the Shifts in ProductionTimeTable - Function(CheckWeekEndAllocation) | Values(Date - " + date + ")");
                    }
                }
                else
                {
                    allocate = false;
                }
            }
            else
            {
                allocate = true;
            }

            return allocate;
        }

        private decimal CalTotMixesLeft(int mixingMachineId, DateTime mDate, decimal bc)
        {
            Int32 mixPrdTimeTableId = 0;
            decimal curMixes = 0;
            decimal totBL = 0;
            //GET CURRENT MIXES FROM MixingCurrentCapacity Table
            if (mixingMachineId > 0)
            {
                List<ProductionTimeTable> pTL = DBAccess.GetProductionTimeTableByID(mixingMachineId, mDate);//Gte the Mixing Time Table ID
                foreach (var itempTL in pTL)
                {
                    mixPrdTimeTableId = itempTL.ID;
                }
                curMixes = DBAccess.GetCurrentMixingCapacity(mixPrdTimeTableId);
            }
            ///GET CURRENT BLOCKS LOGS FROM the list
            //decimal total = CurrentCapacities.Sum(x => x.BlocksLogs);

            //Get Current date id's
            List<ProductionTimeTable> pList = DBAccess.GetProductionTimeTableByID(1, mDate);
            int cId = 0;
            foreach (var itemPL in pList)
            {
                cId = itemPL.ID;
            }

            //GET TOTAL BLOCKSLOGS FROM GradingScheduling Table
            totBL = DBAccess.GetCurrentBlockLogTotal(cId);
            decimal totMixingOccupied = curMixes + totBL + bc;
            decimal maxMixesPerDay = 0;
            if (mixPrdTimeTableId > 0)
            {
                maxMixesPerDay = DBAccess.GetMaxMixes(mixPrdTimeTableId);

            }
            decimal totMixesLeft = maxMixesPerDay - totMixingOccupied;

            return totMixesLeft;
        }

        public List<DateShift> GetMixingShift(DateTime lDate)
        {
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            List<DateShift> dateShift = new List<DateShift>();
            List<ShiftDetails> subShifts = null;

            if (MixingTimeTableDetails.Count != 0)
            {
                foreach (var item in MixingTimeTableDetails)
                {
                    if (dateShift.Count == 0)
                    {
                        if (item.ProductionDate == lDate)
                        {
                            subShifts = new List<ShiftDetails>();
                            if (item.IsMachineActive == true)
                            {
                                //Mixing machine is working, get the grading machine id
                                List<ProductionTimeTable> ptl = DBAccess.GetProductionTimeTableByID(1, lDate);
                                int id = item.ID;

                                foreach (var itemPTL in ptl)
                                {
                                    id = itemPTL.ID;
                                }
                                subShifts.Add(new ShiftDetails() { ProTimeTableID = id });

                                DateShift ds = new DateShift();
                                ds.ProdDate = lDate;
                                ds.ShiftList = subShifts;
                                dateShift.Add(ds);
                            }
                            else
                            {
                                Console.WriteLine("Mixing Machine is switched off - " + lDate);
                                lDate = bdg.AddBusinessDays(lDate, 1);
                                dateShift = GetMixingShift(lDate);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (dateShift.Count == 0)
                {
                    //Create a new date
                    lDate = bdg.AddBusinessDays(lDate, 1);
                    dateShift = GetMixingShift(lDate);
                }
            }
            return dateShift;
        }

        public DateTime SubtractBusinessDays(DateTime current, int days)
        {
            DateTime pDate = CurrentDate;
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();

            pDate = Convert.ToDateTime(bdg.SubstractBusinessDays(current, days).ToString("dd/MM/yyyy"));

            return pDate;
        }
        private decimal CalcBlkLog(decimal Quantity, decimal MaxItemsPer)
        {
            decimal BlockLogQty = 0;
            if (MaxItemsPer == 0)
            {
                MaxItemsPer = 1;
            }

            return BlockLogQty = Math.Ceiling(Quantity / MaxItemsPer);
        }



        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }

        private string GetSlitPeelType(string t)
        {
            string type = string.Empty;

            if (t == "EA" || t == "TILE")
            {
                type = "Slit";
            }
            else if (t == "M2" || t == "ROLL")
            {
                type = "Peel";
            }
            else
            {
                type = "Unknown";
            }

            return type;
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


        //#region Validation

        //static readonly string[] ValidatedProperies = 
        //{
        //    "DiscountedTotal"
        //};

        //public bool IsValid
        //{
        //    get
        //    {
        //        foreach (string property in ValidatedProperies)
        //        {
        //            if (GetValidationError(property) != null)

        //                return false;
        //        }

        //        return true;
        //    }
        //}

        //protected bool CanSave
        //{
        //    get
        //    {
        //        return IsValid;
        //    }
        //}



        //private string ValidateDiscountedTotal()
        //{
        //    if (DiscountedTotal <= 0)
        //    {
        //        return "Production Order cannot be empty! please add items to the production order!";
        //    }
        //    return null;
        //}

        //string GetValidationError(string propertyName)
        //{

        //    string error = null;

        //    switch (propertyName)
        //    {               
        //        case "DiscountedTotal":
        //            error = ValidateDiscountedTotal();
        //            break;

        //        default:
        //            error = null;
        //            throw new Exception("Unexpected property being validated on Service");
        //    }

        //    return error;
        //}


        //string IDataErrorInfo.Error
        //{
        //    get
        //    {
        //        return null;
        //    }
        //}


        //string IDataErrorInfo.this[string propertyName]
        //{
        //    get
        //    {
        //        return GetValidationError(propertyName);
        //    }
        //}

        //#endregion

        private void ShowAddCustomer()
        {
            var childWindow = new ChildWindowView();
            childWindow.customer_Closed += (r =>
            {
                int CustID = r.CustomerId;
                string CompName = r.CompanyName;
                string FirstName = r.FirstName;
                string LastName = r.LastName;
                string Telephone = r.Telephone;
                string Mobile = r.Mobile;
                string Email = r.Email;
                string Address = r.Address;
                string city = r.City;
                string state = r.State;
                string PostCode = r.PostCode;

                CustomerList.Add(new Customer() { CustomerId = CustID, CompanyName = CompName, FirstName = FirstName, LastName = LastName, Telephone = Telephone, Mobile = Mobile, Email = Email, Address = Address, City = city, State = state, PostCode = PostCode });

                //SelectedCustomer = CustID;

            });
            childWindow.Show(1);

        }



        private void ShowAddFreightWindow()
        {
            var childWindow = new ChildWindowView();
            childWindow.freight_Closed += (r =>
            {
                int ID = r.Id;
                string FreightName = r.FreightName;
                string FreightUnit = r.FreightUnit;
                decimal FreightPrice = r.FreightPrice;
                string FreightDescription = r.FreightDescription;

                FreightList.Add(new Freight() { Id = ID, FreightName = FreightName, FreightUnit = FreightUnit, FreightPrice = FreightPrice, FreightDescription = FreightDescription });

                SelectedFreight = ID;

            });
            childWindow.ShowFreight(1);
        }

        private void AddToGradedStock()
        {
            var childWindow = new ChildWindowView();
            //childWindow.addToGradedStock_Closed += (r =>
            //{

            //});
            childWindow.ShowAddGradedStockInside();
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

       

        public delegate void UpdateProgressDelegate(int percentage, int recordCount);

        //public void UpdateProgressText(int percentage, int recordCount)
        //{
        //    pd.ProgressText = string.Format("{0}% of {1} Records", percentage.ToString(), recordCount);
        //    pd.ProgressValue = percentage;
        //}

        //    void CancelProcess(object sender, EventArgs e)
        //    {
        //        worker.CancelAsync();
        //    }

        private bool CheckCapacity(ObservableCollection<OrderDetails> rawMaterialDetails)
        {
            bool validate = true;

            if (GradingDate.Date != CurrentDate.Date)
            {
                if (ConvertOrderTypeToInt() == 1)
                {
                    validate = false;
                    Msg.Show("An urgent order can only be created for today!", "Error Selecting Date", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                }
                else
                {
                    foreach (var itemRMD in rawMaterialDetails)
                    {
                        if (!MixingOnlyList.Select(x => x.RawProductID).Contains(itemRMD.Product.RawProduct.RawProductID))
                        {
                            bool res1 = true;
                            bool res2 = true;
                            bool twoGrading = false;
                            string errMsg = "";
                            if (itemRMD.Product.ProductID != 12)
                            {
                                List<RawProductMachine> rawProductMachineList = DBAccess.GetMachineIdByRawProdId(itemRMD.Product.RawProduct.RawProductID);
                                List<Formulas> fList = DBAccess.GetFormulaDetailsByRawProdID(itemRMD.Product.RawProduct.RawProductID);
                                if (fList.Count > 0)
                                {
                                    if (fList[0].ProductCapacity1 > 0 && fList[0].ProductCapacity2 > 0)
                                    {
                                        twoGrading = true;
                                    }

                                    res1 = CheckCapacityByDate(GradingDate, fList[0].ProductCapacity1, fList[0].GradingWeight1, ConvertOrderTypeToInt(), ref errMsg, rawProductMachineList[0].GradingMachineID);
                                    if (res1)
                                    {
                                        if (twoGrading)
                                        {
                                            res2 = CheckCapacityByDate(GradingDate, fList[0].ProductCapacity2, fList[0].GradingWeight2, ConvertOrderTypeToInt(), ref errMsg, rawProductMachineList[0].GradingMachineID);
                                            if (res2)
                                            {
                                            }
                                            else
                                            {
                                                validate = false;
                                                if (string.IsNullOrEmpty(errMsg))
                                                {
                                                    errMsg = "Capacity is full on the selected date, please choose another date!";
                                                }
                                                Msg.Show(errMsg, "Choose Date", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                                                break;
                                            }
                                        }
                                        else
                                        {
                                        }
                                    }
                                    else
                                    {
                                        validate = false;
                                        if (string.IsNullOrEmpty(errMsg))
                                        {
                                            errMsg = "Capacity is full on the selected date, please choose another date!";
                                        }
                                        Msg.Show(errMsg, "Choose Date", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return validate;
        }

        private int ConvertOrderTypeToInt()
        {
            int n = 0;
            switch (SelectedOrderType)
            {
                case "Normal": n = 3;
                    break;
                case "Urgent": n = 1;
                    break;
                default:
                    break;
            }

            return n;
        }


        private int ConvertOrderPriorityToInt()
        {
            int n = 0;
            switch (SelectedOrderPriority)
            {
                //case "Order": n = 1;
                //    break;
                case "Block/Log Stock": n = 2;
                    break;
                case "Finished Goods Stock": n = 1;
                    break;
                default:
                    break;
            }

            return n;
        }


        private bool CheckCapacityByDate(DateTime IDate, int prodCapId, decimal gradingWeight, int orderTypeId, ref string message, int gradingMachineId)
        {
            int curShift = 0;
            bool result = true;
            decimal capLeft = 0;
            shiftList = new List<ProductionTimeTable>();
            shiftList = DBAccess.GetProductionTimeTableByID(1, IDate);
            List<ProductionTimeTable> pList = new List<ProductionTimeTable>();
            pList = DBAccess.GetProductionTimeTableByID(1, IDate);//Required Date


            if (shiftList.Count == 0)
            {
                //Get the current shhift
                foreach (var item in ShiftDetails)
                {
                    bool isShift = TimeBetween(DateTime.Now, item.StartTime, item.EndTime);

                    if (isShift == true)
                    {
                        curShift = item.ShiftID;
                    }
                }

                do
                {
                    if (shiftList.Count == 0 || shiftList == null)
                    {
                        Production prod = new Production();
                        prod.AddNewDates(CurrentDate, false);
                        shiftList = DBAccess.GetProductionTimeTableByID(1, CurrentDate);//Get current date id
                    }

                }
                while (shiftList.Count == 0 || shiftList == null);

                do
                {
                    Production prod = new Production();
                    prod.AddNewDates(IDate, false);
                    pList = DBAccess.GetProductionTimeTableByID(1, IDate);//Required Date
                }
                while (pList.Count == 0 || pList == null);

            }
            else
            {

                int prodTimeTableId = 0;
                foreach (var itemPL in pList)
                {
                    prodTimeTableId = itemPL.ID;
                }

                //CapacityLimitations = DBAccess.GetCapacityLimitations(prodTimeTableId);
                CapacityLimitations = DBAccess.GetProductCapacityLimitByTimeTable(shiftList);
                foreach (var itemSL in shiftList)
                {
                    capLeft = 0;
                    if (itemSL.MachineID == gradingMachineId)
                    {

                        if (itemSL.IsMachineActive == true)
                        {
                            if (itemSL.IsDayShiftActive == true && curShift <= 2)
                            {
                                var data = new ProductCapacity();
                                //shift = 0;
                                if (prodCapId == 7 || prodCapId == 8 || prodCapId == 9)
                                {
                                    data.CapacityKG = 10000;
                                    data.ProductionTimeTableID = itemSL.ID;
                                    data.RubberGradingID = prodCapId;
                                }
                                else
                                {
                                    data = CapacityLimitations.Single(c => c.ProductionTimeTableID == itemSL.ID && c.RubberGradingID == prodCapId && c.Shift == 1);//ProductCapacity
                                }
                                decimal cWeight = DBAccess.CheckCapacityByDateShift(itemSL.ID, prodCapId, 1, orderTypeId);//CurrentCapacity
                                decimal avaWeight = data.CapacityKG - cWeight;
                                if (avaWeight > 0)
                                {
                                    decimal existingBlkLogs = CalNoOfBlocks(avaWeight, gradingWeight);

                                    //decimal maxBlkLogs = CalNoOfBlocks(data.CapacityKG, gradingWeight);

                                    if (existingBlkLogs > 0)
                                    {
                                        capLeft = 1;
                                    }
                                }
                            }
                            if (itemSL.IsEveningShiftActive == true && curShift < 3)
                            {
                                var data = new ProductCapacity();
                                //shift = 0;
                                if (prodCapId == 7 || prodCapId == 8 || prodCapId == 9)
                                {
                                    data.CapacityKG = 10000;
                                    data.ProductionTimeTableID = itemSL.ID;
                                    data.RubberGradingID = prodCapId;
                                }
                                else
                                {
                                    data = CapacityLimitations.Single(c => c.ProductionTimeTableID == itemSL.ID && c.RubberGradingID == prodCapId && c.Shift == 2);//ProductCapacity
                                }
                                decimal cWeight = DBAccess.CheckCapacityByDateShift(itemSL.ID, prodCapId, 2, orderTypeId);//CurrentCapacity
                                decimal avaWeight = data.CapacityKG - cWeight;
                                if (avaWeight > 0)
                                {
                                    decimal existingBlkLogs = CalNoOfBlocks(avaWeight, gradingWeight);

                                    //decimal maxBlkLogs = CalNoOfBlocks(data.CapacityKG, gradingWeight);

                                    if (existingBlkLogs > 0)
                                    {
                                        capLeft += 1;
                                    }
                                }
                            }
                            if (itemSL.IsNightShiftActive == true)
                            {
                                var data = new ProductCapacity();
                                //shift = 0;
                                if (prodCapId == 7 || prodCapId == 8 || prodCapId == 9)
                                {
                                    data.CapacityKG = 10000;
                                    data.ProductionTimeTableID = itemSL.ID;
                                    data.RubberGradingID = prodCapId;
                                }
                                else
                                {
                                    data = CapacityLimitations.Single(c => c.ProductionTimeTableID == itemSL.ID && c.RubberGradingID == prodCapId && c.Shift == 3);//ProductCapacity
                                }
                                decimal cWeight = DBAccess.CheckCapacityByDateShift(itemSL.ID, prodCapId, 3, orderTypeId);//CurrentCapacity
                                decimal avaWeight = data.CapacityKG - cWeight;
                                if (avaWeight > 0)
                                {
                                    decimal existingBlkLogs = CalNoOfBlocks(avaWeight, gradingWeight);

                                    //decimal maxBlkLogs = CalNoOfBlocks(data.CapacityKG, gradingWeight);

                                    if (existingBlkLogs > 0)
                                    {
                                        capLeft += 1;
                                    }
                                }
                            }

                            if (itemSL.IsDayShiftActive == false && itemSL.IsEveningShiftActive == false && itemSL.IsNightShiftActive == false)
                            {
                                result = false;

                                if (Msg.Show("The shifts are disabled for " + IDate.ToString("dd/MM/yyy") + "(" + IDate.DayOfWeek + ")" + System.Environment.NewLine + "Do you want to enable shifts and allocate this order for " + IDate.ToString("dd/MM/yyy"), "Orders Shifting", MsgBoxButtons.YesNo, MsgBoxImage.Information, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                                {
                                    List<int> machineIds = new List<int>();
                                    machineIds.Add(1);
                                    machineIds.Add(7);
                                    int res = DBAccess.EnableDisableShift(IDate, machineIds, true, true, true, true);
                                    result = true;
                                }
                                if (result == false)
                                {
                                    message = "Please select a differnt date to allocate this order!";
                                }

                                break;
                            }

                            if (capLeft <= 0)
                            {
                                result = false;
                                break;
                            }
                        }
                        else
                        {
                            result = false;
                            message = "Machine is switched off on the selected date.  Please choose another date!";
                            //Msg.Show("Machine is switched off on the selected date.  Please choose another date!", "Choose Date", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                            break;
                        }
                    }
                }

            }

            return result;
        }

        private void ViewGradingFormula(object parameter)
        {
            int index = Order.OrderDetails.IndexOf(parameter as OrderDetails);
            if (index > -1 && index < Order.OrderDetails.Count)
            {
                if (Order.OrderDetails[index].Product != null)
                {
                    var childWindow = new ChildWindow();
                    childWindow.ShowFormula(Order.OrderDetails[index].Product.RawProduct.Formula.GradingFormula.Replace("\\\\", "/"));
                }
                else
                {
                    Msg.Show("Please enter a product to view formula", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }
            }
        }

        private void ViewMixingFormula(object parameter)
        {
            int index = Order.OrderDetails.IndexOf(parameter as OrderDetails);
            if (index > -1 && index < Order.OrderDetails.Count)
            {
                if (Order.OrderDetails[index].Product != null)
                {
                var childWindow = new ChildWindow();
                childWindow.ShowFormula(Order.OrderDetails[index].Product.RawProduct.Formula.MixingFormula.Replace("\\\\", "/"));
                }
                else
                {
                    Msg.Show("Please enter a product to view formula", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }
            }
        }

        private void ClearMixingOrder(Object parameter)
        {
            int index = ProductionHistory.IndexOf(parameter as ProductionHistory);
            if (index >= 0)
            {
                if (Msg.Show("Are you sure you want to clear this mixing order?", "", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {

                    int res = DBAccess.ClearMixingOrders(ProductionHistory[index].ID, userName);
                    if(res > 0)
                    {
                        GetMixingUncompleted();
                    }
                    else
                    {
                        Msg.Show("Could not clear this mixing order" + System.Environment.NewLine + 
                            "Please try again later", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
                    }

                }
            }
        }

        #region COMMANDS


        //private ICommand _selectionChangedCommand;
        //public ICommand SelectionChangedCommand
        //{
        //    get
        //    {
        //        return _selectionChangedCommand ?? (_selectionChangedCommand = new LogOutCommandHandler(() => CalculateQtyToMake(), canExecute));
        //    }
        //}
        public Microsoft.Practices.Prism.Commands.DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }


        public ICommand RemoveCommand
        {
            get
            {
                if (_command == null)
                {
                    _command = new A1QSystem.Commands.DelegateCommand(CanExecute, Execute);
                }
                return _command;
            }
        }


        public ICommand RefreshGrdid
        {
            get
            {
                return _refreshGrdid ?? (_refreshGrdid = new LogOutCommandHandler(() => CalculateProductionTotals(), true));
            }
        }

        public ICommand ChiftChangedCommand
        {
            get
            {
                return _chiftChangedCommand ?? (_chiftChangedCommand = new LogOutCommandHandler(() => CalculateProductionTotals(), true));
            }
        }

        public ICommand ClearFields
        {
            get
            {
                return _clearFields ?? (_clearFields = new LogOutCommandHandler(() => Clear(), canExecute));
            }
        }

        public ICommand AddOrder
        {
            get
            {
                if (_addOrder == null)
                    _addOrder = new A1QSystem.Commands.RelayCommand(param => this.AddOrderProduction(), param => this.canExecute);

                return _addOrder;
            }
        }

        public ICommand SelectionChangedCommand
        {
            get
            {
                if (_selectionChangedCommand == null)
                {
                    _selectionChangedCommand = new DelegateCommand(CanExecute, ProductChanged);
                }
                return _selectionChangedCommand;
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new LogOutCommandHandler(() => Switcher.Switch(new OrdersMainMenuView(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, privilages, metaData)), canExecute));
            }
        }
        public ICommand OrdersCommand
        {
            get
            {
                return _ordersCommand ?? (_ordersCommand = new LogOutCommandHandler(() => Switcher.Switch(new OrdersMainMenuView(userName, state, privilages, metaData)), canExecute));
            }
        }
        public ICommand AdminDashboardCommand
        {
            get
            {
                return _adminDashboardCommand ?? (_adminDashboardCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand GradedStockCommand
        {
            get
            {
                return _gradedStockCommand ?? (_gradedStockCommand = new LogOutCommandHandler(() => AddToGradedStock(), canExecute));
            }
        }

        public ICommand AddNewRowCommand
        {
            get
            {
                return _addNewRowCommand ?? (_addNewRowCommand = new LogOutCommandHandler(() => AddNewRow(), canExecute));
            }
        }



        public ICommand GradingFormulaCommand
        {
            get
            {
                if (_gradingFormulaCommand == null)
                {
                    _gradingFormulaCommand = new DelegateCommand(CanExecute, ViewGradingFormula);
                }
                return _gradingFormulaCommand;
            }
        }

        public ICommand MixingFormulaCommand
        {
            get
            {
                if (_mixingFormulaCommand == null)
                {
                    _mixingFormulaCommand = new DelegateCommand(CanExecute, ViewMixingFormula);
                }
                return _mixingFormulaCommand;
            }
        }

        public ICommand QtyChangedCommand
        {
            get
            {
                return _qtyChangedCommand ?? (_qtyChangedCommand = new LogOutCommandHandler(() => CalculateProductionTotals(), true));
            }
        }

        public Microsoft.Practices.Prism.Commands.DelegateCommand AddCustomerCommand
        {
            get { return _addCustomerCommand; }
        }

        public Microsoft.Practices.Prism.Commands.DelegateCommand AddFreightCommand
        {
            get { return _addFreightCommand; }
        }

        public ICommand ClearMixingOrderCommand
        {
            get
            {
                if (_clearMixingOrderCommand == null)
                {
                    _clearMixingOrderCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, ClearMixingOrder);
                }
                return _clearMixingOrderCommand;
            }
        }

        #endregion


    }
}
