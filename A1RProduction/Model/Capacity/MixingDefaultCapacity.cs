using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Capacity
{
    public class MixingDefaultCapacity
    {
        public int ID { get; set; }
        public Machines Machine { get; set; }
        public decimal MaxMixes { get; set; }
        public string Day { get; set; }
    }
}
