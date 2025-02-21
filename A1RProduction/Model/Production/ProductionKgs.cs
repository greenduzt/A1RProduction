using A1QSystem.Model.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production
{
    public class ProductionKgs
    {
        //public DateTime Date { get; set; }
        //public DateTime GradingDate { get; set; }
        //public DateTime MixingDate { get; set; }
        //public string Type { get; set; }

        public decimal GradingMorningKg { get; set; }
        public decimal GradingArvoKg { get; set; }
        public decimal GradingNightKg { get; set; }

        public decimal Total { get; set; }
        //public string DescriptionStr { get; set; }


        public DateTime Date { get; set; }
        public GradedStock GradedStock { get; set; }
    }
}
