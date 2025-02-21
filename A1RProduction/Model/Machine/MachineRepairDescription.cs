using A1QSystem.Core;
using A1QSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Machine
{
    public class MachineRepairDescription : ObservableObject, ISequencedObject
    {
        public Int32 ID { get; set; }
        public Int32 MachineWorkDescriptionID { get; set; }
        public Int32 MachineWorkOrderID { get; set; }
        public string RepairDescription { get; set; }
        public bool IsActive { get; set; }

        private bool _isCompleted;
        private bool _partsOrdered;
        private int p_SequenceNumber;
        private string _strSequenceNumber;
        private string _repairVisiblity;
        private string _repairCompletedVisibility;
        private ObservableCollection<MachineParts> _machineparts;

        public MachineRepairDescription()
        {
            MachineParts = new ObservableCollection<MachineParts>();
        }

        public int SequenceNumber
        {
            get { return p_SequenceNumber; }

            set
            {
                p_SequenceNumber = value;
                base.RaisePropertyChanged(() => this.SequenceNumber);
                StrSequenceNumber = "R" + SequenceNumber.ToString();
            }
        }

        public string StrSequenceNumber
        {
            get { return _strSequenceNumber; }

            set
            {
                _strSequenceNumber = value;
                base.RaisePropertyChanged(() => this.StrSequenceNumber);
                if (StrSequenceNumber == "Select")
                {
                    RepairVisiblity = "Collapsed";
                }
                else
                {
                    RepairVisiblity = "Visible";
                }
            }
        }

        public bool PartsOrdered
        {
            get { return _partsOrdered; }

            set
            {
                _partsOrdered = value;
                base.RaisePropertyChanged(() => this.PartsOrdered);
            }
        }

        public string RepairVisiblity
        {
            get { return _repairVisiblity; }

            set
            {
                _repairVisiblity = value;
                RaisePropertyChanged(() => this.RepairVisiblity);
            }
        }



        public ObservableCollection<MachineParts> MachineParts
        {
            get { return _machineparts; }

            set
            {
                _machineparts = value;
                RaisePropertyChanged(() => this.MachineParts);
            }
        }



        public string RepairCompletedVisibility
        {
            get { return _repairCompletedVisibility; }

            set
            {
                _repairCompletedVisibility = value;
                RaisePropertyChanged(() => this.RepairCompletedVisibility);
            }
        }

        public bool IsCompleted
        {
            get { return _isCompleted; }

            set
            {
                _isCompleted = value;
                RaisePropertyChanged(() => this.IsCompleted);
            }
        }
    }
}
