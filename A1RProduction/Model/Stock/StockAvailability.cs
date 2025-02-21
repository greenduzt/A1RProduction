using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Stock
{
    public class StockAvailability
    {
        public Product Product { get; set; }
        public decimal QtyOriginal { get; set; }
        public decimal QtyInStock { get; set; }
        public decimal BlkLogOriginal { get; set; }
        public decimal BlkLogInStock { get; set; }
        public int OrdertypeId { get; set; }
        public string Type { get; set; }
    }
}
