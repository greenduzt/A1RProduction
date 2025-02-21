using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.AdminDashboard;
using A1QSystem.View.Orders;
using A1QSystem.ViewModel.PageSwitcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Orders
{
    public class OrdersMainMenuViewModel : ViewModelBase
    {
        public string UserName { get; set; }
        public string State { get; set; }
        public List<UserPrivilages> Privilages { get; set; }
        private List<MetaData> metaData;
        private string _version;
        private ICommand _homeCommand;
        private ICommand _backCommand;
        private ICommand _ordersApprovalCommand;
        private ICommand _orderOverviewCommand;
        private ICommand _pickingOrderCommand;
        private ICommand _newOrderCommand;
        private ICommand _ammendOrderCommand;
        private ICommand _adminDashboardCommand;
        private ICommand _newSalesOrderCommand;
        private bool _canExecute;

        public OrdersMainMenuViewModel(string userName, string state, List<UserPrivilages> pri, List<MetaData> md)
        {
            UserName = userName;
            State = state;
            Privilages = pri;
            metaData = md;
            _canExecute = true;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
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

        public ICommand NewOrderCommand
        {
            get
            {
                return _newOrderCommand ?? (_newOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new AddProductionOrderView(UserName, State, Privilages, metaData)), _canExecute));
            }
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(UserName, State, Privilages, metaData)), _canExecute));
            }
        }

        public ICommand BackCommand
        {
            get
            {

                return _backCommand ?? (_backCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(UserName, State, Privilages, metaData)), _canExecute));
            }
        }


        public ICommand OrdersApprovalCommand
        {
            get
            {

                return null;//_ordersApprovalCommand ?? (_ordersApprovalCommand = new LogOutCommandHandler(() => Switcher.Switch(new OrderPendingView(UserName, State, Privilages)), _canExecute));
            }
        }

        public ICommand OrderStatusCommand
        {
            get
            {
                return null;// _orderOverviewCommand ?? (_orderOverviewCommand = new LogOutCommandHandler(() => Switcher.Switch(new OrderPendingView(UserName, State, Privilages)), _canExecute));
            }
        }

        public ICommand PickingOrderCommand
        {
            get
            {
                return _pickingOrderCommand ?? (_pickingOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new PickingOrderView(UserName, State, Privilages, metaData)), _canExecute));
            }
        }

        public ICommand AmendOrderCommand
        {
            get
            {
                return _ammendOrderCommand ?? (_ammendOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new AmendOrderView(UserName, State, Privilages, metaData)), _canExecute));
            }
        }

        public ICommand AdminDashboardCommand
        {
            get
            {
                return _adminDashboardCommand ?? (_adminDashboardCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(UserName, State, Privilages, metaData)), _canExecute));
            }
        }

        public ICommand NewSalesOrderCommand
        {
            get
            {
                return _newSalesOrderCommand ?? (_newSalesOrderCommand = new A1QSystem.Commands.LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(UserName, State, Privilages, metaData)), _canExecute));
            }
        }
    }
}
