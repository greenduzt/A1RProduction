using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Vehicles
{
    public class VehicleWorkOrderHistory : VehicleWorkOrder
    {
        //public DateTime WorkOrderCompletedTime { get; set; }
        public string WorkOrderCompletedBy { get; set; }
        public ObservableCollection<VehicleWorkDescription> VehicleWorkDescription { get; set; }
        //public ObservableCollection<VehicleWorkOrderDetailsHistory> VehicleWorkOrderDetailsHistory { get; set; }
    }
}
