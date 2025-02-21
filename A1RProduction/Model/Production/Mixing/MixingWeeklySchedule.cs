using A1QSystem.Core;
using A1QSystem.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production.Mixing
{
    public class MixingWeeklySchedule : ViewModelBase
    {
        private Int64 _orderNo;
        private RawProduct _rawProduct;
        private Int64 _mixingBlockLogs;
        private Int64 _totBlockLogs;
        private Int64 _totMixedBlockLogs;
        private DateTime _mixingDate;
        private string _mixingShift;
        private string _comments;
        private string _isCommentsVisible;

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

        public Int64 TotMixedBlockLogs
        {
            get { return _totMixedBlockLogs; }
            set
            {
                _totMixedBlockLogs = value;
                RaisePropertyChanged(() => this.TotMixedBlockLogs);
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
