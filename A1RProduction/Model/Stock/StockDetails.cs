using A1QSystem.Model.Products;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Stock
{
    public class StockDetails
    {
        public int StockID { get; set; }
        public Product product { get; set; }
        public RawProduct RawProduct { get; set; }
        public Stock Stock { get; set; }
        //public decimal Qty { get; set; }
        //public decimal ReOrderQty { get; set; }
        //public string ProductLocation { get; set; }
        //public string StockedDate { get; set; }
        //public string Comments { get; set; }
    }
}
