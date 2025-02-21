using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Vehicles;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using A1QSystem.View.Loading;
using A1QSystem.View.WorkOrders;
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

namespace A1QSystem.ViewModel.WorkOrders
{
    public class VehicleWorkOrdersViewModel : ViewModelBase
    {
        private string userName;
        private string state;
        private List<UserPrivilages> userPrivilages;
        private bool canExecute;
        private DateTime currentDate;
        private DateTime searchDate;
        private string _displayDate;
        private ChildWindowView myChildWindow;
        private ChildWindowView LoadingScreen;
        private List<MetaData> metaData;
        private ObservableCollection<VehicleWorkOrder> _vehicleWorkOrder;
        private string _version;
        private ICommand _homeCommand;
        private ICommand _workOrdersCommand;
        //private ICommand _viewCommand;
        private ICommand _nextDateCommand;
        private ICommand _prevDateCommand;
        private ICommand _completeWorkOrderCommand;
        private ICommand _printCommand;
        private ICommand _viewDescriptionCommand;
        private ICommand _printVehicleOrdersCommand;

        public Dispatcher UIDispatcher { get; set; }
        public VehicleWorkOrdersNotifier vehicleWorkOrdersNotifier { get; set; }

        public VehicleWorkOrdersViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md,Dispatcher uidispatcher)
        {
            userName = UserName;
            state = State;
            userPrivilages = up;
            canExecute = true;
            metaData=md;
            currentDate = DateTime.Now;
            searchDate = currentDate;
            DisplayDate = currentDate.Date.ToString("dd/MM/yyyy") + " " + currentDate.Date.DayOfWeek;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;     
            
            BackgroundWorker worker = new BackgroundWorker();
            LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Loading..");
            //VehicleWorkOrder.Clear();
            worker.DoWork += (_, __) =>
            { 
                this.UIDispatcher = uidispatcher;
                this.vehicleWorkOrdersNotifier = new VehicleWorkOrdersNotifier();

                this.vehicleWorkOrdersNotifier.NewMessage += new EventHandler<SqlNotificationEventArgs>(notifier_NewMessage);
                ObservableCollection<VehicleWorkOrder> opd = this.vehicleWorkOrdersNotifier.RegisterDependency(searchDate, state);
                VehicleWorkOrder = new ObservableCollection<VehicleWorkOrder>();
                VehicleWorkOrder = opd;
                this.LoadData(opd);
            };
            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();
            };
            worker.RunWorkerAsync();

