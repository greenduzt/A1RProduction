using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production
{
    public class OrderProductionDetails
    {
        public int OrderProdDetailsId { get; set; }
        public int OrderProdId { get; set; }
        public Product Product { get; set; }
        public decimal Qty { get; set; }

    }
}
