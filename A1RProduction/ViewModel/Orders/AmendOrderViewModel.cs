using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.DeliveryDetails;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Orders;
using A1QSystem.View;
using A1QSystem.View.Orders;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Orders
{
    public class AmendOrderViewModel : ViewModelBase
    {
        private Order _order;
        private Order _orgOrder;
        private List<Order> _orderList;//Search section
        private ObservableCollection<Customer> _customerList;
        ObservableCollection<Freight> _freightList;
        private List<UserPrivilages> privilages;
        public ObservableCollection<Product> Product { get; set; }
        public DateTime CurrentTime { get; set; }
        public DateTime CurrentDate { get; set; }
        private string userName;
        private string state;
        private string _salesNo;
        private string _gradingComments;
        private string _selectedOrderType;
        private string _orderNo;
        private int _selectedCustomer;
        private int _selectedFreight;
        private Order _selectedOrder;
        private DateTime _freightArrivTime;
        private DateTime _selectedDate;
        private bool canExecute;
        private bool _dataGridEnableDisable;
        private bool _freightTimeAvailable;
        private bool _freightTimeEnabled;
        private bool _isProductionDateAvailable;
        private bool _productionDateAvailable;
        private List<MetaData> metaData;
        private ICommand _ordersCommand;
        private ICommand _homeCommand;
        private ICommand _searchOrder;
        private ICommand _removeCommand;
        private ICommand _clearFields;
        private ICommand _clearSearch;
        private ICommand _updateOrderCommand;
        private Microsoft.Practices.Prism.Commands.DelegateCommand _addCustomerCommand;
        private Microsoft.Practices.Prism.Commands.DelegateCommand _addFreightCommand;

        public AmendOrderViewModel(string UserName, string State, List<UserPrivilages> UserPrivilages, List<MetaData> md)
        {
            CurrentDate = Convert.ToDateTime(NTPServer.GetNetworkTime().ToString("dd/MM/yyyy"));
            userName = UserName;
            state = State;
            privilages = UserPrivilages;
            metaData = md;
            CurrentTime.Date.AddHours(00).AddMinutes(00);
            SelectedDate = CurrentDate;
            FreightArrivTime = CurrentTime;
            FreightTimeEnabled = false;
            FreightTimeAvailable = false;
            canExecute = true;
            OrderList = new List<Order>();
            Order = new Order();
            Order.OrderDetails = new ObservableCollection<OrderDetails>();
            OrgOrder = new Order();
            OrgOrder.OrderDetails = new ObservableCollection<OrderDetails>();
            CustomerList = new ObservableCollection<Customer>();
            _addCustomerCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(ShowAddCustomer);
            _addFreightCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(ShowAddFreightWindow);
            LoadOrders();
            LoadProducts();
            LoadCustomers();
            LoadFreights();
            DataGridEnableDisable = false;
            IsProductionDateAvailable = false;

            SelectedOrderType = "Normal";
            SelectedCustomer = 1560;
            SelectedFreight = 56;
            SalesNo = "Stock Filling";
        }

        private void LoadOrders()
        {
            OrderList = DBAccess.GetAllOrders();
        }

        private void LoadProducts()
        {
            Product = DBAccess.GetAllProds();
        }

        private void LoadCustomers()
        {
            CustomerList = DBAccess.GetCustomerData();            
        }

        private void LoadFreights()
        {
            FreightList = new ObservableCollection<Freight>();
            FreightList.Add(new Freight() { Id = 56, FreightName = "No Freight", FreightUnit = "PLT", FreightPrice = 0, FreightDescription = "No Freight" });
        }

        private void ShowAddCustomer()
        {
        }

        private void ShowAddFreightWindow()
        {
        }

        private void RetrieveOrder()
        {            
            if(SelectedOrder != null)
            {
                if(SelectedOrder.OrderNo != 0)
                {                   
                    DataGridEnableDisable = true;
                    Order o = DBAccess.GetOrderByOrderNo(SelectedOrder.OrderNo);

                    OrgOrder = o;
                    OrgOrder = DBAccess.GetOrderDetailsByOrderNo(SelectedOrder.OrderNo);

                    Order = OrgOrder;

                    //Update properties;
                    OrderNo = o.OrderNo.ToString();
                    SalesNo = o.SalesNo;
                    SelectedCustomer = o.Customer.CustomerId;
                    SelectedFreight = o.DeliveryDetails[0].FreightID;
                    SelectedDate = o.RequiredDate;
                    GradingComments = o.Comments;
                    SelectedOrderType = ConvertIntToOrderType(o.OrderType);
                    ProductionDateAvailable = o.IsRequiredDateSelected;
                    IsProductionDateAvailable = o.IsRequiredDateSelected;
                }
                else
                {
                    DataGridEnableDisable = false;
                    Msg.Show("Order does not exist!" + System.Environment.NewLine + "Please enter valid details to search", "Order Does Not Exist", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                }
            }
            else
            {
                Msg.Show("Order does not exist!" + System.Environment.NewLine + "Please enter valid details to search", "Order Does Not Exist", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
        }

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
            }
            if (Order.OrderDetails.Count == 0)
            {
                Order.OrderDetails = new ObservableCollection<OrderDetails>();
            }
        }

        private void Clear()
        {
            SelectedDate = CurrentDate;
            SalesNo = "Stock Filling";
            SelectedCustomer = 1560;
            SelectedFreight = 56;
            FreightArrivTime = CurrentTime;
            GradingComments = string.Empty;
            ProductionDateAvailable = false;
            IsProductionDateAvailable = false;
            SelectedOrder = new Order();
            Order = new Order();
            DataGridEnableDisable = false;
            SelectedOrderType = "Normal";
            OrderNo = string.Empty;
        }

       
        private void UpdateOrder()
        {
            Order tempOrder = new Order();

            tempOrder.OrderType = ConvertOrderTypeToInt();
            tempOrder.RequiredDate = SelectedDate;
            tempOrder.SalesNo = SalesNo;
            tempOrder.Comments = GradingComments;
            tempOrder.DeliveryDetails = new List<Delivery>() { new Delivery(){FreightID = SelectedFreight}};
            tempOrder.IsRequiredDateSelected = IsProductionDateAvailable;
            tempOrder.OrderCreatedDate = DateTime.Now.Date;
            tempOrder.Customer = new Customer() { CustomerId = SelectedCustomer};

            foreach (var itemOO in OrgOrder.OrderDetails)
            {
                foreach (var itemOD in Order.OrderDetails)
                {
                    if (itemOO.Product.ProductID == itemOD.Product.ProductID)
                    {
                        if (itemOO.Quantity != itemOD.Quantity)
                        {
                            tempOrder.OrderDetails = new ObservableCollection<OrderDetails>();
                            tempOrder.OrderDetails.Add(itemOD);
                        }
                    }
                }
            }

            if(tempOrder.OrderDetails.Count > 0)
            {
               //Calculate Blocks/Logs


               //Update Order


               //Seperate Mixing and Grading

            }
            else
            {
                tempOrder = null;
            }
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

        private string ConvertIntToOrderType(int o)
        {
            string ot = string.Empty;
            switch (o)
            {
                case 3: ot = "Normal";
                    break;
                case 1: ot = "Urgent";
                    break;
                default:
                    break;
            }

            return ot;
        }

        #region PUBLIC_PROPERTIES

        public string OrderNo
        {
            get { return _orderNo; }
            set
            {
                _orderNo = value;
                RaisePropertyChanged(() => this.OrderNo);
            }
        }

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                RaisePropertyChanged(() => this.SelectedDate);
            }
        }

        public Order Order
        {
            get { return _order; }
            set
            {
                _order = value;
                RaisePropertyChanged(() => this.Order);
            }
        }

        public Order OrgOrder
        {
            get { return _orgOrder; }
            set
            {
                _orgOrder = value;
                RaisePropertyChanged(() => this.OrgOrder);
            }
        }

        public List<Order> OrderList
        {
            get
            {
                return _orderList;
            }
            set
            {
                _orderList = value;
                RaisePropertyChanged(() => this.OrderList);
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

        public Order SelectedOrder
        {
            get
            {
                return _selectedOrder;
            }
            set
            {
                _selectedOrder = value;
                RaisePropertyChanged(() => this.SelectedOrder);             
            }
        }

        public bool DataGridEnableDisable
        {
            get
            {
                return _dataGridEnableDisable;
            }
            set
            {
                _dataGridEnableDisable = value;
                RaisePropertyChanged(() => this.DataGridEnableDisable);             
            }
        }

        public string SalesNo
        {
            get
            {
                return _salesNo;
            }
            set
            {
                _salesNo = value;
                RaisePropertyChanged(() => this.SalesNo);
            }
        }

        public int SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                RaisePropertyChanged(() => this.SelectedCustomer);
                if (SelectedCustomer != 0)
                {
                    if (SelectedCustomer == 1560)
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

        public DateTime FreightArrivTime
        {
            get { return _freightArrivTime; }
            set
            {
                _freightArrivTime = value;
                RaisePropertyChanged(() => this.FreightArrivTime);
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
                    SelectedDate = CurrentDate;
                }
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


        public string GradingComments
        {
            get { return _gradingComments; }
            set
            {
                _gradingComments = value;
                RaisePropertyChanged(() => this.GradingComments);
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

        #endregion

        public ICommand UpdateOrderCommand
        {
            get
            {
                if (_updateOrderCommand == null)
                    _updateOrderCommand = new A1QSystem.Commands.RelayCommand(param => this.UpdateOrder(), param => this.canExecute);

                return _updateOrderCommand;
            }
        }

        public ICommand SearchOrder
        {
            get
            {
                return _searchOrder ?? (_searchOrder = new LogOutCommandHandler(() => RetrieveOrder(), canExecute));
            }
        }

        public ICommand ClearSearch
        {
            get
            {
                return _clearSearch ?? (_clearSearch = new LogOutCommandHandler(() => Clear(), canExecute));
            }
        } 

        public ICommand OrdersCommand
        {
            get
            {
                return _ordersCommand ?? (_ordersCommand = new LogOutCommandHandler(() => Switcher.Switch(new OrdersMainMenuView(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, privilages, metaData)), canExecute));
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, Execute);
                }
                return _removeCommand;
            }
        }       

        public ICommand ClearFields
        {
            get
            {
                return _clearFields ?? (_clearFields = new LogOutCommandHandler(() => Clear(), canExecute));
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
    }
}
