using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.ViewModel.Sales
{
    public class SalesOrder
    {
        public Int32 ID { get; set; }
        public Int32 CustomerID { get; set; }
        public int FreightID { get; set; }
        public string SalesOrderNo { get; set; }
        public string Comment { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime MixingDate { get; set; }
    }
}
