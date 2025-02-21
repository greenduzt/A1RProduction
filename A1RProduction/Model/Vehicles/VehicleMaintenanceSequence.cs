using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Vehicles
{
    public class VehicleMaintenanceSequence
    {
        public int ID { get; set; }
        public int VehicleCategoryID { get; set; }
        public Int64 Kmhrs { get; set; }
        public string Unit { get; set; }
    }
}
