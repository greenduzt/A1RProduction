using A1QSystem.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.RawMaterials
{
    public class Curing
    {
        public int id { get; set; }
        public Int64 OrderNo { get; set; }
        public Product Product { get; set; }
        //public RawProduct RawProduct { get; set; }
        public decimal Qty { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCured { get; set; }
        public bool IsEnabled { get; set; }
    }
}
