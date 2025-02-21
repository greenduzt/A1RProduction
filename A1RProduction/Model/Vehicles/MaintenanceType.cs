using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Vehicles
{
    public class MaintenanceType
    {
        public int ID { get; set; }
        public int VehicleMaintenanceGroupID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public List<VehicleParts> VehicleParts { get; set; }
        public bool MaintenanceActive { get; set; }
    }
}
