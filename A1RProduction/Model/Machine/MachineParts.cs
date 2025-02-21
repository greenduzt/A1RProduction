using A1QSystem.Core;
using A1QSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Machine
{
    public class MachineParts : ObservableObject, ISequencedObject
    {
        public Int32 MachineRepairID { get; set; }
        public int PartID { get; set; }
        public string PartCode { get; set; }
        public string PartName { get; set; }
        public string PartDescription { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal CostPrice { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string PurchasedBy { get; set; }
        public bool Active { get; set; }
        private string _strSequenceNumber;
        private int _sequenceNumber;

        public int SequenceNumber
        {
            get { return _sequenceNumber; }

            set
            {
                _sequenceNumber = value;
                base.RaisePropertyChanged(() => this.SequenceNumber);
                StrSequenceNumber = "P" + SequenceNumber.ToString();
            }
        }

        public string StrSequenceNumber
        {
            get { return _strSequenceNumber; }

            set
            {
                _strSequenceNumber = value;
                base.RaisePropertyChanged(() => this.StrSequenceNumber);
            }
        }
    }
}
