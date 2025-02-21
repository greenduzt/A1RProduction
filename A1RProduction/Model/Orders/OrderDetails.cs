using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Products;
using A1QSystem.View.Quoting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace A1QSystem.Model.Orders
{
    public class OrderDetails : ObservableObject
    {
        private Product _product;       
        private string _productNameString;
        private string _reRollingVisible;
        private string _customProductVisible;
        private decimal _quantity;
        private decimal _blocksLogsToMake;
        private decimal _qtyInStock;
        private decimal _qtyToMake;
        private decimal _discount;
        private decimal _total;
        private string _toMakeCellBack;
        private string _toMakeCellFore;
        private string _productSource;
        private DateTime _mixingDate;
        private DateTime _currentDate;
        private bool _isMorningShift;
        private bool _isArvoShift;
        private string _mixingComment;
        private Int64 _orderNo;

        public OrderDetails()
        {
            CurrentDate = DateTime.Now;
            MixingDate = CurrentDate;
            IsMorningShift = true;
            CustomProductVisible = "Collapsed";
            ReRollingVisible = "Collapsed";    
            ToMakeCellBack = "White";
            ToMakeCellFore = "Black";
        }

        private void CalculateTotal()
        {
            if (Product != null)
            {
                decimal subTotal = 0;
                decimal disTotal = 0;

                subTotal = Product.UnitPrice * Quantity;
                disTotal = (subTotal * Discount) / 100;
                Total = subTotal - disTotal;

                QtyToMake = Quantity;
                QtyInStock = 0;
            }
        }

        #region PUBLIC PROPERTIES       


        public DateTime MixingDate
        {
            get { return _mixingDate; }
            set
            {
                _mixingDate = value;

                RaisePropertyChanged(() => this.MixingDate);

            }
        }

        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                _currentDate = value;

                RaisePropertyChanged(() => this.CurrentDate);
            }
        }

        public decimal BlocksLogsToMake
        {
            get { return _blocksLogsToMake; }
            set
            {
                _blocksLogsToMake = value;

                RaisePropertyChanged(() => this.BlocksLogsToMake);              
            }
        }
        public decimal QtyToMake
        {
            get { return _qtyToMake; }
            set
            {
                _qtyToMake = value;

                RaisePropertyChanged(() => this.QtyToMake);              
            }
        }

        public decimal QtyInStock
        {
            get { return _qtyInStock; }
            set
            {
                _qtyInStock = value;

                RaisePropertyChanged(() => this.QtyInStock);              
            }
        }

        public string ProductSource
        {
            get { return _productSource; }
            set
            {
                _productSource = value;

                RaisePropertyChanged(() => this.ProductSource);
            }
        }

        public string ProductNameString
        {
            get { return _productNameString; }
            set
            {
                _productNameString = value;

                RaisePropertyChanged(() => this.ProductNameString);
            }
        }      
        
        public Product Product
        {
            get { return _product; }
            set
            {
                _product = value;

                RaisePropertyChanged(() => this.Product);

                if (Product != null)
                {
                    string density = string.Empty;
                    string thickness = string.Empty;
                    string length = string.Empty;
                    string size = string.Empty;
                    string width = string.Empty;
                    string height = string.Empty;

                    //if (Product.GetType() == typeof(Tile))
                    //{                       
                        
                    //}
                    //else if (Product.GetType() == typeof(BulkRoll))
                    //{

                    //}
                    //else if (Product.GetType() == typeof(StandardRoll))
                    //{

                    //}
                    //else if (Product.GetType() == typeof(CustomRoll))
                    //{

                    //}

                   

                    //if (Product.BulkRoll != null)
                    //{
                    //    if (Product.BulkRoll.Density != "n/a")
                    //    {
                    //        density = Product.BulkRoll.Density;
                    //    }                       

                    //    if (Product.BulkRoll.IsCustomReRoll)
                    //    {
                    //        ReRollingVisible = "Visible";
                    //        CustomProductVisible = "Collapsed";
                    //    }
                    //    else
                    //    {
                    //        CustomProductVisible = "Collapsed";
                    //        ReRollingVisible = "Collapsed";
                    //    }
                    //}
                    //if (Product.StandardRoll != null)
                    //{
                    //    if (Product.StandardRoll.Density != "n/a")
                    //    {
                    //        density = Product.StandardRoll.Density;
                    //    }
                    //    ReRollingVisible = "Collapsed";
                    //    CustomProductVisible = "Collapsed";
                    //}
                    //if (Product.Thickness > 0)
                    //{
                    //    thickness = string.Format("{0:G29}", Product.Thickness) + "mm";
                    //}
                    //if(Product.Tile != null)
                    //{
                    //    width = string.Format("{0:G29}", Product.Width) + "m x ";
                    //    height = string.Format("{0:G29}", Product.Tile.Height) + "m";
                    //    ReRollingVisible = "Collapsed";
                    //    CustomProductVisible = "Collapsed";
                    //}
                    //if (Product.Width > 0)
                    //{
                    //    if (Product.BulkRoll != null)
                    //    {
                    //        if (Product.BulkRoll.Length > 0)
                    //        {
                    //            length = string.Format("{0:G29}", Product.BulkRoll.Length) + "L/m";
                    //        }
                    //        else
                    //        {
                    //            length = "TA";
                    //        }                            
                    //    }

                    //    if (Product.StandardRoll != null)
                    //    {
                    //        if (Product.StandardRoll.Length > 0)
                    //        {
                    //            length = length = string.Format("{0:G29}", Product.StandardRoll.Length) + "L/m";
                    //        }
                    //        else
                    //        {
                    //            length = "TA";
                    //        }
                    //    }

                    //    size = string.Format("{0:G29}", Product.Width) + " x " + string.Format("{0:G29}", length);
                    //}

                    //ProductNameString = Product.ProductName + " " + density + " " + thickness + " " + size + " " + width + height;

                    //if (Product.CustomProduct != null)
                    //{
                    //    if (Product.CustomProduct.CustomJazz != null)
                    //    {
                    //        ReRollingVisible = "Visible";
                    //        CustomProductVisible = "Visible";
                    //    }
                    //    else
                    //    {
                    //        ReRollingVisible = "Collapsed";
                    //        CustomProductVisible = "Collapsed";
                    //    }
                    //}
                }
                CalculateTotal();
            }
        }


        public string CustomProductVisible
        {
            get { return _customProductVisible; }
            set
            {
                _customProductVisible = value;

                RaisePropertyChanged(() => this.CustomProductVisible);
            }
        }    

        public string ReRollingVisible
        {
            get { return _reRollingVisible; }
            set
            {
                _reRollingVisible = value;

                RaisePropertyChanged(() => this.ReRollingVisible);
            }
        }    

        public decimal Quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = value;

                RaisePropertyChanged(() => this.Quantity);
                CalculateTotal();              
            }
        }     

        
        public decimal Discount
        {
            get
            {
                return _discount;
            }
            set
            {
                _discount = value;

                RaisePropertyChanged(() => this.Discount); 
                CalculateTotal();
            }
        }

        
        public decimal Total
        {
            get
            {
                return _total;
            }
            set
            {
                _total = value;
                RaisePropertyChanged(() => this.Total);
            }
        }     

        public string ToMakeCellBack
        {
            get
            {
                return _toMakeCellBack;
            }
            set
            {
                _toMakeCellBack = value;
                RaisePropertyChanged(() => this.ToMakeCellBack);
            }
        }

        public string ToMakeCellFore
        {
            get
            {
                return _toMakeCellFore;
            }
            set
            {
                _toMakeCellFore = value;
                RaisePropertyChanged(() => this.ToMakeCellFore);
            }
        }

        public bool IsMorningShift
        {
            get
            {
                return _isMorningShift;
            }
            set
            {
                _isMorningShift = value;
                RaisePropertyChanged(() => this.IsMorningShift);

                
            }
        }

        public bool IsArvoShift
        {
            get
            {
                return _isArvoShift;
            }
            set
            {
                _isArvoShift = value;
                RaisePropertyChanged(() => this.IsArvoShift);
               
            }
        }

        public string MixingComment
        {
            get
            {
                return _mixingComment;
            }
            set
            {
                _mixingComment = value;
                RaisePropertyChanged(() => this.MixingComment);
            }
        }

        public Int64 OrderNo
        {
            get
            {
                return _orderNo;
            }
            set
            {
                _orderNo = value;
                RaisePropertyChanged(() => this.OrderNo);
            }
        }

        #endregion    
    }
}
