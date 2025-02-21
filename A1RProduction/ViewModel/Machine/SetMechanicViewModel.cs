using A1QSystem.Core;
using A1QSystem.Core.Enumerations;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using A1QSystem.Model.Users;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Machine
{
    public class SetMechanicViewModel : ViewModelBase
    {
        private List<UserPosition> _userPositions;
        private MachineWorkOrder machineWorkOrder;
        private string _selectedMechanic;
        private bool canExecute;
        public bool isSubmiited;
        private UserPosition _selectedUserPosition;
        public event Action<SetMechanicViewModel> Closed;
        private DelegateCommand _closeCommand;
        private ICommand _submitCommand;

        public SetMechanicViewModel(MachineWorkOrder mwo)
        {
            UserPositions = new List<UserPosition>();
            _closeCommand = new DelegateCommand(CloseForm);
            canExecute = true;
            LoadUserPositions();
            isSubmiited = false;
            machineWorkOrder = mwo;
            if (!string.IsNullOrWhiteSpace(machineWorkOrder.User.FullName))
            {
                SelectedMechanic = machineWorkOrder.User.FullName;
            }
            else
            {
                SelectedMechanic = "Select";
            }
        }

        private void LoadUserPositions()
        {
            UserPositions = DBAccess.GetAllUserPositions("MachineMechanic");
            UserPositions.Add(new UserPosition() { FullName = "Select" });
        }

        private void SubmitDetails()
        {
            if (SelectedMechanic == "Select")
            {
                Msg.Show("Please select mechanic's name", "Select Mechanic Name", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else
            {
                SelectedUserPosition = UserPositions.Single(s => s.FullName == SelectedMechanic);

                isSubmiited = true;
                CloseForm();
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {

                Exception exception = null;
                BackgroundWorker worker = new BackgroundWorker();
                ChildWindowView LoadingScreen;
                LoadingScreen = new ChildWindowView();
                LoadingScreen.ShowWaitingScreen("Printing");

                worker.DoWork += (_, __) =>
                {                    
                    if (SelectedUserPosition != null || SelectedMechanic != "Select" && isSubmiited)
                    {
                        machineWorkOrder.User = new User() { ID = SelectedUserPosition.User.ID, FullName = SelectedMechanic };
                        int res = DBAccess.UpdateMachineWorkOrderUser(machineWorkOrder);
                        if (res > 0)
                        {
                            ObservableCollection<MachineWorkDescription> machineWorkDescription = new ObservableCollection<MachineWorkDescription>();
                            if (machineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Maintenance.ToString())
                            {
                                machineWorkDescription = DBAccess.GetMachineWorkDescriptionByID(machineWorkOrder.WorkOrderNo,false);
                                if (machineWorkDescription.Count > 0)
                                {
                                    ObservableCollection<MachineRepairDescription> machineRepairDescriptionList = DBAccess.GetMachineRepairDescriptionByID(machineWorkDescription);
                                    ObservableCollection<MachineParts> machinePartsList = DBAccess.GetMachinePartsDescriptionByID(machineRepairDescriptionList);

                                    foreach (var item in machineWorkDescription)
                                    {
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
                            else if (machineWorkOrder.WorkOrderType == VehicleWorkOrderTypesEnum.Repair.ToString())
                            {
                                ObservableCollection<MachineRepairDescription> machineRepairDescriptionList = DBAccess.GetMachineRepairDescriptionByID2(machineWorkOrder.WorkOrderNo);
                                ObservableCollection<MachineParts> vehiclePartsList = DBAccess.GetMachinePartsDescriptionByID(machineRepairDescriptionList);
                                Int32 id = 0;
                                foreach (var item in machineRepairDescriptionList)
                                {
                                    id = item.MachineWorkDescriptionID;
                                    break;
                                }

                                machineWorkDescription = DBAccess.GetMachineWorkDescriptionForRepair(id);

                                foreach (var item in machineRepairDescriptionList)
                                {
                                    item.MachineParts = new ObservableCollection<MachineParts>();
                                    foreach (var items in vehiclePartsList)
                                    {
                                        if (item.ID == items.MachineRepairID)
                                        {
                                            item.MachineParts.Add(items);
                                        }
                                    }
                                }
                                if (machineRepairDescriptionList.Count > 0 && machineWorkDescription.Count > 0)
                                {
                                    machineWorkDescription[0].MachineRepairDescription = machineRepairDescriptionList;
                                }
                                else
                                {
                                    machineWorkDescription.Add(new MachineWorkDescription() { MachineRepairDescription = machineRepairDescriptionList });
                                }
                            }
                            MachineWorkOrderPDF vwopdf = new MachineWorkOrderPDF(machineWorkOrder, machineWorkDescription);
                            exception = vwopdf.createWorkOrderPDF();
                        }
                    }
                    Closed(this);
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

        #region PUBLIC_PROPERTIES
        public string SelectedMechanic
        {
            get { return _selectedMechanic; }
            set
            {
                _selectedMechanic = value;
                RaisePropertyChanged(() => this.SelectedMechanic);
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

        public UserPosition SelectedUserPosition
        {
            get
            {
                return _selectedUserPosition;
            }
            set
            {
                _selectedUserPosition = value;
                RaisePropertyChanged(() => this.SelectedUserPosition);
            }
        }

        #endregion

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public ICommand SubmitCommand
        {
            get
            {
                return _submitCommand ?? (_submitCommand = new A1QSystem.Commands.LogOutCommandHandler(() => SubmitDetails(), canExecute));
            }
        }
    }
}
