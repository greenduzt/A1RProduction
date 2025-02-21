using A1QSystem.Model.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public class OrderDetailsComparer : IEqualityComparer<OrderDetails>
    {
        public bool Equals(OrderDetails x, OrderDetails y)
        {
            return x.Product.RawProduct.RawProductID == y.Product.RawProduct.RawProductID && x.MixingDate == y.MixingDate && x.IsArvoShift == y.IsArvoShift && x.IsMorningShift == y.IsMorningShift;
        }

        public int GetHashCode(OrderDetails obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;
            int hashName = obj.Product.RawProduct.RawProductID == null ? 0 : obj.Product.RawProduct.RawProductID.GetHashCode();
            int hashOther = obj.MixingDate == null ? 0 : obj.MixingDate.GetHashCode();
            int hashOther2 = obj.IsArvoShift == null ? 0 : obj.IsArvoShift.GetHashCode();
            int hashOther3 = obj.IsMorningShift == null ? 0 : obj.IsMorningShift.GetHashCode();
            return hashName ^ hashOther ^ hashOther2 ^ hashOther3;
        }
    }
}
