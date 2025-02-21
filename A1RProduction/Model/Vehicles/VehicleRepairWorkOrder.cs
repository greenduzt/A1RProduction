using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Vehicles
{
    public class VehicleRepairWorkOrder : VehicleWorkOrder
    {
        public Int32 VehicleWorkDescriptionID { get; set; }
        public Int32 VehicleRepairWorkOrderID { get; set; }
        public Int32 VehicleMaintenanceInfoID { get; set; }
        public ObservableCollection<VehicleRepairDescription> VehicleRepairDescription { get; set; }
        //public int Urgency { get; set; }
        public bool IsActive { get; set; }
        //public bool IsCompleted { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public string CreatedBy { get; set; }

    }
}
