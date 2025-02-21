using A1QSystem.Model.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production
{
    public class ProductionStatus
    {
        public Order Order { get; set; }
        public decimal Pending { get; set; }
        public decimal Completed { get; set; }
    }
}
