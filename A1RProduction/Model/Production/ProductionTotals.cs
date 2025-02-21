using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production
{
    public class ProductionTotals
    {
        public DateTime Date { get; set; }
        public DateTime GradingDate { get; set; }
        public DateTime MixingDate { get; set; }
        
        public string Type { get; set; }
        public string GradingUnit { get; set; }
        public string MixingingUnit { get; set; }

        //Entered
        public decimal GradingNewMorning { get; set; }
        public decimal GradingNewArvo { get; set; }
        public decimal GradingNewNight { get; set; }
        
        //Existing
        public decimal GradingQty { get; set; }
        public decimal GradingMorningQty { get; set; }
        public decimal GradingArvoQty { get; set; }
        public decimal GradingNightQty { get; set; }
        
        public decimal MixingQty { get; set; }
        public decimal MixingMorningQty { get; set; }
        public decimal MixingArvoQty { get; set; }
        public decimal MixingNightQty { get; set; }
       
        public string GradingShift { get; set; }
        public string MixingShift { get; set; }
        
        
        public decimal Total { get; set; }
    }
}
