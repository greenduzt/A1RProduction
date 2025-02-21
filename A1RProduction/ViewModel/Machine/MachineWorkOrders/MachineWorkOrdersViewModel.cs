using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using A1QSystem.Model.Meta;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using A1QSystem.View.WorkOrders;
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

namespace A1QSystem.ViewModel.Machine.MachineWorkOrders
{
    public class MachineWorkOrdersViewModel : ViewModelBase
    {
        private ObservableCollection<MachineWorkOrder> _machineWorkOrder;
        private List<MachineWorkOrder> newMachineWorkOrderList;
        private ListCollectionView _collDay1 = null;
        private string userName;
        private string state;
        private List<UserPrivilages> userPrivilages;
        private List<MetaData> metaData;
        private string _version;
        private DateTime currentDate;
        private DateTime searchDate;
        private string _displayDate;
        private bool canExecute;
        private ChildWindowView myChildWindow;
        private ChildWindowView LoadingScreen;

        private ICommand _homeCommand;
        private ICommand _workOrdersCommand;
        private ICommand _viewCommand;
        private ICommand _nextDateCommand;
        private ICommand _prevDateCommand;
        private ICommand _completeWorkOrderCommand;
        private ICommand _printCommand;
        private ICommand _completeAllCommand;
        private ICommand _printAllCommand;

        public MachineWorkOrdersViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            userPrivilages = up;
            canExecute = true;
            metaData = md;
            currentDate = DateTime.Now;
            searchDate = currentDate;
            DisplayDate = currentDate.Date.ToString("dd/MM/yyyy") + " " + currentDate.Date.DayOfWeek;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;

            MachineWorkOrder = new ObservableCollection<MachineWorkOrder>();
            newMachineWorkOrderList = new List<MachineWorkOrder>();

            LoadMachineOrderDetails();
        }

        private void LoadMachineOrderDetails()
        {
            BackgroundWorker worker = new BackgroundWorker();
            LoadingScreen = new ChildWindowView();
            LoadingScreen.ShowWaitingScreen("Loading..");
            MachineWorkOrder.Clear();
            worker.DoWork += (_, __) =>
            {

            MachineWorkOrder = DBAccess.GetMachineWorkOrdersByDate(searchDate);

            ObservableCollection<MachineWorkOrder> machineWorkOrderTemp = new ObservableCollection<MachineWorkOrder>();

            machineWorkOrderTemp = DBAccess.GetMiscelaniousMachineWorkOrdersByDate(searchDate);

            foreach (var item in machineWorkOrderTemp)
            {
                MachineWorkOrder.Add(item);
            }
            //MachineWorkOrder = new ObservableCollection<MachineWorkOrder>(MachineWorkOrder.OrderBy(i => i.Urgency));

            foreach (var item in MachineWorkOrder)
            {
                DateTime dumDate = item.WorkOrderType == "Maintenance" ? (DateTime)item.NextServiceDate : (DateTime)item.FirstServiceDate;

                if(item.WorkOrderType == "Repair")
                {
                    item.NextServiceDate = item.FirstServiceDate;
                }

                double days = Math.Ceiling((dumDate - DateTime.Now).TotalDays);
                string freqName = "Day";//GetFreqName(item.MachineMaintenanceFrequency.Frequency);

                if (item.MachineMaintenanceFrequency.ID == 2)
                {
                    freqName = "Week";
                    days = Math.Ceiling((dumDate - DateTime.Now).TotalDays / 7);
                }

                if (days < 0)
                {
                    item.DaysToComplete = Math.Abs(days) + " " + freqName + CheckNumber(days) + " Late";
                    item.DaysToCompleteBackgroundCol = "#FFC33333";
                    item.DaysToCompleteForeGroundCol = "White";
                    item.RepeatAnimation = "Forever";
                }
                else if (days > 0)
                {
                    item.DaysToComplete = days + " " + freqName + CheckNumber(days) + " To Go";
                    item.DaysToCompleteBackgroundCol = "#009933";
                    item.DaysToCompleteForeGroundCol = "White";
                    item.RepeatAnimation = "0x";
                }
                else if (days == 0)
                {
                    item.DaysToComplete = "Service Today";
                    item.DaysToCompleteBackgroundCol = "#0424c1";
                    item.DaysToCompleteForeGroundCol = "White";
                    item.RepeatAnimation = "0x";
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

                if (item.Machine.MachineGroup.GroupID == 0)
                {
                    item.ButtonsVisibility = "Collapsed";
                    item.Machine.MachineGroup.GroupName = "Un Grouped Orders";
                }
                else
                {
                    item.ButtonsVisibility = "Visible";
                }

                    if(item.OrderType.Equals("External"))
                    {
                        item.OrderTypeBackgroundCol = "#F5B7B1";
                        item.OrderTypeForeGroundCol = "#17202A";
                    }
                    else
                    {
                        item.OrderTypeBackgroundCol = "White";
                        item.OrderTypeForeGroundCol = "Black";
                        
                    }

                    if(item.OrderType.Equals("Internal"))
                    {
                        item.FontWeight = "Medium";
                        item.FontSize = "15";
                    }
                    else if (item.OrderType.Equals("External"))
                    {
                        item.FontWeight = "Bold";
                        item.FontSize = "17";
                    }
            }
            };
            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
            {
                LoadingScreen.CloseWaitingScreen();
                CollDay1 = new ListCollectionView(MachineWorkOrder);
                CollDay1.GroupDescriptions.Add(new PropertyGroupDescription("Machine.MachineGroup.GroupID"));
                CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("Machine.MachineGroup.GroupID", System.ComponentModel.ListSortDirection.Descending));
                CollDay1.SortDescriptions.Add(new System.ComponentModel.SortDescription("Urgency", System.ComponentModel.ListSortDirection.Ascending));
            };
            worker.RunWorkerAsync();

            
        }

