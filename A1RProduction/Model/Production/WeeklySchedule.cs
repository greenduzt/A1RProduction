using A1QSystem.Core;
using A1QSystem.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production
{
    public class WeeklySchedule : ViewModelBase
    {
        private RawProduct _rawProduct;
        private Int64 _totBlockLogs;
        private Int64 _gradingBlockLogs;
        private Int64 _mixingBlockLogs;
        private Int64 _mixingCompleted;       
        private DateTime _mixingDate;
        private string _mixingShift;
        private string _bottomRowString;
        private string _comments;
        private string _isCommentsVisible;
               

        public RawProduct RawProduct
        {
            get { return _rawProduct; }
            set
            {
                _rawProduct = value;
                RaisePropertyChanged(() => this.RawProduct);
            }
        }

        public Int64 GradingBlockLogs
        {
            get { return _gradingBlockLogs; }
            set
            {
                _gradingBlockLogs = value;
                RaisePropertyChanged(() => this.GradingBlockLogs);
            }
        }

        public Int64 MixingBlockLogs
        {
            get { return _mixingBlockLogs; }
            set
            {
                _mixingBlockLogs = value;
                RaisePropertyChanged(() => this.MixingBlockLogs);
            }
        }

        public Int64 TotBlockLogs
        {
            get { return _totBlockLogs; }
            set
            {
                _totBlockLogs = value;
                RaisePropertyChanged(() => this.TotBlockLogs);
            }
        }

        public Int64 MixingCompleted
        {
            get { return _mixingCompleted; }
            set
            {
                _mixingCompleted = value;
                RaisePropertyChanged(() => this.MixingCompleted);
            }
        }        

        public DateTime MixingDate
        {
            get { return _mixingDate; }
            set
            {
                _mixingDate = value;
                RaisePropertyChanged(() => this.MixingDate);
            }
        }

        public string MixingShift
        {
            get { return _mixingShift; }
            set
            {
                _mixingShift = value;
                RaisePropertyChanged(() => this.MixingShift);
            }
        }

        public string BottomRowString
        {
            get { return _bottomRowString; }
            set
            {
                _bottomRowString = value;
                RaisePropertyChanged(() => this.BottomRowString);
            }
        }

        public string Comments
        {
            get { return _comments; }
            set
            {
                _comments = value;
                RaisePropertyChanged(() => this.Comments);
            }
        }

        public string IsCommentsVisible
        {
            get { return _isCommentsVisible; }
            set
            {
                _isCommentsVisible = value;
                RaisePropertyChanged(() => this.IsCommentsVisible);
            }
        }
    }
}
