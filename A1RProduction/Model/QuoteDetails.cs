using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.View.Quoting;
using A1QSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Model
{
    public class QuoteDetails : ObservableObject, IDataErrorInfo
    {
        private SearchProductByName window;
        private ICommand _searchProductCommand;
        private bool _canExecute;

        public QuoteDetails()
        {
            _canExecute = true;
        }
        
        private void UpdateProduct()
        {
          
            if (_CurrentProduct != null)
            {
                ProductID = CurrentProduct.ProductID;
                ProductCode = CurrentProduct.ProductCode;
                ProductPrice = CurrentProduct.UnitPrice;
                ProductDescription = CurrentProduct.ProductDescription;
                ProductUnit = CurrentProduct.ProductUnit;
            }
            else
            {
                ProductPrice = 0;
                ProductDescription = null;
                ProductUnit = null;

            }
            CalculateTotal();
        }

        private void CalculateTotal()
        {
            decimal subTotal = 0;
            decimal disTotal = 0;

            subTotal = ProductPrice * Quantity;
            disTotal = (subTotal * Discount) / 100;

            Total = subTotal - disTotal;
        }

        #region Public Properties

        private int _id;
        public int ID
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged(() => this.ID);
            }
        }

        public Product _CurrentProduct;
        public Product CurrentProduct
        {
            get { return _CurrentProduct; }
            set
            {
                _CurrentProduct = value;
              //  if (_CurrentProduct != null)
              //  {
                    RaisePropertyChanged(() => this.CurrentProduct);
                    UpdateProduct();
   
              //  }            
 
            }
        }      
        

        private decimal _ProductPrice;
        public decimal ProductPrice
        {
            get { return _ProductPrice; }
            set
            {
                _ProductPrice = value;
                RaisePropertyChanged(() => this.ProductPrice);
            }
        }

        private int _productID;
        public int ProductID
        {
            get
            {
                return _productID;
            }
            set
            {
                _productID = value;

                RaisePropertyChanged(() => this.ProductID);

                

            }
        }

        private string _productCode;
        public string ProductCode
        {
            get
            {
                return _productCode;
            }
            set
            {
                _productCode = value;
                
                RaisePropertyChanged(() => this.ProductCode);
                
            }
        }

        private string _productDescription;
        public string ProductDescription
        {
            get
            {
                return _productDescription;
            }
            set
            {
                _productDescription = value;

                //OnPropertyChanged("ProductDescription");
                RaisePropertyChanged(() => this.ProductDescription);
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

                //OnPropertyChanged("Quantity");
                RaisePropertyChanged(() => this.Quantity);
                CalculateTotal();
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
                //OnPropertyChanged("Discount");
                RaisePropertyChanged(() => this.Discount);
                CalculateTotal();  
            }
        }
        private string _productUnit;
        public string ProductUnit
        {
            get
            {
                return _productUnit;
            }
            set
            {
                _productUnit = value;
                //OnPropertyChanged("ProductUnit");
                RaisePropertyChanged(() => this.ProductUnit);
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
                //OnPropertyChanged("Total");
                RaisePropertyChanged(() => this.Total);
            }
        }

        #endregion

        #region COMMANDS
        public ICommand searchProductCommand
        {
            get
            {
                return _searchProductCommand ?? (_searchProductCommand = new LogOutCommandHandler(() => AddProductCode(), _canExecute));
            }
        }
        #endregion 
        

        void AddProductCode()
        {
            window = new SearchProductByName();
            if (window.ShowDialog() == true)
            {
                
                ProductCode = window.ProductCode;

                if (ProductCode != null)
                {
                    DataView SelectedProductDetails = DBAccess.GetProductsInCategory(ProductCode).Tables["Products"].DefaultView;

                    if (SelectedProductDetails.Count > 0)
                    {
                        ProductID = (int)SelectedProductDetails[0]["ProductID"];
                        ProductPrice = (decimal)SelectedProductDetails[0]["ProductPrice"];
                        ProductCode = (string)SelectedProductDetails[0]["ProductCode"];
                        ProductDescription = (string)SelectedProductDetails[0]["ProductDescription"];
                        ProductUnit = (string)SelectedProductDetails[0]["ProductUnit"];

                        CalculateTotal();
                    }
                }

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
      }
}