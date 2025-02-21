using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace A1QSystem.Model.Production.SlitingPeeling
{
    public class SlitPeelDetails
    {
        public string CustomerName { get; set; }
        public string Description { get; set; }
        public decimal Qty { get; set; }
        public int Shift { get; set; }

    }
}
