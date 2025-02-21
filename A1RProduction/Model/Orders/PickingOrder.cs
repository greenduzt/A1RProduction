using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Orders
{
    public class PickingOrder
    {
        public int PickingID { get; set; }
        public string PickingDate { get; set; }       
        public decimal ReqAmount { get; set; }
        public string Status { get; set; }
        public string StatusCol { get; set; }
       
    }
}
