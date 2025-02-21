using A1QSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace A1QSystem.Model.Other
{
    public class FileUpload : ViewModelBase
    {
        public int ID { get; set; }
        public string Description { get; set; }

        public string FilePathFrom { get; set; }
        public string FilePathTo { get; set; }
        private string _fileName;
        public DateTime UploadedDateTime { get; set; }
        public string UploadedBy { get; set; }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                RaisePropertyChanged(() => this.FileName);

            }
        }
    }
}
