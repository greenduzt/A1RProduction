using A1QSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Orders
{
    public class OrderProgressDetails : ObservableObject
    {


        private int _orderDetailsID;
        public int OrderDetailsID
        {
            get { return _orderDetailsID; }
            set
            {
                _orderDetailsID = value;
                RaisePropertyChanged(() => this.OrderDetailsID);
            }
        }

        private int _orderID;
        public int OrderID
        {
            get { return _orderID; }
            set
            {
                _orderID = value;
                RaisePropertyChanged(() => this.OrderID);
            }
        }

        private string _productCode;
        public string ProductCode
        {
            get { return _productCode; }
            set
            {
                _productCode = value;
                RaisePropertyChanged(() => this.ProductCode);
            }
        }

        private string _productDescription;
        public string ProductDescription
        {
            get { return _productDescription; }
            set
            {
                _productDescription = value;
                RaisePropertyChanged(() => this.ProductDescription);
            }
        }

        private string _productUnit;
        public string ProductUnit
        {
            get { return _productUnit; }
            set
            {
                _productUnit = value;
                RaisePropertyChanged(() => this.ProductUnit);
            }
        }

        private decimal _productPrice;
        public decimal ProductPrice
        {
            get { return _productPrice; }
            set
            {
                _productPrice = value;
                RaisePropertyChanged(() => this.ProductPrice);
            }
        }

        private int _qty;
        public int Qty
        {
            get { return _qty; }
            set
            {
                _qty = value;
                RaisePropertyChanged(() => this.Qty);
            }
        }

        private int _discount;
        public int Discount
        {
            get { return _discount; }
            set
            {
                _discount = value;
                RaisePropertyChanged(() => this.Discount);
            }
        }

        private decimal _total;
        public decimal Total
        {
            get { return _total; }
            set
            {
                _total = value;
                RaisePropertyChanged(() => this.Total);
            }
        }
    }
}
