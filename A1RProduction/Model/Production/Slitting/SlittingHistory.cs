using A1QSystem.Model.Orders;
using A1QSystem.Model.Shifts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production.Slitting
{
    public class SlittingHistory
    {
        public Int32 ID { get; set; }
        public Int32 ProdTimeTableID { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
        public int Blocks { get; set; }
        public int Yield { get; set; }
        public int OffSpec { get; set; }
        public int Shredding { get; set; }
        public Shift Shift { get; set; }
        public string CompletedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public TimeSpan CreatedTime { get; set; }
        public Machines Machine { get; set; }
    }
}
