using A1QSystem.Model.Shifts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public class ShiftItemEqualityComparer : IEqualityComparer<Shift>
    {
        public bool Equals(Shift x, Shift y)
        {
            // Two items are equal if their keys are equal.
            return x.ShiftID == y.ShiftID;
        }

        public int GetHashCode(Shift obj)
        {
            return obj.ShiftID.GetHashCode();
        }
    }
}
