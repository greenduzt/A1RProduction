using A1QSystem.ViewModel.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Vehicles
{
    public class Vehicle
    {
        public int ID { get; set; }
        public StockLocation StockLocation { get; set; }
        public VehicleCategory VehicleCategory { get; set; }
        public string SerialNumber { get; set; }
        public string VehicleCode { get; set; }
        public string VehicleBrand { get; set; }
        public string VehicleDescription { get; set; }
        public bool Active { get; set; }
        public string VehicleString { get; set; }
    }
}
