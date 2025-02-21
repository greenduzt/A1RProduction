using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Orders
{
    public class MasterOrder
    {
        public Int32 MasterOrderID { get; set; }
        public bool IsCreated { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
