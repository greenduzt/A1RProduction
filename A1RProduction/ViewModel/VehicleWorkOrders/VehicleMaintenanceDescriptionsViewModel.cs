using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Vehicles;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.VehicleWorkOrders
{
    public class VehicleMaintenanceDescriptionsViewModel : ViewModelBase
    {
        private string _location;
        private string _vehicleType;
        private int  location;
        private int vehicleType;
        public VehicleMaintenanceInfo SelectedItem { get; set; }
        private ObservableCollection<VehicleMaintenanceInfo> _vehicleMaintenanceInfo;
        private int maintenanceFrequency;
        private bool _oneMonthChecked;
        private bool _sixMonthChecked;
        private bool _oneYearChecked;
        private bool _twoYearsChecked;
        private string _radioVisibility;
        private string userName;
        private int _itemCount;
        private bool canExecute;

        public event Action<int> Closed;
        private DelegateCommand _closeCommand;
        private ICommand _addItemCommand;
        private ICommand _updateCommand;

        public VehicleMaintenanceDescriptionsViewModel(int loc, int vt, string un)
        {
            userName = un;
            canExecute = true;
            location = loc;
            vehicleType = vt;
            VehicleMaintenanceInfo = new ObservableCollection<VehicleMaintenanceInfo>();            
            DeleteItem = new A1QSystem.Commands.DeleteVehicleMaintenanceDes(this);
            _closeCommand = new DelegateCommand(CloseForm);

            VehicleType = GetVehicleTypeByID(vehicleType);
            Location = GetLocationByID(location);
            OneMonthChecked = true;

            if(loc == 1)
            {
                RadioVisibility = "Visible";
            }
            else if (loc == 2)
            {
                RadioVisibility = "Collapsed";
            }

            VehicleMaintenanceInfo.CollectionChanged += OnVehicleRepairListChanged;
            this.VehicleMaintenanceInfo = SequencingService.SetCollectionSequence(this.VehicleMaintenanceInfo);
        }

        void OnVehicleRepairListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Update item count
            this.ItemCount = this.VehicleMaintenanceInfo.Count; 

            // Resequence list
            SequencingService.SetCollectionSequence(this.VehicleMaintenanceInfo);
        }


        private void CloseForm()
        {
            if (Closed != null)
            {
                int r = 1;
                Closed(r);
            }
        }


        private void LoadMaintenanceInfo(int num)
        {
            if (num > 0)
            {                
                    VehicleMaintenanceInfo.Clear();

                    ObservableCollection<VehicleMaintenanceInfo> localColl = DBAccess.GetMaintenanceInfoBySequence(num, vehicleType, location);

                    foreach (var item in localColl)
                    {
                        VehicleMaintenanceInfo.Add(item);
                    }
                       
                    VehicleMaintenanceInfo.CollectionChanged += OnVehicleRepairListChanged;
            }
            else
            {
                Msg.Show("Cannot load vehicle sequence id. Please try again later ", "Cannot Load Vehicle Sequence Id", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
        }

        private void UpdateData()
        {
            ObservableCollection<VehicleMaintenanceInfo> newList = ReArrangeData();

            if (newList.Count > 0)
            {
                int res = DBAccess.UpdateVehicleMaintenanceInfo(location, maintenanceFrequency, userName, newList);
                if (res > 0)
                {
                    Msg.Show("Data has been updated successfully", "Data Updated", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.OK);
                }
                else
                {
                    Msg.Show("There has been a problem while updating details." + System.Environment.NewLine + "Please try again later ", "Cannot Update Details", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
                }
            }
            else
            {
                Msg.Show("There has been a problem while re-creating the collection." + System.Environment.NewLine + "Please try again later ", "Cannot Update Details", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
            }
            CloseForm();
        }

        private ObservableCollection<VehicleMaintenanceInfo> ReArrangeData()
        {
            ObservableCollection<VehicleMaintenanceInfo> vmiNewList = new ObservableCollection<VehicleMaintenanceInfo>();
            ObservableCollection<VehicleMaintenanceInfo> vmiList = DBAccess.GetMaintenanceInfoByLocation(location);

            
                foreach (var item in VehicleMaintenanceInfo)
                {
                    if (!string.IsNullOrWhiteSpace(item.Description))
                    {
                        item.Code = "M" + item.SequenceNumber;
                        vmiNewList.Add(item);
                    }
                }
                VehicleMaintenanceInfo v = vmiNewList.Last();
                int lastCodeId = v.SequenceNumber;

                foreach (var item in vmiList)
                {
                    if(OneMonthChecked)
                    {
                        if (item.VehicleMaintenanceSequence.ID > 2)
                        {
                            lastCodeId += 1;
                            item.Code = "M"+lastCodeId;
                            vmiNewList.Add(item);
                        }
                    }
                    else if (SixMonthChecked)
                    {
                        if (item.VehicleMaintenanceSequence.ID > 3)
                        {
                            lastCodeId += 1;
                            item.Code = "M" + lastCodeId;
                            vmiNewList.Add(item);
                        }
                    }
                    else if (OneYearChecked)
                    {
                        if (item.VehicleMaintenanceSequence.ID > 4)
                        {
                            lastCodeId += 1;
                            item.Code = "M" + lastCodeId;
                            vmiNewList.Add(item);
                        }
                    }
                    else if (TwoYearsChecked)
                    {
                        if (item.VehicleMaintenanceSequence.ID > 5)
                        {
                            lastCodeId += 1;
                            item.Code = "M" + lastCodeId;
                            vmiNewList.Add(item);
                        }
                    }
                }           
          

            return vmiNewList;
        }

        private string GetVehicleTypeByID(int i)
        {
            string name = string.Empty;

            switch (i)
            {
                case 1: name = "Forklift";
                    break;
                case 2: name = "Truck";
                    break;
                case 3: name = "Ute";
                    break;
                case 4: name = "Car";
                    break;
                case 5: name = "Sweeper";
                    break;
                case 6: name = "Buggy";
                    break;
                case 7: name = "Caddy";
                    break;
               
            }

            return name;
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

        private void AddNewItem()
        {
            VehicleMaintenanceInfo v = VehicleMaintenanceInfo.Last();
           
            int vmsId = v.VehicleMaintenanceSequence.ID;
            int vcId = v.VehicleCategory.ID;

            if (VehicleMaintenanceInfo.Count > 0)
            {
                VehicleMaintenanceInfo vd = VehicleMaintenanceInfo.Last();
                if (!string.IsNullOrWhiteSpace(vd.Description))
                {
                    VehicleMaintenanceInfo.Add(new VehicleMaintenanceInfo() { VehicleMaintenanceSequence = new VehicleMaintenanceSequence() { ID = vmsId }, VehicleCategory = new VehicleCategory() { ID = vcId }, WorkItemVisible = "Visible" });
                }
                else
                {
                    Msg.Show("Please enter maintenance description for Item No " + vd.SequenceNumber, "", MsgBoxButtons.OK, MsgBoxImage.Information_Orange, MsgBoxResult.Yes);
                }
            }
            else
            {
                VehicleMaintenanceInfo.Add(new VehicleMaintenanceInfo() { 
                                                                            VehicleMaintenanceSequence = new VehicleMaintenanceSequence() 
                                                                            { 
                                                                                ID = vmsId 
                                                                            }, 
                                                                            VehicleCategory = new VehicleCategory() 
                                                                            {
                                                                                ID = vcId 
                                                                            },
                                                                        
                                                                            WorkItemVisible = "Visible" 
                                                                       });
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

        public string RadioVisibility
        {
            get
            {
                return _radioVisibility;
            }
            set
            {
                _radioVisibility = value;
                RaisePropertyChanged(() => this.RadioVisibility);               
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

        public string VehicleType
        {
            get
            {
                return _vehicleType;
            }
            set
            {
                _vehicleType = value;
                RaisePropertyChanged(() => this.VehicleType);
            }
        }

        public ObservableCollection<VehicleMaintenanceInfo> VehicleMaintenanceInfo
        {
            get
            {
                return _vehicleMaintenanceInfo;
            }
            set
            {
                _vehicleMaintenanceInfo = value;
                RaisePropertyChanged(() => this.VehicleMaintenanceInfo);
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
                    LoadMaintenanceInfo(2);
                    maintenanceFrequency = 2;
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
                    LoadMaintenanceInfo(3);
                    maintenanceFrequency = 3;
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
                    LoadMaintenanceInfo(4);
                    maintenanceFrequency = 4;
                }
            }
        }

        public bool TwoYearsChecked
        {
            get
            {
                return _twoYearsChecked;
            }
            set
            {
                _twoYearsChecked = value;
                RaisePropertyChanged(() => this.TwoYearsChecked);
                if (TwoYearsChecked == true)
                {
                    LoadMaintenanceInfo(5);
                    maintenanceFrequency = 5;
                }
            }
        }

        public ICommand DeleteItem { get; set; }

        public DelegateCommand CloseCommand
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



        
    }
}
