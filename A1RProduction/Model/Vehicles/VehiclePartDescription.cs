using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Vehicles
{
    public class VehiclePartDescription
    {
        public Int32 ID { get; set; }
        public Vehicle Vehicle { get; set; }
        public VehicleParts VehicleParts { get; set; }
        public string VehicleSearchString { get; set; }
    }
}
