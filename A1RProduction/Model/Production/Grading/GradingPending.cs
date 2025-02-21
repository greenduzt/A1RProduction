using A1QSystem.Model.Production.Model;
using A1QSystem.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production.Grading
{
    public class GradingPending : GradingScheduling
    {
        public RawProduct RawProduct { get; set; }
    }
}
