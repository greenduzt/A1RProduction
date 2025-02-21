using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Products.Rolls
{
    public class StandardRoll : BulkRoll
    {
        public int ID { get; set; }
        public decimal MinCutLength { get; set; }
    }
}
