using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Orders.Status
{
    public class GradingStatus
    {
        public Int32 OrderNo { get; set; }
        public Product Product { get; set; }
        public decimal QtyRem { get; set; }
        public decimal QtyComp { get; set; }
    }
}
