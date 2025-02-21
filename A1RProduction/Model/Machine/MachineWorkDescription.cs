using A1QSystem.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Machine
{
    public class MachineWorkDescription : ViewModelBase
    {
        public Int32 ID { get; set; }
        public Int32 MachineWorkOrderNo { get; set; }
        public Int32 MachineRepairWorkOrderNo { get; set; }
        public MachineMaintenanceInfo MachineMaintenanceInfo { get; set; }
        //public string Code { get; set; }
        //public string Description { get; set; }
        private bool _isCompleted;
        public bool Active { get; set; }
        public bool ProblemBtnEnabled { get; set; }
        public bool ItemRepair { get; set; }
        public bool ItemRepairEnabled { get; set; }
        public bool ItemDoneEnabled { get; set; }
        public string NormalVisibility { get; set; }
        public string RepairVisibility { get; set; }
        private bool _result;
        private bool _itemDone;
        private string _completedVisibility;
        private string _repairOrderVisibility;
        private string _rootVisibility;
        private string _maintenanceCompletedVisibility;
        private ObservableCollection<MachineRepairDescription> _machineRepairDescription;
        private bool _tickBoxEnabled;

        public MachineWorkDescription()
        {
            ProblemBtnEnabled = false;
            MachineRepairDescription = new ObservableCollection<MachineRepairDescription>();
        }

        
        public bool TickBoxEnabled
        {
            get
            {
                return _tickBoxEnabled;
            }
            set
            {
                _tickBoxEnabled = value;
                RaisePropertyChanged(() => this.TickBoxEnabled);
            }
        }

        public ObservableCollection<MachineRepairDescription> MachineRepairDescription
        {
            get
            {
                return _machineRepairDescription;
            }
            set
            {
                _machineRepairDescription = value;
                RaisePropertyChanged(() => this.MachineRepairDescription);
            }
        }

        public bool IsCompleted
        {
            get { return _isCompleted; }
            set
            {
                _isCompleted = value;
                RaisePropertyChanged(() => this.IsCompleted);
                if (IsCompleted == true)
                {
                    ItemDone = true;
                }
                else
                {
                    ItemDone = false;
                }
            }
        }

        public bool ItemDone
        {
            get
            {
                return _itemDone;
            }
            set
            {
                _itemDone = value;
                RaisePropertyChanged(() => this.ItemDone);

                if (ItemDone == true)
                {
                    ProblemBtnEnabled = false;
                }
                else if (ItemDone == false && MachineRepairWorkOrderNo == 0)
                {
                    ProblemBtnEnabled = true;
                }
            }
        }

        public bool Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                RaisePropertyChanged(() => this.Result);
            }
        }

        public string CompletedVisibility
        {
            get { return _completedVisibility; }
            set
            {
                _completedVisibility = value;
                RaisePropertyChanged(() => this.CompletedVisibility);
            }
        }

        public string MaintenanceCompletedVisibility
        {
            get { return _maintenanceCompletedVisibility; }
            set
            {
                _maintenanceCompletedVisibility = value;
                RaisePropertyChanged(() => this.MaintenanceCompletedVisibility);
            }
        }

        

        public string RepairOrderVisibility
        {
            get { return _repairOrderVisibility; }
            set
            {
                _repairOrderVisibility = value;
                RaisePropertyChanged(() => this.RepairOrderVisibility);
            }
        }

        public string RootVisibility
        {
            get { return _rootVisibility; }
            set
            {
                _rootVisibility = value;
                RaisePropertyChanged(() => this.RootVisibility);
            }
        }
    }
}
