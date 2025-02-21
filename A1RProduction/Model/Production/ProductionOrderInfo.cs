using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production
{
    public class ProductionOrderInfo
    {
        public int ID { get; set; }
        public int SalesOrderID { get; set; }
        public Product Product { get; set; }
        public decimal Qty { get; set; }
        public decimal BlocksLogs { get; set; }
        public int OrderType { get; set; }
    }
}
