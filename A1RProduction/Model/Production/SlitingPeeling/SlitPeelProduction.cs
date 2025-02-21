using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production.SlitingPeeling
{
    public class SlitPeelProduction
    {
        public int id { get; set; }
        public int SlitPeelID { get; set; }
        public int RawProductID { get; set; }
        public int ProductID { get; set; }
        public decimal BlockLogQty { get; set; }
        public string ProductionDate { get; set; }
        public int Shift { get; set; }
        public string Status { get; set; }
        
    }
}
