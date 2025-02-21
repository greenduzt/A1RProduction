using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Vehicles;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace A1QSystem.ViewModel.VehicleWorkOrders.History
{
    public class VehicleWorkOrderHistoryViewModel : ViewModelBase
    {
        private ObservableCollection<Vehicle> _vehicles;
        private ObservableCollection<VehicleWorkOrderHistory> _vehicleWorkOrderHistory;
        private ICollectionView _itemsView;
        private string userName;
        private string state;
        private string _searchString;
        private List<UserPrivilages> userPrivilages;
        private ChildWindowView myChildWindow;
        private ChildWindowView LoadingScreen;
        private string _version;
        private bool canExecute;
        private ICommand _homeCommand;
        private ICommand _viewCommand;
        private ICommand _printCommand;
        private List<MetaData> metaData;

        public VehicleWorkOrderHistoryViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {            
            userName = UserName;
            state = State;
            userPrivilages = up;
            canExecute = true;
            VehicleWorkOrderHistory = new ObservableCollection<VehicleWorkOrderHistory>();
            metaData = md;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
            BackgroundWorker worker = new BackgroundWorker();
            LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Loading");

            worker.DoWork += (_, __) =>
            {
                LoadData();
                LoadVehicles();                
            };

            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();
                _itemsView = CollectionViewSource.GetDefaultView(VehicleWorkOrderHistory);
                _itemsView.Filter = x => Filter(x as VehicleWorkOrderHistory);

            };
            worker.RunWorkerAsync();
        }

        private void LoadData()
        {
            VehicleWorkOrderHistory=DBAccess.GetVehicleWorkOrderHistory();
            ObservableCollection<VehicleWorkOrderHistory> tempVehicleWorkOrderHistory  = DBAccess.GetVehicleMiscellaniousWorkOrderHistory();
           
        }

        private void LoadVehicles()
        {
            Vehicles = DBAccess.GetAllVehicles();
        }

        private bool Filter(VehicleWorkOrderHistory model)
        {
            var searchstring = (SearchString ?? string.Empty).ToLower();

            return model != null &&
                 ((model.Vehicle.VehicleCode ?? string.Empty).ToLower().Contains(searchstring)) ||
                 ((model.Vehicle.SerialNumber ?? string.Empty).ToLower().Contains(searchstring) ||
                 ((model.Vehicle.VehicleDescription ?? string.Empty).ToLower().Contains(searchstring) ||
                 ((model.Vehicle.ID).ToString() ?? string.Empty).Contains(searchstring) ||
                 ((model.CompletedDate ?? null).ToString() ?? string.Empty).Contains(searchstring) ||
                 (model.Vehicle.VehicleBrand ?? string.Empty).ToLower().Contains(searchstring)));
        }

        private void ViewItems(Object parameter)
        {
            int index = VehicleWorkOrderHistory.IndexOf(parameter as VehicleWorkOrderHistory);
            if (index >= 0)
            {
                myChildWindow = new ChildWindowView();
                myChildWindow.ShowVehicleWorkOrderHistoryItems(VehicleWorkOrderHistory[index]);
            }
        }

        private void PrintOrders(Object parameter)
        {
            int index = VehicleWorkOrderHistory.IndexOf(parameter as VehicleWorkOrderHistory);
            if (index >= 0)
            {
                if (Msg.Show("Are you sure you want to print history for work order " + VehicleWorkOrderHistory[index].VehicleWorkOrderID + "?", "Printing Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Information_Orange, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    Exception exception = null;
                    BackgroundWorker worker = new BackgroundWorker();
                    ChildWindowView LoadingScreen;
                    LoadingScreen = new ChildWindowView();
                    LoadingScreen.ShowWaitingScreen("Printing");

                    worker.DoWork += (_, __) =>
                    {
                        if (VehicleWorkOrderHistory[index].WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
                        {
                            ObservableCollection<VehicleRepairDescription> vehicleRepairDescriptionList = DBAccess.GetVehicleRepairDescriptionByID2(VehicleWorkOrderHistory[index].VehicleWorkOrderID);
                            ObservableCollection<VehicleParts> vehiclePartsList = DBAccess.GetVehiclePartsDescriptionByID(vehicleRepairDescriptionList);
                            Int32 id = 0;
                            foreach (var item in vehicleRepairDescriptionList)
                            {
                                id = item.VehicleWorkDescriptionID;
                                break;
                            }

                            VehicleWorkOrderHistory[index].VehicleWorkDescription = DBAccess.GetCompletedVehicleWorkDescriptionByID(id);

                            foreach (var item in vehicleRepairDescriptionList)
                            {
                                item.Vehicleparts = new ObservableCollection<VehicleParts>();
                                foreach (var items in vehiclePartsList)
                                {
                                    if (item.ID == items.VehicleRepairID)
                                    {
                                        item.Vehicleparts.Add(items);
                                    }
                                }
                            }
                            if (vehicleRepairDescriptionList.Count > 0 && VehicleWorkOrderHistory[index].VehicleWorkDescription.Count > 0)
                            {
                                VehicleWorkOrderHistory[index].VehicleWorkDescription[0].VehicleRepairDescription = vehicleRepairDescriptionList;
                            }
                            else
                            {
                                VehicleWorkOrderHistory[index].VehicleWorkDescription.Add(new VehicleWorkDescription() { Description = "Repair order for " + VehicleWorkOrderHistory[index].Vehicle.SerialNumber, VehicleRepairDescription = vehicleRepairDescriptionList });
                            }
                        }

                        if (VehicleWorkOrderHistory[index].WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
                        {
                            VehicleWorkOrderHistory[index].VehicleWorkDescription = DBAccess.GetCompletedVehicleWorkDescriptionByID(VehicleWorkOrderHistory[index].VehicleWorkOrderID);
                            if (VehicleWorkOrderHistory[index].VehicleWorkDescription.Count > 0)
                            {
                                ObservableCollection<VehicleRepairDescription> vehicleRepairDescriptionList = DBAccess.GetVehicleRepairDescriptionByID(VehicleWorkOrderHistory[index].VehicleWorkDescription);
                                ObservableCollection<VehicleParts> vehiclePartsList = DBAccess.GetVehiclePartsDescriptionByID(vehicleRepairDescriptionList);

                                foreach (var item in VehicleWorkOrderHistory[index].VehicleWorkDescription)
                                {
                                    item.VehicleRepairDescription = new ObservableCollection<VehicleRepairDescription>();
                                    foreach (var items in vehicleRepairDescriptionList)
                                    {
                                        if (item.ID == items.VehicleWorkDescriptionID)
                                        {
                                            item.VehicleRepairDescription.Add(items);
                                            items.Vehicleparts = new ObservableCollection<VehicleParts>();
                                            foreach (var itemz in vehiclePartsList)
                                            {
                                                if (items.ID == itemz.VehicleRepairID)
                                                {
                                                    items.Vehicleparts.Add(itemz);
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            //VehicleWorkOrderHistory[index].VehicleWorkOrderDetailsHistory = DBAccess.GetVehicleWorkDescriptionCompleted(VehicleWorkOrderHistory[index].VehicleWorkOrderID);
                        }

                        VehicleWorkOrderHistoryPDF vwoh = new VehicleWorkOrderHistoryPDF(VehicleWorkOrderHistory[index]);
                        exception = vwoh.createWorkOrderPDF();
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

        public ICollectionView ItemsView
        {
            get { return _itemsView; }
        }   

        public ObservableCollection<VehicleWorkOrderHistory> VehicleWorkOrderHistory
        {
            get { return _vehicleWorkOrderHistory; }
            set
            {
                _vehicleWorkOrderHistory = value;
                RaisePropertyChanged(() => this.VehicleWorkOrderHistory);
            }
        }

        public ObservableCollection<Vehicle> Vehicles
        {
            get { return _vehicles; }
            set
            {
                _vehicles = value;
                RaisePropertyChanged(() => this.Vehicles);
            }
        }

        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                RaisePropertyChanged(() => this.SearchString);
                ItemsView.Refresh();

            }
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand ViewCommand
        {
            get
            {
                if (_viewCommand == null)
                {
                    _viewCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, ViewItems);
                }
                return _viewCommand;
            }
        }

        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                {
                    _printCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, PrintOrders);
                }
                return _printCommand;
            }
        }

        
    }
}
