using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.FormulaGeneration
{
    public class FormulaItems
    {
        public int ID { get; set; }
        public int BID { get; set; }
        public int ParentID { get; set; }
        public int TreeLevel { get; set; }
        public string FormulaName { get; set; }
    }
}
