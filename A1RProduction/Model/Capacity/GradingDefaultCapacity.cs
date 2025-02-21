using A1QSystem.Model.Production.Grading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Capacity
{
    public class GradingDefaultCapacity
    {
        public int ID { get; set; }
        public Machines Machine { get; set; }
        public RubberGrades RubberGrade { get; set; }
        public decimal Capacity { get; set; }
        public int Shift { get; set; }
        public string Day { get; set; }
    }
}
