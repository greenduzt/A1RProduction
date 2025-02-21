using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Vehicles
{
    public class VehicleWorkOrderDetailsHistory : VehicleWorkDescription
    {
        public string VehicleRepairString { get; set; }

        public bool _partsOrded;
        public bool PartsOrded
        {
            get
            {
                return _partsOrded;
            }
            set
            {
                _partsOrded = value;
                RaisePropertyChanged(() => this.PartsOrded);
                
            }
        }
    }
}
