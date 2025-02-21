
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Interfaces;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Stock;
using A1QSystem.View;
using A1QSystem.View.Orders;
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
using System.Windows.Threading;

namespace A1QSystem.ViewModel.Orders
{
    public class PickingOrdersViewModel
    {
        public string UserName { get; set; }
        public string State { get; set; }
        public List<UserPrivilages> Privilages { get; set; }
        public ObservableCollection<Order> orders { get; set; }
        public ObservableCollection<StockDetails> stock { get; set; }
        public ObservableCollection<PickingOrder> pickingOrders { get; set; }
        public ListCollectionView PickingOrderCollView { get; set; }
        private List<MetaData> metaData;
        private ICommand navHomeCommand;
        private ICommand _backCommand;
        private bool _canExecute;

        public Dispatcher UIDispatcher { get; set; }

        public string ProductCode { get; set; }

        public PickingOrdersViewModel(string userName, string state, List<UserPrivilages> pri, Dispatcher uidispatcher, List<MetaData> md)
        {
            UserName = userName;
            State = state;
            Privilages = pri;         
            _canExecute = true;
            metaData = md;
            pickingOrders = new ObservableCollection<PickingOrder>();
            //Get all approved orders
                      

            this.LoadMessage(orders, stock);

          
                       
        }

        private void LoadMessage(ObservableCollection<Order> orders, ObservableCollection<StockDetails> stock)
        {
            this.UIDispatcher.BeginInvoke((Action)delegate()
            {
                if (stock != null)
                {
                    pickingOrders.Clear();
                    //////ObservableCollection<StockAvailabilityDetails> res = StockAvailabilityManager.GetStockAvailabilityInfo(orders, stock);
                    ObservableCollection<StockBalanceDetails> res = null;
                    foreach (var x in res)
                    {
                        //if (x.DisplayString.Equals("IN STOCK"))
                        //{
                            string status = string.Empty;
                            string statColor = string.Empty;
                            if (x.DisplayString.Equals("IN STOCK")) {
                               status = "IN STOCK - Attention Required";
                               statColor = "Green";
                            }
                            else if (x.DisplayString.Equals("OUT OF STOCK"))
                            {
                                status = "OUT OF STOCK - In Production";
                                statColor = "#AEC6CF";
                            }
                            else
                            {
                                status = "NOT IN STOCK";
                                statColor = "#6A995D";
                            }
                          //  pickingOrders.Add(new PickingOrder(UserName, State) { OrderID = x.OrderID, ProductCode = x.ProductCode, ProductDescription = x.ProductDescription, ReqAmount = x.ReqAmount, Quantity = x.StockInHand, ProductUnit = x.Unit, Status = status,OrderRequiredDate=x.PickingDate, StatusCol = statColor});
                        //}
                    }
                   
                    PickingOrderCollView = new ListCollectionView(pickingOrders);
                    PickingOrderCollView.GroupDescriptions.Add(new PropertyGroupDescription("OrderID"));
                }

            });

        }

     


       

        #region COMMANDS

      
        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new MainMenu(UserName, State, Privilages, metaData)), _canExecute));
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new OrdersMainMenuView(UserName, State, Privilages, metaData)), _canExecute));
            }
        }
        #endregion
    }
}
