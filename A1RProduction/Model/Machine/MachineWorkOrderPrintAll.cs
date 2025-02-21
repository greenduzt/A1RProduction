using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Machine
{
    public class MachineWorkOrderPrintAll
    {
        public Int32 WorkOrderNo { get; set; }
        public Machines Machine { get; set; }
        public User User { get; set; }
        public int Urgency { get; set; }
        public string WorkOrderType { get; set; }
        public DateTime FirstServiceDate { get; set; }
        public DateTime NextServiceDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string CompletedBy { get; set; }
        public string Status { get; set; }
        public ObservableCollection<MachineWorkDescription> MachineWorkDescription { get; set; }

        
    }
}