        private string GetFreqName(string fName)
        {
            string name = string.Empty;

            switch(fName)
            {
                case "Daily": name = "Day";
                        break;
                case "Weekly": name = "Week";
                        break;
                case "Monthly": name = "Month";
                        break;
                case "SixMonths": name = "Six Month";
                    break;
                case "OneYear": name = "Year";
                    break;
                case "TwoYears": name = "Year";
                    break;
                case "Fortnightly": name = "Fortnight";
                    break;
                case "2 Months": name = "Months";
                    break;
                case "3 Months": name = "Months";
                    break;
                case "4 Months": name = "Months";
                    break;
                case "5 Months": name = "Months";
                    break;
                case "3 Years": name = "Years";
                    break;
                case "4 Years": name = "Years";
                    break;
                case "5 Years": name = "Years";
                    break;
                case "10 Years": name = "Years";
                    break;
                case "One Off": name = "Day";
                    break;
            }

            return name;

        }

        private void GetNextDateWorkOrders()
        {
            searchDate = searchDate.AddDays(1);
            DisplayDate = searchDate.Date.ToString("dd/MM/yyyy") + " " + searchDate.Date.DayOfWeek;
            LoadMachineOrderDetails();
        }

        private void GetPreviousDateWorkOrders()
        {
            if (searchDate > currentDate)
            {
                searchDate = searchDate.AddDays(-1);
                DisplayDate = searchDate.Date.ToString("dd/MM/yyyy") + " " + searchDate.Date.DayOfWeek;
                LoadMachineOrderDetails();
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



        private bool CanExecute(object parameter)
        {
            return true;
        }

        private void CompleteWorkOrder(Object parameter)
        {
            int index = MachineWorkOrder.IndexOf(parameter as MachineWorkOrder);
            if (index >= 0)
            {
                if (DBAccess.CheckMachineWorkOrderCompleted(MachineWorkOrder[index].WorkOrderNo))
                {
                    myChildWindow = new ChildWindowView();
                    myChildWindow.completeMachineWorkOrder_Closed += (r =>
                    {
                        // Console.WriteLine(r + " CLOSED");

                        MachineWorkOrder.Clear();
                        LoadMachineOrderDetails();

                    });
                    myChildWindow.ShowCompleteMachineWorkOrder(MachineWorkOrder[index], userName);
                }
                else
                {
                    Msg.Show("This order has been already completed!!! ", "", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                    MachineWorkOrder.Clear();
                    LoadMachineOrderDetails();
                }
            }
        }
        private void PrintWorkOrder(Object parameter)
        {
            int index = MachineWorkOrder.IndexOf(parameter as MachineWorkOrder);
            if (index >= 0)
            {
                if (DBAccess.CheckMachineWorkOrderCompleted(MachineWorkOrder[index].WorkOrderNo))
                {
                    myChildWindow = new ChildWindowView();
                    myChildWindow.ShowSetMechanicName(MachineWorkOrder[index]);
                    myChildWindow.setMechanicViewModel_Closed += (r =>
                    {

                    });
                }
                else
                {
                    Msg.Show("This order has been already completed!!! ", "", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                    MachineWorkOrder.Clear();
                    LoadMachineOrderDetails();
                }
            }
        }

        private void ViewWorkOrderDetails(Object parameter)
        {
            int index = MachineWorkOrder.IndexOf(parameter as MachineWorkOrder);
            if (index >= 0)
            {
                if (DBAccess.CheckMachineWorkOrderCompleted(MachineWorkOrder[index].WorkOrderNo))
                {

                    myChildWindow = new ChildWindowView();                    
                    myChildWindow.machineWorkOrderItems_Closed += (r =>
                    {
                        if (r > 0)
                        {
                            LoadMachineOrderDetails();
                        }
                    });
                    myChildWindow.ShowMachineWorkOrderItems(MachineWorkOrder[index]);
                }
                else
                {

                    Msg.Show("This order has been already completed!!! ", "", MsgBoxButtons.OK, MsgBoxImage.Information, MsgBoxResult.Yes);
                    MachineWorkOrder.Clear();
                    LoadMachineOrderDetails();
                }
            }
        }

        private void PrintAllOrders(Object parameter)
        {
            if (Msg.Show("Are you sure you want to print all orders in this group?", "Printing Orders Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
            {
                string fullName = string.Empty;
                //myChildWindow = new ChildWindowView();
                //myChildWindow.ShowSetMechanicNameGroup();
                //myChildWindow.setMechanicGroupViewModel_Closed += (r =>
                //{
                //    if (r != null)
                //    {
                //        fullName = r.User.FirstName + " " + r.User.LastName;
                //        if (!string.IsNullOrWhiteSpace(fullName))
                //        {                           
                            Exception exception = null;
                            BackgroundWorker worker = new BackgroundWorker();
                            ChildWindowView LoadingScreen;
                            LoadingScreen = new ChildWindowView();
                            LoadingScreen.ShowWaitingScreen("Printing");
                            worker.DoWork += (_, __) =>
                            {
                                var para = (CollectionViewGroup)parameter;
                                List<MachineParts> machineParts = new List<MachineParts>();
                                List<MachineWorkOrder> machineWorkOrderList = new List<MachineWorkOrder>();
                                List<MachineWorkDescription> machineWorkDescriptionList = new List<MachineWorkDescription>();
                                List<MachineRepairDescription> machineRepairDescription = new List<MachineRepairDescription>();
                                machineParts = new List<MachineParts>();
                                machineWorkOrderList = DBAccess.GetMachineWorkOrdersByDateAndGroupID(Convert.ToInt16(para.Name), searchDate).ToList();
                                machineWorkDescriptionList = DBAccess.GetMachineMaintenanceInfoByWorkOrderID(machineWorkOrderList);
                                machineRepairDescription = DBAccess.GetMachineRepairDescriptionByIDColl(machineWorkDescriptionList);
                                if (machineRepairDescription.Count > 0)
                                {
                                    machineParts = DBAccess.GetMachinePartsDescriptionByColl(machineRepairDescription);
                                }
                                PrintAllMachineWorkOrdersPDF papdf = new PrintAllMachineWorkOrdersPDF(machineWorkOrderList, machineWorkDescriptionList, machineRepairDescription, machineParts, fullName);
                                exception = papdf.createWorkOrderPDF();
                            };
                            worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                            {
                                LoadingScreen.CloseWaitingScreen();
                                if (exception != null)
                                {
                                    Msg.Show("A problem has occured while the work order is prining. Please try again later." + System.Environment.NewLine + exception, "Printing Error", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                                }
                            };
                            worker.RunWorkerAsync();
                //        }
                        
                //    }               
                //});
                

               
                
                //if(!string.IsNullOrWhiteSpace(fullName))
                //{
                   
                //    Exception exception = null;
                //    BackgroundWorker worker = new BackgroundWorker();
                //    ChildWindowView LoadingScreen;
                //    LoadingScreen = new ChildWindowView();
                //    LoadingScreen.ShowWaitingScreen("Printing");
                //    worker.DoWork += (_, __) =>
                //    {
                //        var para = (CollectionViewGroup)parameter;
                //        List<MachineParts> machineParts = new List<MachineParts>();
                //        List<MachineWorkOrder> machineWorkOrderList = new List<MachineWorkOrder>();
                //        List<MachineWorkDescription> machineWorkDescriptionList = new List<MachineWorkDescription>();
                //        List<MachineRepairDescription> machineRepairDescription = new List<MachineRepairDescription>();
                //        machineParts = new List<MachineParts>();
                //        machineWorkOrderList = DBAccess.GetMachineWorkOrdersByDateAndGroupID(Convert.ToInt16(para.Name), searchDate).ToList();
                //        machineWorkDescriptionList = DBAccess.GetMachineMaintenanceInfoByWorkOrderID(machineWorkOrderList);
                //        machineRepairDescription = DBAccess.GetMachineRepairDescriptionByIDColl(machineWorkDescriptionList);
                //        if (machineRepairDescription.Count > 0)
                //        {
                //            machineParts = DBAccess.GetMachinePartsDescriptionByColl(machineRepairDescription);
                //        }
                //        PrintAllMachineWorkOrdersPDF papdf = new PrintAllMachineWorkOrdersPDF(machineWorkOrderList, machineWorkDescriptionList, machineRepairDescription, machineParts, fullName);
                //        exception = papdf.createWorkOrderPDF();
                //    };
                //    worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                //    {
                //        LoadingScreen.CloseWaitingScreen();
                //        if (exception != null)
                //        {
                //            Msg.Show("A problem has occured while the work order is prining. Please try again later." + System.Environment.NewLine + exception, "Printing Error", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                //        }
                //    };
                //    worker.RunWorkerAsync();
                //}
            }            
        }

        public static DayOfWeek GetDateByDay(string d)
        {
            DateTime today = DateTime.Today;
            DayOfWeek day = DayOfWeek.Monday;

            switch (d)
            {
                case "mon": day = DayOfWeek.Monday;
                    break;
                case "tue": day = DayOfWeek.Tuesday;
                    break;
                case "wed": day = DayOfWeek.Wednesday;
                    break;
                case "thus": day = DayOfWeek.Thursday;
                    break;
                case "fri": day = DayOfWeek.Friday;
                    break;
                case "sat": day = DayOfWeek.Saturday;
                    break;
                case "sun": day = DayOfWeek.Sunday;
                    break;
                default: day = DayOfWeek.Monday;
                    break;
            }          

            //DateTime nextMonday = DateTime.Today.AddDays(((int)day));

            return day;
        }

        private void CompleteAllWorkOrders(Object parameter)
        {
            if (Msg.Show("Are you sure you want to complete this group?", "Completing Orders Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
            {                
                myChildWindow = new ChildWindowView();
                myChildWindow.ShowSetMechanicNameGroup();
                myChildWindow.setMechanicGroupViewModel_Closed += (r =>
                {
                    if (r != null)
                    {
                        int result = 0;
                        BusinessDaysGenerator bdg = new BusinessDaysGenerator();
                        DateTime today = DateTime.Now;
                        DateTime nextDay = bdg.SkipWeekends(today.AddDays(1));
                        BackgroundWorker worker = new BackgroundWorker();
                        LoadingScreen = new ChildWindowView();
                        LoadingScreen.ShowWaitingScreen("Loading..");
                        MachineWorkOrder.Clear();
                        worker.DoWork += (_, __) =>
                        {

                            var para = (CollectionViewGroup)parameter;
                            newMachineWorkOrderList.Clear();

                            List<MachineWorkOrder> bWorkOrders = new List<MachineWorkOrder>();
                            List<MachineParts> machineParts = new List<MachineParts>();
                            List<MachineWorkOrder> machineWorkOrderList = new List<MachineWorkOrder>();
                            List<MachineWorkDescription> machineWorkDescriptionList = new List<MachineWorkDescription>();
                            List<MachineRepairDescription> machineRepairDescription = new List<MachineRepairDescription>();
                            machineParts = new List<MachineParts>();
                            machineWorkOrderList = DBAccess.GetMachineWorkOrdersByDateAndGroupID(Convert.ToInt16(para.Name), searchDate).ToList();
                            machineWorkDescriptionList = DBAccess.GetMachineMaintenanceInfoByWorkOrderID(machineWorkOrderList);                          
                                                      


                            if (machineWorkDescriptionList.Count > 0)
                            {
                                machineRepairDescription = DBAccess.GetMachineRepairDescriptionByIDColl(machineWorkDescriptionList);
                                if (machineRepairDescription.Count > 0)
                                {
                                    machineParts = DBAccess.GetMachinePartsDescriptionByColl(machineRepairDescription);
                                }

                                foreach (var item in machineWorkDescriptionList)
                                {
                                    int x = item.MachineRepairDescription.Count;
                                    if (x > 0)
                                    {
                                        item.ItemDone = false;
                                    }
                                    item.MachineRepairDescription = new ObservableCollection<MachineRepairDescription>();
                                    foreach (var items in machineRepairDescription)
                                    {
                                        if (item.ID == items.MachineWorkDescriptionID)
                                        {
                                            item.ItemRepair = true;
                                            item.MachineRepairDescription.Add(items);
                                            items.MachineParts = new ObservableCollection<MachineParts>();
                                            foreach (var itemz in machineParts)
                                            {
                                                if (items.ID == itemz.MachineRepairID)
                                                {
                                                    items.MachineParts.Add(itemz);
                                                }
                                            }
                                        }
                                    }
                                }

                                ObservableCollection<MachineWorkOrder> machineWorkOrders = new ObservableCollection<MachineWorkOrder>();
                                ObservableCollection<MachineMaintenanceInfo> machineMaintenanceInfo = new ObservableCollection<MachineMaintenanceInfo>();  

                                //for (int i = 0; i < machineWorkDescriptionList.Count; i++)
                                //{
                                //    machineMaintenanceInfo.Clear();
                                //    MachineWorkOrder mwo = new MachineWorkOrder();
                                //    //Find the correct day for the new workorder
                                //    List<Tuple<DateTime, DateTime, DateTime>> nextAvaDates = GetNextDates(machineWorkDescriptionList[i].MachineMaintenanceInfo.MachineMaintenanceFrequency.ID);
                                //    if (machineWorkDescriptionList[i].MachineMaintenanceInfo.MachineMaintenanceFrequency.ID == 2 && !String.IsNullOrWhiteSpace(machineWorkDescriptionList[i].MachineMaintenanceInfo.Repetition))
                                //    {
                                //        string[] tokens = machineWorkDescriptionList[i].MachineMaintenanceInfo.Repetition.Split(',');

                                //        foreach (var itemTO in tokens)
                                //        {
                                //            DateTime sDate = GetNextWeekday(itemTO);
                                //            MachineMaintenanceInfo mmi1 = new MachineMaintenanceInfo();
                                //            mmi1.ID = machineWorkDescriptionList[i].MachineMaintenanceInfo.ID;
                                //            mmi1.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = 2, Frequency = "Weekly" };
                                //            mmi1.MachineCode = machineWorkDescriptionList[i].MachineMaintenanceInfo.MachineCode;
                                //            mmi1.MachineDescription = machineWorkDescriptionList[i].MachineMaintenanceInfo.MachineDescription;
                                //            mmi1.CreatedBy = userName;
                                //            mmi1.CreatedDate = DateTime.Now;
                                //            mmi1.Repetition = machineWorkDescriptionList[i].MachineMaintenanceInfo.Repetition;
                                //            mmi1.FirstDate = nextAvaDates[0].Item1;
                                //            mmi1.PreferredDate = sDate;
                                //            mmi1.LastDate = nextAvaDates[0].Item3;
                                //            machineMaintenanceInfo.Add(mmi1);
                                //        }
                                //        goto a;
                                //    }

                                //    MachineMaintenanceInfo mmi2 = new MachineMaintenanceInfo();
                                //    mmi2.ID = machineWorkDescriptionList[i].MachineMaintenanceInfo.ID;
                                //    mmi2.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = machineWorkDescriptionList[i].MachineMaintenanceInfo.MachineMaintenanceFrequency.ID, Frequency = machineWorkDescriptionList[i].MachineMaintenanceInfo.MachineMaintenanceFrequency.Frequency };
                                //    mmi2.MachineCode = machineWorkDescriptionList[i].MachineMaintenanceInfo.MachineCode;
                                //    mmi2.MachineDescription = machineWorkDescriptionList[i].MachineMaintenanceInfo.MachineDescription;
                                //    mmi2.CreatedBy = userName;
                                //    mmi2.CreatedDate = DateTime.Now;
                                //    mmi2.Repetition = machineWorkDescriptionList[i].MachineMaintenanceInfo.Repetition;
                                //    mmi2.FirstDate = nextAvaDates[0].Item1;
                                //    mmi2.PreferredDate = nextAvaDates[0].Item2;
                                //    mmi2.LastDate = nextAvaDates[0].Item3;
                                //    machineMaintenanceInfo.Add(mmi2);
                                //a:
                                //    Console.WriteLine();


                                //    foreach (var itemMWOL in machineWorkOrderList)
                                //    {
                                //        if (itemMWOL.WorkOrderNo == machineWorkDescriptionList[i].MachineWorkOrderNo)
                                //        {
                                //            itemMWOL.MachineMaintenanceInfo.Add(machineMaintenanceInfo[0]);
                                //        }
                                //    }
                                //}                                
                            }                            


                            //List<MachineWorkOrder> dbWorkOrders = PrepareCollections(machineWorkOrderList);


                            //foreach (var item in dbWorkOrders)
                            //{
                            //    foreach (var items in item.MachineMaintenanceInfo)
                            //    {
                            //        Console.WriteLine(items.MachineCode + " " + items.MachineDescription);
                            //    }
                            //}

                            ////Iterate check and create orders
                            //#region Create WorkOrder If No DB Data Exist
                            //if (dbWorkOrders.Count == 0)
                            //{
                            //    foreach (var items in machineWorkOrderList)
                            //    {
                            //        foreach (var item in items.MachineMaintenanceInfo)
                            //        {
                            //            CreateNewOrders(0, item.PreferredDate, item.MachineMaintenanceFrequency, item, items.Machine);
                            //        }
                            //    }
                            //}
                            //#endregion
                            //else
                            //{
                            //    #region If there are exisitng orders in DB

                            //    foreach (var items in machineWorkOrderList)
                            //    {
                            //        foreach (var item in items.MachineMaintenanceInfo)
                            //        {
                            //            if (String.IsNullOrWhiteSpace(item.Repetition))//No weekly repetition
                            //            {
                            //                IEnumerable<MachineWorkOrder> dbMwo1 = dbWorkOrders.Where(x => x.NextServiceDate.Date >= item.FirstDate.Date && x.NextServiceDate.Date <= item.LastDate.Date && x.Machine.MachineID == items.Machine.MachineID);
                            //                if (dbMwo1 == null)
                            //                {
                            //                    CreateNewOrders(0, item.PreferredDate, item.MachineMaintenanceFrequency, item, items.Machine);
                            //                }
                            //                else
                            //                {
                            //                    MachineWorkOrder mwod = dbMwo1.FirstOrDefault(x => x.NextServiceDate.Date == item.PreferredDate.Date);
                            //                    if (mwod != null)
                            //                    {
                            //                        MachineMaintenanceInfo mmi = mwod.MachineMaintenanceInfo.FirstOrDefault(x => x.ID == item.ID);
                            //                        if (mmi == null)
                            //                        {
                            //                            Tuple<Int16, string> tup = ReFormFrequency(mwod, item.MachineMaintenanceFrequency);
                            //                            CreateNewOrders(mwod.WorkOrderNo, item.PreferredDate, new MachineMaintenanceFrequency() { ID = tup.Item1, Frequency = tup.Item2 }, item, items.Machine);
                            //                        }
                            //                    }
                            //                    else
                            //                    {
                            //                        CreateNewOrders(0, item.PreferredDate, item.MachineMaintenanceFrequency, item, items.Machine);
                            //                    }
                            //                }
                            //            }
                            //            else //There is weekly repetition
                            //            {
                            //                MachineWorkOrder dbMwo2 = dbWorkOrders.FirstOrDefault(x => x.NextServiceDate.Date == item.PreferredDate.Date);
                            //                if (dbMwo2 != null)
                            //                {
                            //                    bool ex = dbMwo2.MachineMaintenanceInfo.Any(y => y.ID == item.ID);
                            //                    if (ex == false)
                            //                    {
                            //                        Tuple<Int16, string> tup = ReFormFrequency(dbMwo2, item.MachineMaintenanceFrequency);
                            //                        CreateNewOrders(dbMwo2.WorkOrderNo, item.PreferredDate, new MachineMaintenanceFrequency() { ID = tup.Item1, Frequency = tup.Item2 }, item, items.Machine);
                            //                    }
                            //                }
                            //                else
                            //                {
                            //                    CreateNewOrders(0, item.PreferredDate, item.MachineMaintenanceFrequency, item, items.Machine);
                            //                }
                            //            }
                            //        }
                            //    }
                            //    #endregion
                            //}

                            //Send to database
                            result = DBAccess.CompleteMachineWorkOrder(newMachineWorkOrderList, machineWorkOrderList, machineWorkDescriptionList,r.User.ID);

                        };
                        worker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                        {
                            LoadingScreen.CloseWaitingScreen();
                            MachineWorkOrder.Clear();
                            LoadMachineOrderDetails();
                            if (result > 0)
                            {
                                Msg.Show("Work Order(s) Completed Successfully", "Work Order(s) Completed Successfully", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
                            }
                            else
                            {
                                Msg.Show("Something went wrong and the work order(s) did not complete successfully", "Work Order Submition Failed", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                            }

                        };
                        worker.RunWorkerAsync();    
                    }
                });
            }
        }


        //private List<MachineWorkOrder> PrepareCollections(List<MachineWorkOrder> machineWorkOrderList)
        //{          

        //    List<MachineWorkOrder> dbWorkOrders = DBAccess.CheckIfMachineWorkOrderExistColl(machineWorkOrderList);
        //    List<MachineWorkDescription> existingMachineWorkDes = DBAccess.GetMachineMaintenanceInfoByWorkOrderID(dbWorkOrders);
        //    //Combine db workorders with db descriptions
        //    foreach (var item in dbWorkOrders)
        //    {
        //        foreach (var itemEM in existingMachineWorkDes)
        //        {
        //            if (item.WorkOrderNo == itemEM.MachineWorkOrderNo)
        //            {
        //                item.MachineMaintenanceInfo.Add(itemEM.MachineMaintenanceInfo);
        //            }
        //        }
        //    }

        //    return dbWorkOrders;

        //}

        private int CreateNewOrders(Int32 woid, DateTime nsd, MachineMaintenanceFrequency mmf, MachineMaintenanceInfo mmi, Machines m)
        {
            int x = 0;
                       
            bool ex = newMachineWorkOrderList.Any(y => y.NextServiceDate.Date == nsd.Date && y.Machine.MachineID == m.MachineID);
            if (ex)
            {
                foreach (var item in newMachineWorkOrderList)
                {
                    if (item.NextServiceDate.Date == nsd.Date && item.Machine.MachineID == m.MachineID)
                    {
                        item.MachineMaintenanceInfo.Add(mmi);
                    }
                }
            }
            else
            {
                MachineWorkOrder mwo = new MachineWorkOrder();
                mwo.WorkOrderNo = woid;
                mwo.User = new User() { ID = 0 };
                mwo.CreatedBy = userName;
                mwo.FirstServiceDate = nsd.Date;
                mwo.NextServiceDate = nsd.Date;
                mwo.Machine = new Machines(0) { MachineID = m.MachineID };
                mwo.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = mmf.ID, Frequency = mmf.Frequency };
                mwo.IsCompleted = false;
                mwo.CreatedDate = DateTime.Now;
                mwo.CreatedBy = userName;
                mwo.Status = VehicleWorkOrderEnum.Pending.ToString();
                mwo.WorkOrderType = VehicleWorkOrderTypesEnum.Maintenance.ToString();
                mwo.Urgency = 2;
                mwo.MachineMaintenanceInfo = new ObservableCollection<MachineMaintenanceInfo>();
                mwo.MachineMaintenanceInfo.Add(mmi);
                newMachineWorkOrderList.Add(mwo);
            }
            x = 1;
            return x;
        }

        

        private List<Tuple<DateTime, DateTime, DateTime>> GetNextDates(int num)
        {
            List<Tuple<DateTime, DateTime, DateTime>> dates = new List<Tuple<DateTime, DateTime, DateTime>>();
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            DateTime currentDate = DateTime.Now;
            DateTime nextDate = currentDate.AddDays(1);
            DateTime d = currentDate;

            switch (num)
            {

                case 1: dates.Add(Tuple.Create(nextDate, bdg.SkipWeekends(currentDate.AddDays(1)), bdg.SkipWeekends(currentDate.AddDays(1))));//Daily
                    break;
                case 2:
                    List<Tuple<DateTime, DateTime>> tup = GetNextTwoWeeks();
                    dates.Add(Tuple.Create(tup[0].Item1, bdg.SkipWeekends(currentDate.AddDays(7)), tup[0].Item2));//Weekly
                    break;
                case 3: dates.Add(Tuple.Create(nextDate, bdg.SkipWeekends(currentDate.AddMonths(1)), bdg.SkipWeekends(currentDate.AddMonths(2))));//Monthly
                    break;
                case 4: dates.Add(Tuple.Create(nextDate, bdg.SkipWeekends(currentDate.AddMonths(6)), bdg.SkipWeekends(currentDate.AddMonths(12))));//Six Months
                    break;
                case 5: dates.Add(Tuple.Create(nextDate, bdg.SkipWeekends(currentDate.AddYears(1)), bdg.SkipWeekends(currentDate.AddYears(2))));//One Year
                    break;
                case 6: dates.Add(Tuple.Create(nextDate, bdg.SkipWeekends(currentDate.AddYears(2)), bdg.SkipWeekends(currentDate.AddYears(4))));//Two Years
                    break;
            }

            return dates;
        }

        public static List<Tuple<DateTime, DateTime>> GetNextTwoWeeks()
        {
            List<Tuple<DateTime, DateTime>> tup = new List<Tuple<DateTime, DateTime>>();
            DateTime[] fiveDates = new DateTime[5];
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();
            DateTime nextMonday = bdg.GetNextDateByDay(DayOfWeek.Monday);
            for (int i = 0; i < 5; i++)
            {
                fiveDates[i] = nextMonday;
                nextMonday = bdg.SkipWeekends(nextMonday.AddDays(1));
            }


            DateTime maxDate = fiveDates.Max(r => r);
            DateTime minDate = fiveDates.Min(r => r);

            tup.Add(Tuple.Create(minDate, maxDate));

            return tup;
        }
        public static DateTime GetNextWeekday(string d)
        {
            DateTime today = DateTime.Today;
            DayOfWeek day = DayOfWeek.Monday;

            switch (d)
            {
                case "mon": day = DayOfWeek.Monday;
                    break;
                case "tue": day = DayOfWeek.Tuesday;
                    break;
                case "wed": day = DayOfWeek.Wednesday;
                    break;
                case "thus": day = DayOfWeek.Thursday;
                    break;
                case "fri": day = DayOfWeek.Friday;
                    break;
                case "sat": day = DayOfWeek.Saturday;
                    break;
                case "sun": day = DayOfWeek.Sunday;
                    break;
                default: day = DayOfWeek.Monday;
                    break;
            }


            //if (today.DayOfWeek <= day)
            //{
            //    today = today.AddDays(1);
            //}

            //daysToAdd = ((int)day - (int)today.DayOfWeek + 7) % 7;
            //return today.AddDays(daysToAdd);

            DateTime nextMonday = DateTime.Today.AddDays(((int)day) - (int)DateTime.Today.DayOfWeek + 7);

            return nextMonday;
        }

        private Tuple<Int16, string> ReFormFrequency(MachineWorkOrder dbMwo1, MachineMaintenanceFrequency mf)
        {
            List<Tuple<int, string>> tList = new List<Tuple<int, string>>();
            if (dbMwo1 != null)
            {
                tList.Add(Tuple.Create(dbMwo1.MachineMaintenanceFrequency.ID, dbMwo1.MachineMaintenanceFrequency.Frequency));
            }

            if (tList.Count > 0)
            {
                for (int i = 0; i < tList.Count; i++)
                {
                    if (CheckContainsDigit(tList[i].Item1, mf.ID) == false)
                    {
                        tList.Add(Tuple.Create(mf.ID, mf.Frequency));
                    }
                }
            }
            else
            {
                tList.Add(Tuple.Create(mf.ID, mf.Frequency));
            }

            Tuple<Int16, string> tup = ConsFreq(tList);
            return tup;
        }

        Boolean CheckContainsDigit(int num, int dgt)
        {
            dgt = dgt % 10;                // silently force contract compliance.
            if ((num == 0) && (dgt == 0))  // Zero contains zero.
                return true;
            while (num != 0)
            {             // While more digits in number.
                if ((num % 10) == dgt)     // Return true if rightmost digit matches.
                    return true;
                num = num / 10;            // Get next digit into rightmost position.
            }
            return false;                  // No matches, return false.
        }

        private Tuple<Int16, string> ConsFreq(List<Tuple<int, string>> tup)
        {
            string strOne = string.Empty;
            string strTwo = string.Empty;
            List<int> num = new List<int>();

            for (int i = 0; i < tup.Count; i++)
            {
                int a = tup[i].Item1;
                while (a != 0)
                {
                    int lastDigit = a % 10;
                    num.Add(lastDigit);
                    //Console.WriteLine(lastDigit);
                    a /= 10;
                }
            }


            var newTup = num.OrderBy(x => x);

            foreach (var item in newTup)
            {
                strOne = strOne + item;
                strTwo = strTwo + GetFrequency(item);
            }
            return Tuple.Create(Convert.ToInt16(strOne), strTwo);
        }

        private string GetFrequency(int n)
        {
            string freq = string.Empty;

            switch (n)
            {
                case 1: freq = "Daily";
                    break;
                case 2: freq = "Weekly";
                    break;
                case 3: freq = "Monthly";
                    break;
                case 4: freq = "SixMonthly";
                    break;
                case 5: freq = "Yearly";
                    break;
                case 6: freq = "TwoYearly";
                    break;
                default:
                    break;
            }

            return freq;
        }

        #region PUBLIC_PROPERTIES

        public ListCollectionView CollDay1
        {
            get { return _collDay1; }
            set
            {
                _collDay1 = value;
                RaisePropertyChanged(() => this.CollDay1);

            }
        }

        public ObservableCollection<MachineWorkOrder> MachineWorkOrder
        {
            get
            {
                return _machineWorkOrder;
            }
            set
            {
                _machineWorkOrder = value;
                RaisePropertyChanged(() => this.MachineWorkOrder);
            }
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

        public ICommand ViewCommand
        {
            get
            {
                if (_viewCommand == null)
                {
                    _viewCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, ViewWorkOrderDetails);
                }
                return _viewCommand;
            }
        }

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


        public ICommand CompleteAllCommand
        {
            get
            {
                if (_completeAllCommand == null)
                {
                    _completeAllCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, CompleteAllWorkOrders);
                }
                return _completeAllCommand;
            }
        }

        public ICommand PrintAllCommand
        {
            get
            {
                if (_printAllCommand == null)
                {
                    _printAllCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, PrintAllOrders);
                }
                return _printAllCommand;
            }           
        }


        #endregion
    }
}
