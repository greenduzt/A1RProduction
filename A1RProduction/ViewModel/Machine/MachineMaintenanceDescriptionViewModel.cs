using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Machine
{
    public class MachineMaintenanceDescriptionViewModel : ViewModelBase
    {
        private string userName;
        private int location;
        public MachineMaintenanceInfo SelectedItem { get; set; }
        private ObservableCollection<Machines> _machines;
        private ObservableCollection<MachineMaintenanceInfo> _machineMaintenanceInfo;
        private int _itemCount, maintenanceFrequency, _selectedMachineID, _noOfDays, _providerID;
        private bool canExecute;
        private string _location, _repetitionVisiblity, _addItemBackground, _orderType;
        private bool _addItemEnabled;
        private bool _radioEnabled;
        private bool _dailyChecked;
        private bool _weeklyChecked;
        private bool _oneMonthChecked;
        private bool _sixMonthChecked;
        private bool _oneYearChecked;
        private bool _monChecked;
        private bool _tueChecked;
        private bool _wedChecked;
        private bool _thusChecked;
        private bool _friChecked;
        public event Action<int> Closed;
        private Microsoft.Practices.Prism.Commands.DelegateCommand _closeCommand;
        private ICommand _addItemCommand;
        private ICommand _updateCommand;

        public MachineMaintenanceDescriptionViewModel(int loc, string un, int machineId,string ot,int pid,string noOfDays)
        {
            OrderType = ot;
            ProviderID = pid;

            NoOfDays = noOfDays.Equals("Select") ? 0 : Convert.ToInt16(noOfDays);
            userName = un;
            location = loc;
            Location = GetLocationByID(location);
            MachineMaintenanceInfo = new ObservableCollection<MachineMaintenanceInfo>();
            DeleteItem = new DeleteMachineDes(this);
            _closeCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(CloseForm);
            RepetitionVisiblity = "Collapsed";
            AddItemEnabled = false;
            AddItemBackground = "#FFDEDEDE";
            RadioEnabled = false;
            canExecute = true;
            LoadMachines();
            SelectedMachineID = machineId;

            MachineMaintenanceInfo.CollectionChanged += OnMachineListChanged;
            this.MachineMaintenanceInfo = SequencingService.SetCollectionSequence(this.MachineMaintenanceInfo);
        }

        void OnMachineListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Update item count
            this.ItemCount = this.MachineMaintenanceInfo.Count;

            // Resequence list
            SequencingService.SetCollectionSequence(this.MachineMaintenanceInfo);
        }

        private void LoadMachines()
        {
            Machines = new ObservableCollection<Machines>();
            Machines = DBAccess.GetMachinesByLocation(location);
            Machines.Add(new Machines(0) { MachineID=0,MachineName="Select"});
            SelectedMachineID = 0;
        }

        private void AddNewItem()
        {           

            if (MachineMaintenanceInfo.Count > 0)
            {
                MachineMaintenanceInfo v = MachineMaintenanceInfo.Last();
                int vmsId = v.ID;

                MachineMaintenanceInfo vd = MachineMaintenanceInfo.Last();
                if (!string.IsNullOrWhiteSpace(vd.MachineDescription))
                {
                    MachineMaintenanceInfo.Add(new MachineMaintenanceInfo() { ID = vmsId, Machine = new Machines(0) { MachineID = SelectedMachineID }, MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = maintenanceFrequency },Repetition=string.Empty,OrderType =OrderType,ProviderID = ProviderID,ShowOrderBeforeDays = NoOfDays });
                }
                else
                {
                    Msg.Show("Please enter maintenance description for Item No " + vd.SequenceNumber, "", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
                }
            }
            else
            {
                MachineMaintenanceInfo.Add(new MachineMaintenanceInfo()
                {                   
                    //ID = vmsId,                    
                    //Machine = new Machines(0)
                    //{
                    //    MachineID = vcId
                    //}

                    //WorkItemVisible = "Visible"
                });
            }
        }

        private void UpdateData()
        {
            ObservableCollection<MachineMaintenanceInfo> newList = ReArrangeData();
            //if(newList.Count > 0)
            //{
                //Update maintenance info
                //int res = DBAccess.UpdateMachineMaintenanceInfo(SelectedMachineID, userName, newList);
                //if (res > 0)
                //{
                //    MachineMaintenanceInfo.Clear();
                //    SelectedMachineID = 0;
                //    DailyChecked = false;
                //    OneMonthChecked = false;
                //    WeeklyChecked = false;
                //    SixMonthChecked = false;
                //    OneYearChecked = false;
                //    RepetitionVisiblity = "Collapsed";
                //    Msg.Show("Details have been updated succesfully", "Details Updated", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.OK);
                //}
                //else
                //{
                //    Msg.Show("No changes were made", "No Changes Made", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
                //}
            //}            
        }

        private ObservableCollection<MachineMaintenanceInfo> ReArrangeData()
        {
            int lastCodeId = 0;
            MachineMaintenanceInfo v = new MachineMaintenanceInfo();
            ObservableCollection<MachineMaintenanceInfo> mmiNewList = new ObservableCollection<MachineMaintenanceInfo>();
            ObservableCollection<MachineMaintenanceInfo> mmiList = DBAccess.GetMachineMaintenanceInfoMachine(SelectedMachineID);

            if (DailyChecked)
            {
                foreach (var item in MachineMaintenanceInfo)
                {
                    if (!string.IsNullOrWhiteSpace(item.MachineDescription))
                    {
                        item.Machine = new Machines(0) { MachineID = SelectedMachineID };
                        item.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = maintenanceFrequency };
                        item.MachineCode = "M" + item.SequenceNumber;
                        mmiNewList.Add(item);
                    }
                }

                lastCodeId = mmiNewList.Count();
                foreach (var item in mmiList)
                {
                    if (item.MachineMaintenanceFrequency.ID > 1 && item.Machine.MachineID == SelectedMachineID)
                    {

                        lastCodeId += 1;
                        item.MachineCode = "M" + lastCodeId;
                        mmiNewList.Add(item);
                    }
                }
            }
            else if (WeeklyChecked)
            {
                foreach (var item in mmiList)
                {
                    if (item.MachineMaintenanceFrequency.ID == 1 && item.Machine.MachineID == SelectedMachineID)
                    {
                        mmiNewList.Add(item);
                    }
                }
                lastCodeId = mmiNewList.Count();

                foreach (var item in MachineMaintenanceInfo)
                {
                    if (!string.IsNullOrWhiteSpace(item.MachineDescription))
                    {
                        lastCodeId += 1;
                        item.Machine = new Machines(0) { MachineID = SelectedMachineID };
                        item.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = maintenanceFrequency };
                        item.MachineCode = "M" + lastCodeId;
                        item.Repetition = string.Empty;
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
                        mmiNewList.Add(item);
                    }
                }
                lastCodeId = mmiNewList.Count();

                foreach (var item in mmiList)
                {
                    if (item.MachineMaintenanceFrequency.ID >= 3 && item.Machine.MachineID == SelectedMachineID)
                    {
                        lastCodeId += 1;
                        item.MachineCode = "M" + lastCodeId;
                        mmiNewList.Add(item);
                    }
                }
            }
            else if (OneMonthChecked)
            {
                foreach (var item in mmiList)
                {
                    if ((item.MachineMaintenanceFrequency.ID >= 1 && item.MachineMaintenanceFrequency.ID <= 2) && item.Machine.MachineID == SelectedMachineID)
                    {
                        mmiNewList.Add(item);
                    }
                }
                lastCodeId = mmiNewList.Count();
                foreach (var item in MachineMaintenanceInfo)
                {
                    if (!string.IsNullOrWhiteSpace(item.MachineDescription))
                    {
                        item.Machine = new Machines(0) { MachineID = SelectedMachineID };
                        item.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = maintenanceFrequency };
                        lastCodeId += 1;
                        item.MachineCode = "M" + lastCodeId;
                        mmiNewList.Add(item);
                    }
                }
                lastCodeId = mmiNewList.Count();
                foreach (var item in mmiList)
                {
                    if (item.MachineMaintenanceFrequency.ID > 3 && item.Machine.MachineID == SelectedMachineID)
                    {
                        mmiNewList.Add(item);
                    }
                }
            }
            else if (SixMonthChecked)
            {
                foreach (var item in mmiList)
                {
                    if ((item.MachineMaintenanceFrequency.ID >= 1 && item.MachineMaintenanceFrequency.ID <= 3) && item.Machine.MachineID == SelectedMachineID)
                    {
                        mmiNewList.Add(item);
                    }
                }
                lastCodeId = mmiNewList.Count();
                foreach (var item in MachineMaintenanceInfo)
                {
                    if (!string.IsNullOrWhiteSpace(item.MachineDescription))
                    {
                        item.Machine = new Machines(0) { MachineID = SelectedMachineID };
                        item.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = maintenanceFrequency };
                        lastCodeId += 1;
                        item.MachineCode = "M" + lastCodeId;
                        mmiNewList.Add(item);
                    }
                }
                lastCodeId = mmiNewList.Count();
                foreach (var item in mmiList)
                {
                    if (item.MachineMaintenanceFrequency.ID > 4 && item.Machine.MachineID == SelectedMachineID)
                    {
                        mmiNewList.Add(item);
                    }
                }
            }
            else if (OneYearChecked)
            {
                foreach (var item in mmiList)
                {
                    if ((item.MachineMaintenanceFrequency.ID >= 1 && item.MachineMaintenanceFrequency.ID <= 4) && item.Machine.MachineID == SelectedMachineID)
                    {
                        mmiNewList.Add(item);
                    }
                }
                lastCodeId = mmiNewList.Count();
                foreach (var item in MachineMaintenanceInfo)
                {
                    if (!string.IsNullOrWhiteSpace(item.MachineDescription))
                    {
                        item.Machine = new Machines(0) { MachineID = SelectedMachineID };
                        item.MachineMaintenanceFrequency = new MachineMaintenanceFrequency() { ID = maintenanceFrequency };
                        lastCodeId += 1;
                        item.MachineCode = "M" + lastCodeId;
                        mmiNewList.Add(item);
                    }
                }
                lastCodeId = mmiNewList.Count();
                foreach (var item in mmiList)
                {
                    if (item.MachineMaintenanceFrequency.ID > 5 && item.Machine.MachineID == SelectedMachineID)
                    {
                        mmiNewList.Add(item);
                    }
                }
            }

            return mmiNewList;
        }


        private string GetLocationByID(int i)
        {
            string name = string.Empty;

            switch (i)
            {
                case 1: name = "QLD";
                    break;
                case 2: name = "NSW";
                    break;
            }

            return name;
        }

        private void LoadMaintenanceInfo(int num)
        {
            if (num > 0)
            {
                MachineMaintenanceInfo.Clear();

                //ObservableCollection<MachineMaintenanceInfo> localColl = DBAccess.GetMachineMaintenanceInfoBySequence(SelectedMachineID,num);

                //foreach (var item in localColl)
                //{
                //    MachineMaintenanceInfo.Add(item);
                //}                              

                MachineMaintenanceInfo.CollectionChanged += OnMachineListChanged;
            }
            else
            {
                Msg.Show("Cannot load machine sequence id. Please try again later ", "Cannot Load Machine Sequence Id", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                int r = 1;
                Closed(r);
            }
        }

        #region PUBLIC_PROPERTIES

        

         public string OrderType
        {
            get
            {
                return _orderType;
            }
            set
            {
                _orderType = value;
                RaisePropertyChanged(() => this.OrderType);
            }
        }

        public string Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
                RaisePropertyChanged(() => this.Location);
            }
        }

        

        public int ProviderID
        {
            get { return _providerID; }

            set
            {
                _providerID = value;
                base.RaisePropertyChanged(() => this.ProviderID);
            }
        }

        public int NoOfDays
        {
            get { return _noOfDays; }

            set
            {
                _noOfDays = value;
                base.RaisePropertyChanged(() => this.NoOfDays);
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

        public ObservableCollection<MachineMaintenanceInfo> MachineMaintenanceInfo
        {
            get
            {
                return _machineMaintenanceInfo;
            }
            set
            {
                _machineMaintenanceInfo = value;
                RaisePropertyChanged(() => this.MachineMaintenanceInfo);
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


        public bool DailyChecked
        {
            get
            {
                return _dailyChecked;
            }
            set
            {
                _dailyChecked = value;
                RaisePropertyChanged(() => this.DailyChecked);
                if (DailyChecked == true)
                {
                    maintenanceFrequency = 1;
                    LoadMaintenanceInfo(1);
                    RepetitionVisiblity = "Collapsed";
                }
            }
        }

        public bool WeeklyChecked
        {
            get
            {
                return _weeklyChecked;
            }
            set
            {
                _weeklyChecked = value;
                RaisePropertyChanged(() => this.WeeklyChecked);
                if (WeeklyChecked == true)
                {
                    maintenanceFrequency = 2;
                    LoadMaintenanceInfo(2);
                    RepetitionVisiblity = "Visible";
                }
            }
        }

        public bool OneMonthChecked
        {
            get
            {
                return _oneMonthChecked;
            }
            set
            {
                _oneMonthChecked = value;
                RaisePropertyChanged(() => this.OneMonthChecked);
                if (OneMonthChecked == true)
                {
                    maintenanceFrequency = 3;
                    LoadMaintenanceInfo(3);
                    RepetitionVisiblity = "Collapsed";
                }
            }
        }

        public bool SixMonthChecked
        {
            get
            {
                return _sixMonthChecked;
            }
            set
            {
                _sixMonthChecked = value;
                RaisePropertyChanged(() => this.SixMonthChecked);
                if (SixMonthChecked == true)
                {
                    maintenanceFrequency = 4;
                    LoadMaintenanceInfo(4);
                    RepetitionVisiblity = "Collapsed";
                }
            }
        }

        public bool OneYearChecked
        {
            get
            {
                return _oneYearChecked;
            }
            set
            {
                _oneYearChecked = value;
                RaisePropertyChanged(() => this.OneYearChecked);
                if (OneYearChecked == true)
                {
                    maintenanceFrequency = 5;
                    LoadMaintenanceInfo(5);
                    RepetitionVisiblity = "Collapsed";
                }
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

                MachineMaintenanceInfo.Clear();
                DailyChecked = false;
                OneMonthChecked = false;
                WeeklyChecked = false;
                SixMonthChecked = false;
                OneYearChecked = false;
                RepetitionVisiblity = "Collapsed";

                if(SelectedMachineID > 0)
                {
                    RadioEnabled = true;
                    DailyChecked = true;
                    AddItemBackground = "#2E8856";
                    AddItemEnabled = true;
                }
                else
                {
                    RadioEnabled = false;
                    AddItemEnabled = false;
                    AddItemBackground = "#FFDEDEDE";
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
        public bool RadioEnabled
        {
            get
            {
                return _radioEnabled;
            }
            set
            {
                _radioEnabled = value;
                RaisePropertyChanged(() => this.RadioEnabled);               
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

        public bool MonChecked
        {
            get
            {
                return _monChecked;
            }
            set
            {
                _monChecked = value;
                RaisePropertyChanged(() => this.MonChecked);
            }
        }

        public bool TueChecked
        {
            get
            {
                return _tueChecked;
            }
            set
            {
                _tueChecked = value;
                RaisePropertyChanged(() => this.TueChecked);
            }
        }

        public bool WedChecked
        {
            get
            {
                return _wedChecked;
            }
            set
            {
                _wedChecked = value;
                RaisePropertyChanged(() => this.WedChecked);
            }
        }

        public bool ThusChecked
        {
            get
            {
                return _thusChecked;
            }
            set
            {
                _thusChecked = value;
                RaisePropertyChanged(() => this.ThusChecked);
            }
        }

        public bool FriChecked
        {
            get
            {
                return _friChecked;
            }
            set
            {
                _friChecked = value;
                RaisePropertyChanged(() => this.FriChecked);
            }
        }
        
        
        #endregion

        #region COMMANDS

        public ICommand DeleteItem { get; set; }

        public Microsoft.Practices.Prism.Commands.DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ICommand AddItemCommand
        {
            get
            {
                return _addItemCommand ?? (_addItemCommand = new A1QSystem.Commands.LogOutCommandHandler(() => AddNewItem(), canExecute));
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                return _updateCommand ?? (_updateCommand = new A1QSystem.Commands.LogOutCommandHandler(() => UpdateData(), canExecute));
            }
        }


        #endregion
    }
}
