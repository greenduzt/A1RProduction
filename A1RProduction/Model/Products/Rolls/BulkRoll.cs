using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Products.Rolls
{
    public class BulkRoll : Product
    {
        public int BulkRollID { get; set; }
        public int BulkRollProductID { get; set; }
        public int ID { get; set; }
        public string Density { get; set; }
        public decimal Length { get; set; }
        public decimal MaxYield { get; set; }
        public decimal MinYield { get; set; }
        public decimal MinCutLength { get; set; }
        public bool IsCustomReRoll { get; set; }
        public List<CustomRoll> CustomRolls { get; set; }
        public new StandardRoll StandardRoll { get; set; }
        public decimal BulkRollLM { get; set; }
    }
}
