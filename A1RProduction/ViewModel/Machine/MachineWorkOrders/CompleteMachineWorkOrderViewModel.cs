
using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Other;
using A1QSystem.Model.Users;
using A1QSystem.View;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Vbe.Interop;
using Microsoft.Win32;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Machine.MachineWorkOrders
{
    public class CompleteMachineWorkOrderViewModel : ViewModelBase
    {
        private string pathFrom, pathTo;
        private ObservableCollection<FileUpload> _fileUploadedList;
        private string _externalMechanicName, _externalMechanicNameVisibility, _reason, _reasonVisibility;
        private List<MetaData> metaData;
        private ObservableCollection<MachineWorkDescription> _machineWorkDescription;
        private MachineWorkOrder _machineWorkOrder;
        private List<UserPosition> _userPositions;
        private ObservableCollection<MachineMaintenanceInfo> machineMaintenanceInfo;
        private List<MachineWorkOrder> newMachineWorkOrderList;
        private List<MachineWorkOrder> dbWorkOrders;
        private string _selectedMechanic;
        private string userCompleted;
        private string _partsOrdedVisibility;
        private bool canExecute, _tickAll, _didNotCompleteChecked;
        private ChildWindowView myChildWindow;
        public event Action<int> Closed;
        private Microsoft.Practices.Prism.Commands.DelegateCommand _closeCommand;
        private ICommand _completeCommand, _problemCommand,  _addNewRowCommand, _uploadDocumentCommand, _removeCommand;

        public CompleteMachineWorkOrderViewModel(MachineWorkOrder mwo, string uN)
        {
            FileUploadedList = new ObservableCollection<FileUpload>();
            ReasonVisibility = "Collapsed";
            DidNotCompleteChecked = false;
            ExternalMechanicName = string.Empty;
            ExternalMechanicNameVisibility = "Collapsed";
            MachineWorkOrder = mwo;
            userCompleted = uN;
            Reason = string.Empty;
            UserPositions = new List<UserPosition>();
            MachineWorkDescription = new ObservableCollection<MachineWorkDescription>();

            metaData = DBAccess.GetMetaData();

            if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
            {
                PartsOrdedVisibility = "Collapsed";
                MachineWorkDescription = DBAccess.GetAllMachineWorkDescriptionByID(MachineWorkOrder.WorkOrderNo);
                if (MachineWorkDescription.Count > 0)
                {
                    ObservableCollection<MachineRepairDescription> machineRepairDescriptionList = DBAccess.GetMachineRepairDescriptionByID(MachineWorkDescription);
                    ObservableCollection<MachineParts> machinePartsList = DBAccess.GetMachinePartsDescriptionByID(machineRepairDescriptionList);

                    foreach (var item in MachineWorkDescription)
                    {
                        int x = item.MachineRepairDescription.Count;
                        if (x > 0)
                        {
                            item.ItemDone = false;
                        }
                        item.MachineRepairDescription = new ObservableCollection<MachineRepairDescription>();
                        foreach (var items in machineRepairDescriptionList)
                        {
                            if (item.ID == items.MachineWorkDescriptionID)
                            {
                                item.ItemRepair = true;
                                items.RepairCompletedVisibility = "Collapsed";
                                item.MachineRepairDescription.Add(items);
                                items.MachineParts = new ObservableCollection<MachineParts>();
                                foreach (var itemz in machinePartsList)
                                {
                                    if (items.ID == itemz.MachineRepairID)
                                    {
                                        items.MachineParts.Add(itemz);
                                    }
                                }
                            }
                        }

                    }
                    MachineWorkDescription.Add(new MachineWorkDescription() { MachineMaintenanceInfo = new MachineMaintenanceInfo() { MachineCode = "Select" }, IsCompleted = true, ItemDone = true });

                    foreach (var item in MachineWorkDescription)
                    {
                        

                            if (String.IsNullOrWhiteSpace(item.MachineMaintenanceInfo.MachineDescription))
                            {
                                item.RootVisibility = "Collapsed";
                            }
                            else
                            {
                                item.RootVisibility = "Visible";
                            }
                            //if (item.MachineMaintenanceInfo.MachineCode != "Select")
                            //{
                                int x = item.MachineRepairDescription.Count;
                                if (x > 0)
                                {
                                    item.CompletedVisibility = "Collapsed";
                                    item.RepairOrderVisibility = "Visible";
                                }
                                else
                                {
                                    item.CompletedVisibility = "Visible";
                                    item.RepairOrderVisibility = "Collapsed";
                                }
                            //}
                    }
                }

                newMachineWorkOrderList = new List<MachineWorkOrder>();
                machineMaintenanceInfo = new ObservableCollection<MachineMaintenanceInfo>();
                dbWorkOrders = new List<MachineWorkOrder>();

                for (int i = 0; i < MachineWorkDescription.Count; i++)
                {
                    if (MachineWorkDescription[i].MachineMaintenanceInfo.MachineCode == "Select")
                    {
                        MachineWorkDescription.RemoveAt(i);
                    }
                }

                for (int i = 0; i < MachineWorkDescription.Count; i++)
                {
                    //Find the correct day for the new workorder
                    List<Tuple<DateTime, DateTime, DateTime>> nextAvaDates = GetNextDates(MachineWorkDescription[i].MachineMaintenanceInfo.MachineMaintenanceFrequency.ID);

                    if (MachineWorkDescription[i].MachineMaintenanceInfo.MachineMaintenanceFrequency.ID == 2 && !String.IsNullOrWhiteSpace(MachineWorkDescription[i].MachineMaintenanceInfo.Repetition))
                    {
                        string[] tokens = MachineWorkDescription[i].MachineMaintenanceInfo.Repetition.Split(',');

                        foreach (var itemTO in tokens)
                        {
                            DateTime sDate = GetNextWeekday(itemTO);
                            MachineMaintenanceInfo mmi1 = new MachineMaintenanceInfo();
                            mmi1.ID = MachineWorkDescription[i].MachineMaintenanceInfo.ID;
                            mmi1.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = 2, Frequency = "Weekly" };
                            mmi1.MachineCode = MachineWorkDescription[i].MachineMaintenanceInfo.MachineCode;
                            mmi1.MachineDescription = MachineWorkDescription[i].MachineMaintenanceInfo.MachineDescription;
                            mmi1.CreatedBy = userCompleted;
                            mmi1.CreatedDate = DateTime.Now;
                            mmi1.Repetition = MachineWorkDescription[i].MachineMaintenanceInfo.Repetition;
                            mmi1.FirstDate = nextAvaDates[0].Item1;
                            mmi1.PreferredDate = sDate;
                            mmi1.LastDate = nextAvaDates[0].Item3;
                            machineMaintenanceInfo.Add(mmi1);
                        }
                        goto a;
                    }

                    MachineMaintenanceInfo mmi2 = new MachineMaintenanceInfo();
                    mmi2.ID = MachineWorkDescription[i].MachineMaintenanceInfo.ID;
                    mmi2.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = MachineWorkDescription[i].MachineMaintenanceInfo.MachineMaintenanceFrequency.ID, Frequency = MachineWorkDescription[i].MachineMaintenanceInfo.MachineMaintenanceFrequency.Frequency };
                    mmi2.MachineCode = MachineWorkDescription[i].MachineMaintenanceInfo.MachineCode;
                    mmi2.MachineDescription = MachineWorkDescription[i].MachineMaintenanceInfo.MachineDescription;
                    mmi2.CreatedBy = userCompleted;
                    mmi2.CreatedDate = DateTime.Now;
                    mmi2.Repetition = MachineWorkDescription[i].MachineMaintenanceInfo.Repetition;
                    mmi2.FirstDate = nextAvaDates[0].Item1;
                    mmi2.PreferredDate = nextAvaDates[0].Item2;
                    mmi2.LastDate = nextAvaDates[0].Item3;
                    machineMaintenanceInfo.Add(mmi2);
                a:
                    Console.WriteLine();

                }

                MachineWorkOrder.MachineMaintenanceInfo = machineMaintenanceInfo;

            }
            else if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
            {
                PartsOrdedVisibility = "Visible";
                ObservableCollection<MachineRepairDescription> vehicleRepairDescriptionList = DBAccess.GetMahcineRepairDescriptionByID2(MachineWorkOrder.WorkOrderNo);
                ObservableCollection<MachineParts> machinePartsList = DBAccess.GetMachinePartsDescriptionByID(vehicleRepairDescriptionList);
                Int32 id = 0;
                foreach (var item in vehicleRepairDescriptionList)
                {
                    id = item.MachineWorkDescriptionID;
                    break;
                }

                MachineWorkDescription = DBAccess.GetMachineWorkDescriptionForRepair(id);

                foreach (var item in vehicleRepairDescriptionList)
                {
                    item.MachineParts = new ObservableCollection<MachineParts>();
                    foreach (var items in machinePartsList)
                    {
                        if (item.ID == items.MachineRepairID)
                        {
                            item.MachineParts.Add(items);
                        }
                        item.RepairCompletedVisibility = "Visible";
                    }
                }
                if (vehicleRepairDescriptionList.Count > 0 && MachineWorkDescription.Count > 0)
                {


                    MachineWorkDescription[0].MachineRepairDescription = vehicleRepairDescriptionList;
                    //VehicleWorkDescription.Add(new VehicleWorkDescription() { VehicleRepairDescription = vehicleRepairDescriptionList });
                }
                else
                {
                    if (MachineWorkOrder.Machine.MachineID == 0)
                    {
                        MachineWorkDescription.Add(new MachineWorkDescription() { MachineMaintenanceInfo = new MachineMaintenanceInfo() { MachineDescription = MachineWorkOrder.Machine.MachineName }, MachineRepairDescription = vehicleRepairDescriptionList });

                    }
                    else
                    {
                        MachineWorkDescription.Add(new MachineWorkDescription() { MachineMaintenanceInfo = new MachineMaintenanceInfo() { MachineDescription = "Machine Repair for " + MachineWorkOrder.Machine.MachineName }, MachineRepairDescription = vehicleRepairDescriptionList });
                    }
                }

                foreach (var item in MachineWorkDescription)
                {
                    int x = item.MachineRepairDescription.Count;
                    if (x > 0)
                    {
                        item.CompletedVisibility = "Collapsed";
                        item.RepairOrderVisibility = "Collapsed";
                    }
                    else
                    {
                        item.CompletedVisibility = "Visible";
                        item.RepairOrderVisibility = "Visible";
                    }
                    item.RootVisibility = "Visible";                   
                }               
            }

            _closeCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(CloseForm);
            canExecute = true;
            LoadUserPositions();

            if (!string.IsNullOrWhiteSpace(MachineWorkOrder.User.FullName))
            {
                SelectedMechanic = MachineWorkOrder.User.FullName;
            }
            else
            {
                SelectedMechanic = "Select";
            }

            if (metaData != null)
            {
                var data1 = metaData.FirstOrDefault(x => x.KeyName.Equals("machine_file_upload_location"));
                if (data1 != null)
                {
                    pathFrom = data1.Description.Replace("\\\\", @"\");
                }

                var data2 = metaData.FirstOrDefault(x => x.KeyName.Equals("machine_download_location"));
                if (data2 != null)
                {
                    pathTo = data2.Description.Replace("\\\\", @"\");
                }
            }
        }

        private void UploadDocuments(object parameter)
        {
            int index = FileUploadedList.IndexOf(parameter as FileUpload);
            if (index > -1 && index < FileUploadedList.Count)
            {

                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();


                if (Directory.Exists(pathFrom))
                {
                    openFileDialog.InitialDirectory = pathFrom;
                }
                else
                {
                    openFileDialog.InitialDirectory = @"C:\";
                }

                if (openFileDialog.ShowDialog() == true)
                {
                    string filename = openFileDialog.FileName;
                    if (File.Exists(filename))
                    {
                        string name = Path.GetFileName(filename);
                        string destinationFilename = Path.Combine(pathTo, name);

                        var filenNme = Path.GetFileName(name);

                        if (!File.Exists(pathTo + @"\" + filenNme))
                        {
                            FileUploadedList[index].FilePathFrom = filename;
                            FileUploadedList[index].FileName = filenNme;
                            FileUploadedList[index].UploadedDateTime = DateTime.Now;
                            FileUploadedList[index].UploadedBy = userCompleted;
                        }
                        else
                        {
                            Msg.Show("This file already exists", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                        }
                    }
                }


                FileUploadedList[index].FilePathTo = pathTo;               
            }
        }

        private void RemoveRecod(object parameter)
        {
            int index = FileUploadedList.IndexOf(parameter as FileUpload);
            if (index > -1 && index < FileUploadedList.Count)
            {
                FileUploadedList.RemoveAt(index);
            }
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
                case 7:
                    dates.Add(Tuple.Create(nextDate, bdg.SkipWeekends(currentDate.AddDays(14)), bdg.SkipWeekends(currentDate.AddDays(14))));//14 days
                    break;
                case 8:
                    dates.Add(Tuple.Create(nextDate, bdg.SkipWeekends(currentDate.AddMonths(2)), bdg.SkipWeekends(currentDate.AddMonths(2))));//2 Months
                    break;
                case 9:
                    dates.Add(Tuple.Create(nextDate, bdg.SkipWeekends(currentDate.AddMonths(3)), bdg.SkipWeekends(currentDate.AddMonths(3))));//3 Months
                    break;
                case 10:
                    dates.Add(Tuple.Create(nextDate, bdg.SkipWeekends(currentDate.AddMonths(4)), bdg.SkipWeekends(currentDate.AddMonths(4))));//4 Months
                    break;
                case 11:
                    dates.Add(Tuple.Create(nextDate, bdg.SkipWeekends(currentDate.AddMonths(5)), bdg.SkipWeekends(currentDate.AddMonths(5))));//5 Months
                    break;
                case 12:
                    dates.Add(Tuple.Create(nextDate, bdg.SkipWeekends(currentDate.AddYears(3)), bdg.SkipWeekends(currentDate.AddYears(3))));//3 Years
                    break;
                case 13:
                    dates.Add(Tuple.Create(nextDate, bdg.SkipWeekends(currentDate.AddYears(4)), bdg.SkipWeekends(currentDate.AddYears(4))));//4 Years
                    break;
                case 14:
                    dates.Add(Tuple.Create(nextDate, bdg.SkipWeekends(currentDate.AddYears(5)), bdg.SkipWeekends(currentDate.AddYears(5))));//5 Years
                    break;
                case 15:
                    dates.Add(Tuple.Create(nextDate, bdg.SkipWeekends(currentDate.AddYears(10)), bdg.SkipWeekends(currentDate.AddYears(10))));//10 Years
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

        private void CloseForm()
        {
            if (Closed != null)
            {
                FileUploadedList.Clear();
                int x = 1;
                Closed(x);
            }
        }
        private void AddNewRow()
        {
            if (FileUploadedList != null)
            {
                bool has = FileUploadedList.Any(x => string.IsNullOrWhiteSpace(x.Description));
                if (has == false)
                {
                    FileUpload od = null;
                    if (FileUploadedList.Count > 0)
                    {
                        od = FileUploadedList[FileUploadedList.Count - 1];
                    }

                    FileUploadedList.Add(new FileUpload() { Description = String.Empty,FileName="" });
                }
            }
        }

        private void LoadUserPositions()
        {
            UserPositions = DBAccess.GetAllUserPositions("MachineMechanic");
            UserPositions.Insert(0,new UserPosition() { FullName = "Select" });
            UserPositions.Add(new UserPosition() { FullName = "External" });

        }

        private int UpdateNewOrders(MachineMaintenanceInfo mmi, DateTime date)
        {
            int x = 0;

            foreach (var itemNMWO in newMachineWorkOrderList)
            {
                if (itemNMWO.NextServiceDate.Date == date.Date)
                {
                    itemNMWO.MachineMaintenanceInfo.Add(mmi);
                }
            }

            if (newMachineWorkOrderList.Count > 0)
            {
                x = 2;
            }
            return x;
        }

        private int CreateNewOrders(Int32 woid,DateTime nsd,MachineMaintenanceFrequency mmf,MachineMaintenanceInfo mmi)
        {
            int x = 0;

            //Check if the order exist
            bool ex = newMachineWorkOrderList.Any(y=>y.NextServiceDate.Date == nsd.Date);
            if(ex)
            {
                foreach (var item in newMachineWorkOrderList)
                {
                    if (item.NextServiceDate.Date == nsd.Date)
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
                mwo.CreatedBy = userCompleted;
                mwo.FirstServiceDate = nsd;
                mwo.NextServiceDate = nsd;
                mwo.Machine = new Machines(0) { MachineID = MachineWorkOrder.Machine.MachineID };
                mwo.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = mmf.ID, Frequency = mmf.Frequency };
                mwo.IsCompleted = false;
                mwo.CreatedDate = DateTime.Now;
                mwo.CreatedBy = userCompleted;
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

        private void PrepareCollections()
        {
            //Remove duplicates in the collection
            List<MachineMaintenanceInfo> mmiSList = new List<MachineMaintenanceInfo>();
            foreach (var item in MachineWorkOrder.MachineMaintenanceInfo)
            {
                if (mmiSList.Count == 0)
                {
                    mmiSList.Add(item);
                }
                else
                {
                    bool ex = mmiSList.Any(x => x.MachineMaintenanceFrequency.ID == item.MachineMaintenanceFrequency.ID);
                    if (ex == false)
                    {
                        mmiSList.Add(item);
                    }
                }
            }

            dbWorkOrders = DBAccess.CheckIfMachineWorkOrderExist(MachineWorkOrder.WorkOrderNo, MachineWorkOrder.Machine.MachineID, mmiSList);
            List<MachineWorkDescription> existingMachineWorkDes = DBAccess.GetMachineMaintenanceInfoByWorkOrderID(dbWorkOrders);
            //Combine db workorders with db descriptions
            foreach (var item in dbWorkOrders)
            {
                foreach (var itemEM in existingMachineWorkDes)
                {
                    if (item.WorkOrderNo == itemEM.MachineWorkOrderNo)
                    {
                        item.MachineMaintenanceInfo.Add(itemEM.MachineMaintenanceInfo);
                    }
                }
            }                  
        }

        private void CompleteOrder()
        {

            bool y = false, z = false;
            if (FileUploadedList != null && FileUploadedList.Count > 0)
            { 
                //Check duplicates for description
                List<FileUpload> newList1 = FileUploadedList.Distinct(new FileUploadComparer()).ToList();

                if (newList1.Count != FileUploadedList.Count)
                {
                    y = true;
                }

                //Check duplicates for filename
                List<FileUpload> newList2 = FileUploadedList.Distinct(new FileUploadFileNameComparer()).ToList();

                if (newList2.Count != FileUploadedList.Count)
                {
                    z = true;
                }
            }

            int result = 0;
            if (SelectedMechanic == "Select")
            {
                Msg.Show("Please select your name", "Select Mechanic Name", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else if (SelectedMechanic.Equals("External") && string.IsNullOrWhiteSpace(ExternalMechanicName))
            {
                Msg.Show("Please enter external mechanic name", "Enter Mechanic Name", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else if(DidNotCompleteChecked && string.IsNullOrWhiteSpace(Reason))
            {
                Msg.Show("Please enter reasons for not completing this order", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else if (FileUploadedList.Any(x => string.IsNullOrWhiteSpace(x.Description)))
            {
                Msg.Show("Please enter file description", "Description Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else if (FileUploadedList.Any(x => string.IsNullOrWhiteSpace(x.FileName)))
            {
                var data = FileUploadedList.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.FileName));
                if (data != null)
                {
                    Msg.Show("Upload file for the description " + data.Description, "Upload File", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }
            }
            else if(y)
            {
                Msg.Show("Duplicate descriptions exist. Please change the description", "Duplicate Descriptions", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else if (z)
            {
                Msg.Show("Duplicate files exist.", "Duplicate Files", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else
            {
                string codes = string.Empty;
                string repCodes = string.Empty;

                if (!DidNotCompleteChecked && string.IsNullOrWhiteSpace(Reason))
                {
                    if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
                    {
                        foreach (var item in MachineWorkDescription)
                        {

                            if (item.ItemDone == false && item.ItemRepair == false)
                            {
                                codes += item.MachineMaintenanceInfo.MachineCode + System.Environment.NewLine;
                            }
                        }
                    }
                    else if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
                    {
                        foreach (var item in MachineWorkDescription)
                        {
                            if (item.Result == false)
                            {
                                foreach (var items in item.MachineRepairDescription)
                                {
                                    if (items.IsCompleted == false)
                                    {
                                        codes += items.StrSequenceNumber + System.Environment.NewLine;
                                    }
                                    else
                                    {
                                        item.IsCompleted = true;
                                    }
                                }
                            }
                        }
                    }
                }

                if (!String.IsNullOrEmpty(codes))
                {
                    Msg.Show("Following task(s) were not done" + System.Environment.NewLine + codes + System.Environment.NewLine + "Please complete the above task(s) to complete work order", "Task(s) Incomplete", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }
                else if (!String.IsNullOrEmpty(repCodes))
                {
                    Msg.Show("Following task(s) require repair order(s)" + System.Environment.NewLine + repCodes + System.Environment.NewLine + "Please create repair orders for above task(s) to complete work order", "Task(s) Incomplete", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }
                else
                {
                    var data = UserPositions.SingleOrDefault(x => x.FullName == SelectedMechanic);
                    if (data != null)
                    {
                        int errNo = 0;

                        if (SelectedMechanic.Equals("External"))
                        {
                            MachineWorkOrder.User.ID = 0;
                        }
                        else
                        {
                            MachineWorkOrder.User = new User() { ID = data.User.ID };
                        }
                        MachineWorkOrder.CreatedBy = userCompleted;
                        MachineWorkOrder.CompletedDate = DateTime.Now;
                        MachineWorkOrder.CompletedBy = userCompleted;

                        if (FileUploadedList.Count > 0)
                        {
                            foreach (var item in FileUploadedList)
                            {                               

                                try
                                {
                                    File.Copy(item.FilePathFrom, pathTo + @"\" + item.FileName);

                                    //item.FilePath = pathTo + @"\" + item.FileName;
                                }
                                catch (IOException rEx)
                                {
                                    errNo = 1;
                                }
                            }

                            if (errNo == 0)
                            {
                                MachineWorkOrder.FileUploadList = new ObservableCollection<FileUpload>();
                                MachineWorkOrder.FileUploadList = FileUploadedList;
                            }
                        }      
                        
                        //Add reason
                        if(DidNotCompleteChecked && !string.IsNullOrWhiteSpace(Reason))
                        {
                            MachineWorkOrder.IsCompleted = false;
                            MachineWorkOrder.Reason = Reason;
                            MachineWorkOrder.Status = "Not Completed";
                        }
                        else
                        {
                            MachineWorkOrder.IsCompleted = true;
                            MachineWorkOrder.Reason = string.Empty;
                            MachineWorkOrder.Status = "Completed";
                        }

                        MachineWorkOrder.ExternalMechanicName = ExternalMechanicName;

                        result = DBAccess.MachineWorkOrderCompleted(MachineWorkOrder, MachineWorkDescription);                     
                       
                        if (result > 0 && errNo == 0)
                        {
                            Msg.Show("Work Order Completed Successfully", "Work Order Completed Successfully", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
                            FileUploadedList.Clear();
                        }
                        else if (errNo == 1)
                        {
                            Msg.Show("Could not access the server location to upload file" + System.Environment.NewLine + "The file path not found!", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);

                        }
                        else
                        {
                            Msg.Show("Something went wrong and the work order did not create", "Work Order Submition Failed", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);

                            if (FileUploadedList.Count > 0)
                            {                             
                                foreach (var item in FileUploadedList)
                                {
                                    File.Delete(pathTo + @"\" + item.FileName);
                                }

                                FileUploadedList.Clear();
                            }
                        }

                        CloseForm();
                    }
                }
            }
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

        private Tuple<Int16, string> ReFormFrequency(MachineWorkOrder dbMwo1,MachineMaintenanceFrequency mf)
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

        public static DayOfWeek GetWeekday(string d)
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

            return day;
        }

        private Tuple<Int16,string> ConsFreq(List<Tuple<int, string>> tup)
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
                    Console.WriteLine(lastDigit);
                    a /= 10;
                }
            }


            var newTup = num.OrderBy(x => x);

            foreach (var item in newTup)
            {
                strOne = strOne + item;
                strTwo = strTwo + GetFrequency(item);
            }
            return Tuple.Create(Convert.ToInt16(strOne),strTwo);
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
        

        private void CreateRepairWorkOrder(Object param)
        {

        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        #region PUBLIC_PROPERTIES

        
        public string ExternalMechanicName
        {
            get
            {
                return _externalMechanicName;
            }
            set
            {
                _externalMechanicName = value;
                RaisePropertyChanged(() => this.ExternalMechanicName);
            }
        }

        
        public string ExternalMechanicNameVisibility
        {
            get
            {
                return _externalMechanicNameVisibility;
            }
            set
            {
                _externalMechanicNameVisibility = value;
                RaisePropertyChanged(() => this.ExternalMechanicNameVisibility);
            }
        }

        

        public bool DidNotCompleteChecked
        {
            get
            {
                return _didNotCompleteChecked;
            }
            set
            {
                _didNotCompleteChecked = value;
                RaisePropertyChanged(() => this.DidNotCompleteChecked);

                if(DidNotCompleteChecked)
                {
                    ReasonVisibility = "Visible";
                }
                else
                {
                    Reason = string.Empty;
                    ReasonVisibility = "Collapsed";                    
                }

                
            }
        }

        public string Reason
        {
            get
            {
                return _reason;
            }
            set
            {
                _reason = value;
                RaisePropertyChanged(() => this.Reason);
            }
        }
        public MachineWorkOrder MachineWorkOrder
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

        public string ReasonVisibility
        {
            get
            {
                return _reasonVisibility;
            }
            set
            {
                _reasonVisibility = value;
                RaisePropertyChanged(() => this.ReasonVisibility);
            }
        }
        

        public string SelectedMechanic
        {
            get
            {
                return _selectedMechanic;
            }
            set
            {
                _selectedMechanic = value;
                RaisePropertyChanged(() => this.SelectedMechanic);
                if (SelectedMechanic.Equals("External"))
                {
                    ExternalMechanicNameVisibility = "Visible";
                }
                else
                {
                    ExternalMechanicNameVisibility = "Collapsed";
                    ExternalMechanicName=string.Empty;
                }
            }
        }

        public ObservableCollection<MachineWorkDescription> MachineWorkDescription
        {
            get
            {
                return _machineWorkDescription;
            }
            set
            {
                _machineWorkDescription = value;
                RaisePropertyChanged(() => this.MachineWorkDescription);
            }
        }

        public bool TickAll
        {
            get
            {
                return _tickAll;
            }
            set
            {
                _tickAll = value;
                RaisePropertyChanged(() => this.TickAll);

                if (TickAll == true)
                {
                    foreach (var item in MachineWorkDescription)
                    {
                        if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
                        {
                            if (item.MachineRepairWorkOrderNo == 0 && item.ItemRepair == false)
                            {
                                item.ItemDone = true;
                                item.IsCompleted = true;
                            }
                            //item.ItemRepair = false;
                        }
                        else if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
                        {
                            item.Result = true;
                            item.ItemDone = true;

                            foreach (var items in item.MachineRepairDescription)
                            {
                                items.IsCompleted = true;
                            }

                        }
                    }

                    
                }
                else
                {
                    if (MachineWorkDescription != null)
                    {
                        foreach (var item in MachineWorkDescription)
                        {
                            if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
                            {
                                item.ItemDone = false;
                                //item.ItemRepair = false;
                            }
                            else if (MachineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
                            {
                                item.Result = false;
                                item.ItemDone = false;
                                foreach (var items in item.MachineRepairDescription)
                                {
                                    items.IsCompleted = false;
                                }
                            }
                        }
                    }
                }
            }
        }
                
        

        public string PartsOrdedVisibility
        {
            get { return _partsOrdedVisibility; }
            set
            {
                _partsOrdedVisibility = value;
                RaisePropertyChanged(() => this.PartsOrdedVisibility);
            }
        }

        public List<UserPosition> UserPositions
        {
            get
            {
                return _userPositions;
            }
            set
            {
                _userPositions = value;
                RaisePropertyChanged(() => this.UserPositions);
            }
        }

        #endregion

        #region COMMANDS

        public ICommand CompleteCommand
        {
            get
            {
                return _completeCommand ?? (_completeCommand = new A1QSystem.Commands.LogOutCommandHandler(() => CompleteOrder(), canExecute));
            }
        }

       
        public ICommand AddNewRowCommand
        {
            get
            {
                return _addNewRowCommand ?? (_addNewRowCommand = new LogOutCommandHandler(() => AddNewRow(), true));
            }
        }

        private void TickAllCheckCommand()
        {
            if(TickAll)
            {
                DidNotCompleteChecked = false;
            }
        }

        private void DidNotComCommand()
        {
            if (DidNotCompleteChecked)
            {
                TickAll = false;
                if(MachineWorkDescription != null && MachineWorkDescription.Count > 0)
                {
                    foreach (var item in MachineWorkDescription)
                    {
                        item.IsCompleted = false;
                    }
                }
            }
        }

        public ObservableCollection<FileUpload> FileUploadedList
        {
            get
            {
                return _fileUploadedList;
            }
            set
            {
                _fileUploadedList = value;
                RaisePropertyChanged(() => this.FileUploadedList);
            }
        }

        private ICommand _tickAllCommand;
        public ICommand TickAllCommand
        {
            get
            {
                return _tickAllCommand ?? (_tickAllCommand = new A1QSystem.Commands.LogOutCommandHandler(() => TickAllCheckCommand(), canExecute));
            }
        }

        private ICommand _didNotCompleteCommand;
        public ICommand DidNotCompleteCommand
        {
            get
            {
                return _didNotCompleteCommand ?? (_didNotCompleteCommand = new A1QSystem.Commands.LogOutCommandHandler(() => DidNotComCommand(), canExecute));
            }
        }


        public ICommand ProblemCommand
        {
            get
            {
                if (_problemCommand == null)
                {
                    _problemCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, CreateRepairWorkOrder);
                }
                return _problemCommand;
            }
        }

        public ICommand UploadDocumentCommand
        {
            get
            {
                if (_uploadDocumentCommand == null)
                {
                    _uploadDocumentCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, UploadDocuments);
                }
                return _uploadDocumentCommand;
            }
        }

        public Microsoft.Practices.Prism.Commands.DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }


        public ICommand RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, RemoveRecod);
                }
                return _removeCommand;
            }
        }

        #endregion
    }
}
