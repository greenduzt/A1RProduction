using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model
{
    public class RubberProduction
    {
        public int ProFormulaID { get; set; }
        public int ManuProductID { get; set; }
        public string FormulaType { get; set; }
        public string HeaderName { get; set; }
        public string MouldType { get; set; }
        public int NoOfBins { get; set; }
        public int GradingSize12 { get; set; }
        public int GradingSize16 { get; set; }
        public int GradingSize30 { get; set; }
        public int GradingSize3040 { get; set; }
        public int GradingSize1620 { get; set; }
        public int GradingSize12mg { get; set; }
        public int GradingSize4 { get; set; }
        public int GradingSizeRegrind { get; set; }      
        public int Binder { get; set; }
        public string BinderType { get; set; }
        public int Minutes { get; set; }
        public string SpecialInstructions { get; set; }
        public string ColourInstructions { get; set; }
        public string MethodPS { get; set; }
        public string HeaderColours { get; set; }
        public decimal HeaderFontSize { get; set; }
        public decimal TopicFontSize { get; set; }
        public decimal SpecialInsHeight { get; set; }
        public decimal SpecialInsTextPosHeight { get; set; }
        public bool Enable { get; set; }
        public string Lift1 { get; set; }
        public string Lift2 { get; set; }
        public string MixingNotes { get; set; }
    }
}
