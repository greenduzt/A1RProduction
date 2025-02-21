using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model.Machine;
using A1QSystem.Model.Users;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;

namespace A1QSystem.ViewModel.Machine.MachineHistory
{
    public class MachineWorkOrderHistoryItemsViewModel : ViewModelBase
    {
        private List<UserPosition> _userPositions;
        private ObservableCollection<MachineWorkDescription> _machineWorkDescription;
        private MachineWorkOrderHistory _machineWorkOrderHistory;
        private bool _isReadOnly, _isMechanicEnabled;
        private string _partsOrdedVisibility, _repairWONoVisibility, _reasonVisibility, _selectedMechanic, _externalMechanicName, _externalMechanicNameVisibility, _addReviewButtonVisibility;

        public event Action<int> Closed;
        private ICommand _addReviewCommand;
        private Microsoft.Practices.Prism.Commands.DelegateCommand _closeCommand;

        public MachineWorkOrderHistoryItemsViewModel(MachineWorkOrderHistory mwoh)
        {
            MachineWorkOrderHistory = mwoh;
            MachineWorkOrderHistory.MahcineWorkDescription = new ObservableCollection<MachineWorkDescription>();
            UserPositions = new List<UserPosition>();
            ReasonVisibility = "Collapsed";
            ExternalMechanicNameVisibility = "Collapsed";

            if (!string.IsNullOrWhiteSpace(MachineWorkOrderHistory.Reason))
            {
                ReasonVisibility = "Visible";
            }

            if (MachineWorkOrderHistory.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
            {
                RepairWONoVisibility = "Visible";
                PartsOrdedVisibility = "Collapsed";

                MachineWorkDescription = DBAccess.GetAllMachineWorkDescriptionByID(MachineWorkOrderHistory.WorkOrderNo);
                if (MachineWorkDescription.Count > 0)
                {
                    ObservableCollection<MachineRepairDescription> machineRepairDescriptionList = DBAccess.GetMachineRepairDescriptionByID(MachineWorkDescription);
                    ObservableCollection<MachineParts> machinePartsList = DBAccess.GetMachinePartsDescriptionByID(machineRepairDescriptionList);

                    foreach (var item in MachineWorkDescription)
                    {
                        item.MaintenanceCompletedVisibility = "Visible";
                        item.MachineRepairDescription = new ObservableCollection<MachineRepairDescription>();
                        foreach (var items in machineRepairDescriptionList)
                        {
                            if (item.ID == items.MachineWorkDescriptionID)
                            {
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
                }
            }
            else if (MachineWorkOrderHistory.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
            {
                PartsOrdedVisibility = "Visible";
                RepairWONoVisibility = "Collapsed";

                ObservableCollection<MachineRepairDescription> machineRepairDescriptionList = DBAccess.GetMachineRepairDescriptionByID2(MachineWorkOrderHistory.WorkOrderNo);
                ObservableCollection<MachineParts> machinePartsList = DBAccess.GetMachinePartsDescriptionByID(machineRepairDescriptionList);
                Int32 id = 0;
                foreach (var item in machineRepairDescriptionList)
                {
                    id = item.MachineWorkDescriptionID;
                    break;
                }

                MachineWorkDescription = DBAccess.GetMachineWorkDescriptionForRepair(id);

                foreach (var item in machineRepairDescriptionList)
                {
                    item.MachineParts = new ObservableCollection<MachineParts>();
                    foreach (var items in machinePartsList)
                    {
                        if (item.ID == items.MachineRepairID)
                        {
                            item.MachineParts.Add(items);
                        }
                    }
                }
                if (machineRepairDescriptionList.Count > 0 && MachineWorkDescription.Count > 0)
                {
                    MachineWorkDescription[0].MaintenanceCompletedVisibility = "Collapsed";
                    MachineWorkDescription[0].MachineRepairDescription = machineRepairDescriptionList;
                }
                else
                {
                    MachineWorkDescription.Add(new MachineWorkDescription() { MachineMaintenanceInfo = new MachineMaintenanceInfo() { MachineDescription = "Repair order for " + MachineWorkOrderHistory.Machine.MachineName }, MachineRepairDescription = machineRepairDescriptionList, MaintenanceCompletedVisibility = "Collapsed" });
                }
            }

            LoadUserPositions();

            SelectedMechanic = "Select";

            if (!string.IsNullOrEmpty(MachineWorkOrderHistory.Review) )
            {
                IsReadOnly = true;
                IsMechanicEnabled = false;
                AddReviewButtonVisibility = "Collapsed";
                //Check if the user exist in the user list
                var data = UserPositions.SingleOrDefault(x => x.FullName.Equals(MachineWorkOrderHistory.ReviewAddedBy));
                if (data != null)
                {
                    SelectedMechanic = data.FullName;
                    ExternalMechanicNameVisibility = "Collapsed";
                }
                else
                {
                    ExternalMechanicNameVisibility = "Visible";
                    SelectedMechanic = "External";
                    ExternalMechanicName = MachineWorkOrderHistory.ReviewAddedBy;
                }
            }
            else
            {
                if (!MachineWorkOrderHistory.IsCompleted)
                {
                    IsReadOnly = false;
                    IsMechanicEnabled = true;
                    AddReviewButtonVisibility = "Visible";
                }
                else
                {
                    IsReadOnly = true;
                    IsMechanicEnabled = false;
                    AddReviewButtonVisibility = "Collapsed";
                }
            }
            _closeCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(CloseForm);
        }

        private void LoadUserPositions()
        {
            UserPositions = DBAccess.GetAllUserPositions("MachineMechanicManager");
            UserPositions.Insert(0, new UserPosition() { FullName = "Select" });
            UserPositions.Add(new UserPosition() { FullName = "External" });
        }

        private void AddReview()
        {
            bool allCompleted = false;
            bool check = false;

            foreach (var item in MachineWorkDescription)
            {
                if (item.IsCompleted)
                {
                    allCompleted = MachineWorkDescription.All(x => x.IsCompleted);
                    if(allCompleted)
                    {
                        check=false;
                    }
                    else
                    {
                        check = true;
                    }
                    break;
                }
            }


            if (SelectedMechanic == "Select")
            {
                Msg.Show("Please select your name", "Select Mechanic Name", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else if (SelectedMechanic.Equals("External") && string.IsNullOrWhiteSpace(ExternalMechanicName))
            {
                Msg.Show("Please enter external mechanic name", "Enter Mechanic Name", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else if (string.IsNullOrWhiteSpace(MachineWorkOrderHistory.Review))
            {
                Msg.Show("Please enter review", "Review Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else if(check && !allCompleted)
            {
                Msg.Show("Please complete all items for this work order to submit", "Complete All Items", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else
            {
                if (SelectedMechanic.Equals("External"))
                {
                    MachineWorkOrderHistory.ReviewAddedBy = ExternalMechanicName;
                }
                else
                {
                    MachineWorkOrderHistory.ReviewAddedBy = SelectedMechanic;
                }
                MachineWorkOrderHistory.ReviewAddedDateTime = DateTime.Now;               


                int res = DBAccess.UpdateReview(MachineWorkOrderHistory, MachineWorkDescription, allCompleted);
                if (res > 0)
                {
                    Msg.Show("Review added successfully!", "", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.OK);
                }
                else
                {
                    Msg.Show("Could not add review" + System.Environment.NewLine + "Please try again later", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.OK);
                }
                CloseForm();
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed(1);
            }
        }

        #region PUBLIC_PROPERTIES

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
                    ExternalMechanicName = string.Empty;
                }
            }
        }


        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
            set
            {
                _isReadOnly = value;
                RaisePropertyChanged(() => this.IsReadOnly);
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

        public MachineWorkOrderHistory MachineWorkOrderHistory
        {
            get
            {
                return _machineWorkOrderHistory;
            }
            set
            {
                _machineWorkOrderHistory = value;
                RaisePropertyChanged(() => this.MachineWorkOrderHistory);
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

        public string RepairWONoVisibility
        {
            get { return _repairWONoVisibility; }
            set
            {
                _repairWONoVisibility = value;
                RaisePropertyChanged(() => this.RepairWONoVisibility);
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

        public string AddReviewButtonVisibility
        {
            get
            {
                return _addReviewButtonVisibility;
            }
            set
            {
                _addReviewButtonVisibility = value;
                RaisePropertyChanged(() => this.AddReviewButtonVisibility);
            }
        }


        public bool IsMechanicEnabled
        {
            get
            {
                return _isMechanicEnabled;
            }
            set
            {
                _isMechanicEnabled = value;
                RaisePropertyChanged(() => this.IsMechanicEnabled);
            }
        }



        #endregion
        public Microsoft.Practices.Prism.Commands.DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ICommand AddReviewCommand
        {
            get
            {
                return _addReviewCommand ?? (_addReviewCommand = new LogOutCommandHandler(() => AddReview(), true));
            }
        }
    }
}
