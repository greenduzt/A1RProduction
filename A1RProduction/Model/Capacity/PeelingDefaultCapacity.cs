using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Capacity
{
    public class PeelingDefaultCapacity
    {
        public int ID { get; set; }
        public Machines Machine { get; set; }
        public decimal DollarValue { get; set; }
        public int Shift { get; set; }
        public string Day { get; set; }
    }
}
