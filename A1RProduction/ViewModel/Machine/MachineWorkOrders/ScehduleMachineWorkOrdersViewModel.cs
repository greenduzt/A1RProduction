using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Other;
using A1QSystem.View;
using A1QSystem.View.AdminDashboard;
using A1QSystem.View.Maintenance;
using A1QSystem.ViewModel.Stock;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Machine.MachineWorkOrders
{
    public class ScehduleMachineWorkOrdersViewModel : ViewModelBase
    {
        private ObservableCollection<MachineMaintenanceInfo> _machineMaintenanceInfoList;
        private ObservableCollection<Machines> _machines;
        private int _selectedLocationID, _selectedProviderID, _itemCount, maintenanceFrequency, _selectedMachineID;
        private DateTime _currentDate;
        private List<StockLocation> _stockLocation;
        private string userName, state, _providerVisibility,_selectedDays;
        private List<UserPrivilages> userPrivilages;
        private bool canExecute, _submitEnabled, _clearEnabled, _isMachineEnabled;
        private List<MetaData> metaData;
        private string _version, _submitBackground, _clearBackground, _mainDesBackground, _selectedOrderType, _repetitionVisiblity, _addItemBackground, 
            _frequencyVisibility, _selectedFrequency;
        private bool _checkEnabled,  _mainDisEnabled, _orderTypeEnabled, _addItemEnabled;
        private List<Int64> removedList;
        private DateTime? _selectedDate;
        private ObservableCollection<MachineProvider> _providers;
        private ICommand _homeCommand;
        private ICommand _adminDashboardCommand;
        private ICommand _maintenanceCommand;
        private ICommand _machinesCommand;
        private ICommand _clearCommand;
        private ICommand _createWorkOrderCommand;
        private ICommand _viewMaintenanceCommand;
        private ICommand _addItemCommand;
        private ICommand _removeCommand;
        private ICommand _updateCommand;

        public ScehduleMachineWorkOrdersViewModel(string UserName, string State, List<UserPrivilages> up, List<MetaData> md)
        {
            userName = UserName;
            state = State;
            userPrivilages = up;
            canExecute = true;
            metaData = md;
            var data = metaData.SingleOrDefault(x => x.KeyName == "version");
            Version = data.Description;
            Machines = new ObservableCollection<Machines>();
            CurrentDate = DateTime.Now;
            SelectedDate = null;
            removedList = new List<long>();
            SubmitBackground = "#FFDEDEDE";
            ClearBackground = "#FFDEDEDE";
            MainDesBackground = "#FFDEDEDE";
            AddItemBackground = "#FFDEDEDE";
            AddItemEnabled = false;
            ClearEnabled = false;
            SubmitEnabled = false;
            IsMachineEnabled = false;
            CheckEnabled = false;
            SelectedOrderType = "Select";
            ProviderVisibility = "Collapsed";
            FrequencyVisibility = "Collapsed";
            RepetitionVisiblity = "Collapsed";
            SelectedDays = "Select";
            SelectedFrequency = "Select";
            MachineMaintenanceInfoList = new ObservableCollection<MachineMaintenanceInfo>();

            LoadLocations();
            LoadProviders();

            MachineMaintenanceInfoList.CollectionChanged += OnMachineListChanged;
        }

        void OnMachineListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Update item count
            this.ItemCount = this.MachineMaintenanceInfoList.Count;

            // Resequence list
            SequencingService.SetCollectionSequence(this.MachineMaintenanceInfoList);
        }

        private void LoadLocations()
        {
            StockLocation = DBAccess.GetStockLocations();
            StockLocation.Add(new StockLocation() { ID = 0, StockName = "Select" });
            SelectedLocationID = 0;
        }

        private void LoadProviders()
        {
            Providers = new ObservableCollection<MachineProvider>(DBAccess.GetAllProviders("Active"));
            Providers.Insert(0, new MachineProvider() { ProviderID = 0, ProviderName = "Select" });
            SelectedProviderID = 0;
        }

        private void LoadMachines()
        {
            if(SelectedLocationID > 0)
            {
                Machines=DBAccess.GetMachinesByLocation(SelectedLocationID);
                Machines.Add(new Machines(0) { MachineID = 0, MachineName = "Select" });
                SelectedMachineID = 0;
                IsMachineEnabled = true;
                if (Machines.Count == 0)
                {
                    StockLocation m = StockLocation.SingleOrDefault(x => x.ID == SelectedLocationID);
                    Msg.Show("Machine(s) not available for " + m.StockName, "Machine(s) Not Available", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.OK);
                }
            }           
        }

        private void EnableCreateClear()
        {
            if (SelectedLocationID != 0 && SelectedDate != null && !SelectedFrequency.Equals("Select"))
            {
                ClearEnabled = true;
                SubmitEnabled = true;
                SubmitBackground = "#FF787C7A";
                ClearBackground = "#FF787C7A";
            }
            else
            {
                ClearBackground = "#FFDEDEDE";
                SubmitBackground = "#FFDEDEDE";
                SubmitEnabled = false;
                ClearEnabled = false;
            }
        }

        private void EnDisMainDesButton()
        {
            if (SelectedLocationID > 0)
            {                
                OrderTypeEnabled = true;
            }
            else
            {                
                OrderTypeEnabled = false;
            }
        }

        private void ClearData()
        {
            SelectedDate = null;
            
            SelectedLocationID = 0;
            SelectedProviderID = 0;            
            Machines.Clear();
            ClearBackground = "#FFDEDEDE";
            SubmitBackground = "#FFDEDEDE";
            MainDesBackground = "#FFDEDEDE";
            SubmitEnabled = false;
            ClearEnabled = false;
            OrderTypeEnabled = false;
            IsMachineEnabled = false;
            SelectedMachineID = 0;
            SelectedOrderType = "Select";
            SelectedProviderID = 0;
            removedList.Clear();
            SelectedDays = "Select";
            SelectedFrequency = "Select";
        }

        private int GetFreqID()
        {
            int freqId = 0;
            switch (SelectedFrequency)
            {
                case "Daily":
                    freqId = 1;
                    break;
                case "Weekly":
                    freqId = 2;
                    break;               
                case "1 Month":
                    freqId = 3;
                    break;               
                case "6 Months":
                    freqId = 4;
                    break;
                case "1 Year":
                    freqId = 5;
                    break;
                case "2 Years":
                    freqId = 6;
                    break;
                case "Fortnightly":
                    freqId = 7;
                    break;
                case "2 Months":
                    freqId = 8;
                    break;
                case "3 Months":
                    freqId = 9;
                    break;
                case "4 Months":
                    freqId = 10;
                    break;
                case "5 Months":
                    freqId = 11;
                    break;                
                case "3 Years":
                    freqId = 12;
                    break;
                case "4 Years":
                    freqId = 13;
                    break;
                case "5 Years":
                    freqId = 14;
                    break;
                case "10 Years":
                    freqId = 15;
                    break;
                default:
                    break;
            }

            return freqId;
        }

        private void CreateWorkOrder()
        {            
            if (SelectedOrderType.Equals("Select"))
            {
                Msg.Show("Please select Order Type", "Order Type Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }
            else if(SelectedOrderType.Equals("External") && SelectedProviderID == 0)
            {
                Msg.Show("Please select provider", "Provider Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }
            else if (SelectedOrderType.Equals("External") && SelectedDays.Equals("Select"))
            {
                Msg.Show("Please select a date to show order on screen", "Date Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }
            else if (SelectedFrequency.Equals("Select"))
            {
                Msg.Show("Please select a Frequency", "Frequency Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }
            else
            {
                UpdateData("silent");
                List<MachineWorkOrder> machineWorkOrderList = new List<MachineWorkOrder>();
                ObservableCollection<MachineMaintenanceInfo> MachineMaintenanceInfo = new ObservableCollection<MachineMaintenanceInfo>();
                MachineMaintenanceInfo = DBAccess.GetMachineMaintenanceInfo();
                ObservableCollection<MachineWorkOrder> exMachineWorkOrders = DBAccess.GetAllMachineWorkOrders();
                int freqId = GetFreqID();
                
                var data = Machines.FirstOrDefault(x=>x.MachineID == SelectedMachineID);
                if (data != null)
                {                        
                    bool createOrder = false;
                    bool runOtherDes = false;
                    int days = SelectedDays.Equals("Select") ? 0 : Convert.ToInt16(SelectedDays);
                    int providerID = SelectedProviderID.Equals("Select") ? 0 : Convert.ToInt16(SelectedProviderID);

                    bool isRep = MachineMaintenanceInfo.Any(x => x.Machine.MachineID == data.MachineID && x.MachineMaintenanceFrequency.ID == freqId && !String.IsNullOrWhiteSpace(x.Repetition) && x.OrderType.Equals(SelectedOrderType) && x.ProviderID == providerID && x.ShowOrderBeforeDays == days);

                    //Not weekly
                    if (freqId != 2 || isRep == false)
                    {
                        //DateTime sDate = (freqId == 1) ? (DateTime)SelectedDate : CalculateNextServiceDate(freqId, (DateTime)SelectedDate);
                        //Check if the workorder already existing
                        MachineWorkOrder m = exMachineWorkOrders.FirstOrDefault(x => x.Machine.MachineID == data.MachineID && 
                                                        x.NextServiceDate.Date == Convert.ToDateTime(SelectedDate).Date && x.OrderType.Equals(SelectedOrderType) && x.MachineProvider.ProviderID == providerID);
                        if (m != null)
                        {
                            if (m.MachineMaintenanceFrequency.ID == freqId)
                            {
                                createOrder = false;
                            }
                            else
                            {
                                createOrder = true;
                            }
                        }
                        else
                        {
                            createOrder = true;
                        }
                        if (createOrder)
                        {

                            MachineWorkOrder mwo = new MachineWorkOrder();
                            mwo.Machine = data;
                            mwo.User = new User() { ID = 0 };
                            mwo.Urgency = 2;
                            mwo.WorkOrderType = VehicleWorkOrderTypesEnum.Maintenance.ToString();
                            mwo.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = freqId, Frequency = GetMaintenanceFreqByString(freqId) };
                            mwo.FirstServiceDate = Convert.ToDateTime(SelectedDate).Date;
                            mwo.NextServiceDate = Convert.ToDateTime(SelectedDate).Date; 
                            mwo.CreatedDate = DateTime.Now;
                            mwo.CreatedBy = userName;
                            mwo.IsCompleted = false;
                            mwo.CompletedDate = null;
                            mwo.CompletedBy = string.Empty;
                            mwo.OrderType = SelectedOrderType;
                            if (mwo.OrderType.Equals("External"))
                            {
                                DateTime date = Convert.ToDateTime(SelectedDate).AddDays(-Convert.ToInt16(SelectedDays));
                                if(date <= DateTime.Now.Date)
                                {
                                    date = DateTime.Now.Date;
                                }
                                mwo.ShowOrderDate = date;
                            }
                            else
                            {
                                mwo.ShowOrderDate = null;
                            }
                            mwo.MachineProvider = new MachineProvider() { ProviderID = SelectedProviderID };
                            mwo.ExternalMechanicName = String.Empty;
                            mwo.Reason = String.Empty;
                            mwo.Status = VehicleWorkOrderEnum.Pending.ToString();
                            mwo.MachineMaintenanceInfo = new ObservableCollection<MachineMaintenanceInfo>();
                            foreach (var itemMMI in MachineMaintenanceInfo)
                            {
                                if (itemMMI.Machine.MachineID == data.MachineID && itemMMI.MachineMaintenanceFrequency.ID == freqId && itemMMI.OrderType == SelectedOrderType)
                                {
                                    mwo.MachineMaintenanceInfo.Add(new MachineMaintenanceInfo() { ID = itemMMI.ID, Machine = new Machines(0) { MachineID = itemMMI.Machine.MachineID }, MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = itemMMI.MachineMaintenanceFrequency.ID }, MachineCode = itemMMI.MachineCode, MachineDescription = itemMMI.MachineDescription, Repetition = itemMMI.Repetition, CreatedDate = itemMMI.CreatedDate, CreatedBy = itemMMI.CreatedBy, IsActive = itemMMI.IsActive, OrderType = itemMMI.OrderType,ProviderID = itemMMI.ProviderID,ShowOrderBeforeDays = itemMMI.ShowOrderBeforeDays});
                                }
                            }
                            if (mwo.MachineMaintenanceInfo.Count > 0)
                            {
                                machineWorkOrderList.Add(mwo);
                            }
                        }
                    }
                    else if (freqId == 2)
                    {
                        if (isRep)
                        {
                            foreach (var itemMMI in MachineMaintenanceInfo)
                            {
                                if (itemMMI.Machine.MachineID == data.MachineID && itemMMI.MachineMaintenanceFrequency.ID == freqId)
                                {
                                    if (!String.IsNullOrWhiteSpace(itemMMI.Repetition))
                                    {
                                        string[] tokens = itemMMI.Repetition.Split(',');
                                        int c = tokens.Count();
                                        int n = 1;
                                        foreach (var itemT in tokens)
                                        {
                                            DateTime sDate = GetNextWeekday((DateTime)SelectedDate, itemT);
                                            //Check if the workorder already existing
                                            MachineWorkOrder m = exMachineWorkOrders.FirstOrDefault(x => x.Machine.MachineID == data.MachineID && x.NextServiceDate.Date.Equals(sDate.Date));
                                            if (m != null)
                                            {
                                                if (m.MachineMaintenanceFrequency.ID == freqId)
                                                {
                                                    createOrder = false;
                                                }
                                                else
                                                {
                                                    createOrder = true;
                                                }
                                            }
                                            else
                                            {
                                                createOrder = true;
                                            }

                                            if (createOrder)
                                            {
                                                bool run = false;
                                                MachineWorkOrder mwo = new MachineWorkOrder();
                                                mwo.Machine = data;
                                                mwo.User = new User() { ID = 0 };
                                                mwo.Urgency = 2;
                                                mwo.WorkOrderType = VehicleWorkOrderTypesEnum.Maintenance.ToString();
                                                mwo.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = freqId, Frequency = GetMaintenanceFreqByString(freqId) };
                                                mwo.FirstServiceDate = sDate;
                                                mwo.NextServiceDate = sDate;
                                                mwo.CreatedDate = DateTime.Now;
                                                mwo.CreatedBy = userName;
                                                mwo.IsCompleted = false;
                                                mwo.CompletedDate = null;
                                                mwo.CompletedBy = string.Empty;
                                                mwo.Status = VehicleWorkOrderEnum.Pending.ToString();
                                                mwo.OrderType = SelectedOrderType;
                                                mwo.MachineProvider = new MachineProvider() { ProviderID = SelectedProviderID};
                                                mwo.ExternalMechanicName = String.Empty;
                                                mwo.Reason = String.Empty;
                                                if (mwo.OrderType.Equals("External"))
                                                {
                                                    mwo.ShowOrderDate = GetLastCompletedDate(sDate);                                                            
                                                }
                                                else
                                                {
                                                    mwo.ShowOrderDate = null;                                                            
                                                }
                                                   
                                                mwo.MachineMaintenanceInfo = new ObservableCollection<MachineMaintenanceInfo>();

                                                MachineMaintenanceInfo mmi = new MachineMaintenanceInfo();
                                                mmi.ID = itemMMI.ID;
                                                mmi.Machine = new Machines(0) { MachineID = itemMMI.Machine.MachineID };
                                                mmi.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = itemMMI.MachineMaintenanceFrequency.ID, Frequency = GetMaintenanceFreqByString(itemMMI.MachineMaintenanceFrequency.ID) };
                                                mmi.MachineCode = itemMMI.MachineCode;
                                                mmi.MachineDescription = itemMMI.MachineDescription;
                                                mmi.Repetition = itemMMI.Repetition;
                                                mmi.CreatedDate = itemMMI.CreatedDate;
                                                mmi.CreatedBy = itemMMI.CreatedBy;
                                                mmi.IsActive = itemMMI.IsActive;
                                                mmi.OrderType = itemMMI.OrderType;
                                                mmi.ProviderID = itemMMI.ProviderID;
                                                mmi.ShowOrderBeforeDays = itemMMI.ShowOrderBeforeDays;

                                                for (int i = 0; i < machineWorkOrderList.Count; i++)
                                                {
                                                    for (int x = 0; x < machineWorkOrderList[i].MachineMaintenanceInfo.Count; x++)
                                                    {
                                                        if (machineWorkOrderList[i].MachineMaintenanceInfo[x].Machine.MachineID == data.MachineID &&
                                                            machineWorkOrderList[i].MachineMaintenanceInfo[x].MachineMaintenanceFrequency.ID == freqId &&
                                                            machineWorkOrderList[i].NextServiceDate == mwo.NextServiceDate)
                                                        {
                                                            machineWorkOrderList[i].MachineMaintenanceInfo.Add(mmi);
                                                            run = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (run == false)
                                                {
                                                    mwo.MachineMaintenanceInfo.Add(mmi);
                                                    machineWorkOrderList.Add(mwo);
                                                    if (n == c && runOtherDes == false)
                                                    {
                                                        foreach (var itemMMI2 in MachineMaintenanceInfo)
                                                        {
                                                            if (itemMMI2.Machine.MachineID == data.MachineID && itemMMI2.MachineMaintenanceFrequency.ID == freqId && String.IsNullOrWhiteSpace(itemMMI2.Repetition))
                                                            {
                                                                mwo.MachineMaintenanceInfo.Add(itemMMI2);
                                                                runOtherDes = true;
                                                            }
                                                        }
                                                    }
                                                }
                                                n++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }  
                }
                    
                if (machineWorkOrderList.Count > 0)
                {  
                    int res1 = DBAccess.CreateMachineWorkOrder(userName, machineWorkOrderList, SelectedOrderType, SelectedProviderID, MachineMaintenanceInfoList, removedList);
                    if (res1 > 0)
                    {
                        Msg.Show("Machine work order created" + System.Environment.NewLine + res1 + " Work orders created", "Machine Work Order Created", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.OK);
                        ClearData();
                    }
                }
                else
                {
                    Msg.Show("Did not create any orders", "Orders Not Created", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.OK);

                }
               
            }                      
        }


        private void UpdateData(string type)
        {
            ObservableCollection<MachineMaintenanceInfo> newList = ReArrangeData();

            int res = DBAccess.UpdateMachineMaintenanceInfo(SelectedMachineID, userName, newList, removedList);
            if (type.Equals("loud"))
            {
                if (res > 0)
                {
                    Msg.Show("Maintenance details have been updated succesfully", "Details Updated", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.OK);
                }
                else if (res == -1)
                {
                    Msg.Show("There were no changes found to update", "No Changes Made", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.OK);
                }
                else
                {
                    Msg.Show("No changes were made", "No Changes Made", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
                }
            }
        }

        private DateTime GetLastCompletedDate(DateTime date)
        {
            DateTime lastCompletedDate = DateTime.Now.Date;
            if (SelectedOrderType.Equals("External"))
            {
                lastCompletedDate = Convert.ToDateTime(date).Date.AddDays(-Convert.ToInt16(SelectedDays));
                if (lastCompletedDate.Date < DateTime.Now.Date)
                {
                    lastCompletedDate = DateTime.Now.Date;
                }
            }

            return lastCompletedDate;
        }

        private ObservableCollection<MachineMaintenanceInfo> ReArrangeData()
        {
            MachineMaintenanceInfo v = new MachineMaintenanceInfo();
            ObservableCollection<MachineMaintenanceInfo> mmiNewList = new ObservableCollection<MachineMaintenanceInfo>();

            DateTime lastCompletedDate = GetLastCompletedDate(Convert.ToDateTime(SelectedDate).Date);
                           
            foreach (var item in MachineMaintenanceInfoList)
            {
                if (!string.IsNullOrWhiteSpace(item.MachineDescription))
                {
                    item.Machine = new Machines(0) { MachineID = SelectedMachineID };
                    item.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = maintenanceFrequency };
                    item.MachineCode = item.SequenceStr;
                    item.Repetition = string.Empty;
                    item.ShowOrderBeforeDays = SelectedDays.Equals("Select") ? 0 : Convert.ToInt16(SelectedDays);
                    item.LastDate = lastCompletedDate;
                    item.OrderType = SelectedOrderType;
                    item.ProviderID = SelectedProviderID;
                    if (SelectedFrequency.Equals("Weekly"))
                    {
                        string str = string.Empty;
                        if (item.MonChecked)
                        {
                            str += "mon";
                        }
                        if (item.TueChecked)
                        {
                            str += String.IsNullOrWhiteSpace(str) ? "tue" : ",tue";
                        }
                        if (item.WedChecked)
                        {
                            str += String.IsNullOrWhiteSpace(str) ? "wed" : ",wed";
                        }
                        if (item.ThusChecked)
                        {
                            str += String.IsNullOrWhiteSpace(str) ? "thus" : ",thus";
                        }
                        if (item.FriChecked)
                        {
                            str += String.IsNullOrWhiteSpace(str) ? "fri" : ",fri";
                        }
                        item.Repetition = str;
                    }
                    mmiNewList.Add(item);
                }
            }  

            return mmiNewList;
        }

        public static DateTime GetNextWeekday(DateTime start, string d)
        {
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
                default: day = DayOfWeek.Monday;
                    break;
            }

            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        private void RemoveRecord(object parameter)
        {
            int index = MachineMaintenanceInfoList.IndexOf(parameter as MachineMaintenanceInfo);
            if (index > -1 && index < MachineMaintenanceInfoList.Count)
            {
                removedList.Add(MachineMaintenanceInfoList[index].ID);
                MachineMaintenanceInfoList.RemoveAt(index);
            }
        }

        private string GetMaintenanceFreqByString(int num)
        {
            string str = string.Empty;

            switch (num)
            {
                case 1: str= "Daily";
                    break;
                case 2 : str="Weekly";
                    break;
                case 3:  str = "1 Month";
                    break;
                case 4:  str = "6 Months";
                    break;
                case 5:  str = "1 Year";
                    break;
                case 6:  str = "2 Years";
                    break;
                case 7:  str = "Fortnightly";
                    break;               
                case 8: str = "2 Months";
                    break;
                case 9: str = "3 Months";
                    break;
                case 10: str = "4 Months";
                    break;
                case 11: str = "5 Months";
                    break;
                case 12:str = "3 Years";
                    break;
                case 13:str = "4 Years";
                    break;
                case 14:str = "5 Years";
                    break;
                case 15:str = "10 Years";
                    break;
            }
            return str;
        }
        private void LoadMaintenanceInfo(int num)
        {
            if (num > 0)
            {
                MachineMaintenanceInfoList.Clear();
                removedList.Clear();

                ObservableCollection<MachineMaintenanceInfo> localColl = DBAccess.GetMachineMaintenanceInfoBySequence(SelectedMachineID, num,SelectedOrderType);

                foreach (var item in localColl)
                {
                    MachineMaintenanceInfoList.Add(item);
                }

                MachineMaintenanceInfoList.CollectionChanged += OnMachineListChanged;
            }
            else
            {
                Msg.Show("Cannot load machine sequence id. Please try again later ", "Cannot Load Machine Sequence Id", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
        }

        private DateTime CalculateNextServiceDate(int n, DateTime date)
        {
            DateTime d = CurrentDate;
            BusinessDaysGenerator bdg = new BusinessDaysGenerator();

            switch (n)
            {
                case 2: d = bdg.SkipWeekends(date.AddDays(7));
                    break;                
                case 3: d = bdg.SkipWeekends(date.AddMonths(1));
                    break;
                case 4: d = bdg.SkipWeekends(date.AddMonths(6));
                    break;
                case 5: d = bdg.SkipWeekends(date.AddYears(1));
                    break;
                case 6: d = bdg.SkipWeekends(date.AddYears(2));
                    break;
                case 7: d = bdg.SkipWeekends(date.AddDays(14));
                    break;
                case 8: d = bdg.SkipWeekends(date.AddMonths(2));
                    break;
                case 9: d = bdg.SkipWeekends(date.AddMonths(3));
                    break;
                case 10: d = bdg.SkipWeekends(date.AddMonths(4));
                    break;
                case 11: d = bdg.SkipWeekends(date.AddMonths(5));
                    break;
                case 12: d = bdg.SkipWeekends(date.AddYears(3));
                    break;
                case 13: d = bdg.SkipWeekends(date.AddYears(4));
                    break;
                case 14: d = bdg.SkipWeekends(date.AddYears(5));
                    break;
                case 15: d = bdg.SkipWeekends(date.AddYears(10));
                    break;
                
            }

            return d;
        }

        private void ViewMaintenanceData()
        {
            if (Machines.Count == 0)
            {
                Msg.Show("Machine(s) not available for " + GetLocationName() + System.Environment.NewLine + "Please add machines to " + GetLocationName(), "Machine(s) Not Available", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.OK);
            }
            else
            {           
                ChildWindowView myChildWindow = new ChildWindowView();
                myChildWindow.machineMaintDes_Closed += (r =>
                {
                    if (r == 1)
                    {
                        Console.WriteLine("It is closed now");
                    }
                });
                myChildWindow.ShowMachineMainDes(SelectedLocationID, userName,SelectedMachineID, SelectedOrderType,SelectedProviderID, SelectedDays);
            }
        }

        private string GetLocationName()
        {
            string name = string.Empty;
            switch (SelectedLocationID)
            {
                case 1: name = "Qld";
                    break;
                case 2: name = "Nsw";
                    break;
                default: name = "Qld";
                    break;
            }

            return name;
        }

        private void AddNewItem()
        {
            DateTime lastCompletedDate = DateTime.Now.Date;
            if(SelectedOrderType.Equals("External"))
            {
                lastCompletedDate=Convert.ToDateTime(SelectedDate).Date.AddDays(-Convert.ToInt16(SelectedDays));
                if(lastCompletedDate.Date < DateTime.Now.Date)
                {
                    lastCompletedDate=DateTime.Now.Date;
                }
            }
            if (MachineMaintenanceInfoList.Count > 0)
            {
                MachineMaintenanceInfo v = MachineMaintenanceInfoList.Last();
                int vmsId = v.ID;

                MachineMaintenanceInfo vd = MachineMaintenanceInfoList.Last();
                if (!string.IsNullOrWhiteSpace(vd.MachineDescription))
                {
                    MachineMaintenanceInfoList.Add(new MachineMaintenanceInfo() 
                                                        { 
                                                            ID = 0, Machine = new Machines(0) 
                                                            { 
                                                                MachineID = SelectedMachineID 
                                                            }, 
                                                            MachineMaintenanceFrequency = new MachineMaintenanceFrequency() 
                                                            { 
                                                                ID = maintenanceFrequency 
                                                            }, 
                                                            LastDate = lastCompletedDate,
                                                            Repetition = string.Empty, 
                                                            OrderType = SelectedOrderType.Equals("Select") ? "Internal" : "External", 
                                                            ProviderID =  SelectedProviderID, 
                                                            ShowOrderBeforeDays = SelectedDays.Equals("Select") ? 0 : Convert.ToInt16(SelectedDays)
                    });
                }
                else
                {
                    Msg.Show("Please enter maintenance description for Item No " + vd.SequenceNumber, "", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
                }
            }
            else
            {
                if (SelectedFrequency.Equals("Select"))
                {
                    Msg.Show("Please tick maintenance sequance", "", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
                }
                else
                {
                    MachineMaintenanceInfoList.Add(new MachineMaintenanceInfo()
                    {                        
                        LastDate = lastCompletedDate
                    });
                }
            }
        }


        public string SelectedDays
        {
            get
            {
                return _selectedDays;
            }
            set
            {
                _selectedDays = value;
                RaisePropertyChanged(() => this.SelectedDays);

                EnableDisableUpdateAddBtn();
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

        public DateTime CurrentDate
        {
            get
            {
                return _currentDate;
            }
            set
            {
                _currentDate = value;
                RaisePropertyChanged(() => this.CurrentDate);
            }
        }

        public List<StockLocation> StockLocation
        {
            get
            {
                return _stockLocation;
            }
            set
            {
                _stockLocation = value;
                RaisePropertyChanged(() => this.StockLocation);

            }
        }

        public int SelectedLocationID
        {
            get
            {
                return _selectedLocationID;
            }
            set
            {
                _selectedLocationID = value;
                RaisePropertyChanged(() => this.SelectedLocationID);
                LoadMachines();
                EnableCreateClear();
                EnDisMainDesButton();              

            }
        }

        public ObservableCollection<Machines> Machines
        {
            get
            {
                return _machines;
            }
            set
            {
                _machines = value;
                RaisePropertyChanged(() => this.Machines);
            }
        }

        public int SelectedMachineID
        {
            get
            {
                return _selectedMachineID;
            }
            set
            {
                _selectedMachineID = value;
                RaisePropertyChanged(() => this.SelectedMachineID);

                SelectedFrequency = "Select";

                MachineMaintenanceInfoList.Clear();
                removedList.Clear();

                if (SelectedMachineID == 0)
                {                   
                    CheckEnabled = false;                   
                    
                }
                else
                {   
                    CheckEnabled = true;
                }

                EnableDisableUpdateAddBtn();
            }
        }
       
        public bool AddItemEnabled
        {
            get
            {
                return _addItemEnabled;
            }
            set
            {
                _addItemEnabled = value;
                RaisePropertyChanged(() => this.AddItemEnabled);
            }
        }

        public string MainDesBackground
        {
            get
            {
                return _mainDesBackground;
            }
            set
            {
                _mainDesBackground = value;
                RaisePropertyChanged(() => this.MainDesBackground);
            }
        }

        public bool IsMachineEnabled
        {
            get
            {
                return _isMachineEnabled;
            }
            set
            {
                _isMachineEnabled = value;
                RaisePropertyChanged(() => this.IsMachineEnabled);
            }
        }

        public string AddItemBackground
        {
            get
            {
                return _addItemBackground;
            }
            set
            {
                _addItemBackground = value;
                RaisePropertyChanged(() => this.AddItemBackground);
            }
        }

        public string FrequencyVisibility
        {
            get
            {
                return _frequencyVisibility;
            }
            set
            {
                _frequencyVisibility = value;
                RaisePropertyChanged(() => this.FrequencyVisibility);
            }
        }
               
        
        private void EnableDisableUpdateAddBtn()
        {
            if (!SelectedFrequency.Equals("Select") && !SelectedOrderType.Equals("Select") && SelectedMachineID > 0)
            {
                if(SelectedOrderType.Equals("External") && (!SelectedDays.Equals("Select")))
                {
                    AddItemEnabled = true;
                    AddItemBackground = "#2E8856";
                }
                else if(SelectedOrderType.Equals("Internal"))
                {
                    AddItemEnabled = true;
                    AddItemBackground = "#2E8856";
                }
                else
                {
                    AddItemEnabled = false;
                    AddItemBackground = "#FFDEDEDE";
                }
            }
            else
            {
                AddItemEnabled = false;
                AddItemBackground = "#FFDEDEDE";
            }
        }       

        public string SelectedFrequency
        {
            get
            {
                return _selectedFrequency;
            }
            set
            {
                _selectedFrequency = value;
                RaisePropertyChanged(() => this.SelectedFrequency);

                if (MachineMaintenanceInfoList != null)
                {
                    MachineMaintenanceInfoList.Clear();
                }

                EnableCreateClear();

                if (!SelectedFrequency.Equals("Select"))
                {
                    int freqId = GetFreqID();
                    LoadMaintenanceInfo(freqId);
                    EnableDisableUpdateAddBtn();
                    maintenanceFrequency = freqId;
                    RepetitionVisiblity = "Collapsed";
                    if (SelectedFrequency.Equals("Weekly"))
                    {
                        RepetitionVisiblity = "Visible";
                    }
                }
            }
        }
        public string RepetitionVisiblity
        {
            get
            {
                return _repetitionVisiblity;
            }
            set
            {
                _repetitionVisiblity = value;
                RaisePropertyChanged(() => this.RepetitionVisiblity);
            }
        }

        public string SubmitBackground
        {
            get
            {
                return _submitBackground;
            }
            set
            {
                _submitBackground = value;
                RaisePropertyChanged(() => this.SubmitBackground);
            }
        }

        public string ClearBackground
        {
            get
            {
                return _clearBackground;
            }
            set
            {
                _clearBackground = value;
                RaisePropertyChanged(() => this.ClearBackground);
            }
        }

        public bool SubmitEnabled
        {
            get
            {
                return _submitEnabled;
            }
            set
            {
                _submitEnabled = value;
                RaisePropertyChanged(() => this.SubmitEnabled);
            }
        }

        public bool ClearEnabled
        {
            get
            {
                return _clearEnabled;
            }
            set
            {
                _clearEnabled = value;
                RaisePropertyChanged(() => this.ClearEnabled);
            }
        }

        public DateTime? SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                RaisePropertyChanged(() => this.SelectedDate);
                EnableCreateClear();               
            }
        }

        public ObservableCollection<MachineMaintenanceInfo> MachineMaintenanceInfoList
        {
            get
            {
                return _machineMaintenanceInfoList;
            }
            set
            {
                _machineMaintenanceInfoList = value;
                RaisePropertyChanged(() => this.MachineMaintenanceInfoList);
            }
        }

        public bool MainDisEnabled
        {
            get
            {
                return _mainDisEnabled;
            }
            set
            {
                _mainDisEnabled = value;
                RaisePropertyChanged(() => this.MainDisEnabled);
            }
        }

        public bool CheckEnabled
        {
            get
            {
                return _checkEnabled;
            }
            set
            {
                _checkEnabled = value;
                RaisePropertyChanged(() => this.CheckEnabled);
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

                SelectedProviderID = 0;         

                if (MachineMaintenanceInfoList != null)
                {
                    MachineMaintenanceInfoList.Clear();
                    removedList.Clear();
                }

                SelectedFrequency = "Select";
                SelectedDays = "Select";

                if (SelectedOrderType.Equals("Internal") || SelectedOrderType.Equals("Select"))
                {
                    ProviderVisibility = "Collapsed";
                    FrequencyVisibility = "Collapsed";
                    if (SelectedOrderType.Equals("Internal"))
                    {
                        FrequencyVisibility = "Visible";
                    }
                }
                else if (SelectedOrderType.Equals("External"))
                {
                    ProviderVisibility = "Visible";
                    FrequencyVisibility = "Visible";
                }

                EnableDisableUpdateAddBtn();

            }
        }

        public int ItemCount
        {
            get { return _itemCount; }

            set
            {
                _itemCount = value;
                base.RaisePropertyChanged(() => this.ItemCount);
            }
        }

        public string ProviderVisibility
        {
            get
            {
                return _providerVisibility;
            }
            set
            {
                _providerVisibility = value;
                RaisePropertyChanged(() => this.ProviderVisibility);
            }
        }

        public ObservableCollection<MachineProvider> Providers
        {
            get
            {
                return _providers;
            }
            set
            {
                _providers = value;
                RaisePropertyChanged(() => this.Providers);
            }
        }

        public int SelectedProviderID
        {
            get
            {
                return _selectedProviderID;
            }
            set
            {
                _selectedProviderID = value;
                RaisePropertyChanged(() => this.SelectedProviderID);
            }
        }       

        public bool OrderTypeEnabled
        {
            get
            {
                return _orderTypeEnabled;
            }
            set
            {
                _orderTypeEnabled = value;
                RaisePropertyChanged(() => this.OrderTypeEnabled);
            }
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        #region COMMANDS

        public ICommand RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, RemoveRecord);
                }
                return _removeCommand;
            }
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return _homeCommand ?? (_homeCommand = new LogOutCommandHandler(() => Switcher.Switch(new MainMenu(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand AdminDashboardCommand
        {
            get
            {
                return _adminDashboardCommand ?? (_adminDashboardCommand = new LogOutCommandHandler(() => Switcher.Switch(new AdminDashboardView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand MaintenanceCommand
        {
            get
            {
                return _maintenanceCommand ?? (_maintenanceCommand = new LogOutCommandHandler(() => Switcher.Switch(new MaintenanceMenuView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand MachinesCommand
        {
            get
            {
                return _machinesCommand ?? (_machinesCommand = new LogOutCommandHandler(() => Switcher.Switch(new MaintenanceMenuView(userName, state, userPrivilages, metaData)), canExecute));
            }
        }

        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new LogOutCommandHandler(() => ClearData(), canExecute));
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                return _updateCommand ?? (_updateCommand = new A1QSystem.Commands.LogOutCommandHandler(() => UpdateData("loud"), canExecute));
            }
        }
        public ICommand CreateWorkOrderCommand
        {
            get
            {
                return _createWorkOrderCommand ?? (_createWorkOrderCommand = new LogOutCommandHandler(() => CreateWorkOrder(), canExecute));
            }
        }

        public ICommand ViewMaintenanceCommand
        {
            get
            {
                return _viewMaintenanceCommand ?? (_viewMaintenanceCommand = new LogOutCommandHandler(() => ViewMaintenanceData(), canExecute));
            }
        }

        public ICommand AddItemCommand
        {
            get
            {
                return _addItemCommand ?? (_addItemCommand = new A1QSystem.Commands.LogOutCommandHandler(() => AddNewItem(), canExecute));
            }
        }

        #endregion
    }
}