            //CalculateMaintenacneSchedule();
        }


        //private void CalculateMaintenacneSchedule()
        //{
        //    int lastMainSeq = 3;
        //    Int64 PrevOdo = 5250;
        //    Int64 Odometer = 5600;
            
        //    int id = 0;

        //    List<VehicleMaintenanceSequence> VehicleMaintenanceSequenceList = DBAccess.GetVehicleMaintenanceSequence(1);
        //    int count = VehicleMaintenanceSequenceList.Count;
        //    int y = 0;
        //    for (int i = 0; i < VehicleMaintenanceSequenceList.Count; i++)
        //    {
        //        y = i + 1;

        //        if (y != count)
        //        {
        //            if (Odometer >= VehicleMaintenanceSequenceList[i].Kmhrs && Odometer < VehicleMaintenanceSequenceList[y].Kmhrs)
        //            {
        //                id = VehicleMaintenanceSequenceList[i].ID;
        //            }
        //        }
        //        else if (Odometer > VehicleMaintenanceSequenceList[i].Kmhrs)
        //        {
        //            id = VehicleMaintenanceSequenceList[i].ID;

        //        }
        //        else if (Odometer == 0 && VehicleMaintenanceSequenceList[i].Kmhrs == 0)
        //        {
        //            id = VehicleMaintenanceSequenceList[i].ID;
        //        }
        //        else if (Odometer >= VehicleMaintenanceSequenceList[i].Kmhrs)
        //        {
        //            id = VehicleMaintenanceSequenceList[i].ID;
        //        }
        //    }

        //    Console.WriteLine("FIRST " + id);

        //    if(id == lastMainSeq)
        //    {
        //        Int64 diff = Odometer - PrevOdo;//200
        //        y = 0;
        //        for (int i = 0; i < VehicleMaintenanceSequenceList.Count; i++)
        //        {
        //            y = i + 1;

        //            if (y != count)
        //            {
        //                if (diff >= VehicleMaintenanceSequenceList[i].Kmhrs && diff < VehicleMaintenanceSequenceList[y].Kmhrs)
        //                {
        //                    id = VehicleMaintenanceSequenceList[i].ID;
        //                }
        //            }
        //            else if (diff > VehicleMaintenanceSequenceList[i].Kmhrs)
        //            {
        //                id = VehicleMaintenanceSequenceList[i].ID;

        //            }
        //            else if (diff == 0 && VehicleMaintenanceSequenceList[i].Kmhrs == 0)
        //            {
        //                id = VehicleMaintenanceSequenceList[i].ID;
        //            }
        //        }
        //    }

        //    Console.WriteLine("LAST "  + id);
        //}


        private void LoadData(ObservableCollection<VehicleWorkOrder> psd)
        {
            
                this.UIDispatcher.BeginInvoke((Action)delegate()
                {
                    if (psd != null)
                    {
                        foreach (var item in psd)
                        {
                            DateTime dumDate = (DateTime)item.NextServiceDate;
                            double days = Math.Ceiling((dumDate - DateTime.Now).TotalDays);
                            if (days < 0)
                            {                               
                                item.DaysToComplete = Math.Abs(days) + " Day" + CheckNumber(days) + " Late";
                                item.DaysToCompleteBackgroundCol = "#FFC33333";
                                item.DaysToCompleteForeGroundCol = "White";
                              
                                if (item.IsViewed)
                                {
                                    item.RepeatAnimation = "Forever";
                                }
                                else
                                {
                                    item.CompleteBackCol = "#81b799";
                                    item.ViewRepeatAnimation = "Forever";
                                }
                            }
                            else if (days > 0)
                            {
                                item.DaysToComplete = days + " Day" + CheckNumber(days) + " To Go";
                                item.DaysToCompleteBackgroundCol = "#009933";
                                item.DaysToCompleteForeGroundCol = "White";
                                //item.RepeatAnimation = "0x";
                            }
                            else if (days == 0)
                            {
                                item.DaysToComplete = (item.Urgency == 2 ? "Service Today" : "Repair Today");
                                item.DaysToCompleteBackgroundCol = "#0424c1";
                                item.DaysToCompleteForeGroundCol = "White";
                                //item.RepeatAnimation = "0x";
                            }

                            if (item.Urgency == 1)
                            {
                                item.UrgencyBackgroundCol = "#FFC33333";
                                item.UrgencyForeGroundCol = "White";
                            }
                            else if (item.Urgency == 2)
                            {
                                item.UrgencyBackgroundCol = "White";
                                item.UrgencyForeGroundCol = "Black";
                            }

                            //if (item.IsViewed == true)
                            //{
                            //    item.CompleteBackCol = "#2E8856";
                            //}
                            //else
                            //{
                            //    item.CompleteBackCol = "#81b799";
                            //}                       
                        }

                        VehicleWorkOrder = psd;
                    }
                });

           
        }

        void notifier_NewMessage(object sender, SqlNotificationEventArgs e)
        {
            this.LoadData(this.vehicleWorkOrdersNotifier.RegisterDependency(searchDate, state));
        }

        private void LoadVehicleOrderDetails()
        {
            BackgroundWorker worker = new BackgroundWorker();
            LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Loading..");
            VehicleWorkOrder.Clear();
            worker.DoWork += (_, __) =>
            {                
                VehicleWorkOrder = DBAccess.GetVehicleWorkOrdersByDate(searchDate,state);
                foreach (var item in VehicleWorkOrder)
                {                    
                    DateTime dumDate = (DateTime)item.NextServiceDate;
                    double days = Math.Ceiling((dumDate - DateTime.Now).TotalDays);
                    if (days < 0)
                    {
                        item.DaysToComplete = Math.Abs(days) + " Day" + CheckNumber(days) + " Late";
                        item.DaysToCompleteBackgroundCol = "#FFC33333";
                        item.DaysToCompleteForeGroundCol = "White";
                        item.RepeatAnimation = "Forever";
                    }
                    else if (days > 0)
                    {
                        item.DaysToComplete = days + " Day" + CheckNumber(days) + " To Go";
                        item.DaysToCompleteBackgroundCol = "#009933";
                        item.DaysToCompleteForeGroundCol = "White";
                        item.RepeatAnimation = "0x";           
                    }
                    else if (days == 0)
                    {
                        item.DaysToComplete = (item.Urgency == 2 ? "Service Today" : "Repair Today");
                        item.DaysToCompleteBackgroundCol = "#0424c1";
                        item.DaysToCompleteForeGroundCol = "White";
                        item.RepeatAnimation = "0x";           
                    }
                    
                    if(item.Urgency ==1)
                    {
                        item.UrgencyBackgroundCol = "#FFC33333";
                        item.UrgencyForeGroundCol = "White";
                    }
                    else if(item.Urgency == 2)
                    {
                        item.UrgencyBackgroundCol = "White";
                        item.UrgencyForeGroundCol = "Black";
                    }

                    if(item.IsViewed == true)
                    {
                        item.CompleteBackCol = "#2E8856";
                    }
                    else
                    {
                        item.CompleteBackCol = "#81b799";
                    }
                }
            };
            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();
            };
            worker.RunWorkerAsync();   
        }

        private void GetNextDateWorkOrders()
        {
            searchDate = searchDate.AddDays(1);
            DisplayDate = searchDate.Date.ToString("dd/MM/yyyy") + " " + searchDate.Date.DayOfWeek;
            //LoadVehicleOrderDetails();
            this.LoadData(this.vehicleWorkOrdersNotifier.RegisterDependency(searchDate, state));
        }

        private void GetPreviousDateWorkOrders()
        {
            if (searchDate > currentDate)
            {
                searchDate = searchDate.AddDays(-1);
                DisplayDate = searchDate.Date.ToString("dd/MM/yyyy") + " " + searchDate.Date.DayOfWeek;
                //LoadVehicleOrderDetails();
                this.LoadData(this.vehicleWorkOrdersNotifier.RegisterDependency(searchDate, state));
            }
        }

        private string CheckNumber(double num)
        {
            string str = string.Empty;

            if ((num > 1) || (num < -1))
            {
                str = "s";
            }

            return str;
        }

        //private void ViewWorkOrderDetails(Object parameter)
        //{
        //    int index = VehicleWorkOrder.IndexOf(parameter as VehicleWorkOrder);
        //    if (index >= 0)
        //    {
        //        if (DBAccess.CheckVehicleWorkOrderCompleted(VehicleWorkOrder[index].VehicleWorkOrderID))
        //        {
        //            myChildWindow = new ChildWindowView();
        //            //myChildWindow.viewVehicleMaintenanceDesc_Closed += (r =>
        //            //{
        //            //    Console.WriteLine(r.ToString());

        //            //});
        //            myChildWindow.ShowOdometerAcceptanceView(VehicleWorkOrder[index]);
        //        }
        //        else
        //        {
        //            Msg.Show("This order has already been completed!!! ", "", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
        //            VehicleWorkOrder.Clear();
        //            LoadVehicleOrderDetails();
        //        }

        //        myChildWindow = new ChildWindowView();
        //        myChildWindow.ShowVehicleRepairDescription(VehicleWorkOrder[index]);

        //            //if (DBAccess.CheckVehicleWorkOrderCompleted(VehicleWorkOrder[index].VehicleWorkOrderID))
        //            //{
        //            //    myChildWindow = new ChildWindowView();
        //            //    //myChildWindow.viewVehicleMaintenanceDesc_Closed += (r =>
        //            //    //{
        //            //    //    Console.WriteLine(r.ToString());

        //            //    //});
        //            //    myChildWindow.ShowVehicleWorkOrderItems(VehicleWorkOrder[index]);
        //            //}
        //            //else
        //            //{
        //            //    Msg.Show("This order has already been completed!!! ", "", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
        //            //    VehicleWorkOrder.Clear();
        //            //    LoadVehicleOrderDetails();
        //            //}
                
        //            //myChildWindow = new ChildWindowView();
        //            //myChildWindow.ShowVehicleRepairDescription(VehicleWorkOrder[index]);
                
        //    }
        //}

        private void CompleteWorkOrder(Object parameter)
        {
            int index = VehicleWorkOrder.IndexOf(parameter as VehicleWorkOrder);
            if (index >= 0)
            {
                if (DBAccess.CheckVehicleWorkOrderCompleted(VehicleWorkOrder[index].VehicleWorkOrderID))
                {
                    if (VehicleWorkOrder[index].WorkOrderType == "Maintenance")
                    {

                        ChildWindowView myChildWindow = new ChildWindowView();
                        myChildWindow.completeWorkOrder_Closed += (r =>
                        {


                        });
                        myChildWindow.ShowCompleteVehicleWorkOrder(VehicleWorkOrder[index]);
         
                            //if (VehicleWorkOrder[index].Vehicle.VehicleBrand != "HDK")
                            //{
                            //    myChildWindow = new ChildWindowView();
                            //    myChildWindow.ShowOdometerAcceptanceView(VehicleWorkOrder[index],"complete");
                            //    myChildWindow.odometerAcceptance_Closed += (r =>
                            //    {
                            //        Console.WriteLine(r.ToString());

                            //    });
                            //}
                            //else
                            //{
                            //    myChildWindow = new ChildWindowView();
                            //    myChildWindow.completeWorkOrder_Closed += (r =>
                            //    {
                            //        VehicleWorkOrder.Clear();
                            //        LoadVehicleOrderDetails();

                            //    });
                            //    myChildWindow.ShowCompleteVehicleWorkOrder(VehicleWorkOrder[index], userName);
                            //}
                    
                    }
                    else if (VehicleWorkOrder[index].WorkOrderType == "Repair")
                    {
                        myChildWindow = new ChildWindowView();
                        myChildWindow.completeVehicleRepairDesc_Closed += (r =>
                        { 
                            //VehicleWorkOrder.Clear();
                            //LoadVehicleOrderDetails();

                        });
                        myChildWindow.ShowCompleteVehicleRepairDescription(VehicleWorkOrder[index],userName);
                    }
                }
                else
                {
                    Msg.Show("This order has been already completed!!! ", "", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                    VehicleWorkOrder.Clear();
                    LoadVehicleOrderDetails();
                }
            }            
        }

        private void PrintWorkOrder(Object parameter)
        {
            int index = VehicleWorkOrder.IndexOf(parameter as VehicleWorkOrder);
            if (index >= 0)
            {

                if (DBAccess.CheckVehicleWorkOrderCompleted(VehicleWorkOrder[index].VehicleWorkOrderID))
                {
                    VehicleWorkOrder vwo = DBAccess.GetVehicleWorkOrderByID(VehicleWorkOrder[index].VehicleWorkOrderID);

                    if (vwo.User != null)
                    {               
                        VehicleWorkOrder[index].User.FullName = vwo.User.FullName;
                    }

                    if (vwo.OdometerReading != 0)
                    {
                        VehicleWorkOrder[index].OdometerReading = vwo.OdometerReading;
                    }
                    myChildWindow = new ChildWindowView();
                    myChildWindow.ShowMaintenanceAcceptance(VehicleWorkOrder[index]);
                    myChildWindow.maintenanceAcceptance_Closed += (r =>
                    {
                        //VehicleWorkOrderPDF vwopdf = new VehicleWorkOrderPDF(VehicleWorkOrder, VehicleMaintenanceInfo, SafetyInspectionInfo, frequency,unit,Odometer);
                        //vwopdf.createWorkOrderPDF();
                    });
                }
                else
                {
                    Msg.Show("This order has been already completed!!! ", "", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                    VehicleWorkOrder.Clear();
                    LoadVehicleOrderDetails();
                }
              
            }
        }

        private void ViewVehicleMaintenanceDescription(Object parameter)
        {
            int index = VehicleWorkOrder.IndexOf(parameter as VehicleWorkOrder);
            if (index >= 0)
            {
                if (DBAccess.CheckVehicleWorkOrderCompleted(VehicleWorkOrder[index].VehicleWorkOrderID))
                {
                    if (VehicleWorkOrder[index].WorkOrderType == "Maintenance")
                    {
                        //if (VehicleWorkOrder[index].Vehicle.ID != 51)
                        //{
                        //    ChildWindowView myChildWindow = new ChildWindowView();
                        //    myChildWindow.ShowVehicleMaintenanceDescription(VehicleWorkOrder[index]);
                        //    myChildWindow.viewVehicleMaintenanceDesc_Closed += (r =>
                        //    {

                        //    });  
                        //}
                        if (VehicleWorkOrder[index].Vehicle.VehicleBrand != "HDK")
                        {
                            myChildWindow = new ChildWindowView();
                            myChildWindow.ShowOdometerAcceptanceView(VehicleWorkOrder[index]);
                            myChildWindow.odometerAcceptance_Closed += (r =>
                            {
                                //Console.WriteLine(r.ToString());
                                //VehicleWorkOrder.Clear();
                                //LoadVehicleOrderDetails();

                            });
                        }
                        else
                        {
                            myChildWindow = new ChildWindowView();
                            myChildWindow.ShowVehicleMaintenanceDescription(VehicleWorkOrder[index]);
                            myChildWindow.viewVehicleMaintenanceDesc_Closed += (r =>
                            {
                                //VehicleWorkOrder.Clear();
                                //LoadVehicleOrderDetails();
                            });
                        }
                    }
                    else if (VehicleWorkOrder[index].WorkOrderType == "Repair")
                    {
                        myChildWindow = new ChildWindowView();
                        myChildWindow.ShowVehicleRepairDescription(VehicleWorkOrder[index]);
                    }
                }
                else
                {
                    Msg.Show("This order has been already completed!!! ", "", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                    VehicleWorkOrder.Clear();
                    LoadVehicleOrderDetails();
                }
            }
        }

        private void PrintVehicleWorkOrders()
        {
            if(VehicleWorkOrder != null && VehicleWorkOrder.Count > 0)
            {
                PrintAllVehicleWorkOrdersPDF vwopdf = new PrintAllVehicleWorkOrdersPDF(VehicleWorkOrder);
                Exception exception = vwopdf.createWorkOrderPDF();
                if(exception != null)
                {
                    Msg.Show("Could not print vehicle work orders " + System.Environment.NewLine + exception.ToString(), "Error Printing", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.OK);
                }
            }
            else
            {
                Msg.Show("No vehicle work orders available to print", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }

        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        #region PUBLIC_PROPERTIES

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

        public ObservableCollection<VehicleWorkOrder> VehicleWorkOrder
        {
            get { return _vehicleWorkOrder; }
            set
            {
                _vehicleWorkOrder = value;
                RaisePropertyChanged(() => this.VehicleWorkOrder);
            }
        }

        public string DisplayDate
        {
            get { return _displayDate; }
            set
            {
                _displayDate = value;
                RaisePropertyChanged(() => this.DisplayDate);
            }
        }

        #endregion

        #region COMMANDS

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand WorkOrdersCommand
        {
            get
            {
                return _workOrdersCommand ?? (_workOrdersCommand = new LogOutCommandHandler(() => Switcher.Switch(new WorkOrdersMenuView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand NextDateCommand
        {
            get
            {
                return _nextDateCommand ?? (_nextDateCommand = new LogOutCommandHandler(() => GetNextDateWorkOrders(), canExecute));
            }
        }

        public ICommand PrevDateCommand
        {
            get
            {
                return _prevDateCommand ?? (_prevDateCommand = new LogOutCommandHandler(() => GetPreviousDateWorkOrders(), canExecute));
            }
        }

        public ICommand PrintVehicleOrdersCommand
        {
            get
            {
                return _printVehicleOrdersCommand ?? (_printVehicleOrdersCommand = new LogOutCommandHandler(() => PrintVehicleWorkOrders(), canExecute));
            }
        }

        

        //public ICommand ViewCommand
        //{
        //    get
        //    {
        //        if (_viewCommand == null)
        //        {
        //            _viewCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, ViewWorkOrderDetails);
        //        }
        //        return _viewCommand;
        //    }
        //}

        public ICommand CompleteWorkOrderCommand
        {
            get
            {
                if (_completeWorkOrderCommand == null)
                {
                    _completeWorkOrderCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, CompleteWorkOrder);
                }
                return _completeWorkOrderCommand;
            }
        }

        public ICommand PrintCommand
        {
            get
            {
                if (_printCommand == null)
                {
                    _printCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, PrintWorkOrder);
                }
                return _printCommand;
            }
        }


        public ICommand ViewDescriptionCommand
        {
            get
            {
                if (_viewDescriptionCommand == null)
                {
                    _viewDescriptionCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, ViewVehicleMaintenanceDescription);
                }
                return _viewDescriptionCommand;
            }
        }

        

        #endregion
    }
}
