using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Machine
{
    public class MachinePartDescription
    {
        public Int32 ID { get; set; }
        public Machines Machine { get; set; }
        public MachineParts MachinePart { get; set; }
        public string MachineSearchString { get; set; }
    }
}
