using A1QSystem.Model.Orders;
using A1QSystem.Model.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public class FileUploadComparer : IEqualityComparer<FileUpload>
    {
        public bool Equals(FileUpload x, FileUpload y)
        {
            return x.Description == y.Description;
        }

        public int GetHashCode(FileUpload obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;
            int hashName = obj.Description == null ? 0 : obj.Description.GetHashCode();
            
            return hashName;
        }
    }
}
