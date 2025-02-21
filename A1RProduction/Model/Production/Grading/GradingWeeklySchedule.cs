using A1QSystem.Core;
using A1QSystem.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production.Grading
{
    public class GradingWeeklySchedule : ViewModelBase
    {
        private Int64 _orderNo;
        private RawProduct _rawProduct;
        private Int64 _gradingBlockLogs;
        private Int64 _totBlockLogs;
        private int _mixingCompleted;
        private DateTime _mixingDate;
        private string _mixingShift;
        private string _comments;
        private string _isCommentsVisible;

        public int MixingCompleted
        {
            get { return _mixingCompleted; }
            set
            {
                _mixingCompleted = value;
                RaisePropertyChanged(() => this.MixingCompleted);
            }
        }

        public Int64 OrderNo
        {
            get { return _orderNo; }
            set
            {
                _orderNo = value;
                RaisePropertyChanged(() => this.OrderNo);
            }
        }

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

        public Int64 TotBlockLogs
        {
            get { return _totBlockLogs; }
            set
            {
                _totBlockLogs = value;
                RaisePropertyChanged(() => this.TotBlockLogs);
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
