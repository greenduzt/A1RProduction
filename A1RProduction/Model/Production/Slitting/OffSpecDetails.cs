using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production.Slitting
{
    public class OffSpecDetails
    {
        public Int32 OrderNo { get; set; }
        public Product Product { get; set; }
        public decimal Blocks { get; set; }
        public decimal OffSpecTiles { get; set; }
        public DateTime CompletedDate { get; set; }
        public TimeSpan CompletedTime { get; set; }
        public bool LiftedOffBoard { get; set; }
        public bool UnevenThickness { get; set; }
        public bool StoneLines { get; set; }
        public bool TooThick { get; set; }
        public bool TooThin { get; set; }
        public bool DamagedBlockLog { get; set; }
        public bool Contaminated { get; set; }
        public bool OperatorError { get; set; }
        public bool Other { get; set; }
        public string OtherComment { get; set; }
    }
}
