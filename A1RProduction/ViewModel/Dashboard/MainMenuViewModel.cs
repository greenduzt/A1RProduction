using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.AdminDashboard;
using A1QSystem.View.Customers;
using A1QSystem.View.Dashboard;
using A1QSystem.View.Machine.MachineHistory;
using A1QSystem.View.Machine.MachinePart;
using A1QSystem.View.Machine.MachineWorkOrders;
using A1QSystem.View.Maintenance;
using A1QSystem.View.Orders;
using A1QSystem.View.Production;
using A1QSystem.View.Production.Grading;
using A1QSystem.View.Production.Mixing;
using A1QSystem.View.Products;
using A1QSystem.View.Quoting;
using A1QSystem.View.Sales;
using A1QSystem.View.Stock;
using A1QSystem.View.Vehicles;
using A1QSystem.View.VehicleWorkOrders;
using A1QSystem.View.VehicleWorkOrders.History;
using A1QSystem.View.WorkOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel
{
    public class MainMenuViewModel : ViewModelBase
    {
        private string _adminVisibility;
        private string _workStationsVisibility;
        private string _version;
        private string _userName;
        private string _state;
        //private string _btnQuoting;
        //private string _btnSales;
        //private string _btnCustomers;
        //private string _btnProduction;
        //private string _btnMaintenance;
        //private string _btnProducts;
        //private string _btnFreight;
        //private string _btnActiveUsers;
        //private string _btnUsers;
        //private string _btnOrders;

        private List<UserPrivilages> _privilages;
        private List<MetaData> metaData;
        private bool _canExecute;

        private ICommand _quotingCommand;
        private ICommand _workStationsCommand;
        private ICommand _ordersCommand;
        //private ICommand _customersCommand;
        //private ICommand _productsCommand;
        private ICommand _vehiclesCommand;
        private ICommand _workOrdersCommand;
        private ICommand _adminCommand;
        private ICommand _newRepairWorkOrderCommand;
        private ICommand _vehicleWorkOrderHistoryCommand;
        private ICommand _printVehiclePartsCommand;
        private ICommand _machineWorkOrdersCommand;
        private ICommand _newMachineRepairWorkOrderCommand;
        private ICommand _machineWorkOrderHistoryCommand;
        private ICommand _printMachinePartsCommand;


        public MainMenuViewModel(string UserName, string State, List<UserPrivilages> uPriv,List<MetaData> md)
        {
           _userName = UserName;
           _state = State;
           _privilages = uPriv;
           metaData = md;
           _canExecute = true;
           var data = metaData.SingleOrDefault(x=>x.KeyName=="version");
           Version = data.Description;

           if (uPriv != null)
           {
               foreach (var item in uPriv)
               {
                   if (item.Area == "Admin")
                   {
                       AdminVisibility = item.Visibility;
                   }
                   if (item.Area == "WorkStations")
                   {
                       WorkStationsVisibility = item.Visibility;
                   }
               }

               //for (int i = 0; i < uPriv.Count; i++)
               //{
               //    BtnQuoting = uPriv[i].Quoting;
               //    BtnSales = uPriv[i].AddQuote;
               //    BtnMaintenance = uPriv[i].Maintenance;
               //    BtnProduction = uPriv[i].Production;
               //    BtnCustomers = uPriv[i].Customers;
               //    BtnProducts = uPriv[i].Products;
               //    BtnFreight = uPriv[i].Freight;
               //    BtnUsers = uPriv[i].Users;
               //    BtnOrders = uPriv[i].Orders;
               //}
           }
        }
        private void LoadVehicleHistory()
        {


            Switcher.Switch(new VehicleWorkOrderHistoryView(_userName, _state, _privilages, metaData));
        }

        #region Public Properties

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

        public string AdminVisibility
        {
            get
            {
                return _adminVisibility;
            }
            set
            {
                _adminVisibility = value;
                RaisePropertyChanged(() => this.AdminVisibility);
            }
        }

        public string WorkStationsVisibility
        {
            get
            {
                return _workStationsVisibility;
            }
            set
            {
                _workStationsVisibility = value;
                RaisePropertyChanged(() => this.WorkStationsVisibility);
            }
        }


        

        //public string AdminVisibility
        //{
        //    get { return _adminVisibility; }
        //    set
        //    {
        //        if (value != _adminVisibility)
        //        {
        //            _btnQuoting = value;
        //            RaisePropertyChanged("AdminVisibility");
        //        }
        //    }
        //}

        //public string BtnQuoting
        //{
        //    get { return _btnQuoting; }
        //    set
        //    {
        //        if (value != _btnQuoting)
        //        {
        //            _btnQuoting = value;
        //            RaisePropertyChanged("BtnQuoting");
        //        }
        //    }
        //}

        //public string BtnSales
        //{
        //    get { return _btnSales; }
        //    set
        //    {
        //        if (value != _btnSales)
        //        {
        //            _btnSales = value;
        //            RaisePropertyChanged("BtnSales");
        //        }
        //    }
        //}

        
        //public string BtnMaintenance
        //{
        //    get { return _btnMaintenance; }
        //    set
        //    {
        //        if (value != _btnMaintenance)
        //        {
        //            _btnMaintenance = value;
        //            RaisePropertyChanged("BtnMaintenance");
        //        }
        //    }
        //}
        //public string BtnProduction
        //{
        //    get { return _btnProduction; }
        //    set
        //    {
        //        if (value != _btnProduction)
        //        {
        //            _btnProduction = value;
        //            RaisePropertyChanged("BtnProduction");
        //        }
        //    }
        //}
       
        //public string BtnCustomers
        //{
        //    get { return _btnCustomers; }
        //    set
        //    {
        //        if (value != _btnCustomers)
        //        {
        //            _btnCustomers = value;
        //            RaisePropertyChanged("BtnCustomers");
        //        }
        //    }
        //}

       

        //public string BtnProducts
        //{
        //    get { return _btnProducts; }
        //    set
        //    {
        //        if (value != _btnProducts)
        //        {
        //            _btnProducts = value;
        //            RaisePropertyChanged("BtnProducts");
        //        }
        //    }
        //}

        //public string BtnFreight
        //{
        //    get { return _btnFreight; }
        //    set
        //    {
        //        if (value != _btnFreight)
        //        {
        //            _btnFreight = value;
        //            RaisePropertyChanged("BtnFreight");
        //        }
        //    }
        //}

        //public string BtnUsers
        //{
        //    get { return _btnUsers; }
        //    set
        //    {
        //        if (value != _btnUsers)
        //        {
        //            _btnUsers = value;
        //            RaisePropertyChanged("BtnUsers");
        //        }
        //    }
        //}
        //public string BtnOrders
        //{
        //    get { return _btnOrders; }
        //    set
        //    {
        //        if (value != _btnOrders)
        //        {
        //            _btnOrders = value;
        //            RaisePropertyChanged("BtnOrders");
        //        }
        //    }
        //}
       

        #endregion

        #region Commands



        public ICommand VehiclesCommand
        {
            get
            {
                return _vehiclesCommand ?? (_vehiclesCommand = new LogOutCommandHandler(() => Switcher.Switch(new VehicleMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }


        public ICommand WorkStationsCommand
        {
            get
            {
                return _workStationsCommand ?? (_workStationsCommand = new LogOutCommandHandler(() => Switcher.Switch(new WorkStationsView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
        public ICommand QuotingCommand
        {
            get
            {
                return _quotingCommand ?? (_quotingCommand = new LogOutCommandHandler(() => Switcher.Switch(new QuotingMainMenu(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand OrdersCommand
        {
            get
            {
                return _ordersCommand ?? (_ordersCommand = new LogOutCommandHandler(() => Switcher.Switch(new OrdersMainMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }     


        public ICommand WorkOrdersCommand
        {
            get
            {
                return _workOrdersCommand ?? (_workOrdersCommand = new LogOutCommandHandler(() => Switcher.Switch(new VehicleWorkOrdersView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand AdminCommand
        {
            get
            {
                return _adminCommand ?? (_adminCommand = new LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand NewRepairWorkOrderCommand
        {
            get
            {
                return _newRepairWorkOrderCommand ?? (_newRepairWorkOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new NewVehicleRepairWorkOrderView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }


        public ICommand VehicleWorkOrderHistoryCommand
        {
            get
            {
                return _vehicleWorkOrderHistoryCommand ?? (_vehicleWorkOrderHistoryCommand = new LogOutCommandHandler(() => LoadVehicleHistory(), _canExecute));
            }
        }

        public ICommand PrintVehiclePartsCommand
        {
            get
            {
                return _printVehiclePartsCommand ?? (_printVehiclePartsCommand = new LogOutCommandHandler(() => Switcher.Switch(new PrintVehiclePartsView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand MachineWorkOrdersCommand
        {
            get
            {
                return _machineWorkOrdersCommand ?? (_machineWorkOrdersCommand = new LogOutCommandHandler(() => Switcher.Switch(new MachineWorkOrdersView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand NewMachineRepairWorkOrderCommand
        {
            get
            {
                return _newMachineRepairWorkOrderCommand ?? (_newMachineRepairWorkOrderCommand = new LogOutCommandHandler(() => Switcher.Switch(new NewMachineRepairWorkOrderView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand MachineWorkOrderHistoryCommand
        {
            get
            {
                return _machineWorkOrderHistoryCommand ?? (_machineWorkOrderHistoryCommand = new LogOutCommandHandler(() => Switcher.Switch(new MachineWorkOrderHistoryView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand PrintMachinePartsCommand
        {
            get
            {
                return _printMachinePartsCommand ?? (_printMachinePartsCommand = new LogOutCommandHandler(() => Switcher.Switch(new PrintMachinePartsView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }

        //public ICommand FreightCommand
        //{
        //    get
        //    {
        //        return null;
        //    }
        //}
        //public ICommand UserCommand
        //{
        //    get
        //    {
        //        return null;
        //    }
        //}

#endregion
        
    }
}
