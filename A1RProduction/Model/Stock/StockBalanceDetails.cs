using A1QSystem.Model.Orders;
using A1QSystem.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Stock
{
    public class StockBalanceDetails
    {
       
        public StockDetails StockDetails { get; set; }
        public OrderDetails OrderDetails { get; set; }
        public RawProduct RawProduct { get; set; }
        public decimal StockInHand { get; set; }
        public decimal QtyToMake { get; set; }
        public string DisplayString { get; set; }
        public string DisplayStrBackColour { get; set; }    

        
    }
}
