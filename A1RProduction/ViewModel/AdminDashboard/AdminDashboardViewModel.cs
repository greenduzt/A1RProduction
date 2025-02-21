using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.Customers;
using A1QSystem.View.Maintenance;
using A1QSystem.View.Orders;
using A1QSystem.View.Production;
using A1QSystem.View.Production.Mixing;
using A1QSystem.View.Stock;
using A1QSystem.View.VehicleWorkOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.AdminDashboard
{
    public class AdminDashboardViewModel : ViewModelBase
    {
        private string userName;
        private string state;
        private List<UserPrivilages> userPrivilages;
        private bool canExecute;
        private string _productionMaintenanceVisibility;
        private string _ordersVisiblity;
        private string _stockVisiblity;
        private string _version;
        private List<MetaData> metaData;
        private ICommand _homeCommand;
        private ICommand _maintenanceCommand;
        private ICommand _ordersCommand;
        private ICommand _prodMaintenanceCommand;
        private ICommand _stockCommand;
        private ICommand _customerCommand;
        private ICommand _offSpecCommand;
        private ICommand _iBCChangeCommand;

        public AdminDashboardViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            userPrivilages = up;
            canExecute = true;
            metaData = md;
            if (up != null)
            {
                foreach (var item in up)
                {
                    if (item.Area == "Production")
                    {
                        ProductionMaintenanceVisibility = item.Visibility;
                    }
                    else if (item.Area == "Orders")
                    {
                        OrdersVisiblity = item.Visibility;
                    }
                    else if (item.Area == "Stock")
                    {
                        StockVisiblity = item.Visibility;
                    }
                }
            }
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");

            Version = data.Description;
        }

        private void ShowOffSepcReportView()
        {
            var childWindow = new ChildWindowView();
            //childWindow.addToGradedStock_Closed += (r =>
            //{

            //});
            childWindow.ShowOffSpecReport();
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

        public string ProductionMaintenanceVisibility
        {
            get
            {
                return _productionMaintenanceVisibility;
            }
            set
            {
                _productionMaintenanceVisibility = value;
                RaisePropertyChanged(() => this.ProductionMaintenanceVisibility);
            }
        }

        public string OrdersVisiblity
        {
            get
            {
                return _ordersVisiblity;
            }
            set
            {
                _ordersVisiblity = value;
                RaisePropertyChanged(() => this.OrdersVisiblity);
            }
        }

        public string StockVisiblity
        {
            get
            {
                return _stockVisiblity;
            }
            set
            {
                _stockVisiblity = value;
                RaisePropertyChanged(() => this.StockVisiblity);
            }
        }



        #region COMMANDS

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand MaintenanceCommand
        {
            get
            {
                return _maintenanceCommand ?? (_maintenanceCommand = new LogOutCommandHandler(() => Switcher.Switch(new MaintenanceMenuView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand OrdersCommand
        {
            get
            {
                return _ordersCommand ?? (_ordersCommand = new LogOutCommandHandler(() => Switcher.Switch(new OrdersMainMenuView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand ProdMaintenanceCommand
        {
            get
            {
                return _prodMaintenanceCommand ?? (_prodMaintenanceCommand = new LogOutCommandHandler(() => Switcher.Switch(new ProductionMaintenanceView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand StockCommand
        {
            get
            {
                return _stockCommand ?? (_stockCommand = new LogOutCommandHandler(() => Switcher.Switch(new StockMenuView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand CustomerCommand
        {
            get
            {
                return _customerCommand ?? (_customerCommand = new LogOutCommandHandler(() => Switcher.Switch(new CustomerDetailsView(userName, metaData)), canExecute));
            }
        }

        public ICommand OffSpecCommand
        {
            get
            {
                return _offSpecCommand ?? (_offSpecCommand = new LogOutCommandHandler(() => ShowOffSepcReportView(), canExecute));
            }
        }

        public ICommand IBCChangeCommand
        {
            get
            {
                return _iBCChangeCommand ?? (_iBCChangeCommand = new LogOutCommandHandler(() => Switcher.Switch(new ViewIBCChangeOverDetailsView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }




        #endregion
    }
}
