using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Maintenance
{
    public class MiscellaneousWorkOrder
    {
        public string Area { get; set; }
        public string Title { get; set; }
        public string WorkDescription { get; set; }
        public DateTime StartDate { get; set; }
    }
}
