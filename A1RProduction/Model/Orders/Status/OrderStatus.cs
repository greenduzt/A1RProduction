using A1QSystem.Model.Production;
using A1QSystem.Model.Production.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Orders.Status
{
    public class OrderStatus
    {
        public Order Order { get; set; }
        public GradingStatus GradingStatus { get; set; }
    }
}
