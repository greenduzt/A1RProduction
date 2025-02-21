using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Stock
{
    public class Stock
    {
        public int StockID { get; set; }
        public int ProductID { get; set; }
        public decimal StockInHand { get; set; }
        public decimal ReOrderQty { get; set; }
        public string ProductLocation { get; set; }
        public string StockedDate { get; set; }
        public string Comments { get; set; }    
    }
}
