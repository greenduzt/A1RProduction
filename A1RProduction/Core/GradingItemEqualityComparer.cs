using A1QSystem.Model.Production.Grading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public class GradingItemEqualityComparer : IEqualityComparer<GradingProductionDetails>
    {
        public bool Equals(GradingProductionDetails x, GradingProductionDetails y)
        {
            // Two items are equal if their keys are equal.
            return x.RawProduct.RawProductID == y.RawProduct.RawProductID;
        }

        public int GetHashCode(GradingProductionDetails obj)
        {
            return obj.RawProduct.RawProductID.GetHashCode();
        }
    }
}
