using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Vehicles
{
    public class VehicleMaintenanceDescriptionsCompleted
    {
        public int ID { get; set; }
        public VehicleMaintenanceInfo VehicleMaintenanceInfo { get; set; }
        public Int32 VehicleWorkOrderID { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CompletedDate { get; set; }
    }
}
