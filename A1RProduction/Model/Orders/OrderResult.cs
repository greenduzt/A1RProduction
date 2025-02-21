using A1QSystem.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Orders
{
    public class OrderResult
    {
        public DateTime Date { get; set; }
        public string ProdStartDate { get; set; }
        public string ShiftName { get; set; }
        public RawProduct RawProduct { get; set; }
    }
}
