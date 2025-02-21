using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production.Mixing
{
    public class IBCChangeOver
    {
        public int ID { get; set; }
        public string BinderType { get; set; }
        public string BatchNo { get; set; }
        public DateTime DateTime { get; set; }
    }
}
