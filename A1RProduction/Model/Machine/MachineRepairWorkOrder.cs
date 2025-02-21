using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Machine
{
    public class MachineRepairWorkOrder : MachineWorkOrder
    {
        public Int32 MachineWorkDescriptionID { get; set; }
        public Int32 MachineRepairWorkOrderID { get; set; }
        public Int32 MachineMaintenanceInfoID { get; set; }
        public ObservableCollection<MachineRepairDescription> MachineRepairDescription { get; set; }
        public bool IsActive { get; set; }
    }
}
