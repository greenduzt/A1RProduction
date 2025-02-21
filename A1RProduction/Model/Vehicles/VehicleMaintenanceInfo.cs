using A1QSystem.Core;
using A1QSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Vehicles
{
    public class VehicleMaintenanceInfo : ViewModelBase, ISequencedObject
    {
        public int ID { get; set; }
        public VehicleMaintenanceSequence VehicleMaintenanceSequence { get; set; }
        public VehicleCategory VehicleCategory { get; set; }
        //public string Code { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public int LocationID { get; set; }
        private string _code;
        private string _workItemVisible;
        private int p_SequenceNumber;
        private string _sequenceStr;
        private bool _itemDone;

        public string WorkItemVisible
        {
            get { return _workItemVisible; }

            set
            {
                _workItemVisible = value;
                base.RaisePropertyChanged(() => this.WorkItemVisible);
            }
        }

        public string SequenceStr
        {
            get { return _sequenceStr; }

            set
            {
                _sequenceStr = value;
                base.RaisePropertyChanged(() => this.SequenceStr);
            }
        }

        public string Code
        {
            get { return _code; }

            set
            {
                _code = value;
                base.RaisePropertyChanged(() => this.Code);
                if(!string.IsNullOrWhiteSpace(Code))
                {
                    if(Code == "Select")
                    {
                        WorkItemVisible = "Collapsed";
                    }
                    else
                    {
                        WorkItemVisible = "Visible";
                    }
                }
            }
        }

        public int SequenceNumber
        {
            get { return p_SequenceNumber; }

            set
            {
                p_SequenceNumber = value;
                base.RaisePropertyChanged(() => this.SequenceNumber);

                SequenceStr = "M" + SequenceNumber;
            }
        }


        public bool ItemDone
        {
            get { return _itemDone; }

            set
            {
                _itemDone = value;
                base.RaisePropertyChanged(() => this.ItemDone);

            }
        }

        

    }
}
