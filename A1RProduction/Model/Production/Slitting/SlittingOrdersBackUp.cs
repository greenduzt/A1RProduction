using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production.Slitting
{
    public class SlittingOrdersBackUp
    {
        public DateTime BackupDate { get; set; }
        public int BackupShiftID { get; set; }
        public string BackupType { get; set; }
    }
}
