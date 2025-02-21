using A1QSystem.Model.Shifts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production.Grading
{
    public class GradedStockHistory
    {
        public DateTime Date { get; set; }
        public RubberGrades RubberGrades { get; set; }
        public decimal Qty { get; set; }
        public Shift Shift { get; set; }
    }
}
