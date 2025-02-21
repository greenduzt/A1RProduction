using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Products.Rolls
{

    public class CustomRollLength
    {
        public int LogNo { get; set; }
        public decimal Qty { get; set; }
        public decimal Length { get; set; }
        public bool NewRowEnabled { get; set; }
        public bool AddRemainderEnabled { get; set; }
    }
}
