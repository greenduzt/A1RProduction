using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.View.Quoting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Model.Products
{
    public class ProductDetails : ObservableObject, IDataErrorInfo
    {
        private SearchProductByName window;
        private ICommand _searchProductCommand;
        private bool _canExecute;

        public ProductDetails()
        {
            _canExecute = true; 

        }

        public Product _product;
        public Product Product
        {
            get { return _product; }
            set
            {
                _product = value;
               
                RaisePropertyChanged(() => this.Product);               
            }
        }

        public Product _CurrentProduct;
        public Product CurrentProduct
        {
            get { return _CurrentProduct; }
            set
            {
                _CurrentProduct = value;
               
                RaisePropertyChanged(() => this.CurrentProduct);
                UpdateProduct();

               
            }
        }

        private decimal _quantity;
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
            }
        }

        private decimal _discount;
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

        private decimal _total;
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

        public string Error
        {
            get { throw new NotImplementedException(); }         
        }     
            
            public string this[string columnName]
            {
                get 
                {                     
                    string error = string.Empty;
                    switch(columnName)
                    {
                        case "Quantity":
                        if (_quantity <= 0)
                        {
                           error = "Quantity cannot be zero";
                        }                                  
                          break;
                       
                        default:
                        error = null;
                        throw new Exception("Unexpected property being validated on Service");
                    }
                    //just return the error or empty string if there is no error
                    return error;
                }
             
            }

            private void UpdateProduct()
            {
                if (_CurrentProduct != null)
                {
                    Product = new Product() { ProductID = CurrentProduct.ProductID, ProductCode = CurrentProduct.ProductCode, UnitPrice = CurrentProduct.UnitPrice, ProductDescription = CurrentProduct.ProductDescription, ProductUnit = CurrentProduct.ProductUnit };

                    
                    
                    
                }
                
                CalculateTotal();
            }

        void AddProductCode()
        {
            window = new SearchProductByName();
            if (window.ShowDialog() == true)
            {
                
               Product = new Product(){ProductCode = window.ProductCode};

               if (Product.ProductCode != null)
                {
                    DataView SelectedProductDetails = DBAccess.GetProductsInCategory(Product.ProductCode).Tables["Products"].DefaultView;

                    if (SelectedProductDetails.Count > 0)
                    {

                        Product = new Product() { ProductCode = (string)SelectedProductDetails[0]["ProductCode"], ProductDescription = (string)SelectedProductDetails[0]["ProductDescription"], ProductUnit = (string)SelectedProductDetails[0]["ProductUnit"], UnitPrice = (decimal)SelectedProductDetails[0]["ProductPrice"] };

                        CalculateTotal();
                    }
                }

            }
        }

       

        private void CalculateTotal()
        {
            decimal subTotal = 0;
            decimal disTotal = 0;

            subTotal = Product.UnitPrice * Quantity;
            disTotal = (subTotal * Discount) / 100;

            Total = subTotal - disTotal;

           
        }

        #region COMMANDS
        public ICommand searchProductCommand
        {
            get
            {
                return _searchProductCommand ?? (_searchProductCommand = new LogOutCommandHandler(() => AddProductCode(), _canExecute));
            }
        }
       
        #endregion 
    }
}
