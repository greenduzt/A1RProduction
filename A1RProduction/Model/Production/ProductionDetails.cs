using A1QSystem.Model.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production
{
    public class ProductionDetails
    {
        public BindingList<ProductDetails> ProductDetails { get; set; }
        public Customer Customer { get; set; }
        public string RequiredDate { get; set; }
        public string OrderCreatedDate { get; set; }
        public decimal Quantity { get; set; }
    }   
}
