using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model
{
    public class FreightDetails
    {
        public int Id { get; set; }
        public int QuoteID { get; set; }
        public string FreightName { get; set; }
        public string FreightUnit { get; set; }
        public decimal FreightPallets { get; set; }
        public decimal FreightPrice { get; set; }
        public decimal FreightDiscount { get; set; }
        public decimal FreightTotal { get; set; }
        public string FreightDescription { get; set; }
        public string ShipToAddress { get; set; }
    }
}
