using A1QSystem.Model.Orders;
using A1QSystem.Model.Shifts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production.ReRoll
{
    public class ReRollingHistory
    {
        public Int32 ID { get; set; }
        public Int32 ProdTimeTableID { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
        public int Rolls { get; set; }
        public int ShortRolls { get; set; }
        public int OffSpecRolls { get; set; }
        public string CompletedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public TimeSpan CreatedTime { get; set; }
        public Shift Shift { get; set; }
    }
}
