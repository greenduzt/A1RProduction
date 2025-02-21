using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Stock
{
    public class GradedStockReserve
    {
        public Int64 ID { get; set; }
        public string  RowID { get; set; }
        public int GradedID { get; set; }
        public decimal ReserveQty { get; set; }
    }
}
