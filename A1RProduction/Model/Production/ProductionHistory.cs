using A1QSystem.Core;
using A1QSystem.Model.Products;
using A1QSystem.ViewModel.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Production
{
    public class ProductionHistory : ViewModelBase
    {
        private Int32 _id;
        private Int32 _prodTimeTableID;
        private SalesOrder _salesOrder;
        private RawProduct _rawProduct;
        private decimal _qty;
        private int _shift;
        private int _orderType;
        private string _completedBy;
        private DateTime _createdDateTime;
        private TimeSpan _completedTime;
        private string _shiftName;
        private string _status;
        private string _cancelBackCol;
        private bool _cancelBtnEnabled;

        private string GetShiftNameByID(int shiftId)
        {
            string result = string.Empty;
            switch (shiftId)
            {
                case 1: result = "Day";
                    break;
                case 2: result = "Afternoon";
                    break;
                case 3: result = "Night";
                    break;
                default: result = "Unspecified";
                    break;
            }

            return result;
        }

        private decimal GetQtyByUnit(decimal q, string t)
        {
            decimal quantity = 0;

            if (t == "Block" || t == "Pallet")
            {
                quantity = Math.Ceiling(q);
            }
            else if (t == "Log")
            {
                quantity = Math.Round(q, 2);
            }
            return quantity;
        }

        #region PUBLIC_PROPERTIES

        public Int32 ID
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged(() => this.ID);
            }
        }

        public Int32 ProdTimeTableID
        {
            get { return _prodTimeTableID; }
            set
            {
                _prodTimeTableID = value;
                RaisePropertyChanged(() => this.ProdTimeTableID);
            }
        }

        public SalesOrder SalesOrder
        {
            get { return _salesOrder; }
            set
            {
                _salesOrder = value;
                RaisePropertyChanged(() => this.SalesOrder);
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

        public decimal Qty
        {
            get { return _qty; }
            set
            {
                _qty = value;
                RaisePropertyChanged(() => this.Qty);
              
            }
        }

        public int Shift
        {
            get { return _shift; }
            set
            {
                _shift = value;
                RaisePropertyChanged(() => this.Shift);
                ShiftName =GetShiftNameByID(Shift);
            }
        }

        public int OrderType
        {
            get { return _orderType; }
            set
            {
                _orderType = value;
                RaisePropertyChanged(() => this.OrderType);
            }
        }

        public string CompletedBy
        {
            get { return _completedBy; }
            set
            {
                _completedBy = value;
                RaisePropertyChanged(() => this.CompletedBy);
            }
        }

        public DateTime CreatedDateTime
        {
            get { return _createdDateTime; }
            set
            {
                _createdDateTime = value;
                RaisePropertyChanged(() => this.CreatedDateTime);
            }
        }

        public TimeSpan CompletedTime
        {
            get { return _completedTime; }
            set
            {
                _completedTime = value;
                RaisePropertyChanged(() => this.CompletedTime);
            }
        }

        public string ShiftName
        {
            get { return _shiftName; }
            set
            {
                _shiftName = value;
                RaisePropertyChanged(() => this.ShiftName);
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged(() => this.Status);
            }
        }

        public bool CancelBtnEnabled
        {
            get { return _cancelBtnEnabled; }
            set
            {
                _cancelBtnEnabled = value;
                RaisePropertyChanged(() => this.CancelBtnEnabled);
            }
        }

        public string CancelBackCol
        {
            get { return _cancelBackCol; }
            set
            {
                _cancelBackCol = value;
                RaisePropertyChanged(() => this.CancelBackCol);
            }
        }       
        

        #endregion

    }
}
