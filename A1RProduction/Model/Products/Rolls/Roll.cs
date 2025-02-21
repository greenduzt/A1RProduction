using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Products.Rolls
{
    public class Roll
    {
        public int RollID { get; set; }
        public int BulkRollID { get; set; }
        public int RollProductID { get; set; }
        public string Density { get; set; }
        public decimal Length { get; set; }
        public decimal RollLM { get; set; }
    }
}
