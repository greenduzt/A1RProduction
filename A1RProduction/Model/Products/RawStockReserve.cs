using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Products
{
    public class RawStockReserve
    {
        public Int32 OrderID { get; set; }
        public Product Product { get; set; }
        public decimal Qty { get; set; }
        public decimal BlocksLogs { get; set; }
    }
}
