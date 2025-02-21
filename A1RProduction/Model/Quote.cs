using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model
{
    public class Quote
    {
        public int ID { get; set; }
        public string Prefix { get; set; }
        public int QuoteID { get; set; }
        public string CustomerName { get; set; }
        public string QuoteDate { get; set; }
        public decimal FreightTotal { get; set; }
        public decimal ListPriceTot { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal TotAmount { get; set; }
        public string SalesPerson { get; set; }
        public string ProName { get; set; }
        public string Instructions { get; set; }
        public string InternalComments { get; set; }

        public Customer customer { get; set; }
        public FreightDetails freightDetails { get; set; }    
        public BindingList<QuoteDetails> quoteDetails { get; set; }    
    }
}
