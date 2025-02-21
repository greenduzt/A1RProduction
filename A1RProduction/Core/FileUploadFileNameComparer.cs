using A1QSystem.Model.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Core
{
    public  class FileUploadFileNameComparer : IEqualityComparer<FileUpload>
    {
        public bool Equals(FileUpload x, FileUpload y)
        {
            return x.FileName == y.FileName;
        }

        public int GetHashCode(FileUpload obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;
            int hashName = obj.FileName == null ? 0 : obj.FileName.GetHashCode();

            return hashName;
        }
    }
}
