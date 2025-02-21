using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Orders
{
    public class PendingOrder
    {
        public Int32 ID { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
        public decimal Qty { get; set; }
        public decimal BlockLogQty { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
    }
}
