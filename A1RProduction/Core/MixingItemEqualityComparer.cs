using A1QSystem.Model.Production.Mixing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public class MixingItemEqualityComparer : IEqualityComparer<MixingProductionDetails>
    {
        public bool Equals(MixingProductionDetails x, MixingProductionDetails y)
        {
            // Two items are equal if their keys are equal.
            return x.RawProduct.RawProductID == y.RawProduct.RawProductID;
        }

        public int GetHashCode(MixingProductionDetails obj)
        {
            return obj.RawProduct.RawProductID.GetHashCode();
        }
    }
}
