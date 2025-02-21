using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Formula
{
    public class Formulas
    {
        public int ID { get; set; }
        public int RawProductID { get; set; }
        public int ProductCapacity1 { get; set; }
        public string FormulaName1 { get; set; }
        public int ProductCapacity2 { get; set; }
        public string FormulaName2 { get; set; }
        public decimal GradingWeight1 { get; set; }
        public decimal GradingWeight2 { get; set; }
        public decimal GradingWeight3 { get; set; }
        public int NoOfMixes { get; set; }
        public int MachineID { get; set; }
        public string GradingFormula { get; set; }
        public string MixingFormula { get; set; }
        public string GradingFormulaBtnVisibility { get; set; }
        public string MixingFormulaBtnVisibility { get; set; }

    }
}
