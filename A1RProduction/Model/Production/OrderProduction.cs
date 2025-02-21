using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production
{
    public class OrderProduction
    {
        public int OrderProductionId { get; set; }
        public int CustomerId { get; set; }
        public string RequiredDate { get; set; }
        public string OrderCreatedDate { get; set; }

    }
}
