using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Shreding
{
    public class ShreddingHistory
    {
        public DateTime DateTime { get; set; }
        public int Shift { get; set; }
        public ShredStock ShredStock { get; set; }
        public decimal Total { get; set; }
    }
}
