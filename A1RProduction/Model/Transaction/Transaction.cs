using A1QSystem.Model.Stock;
using System;
using System.Collections.Generic;

namespace A1QSystem.Model.Transaction
{
    public class Transaction
    {
        public DateTime TransDateTime { get; set; }
        public string Transtype { get; set; }
        public Int64 SalesOrderID { get; set; }
        public List<RawStock> Products { get; set; }
        //public int RawProductID { get; set; }
        //public decimal Qty { get; set; }
        public string CreatedBy { get; set; }
    }
}
