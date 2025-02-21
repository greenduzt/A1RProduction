using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Machine
{
    public class MachineWorkOrderHistory : MachineWorkOrder
    {
        public string WorkOrderCompletedBy { get; set; }
        //public string RowBackground { get; set; }
        public ObservableCollection<MachineWorkDescription> MahcineWorkDescription { get; set; }

        private string _rowBackground;
        private int _noOfFiles;
        private string _recordBtnVisibility;
        private string _reviewReqVisibility;

     
        public string ReviewReqVisibility
        {
            get
            {
                return _reviewReqVisibility;
            }
            set
            {
                _reviewReqVisibility = value;
                RaisePropertyChanged(() => this.ReviewReqVisibility);
            }
        }

        public string RowBackground
        {
            get
            {
                return _rowBackground;
            }
            set
            {
                _rowBackground = value;
                RaisePropertyChanged(() => this.RowBackground);
            }
        }

        public int NoOfFiles
        {
            get
            {
                return _noOfFiles;
            }
            set
            {
                _noOfFiles = value;
                RaisePropertyChanged(() => this.NoOfFiles);
            }
        }

        public string RecordBtnVisibility
        {
            get
            {
                return _recordBtnVisibility;
            }
            set
            {
                _recordBtnVisibility = value;
                RaisePropertyChanged(() => this.RecordBtnVisibility);
            }
        }
        
    }
}
