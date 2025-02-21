using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production
{
    public class OffSpecShredding
    {
        public int Index { get; set; }
        public bool LiftedOffBoard { get; set; }
        public bool UnevenThickness { get; set; }
        public bool TooThick { get; set; }
        public bool TooThin { get; set; }
        public bool StonLines { get; set; }
        public bool DamagedBlockLog { get; set; }
        public bool Contaminated { get; set; }
    }
}
