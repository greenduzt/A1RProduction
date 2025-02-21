using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production.Grading
{
    public class GradingProduction
    {
        public int SalesOrderID { get; set; }
        public Product Product { get; set; }
        public decimal QtyOriginal { get; set; }
        public decimal QtyToDo { get; set; }
        public decimal BlkLogsOriginal { get; set; }
        public int OrderTypeID { get; set; }

    }
}
