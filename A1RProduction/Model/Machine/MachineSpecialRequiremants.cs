using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Machine
{
    public class MachineSpecialRequiremants
    {
        public int ID { get; set; }
        public Machines Machine { get; set; }
        public string Description { get; set; }
    }
}
