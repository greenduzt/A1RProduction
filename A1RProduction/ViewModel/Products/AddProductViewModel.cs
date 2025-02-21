using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.View.Quoting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MsgBox;
using A1QSystem.Model;
using A1QSystem.View.Products;
using A1QSystem.View;
using A1QSystem.Model.Meta;

namespace A1QSystem.ViewModel
{
    public class AddProductViewModel : IDataErrorInfo, INotifyPropertyChanged
    {
        private string _userName;
        private string state;
        private List<UserPrivilages> _privilages;
        private List<MetaData> metaData;
        private ICommand _addProductCommand;
        private ICommand _clearProductCommand;
        private ICommand _backCommand;
        private ICommand navHomeCommand;
        private ICommand navProductCommand;

        private bool _addProdEnabled;

        private bool _canExecute;

        public AddProductViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            _userName = UserName;
            state = State;
            _privilages = Privilages;
            _addProdEnabled = false;
            _canExecute = true;
            metaData = md;
        }

        private string _productName;
        public string ProductName
        {
            get
            {
                return _productName;
            }
            set
            {
                _productName = value;

                OnPropertyChanged("ProductName");                
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

                OnPropertyChanged("ProductCode");
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

                OnPropertyChanged("ProductDescription");
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

                OnPropertyChanged("ProductUnit");
            }
        }

        private string _productPrice;
        public string ProductPrice
        {
            get
            {
                return _productPrice;
            }
            set
            {
                _productPrice = value;

                OnPropertyChanged("ProductPrice");
            }
        }

        public bool AddProdEnabled
        {
            get
            {
                return _addProdEnabled;
            }
            set
            {
                _addProdEnabled = value;

                OnPropertyChanged("AddProdEnabled");
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = UserName;
            }
        }

        void AddProduct()
        {
            Product newProduct = new Product();
            newProduct.ProductCode = ProductCode;
            newProduct.ProductName = ProductName;
            newProduct.ProductDescription = ProductDescription;
            newProduct.ProductUnit = ProductUnit;
            newProduct.UnitPrice = Convert.ToDecimal(ProductPrice);

            int result = DBAccess.InsertProduct(newProduct);

            if (result > 0)
            {
                Msg.Show("Product was added successfully!", "Product Added", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes); 
                ClearProduct();
            }
            else
            {
                Msg.Show("Something went wrong and details haven't added to the database! Please try again later", "Data cannot save", MsgBoxButtons.OK, MsgBoxImage.Alert, MsgBoxResult.Yes); 
            }
            
        }

        void ClearProduct()
        {
            ProductName = "";
            ProductCode = "";
            ProductDescription = "";
            ProductUnit = "";
            ProductPrice = "";
        }
        private void NavigateHome()
        {
            if (!String.IsNullOrWhiteSpace(ProductName))
            {
                if (Msg.Show("Are you sure you want to navigate to HOME screen?" + System.Environment.NewLine + "You haven't finished filling up fields!", "Naivgation Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    Switcher.Switch(new MainMenu(_userName, state, _privilages, metaData));
                }
            }
            else
            {
                Switcher.Switch(new MainMenu(_userName, state, _privilages, metaData));
            }
        }
        private void NavigateProducts()
        {
            if (!String.IsNullOrWhiteSpace(ProductName))
            {
                if (Msg.Show("Are you sure you want to navigate to PRODUCTS screen?" + System.Environment.NewLine + "You haven't finished filling up fields!", "Naivgation Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    Switcher.Switch(new ProductsMenu(_userName, state, _privilages, metaData));
                }
            }
            else
            {
                Switcher.Switch(new ProductsMenu(_userName, state, _privilages, metaData));
            }
        }

        string IDataErrorInfo.Error
        {
            get
            {
                return null;
            }
        }
      

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                return GetValidationError(propertyName);
            }
        }
             
        
      
        #region Validation

        static readonly string[] ValidatedProperies = 
        {
            "ProductName",
            "ProductCode",
            "ProductDescription",
            "ProductUnit",
            "ProductPrice"
        };

        public bool IsValid
        {
            get
            {
                foreach (string property in ValidatedProperies)
                {
                    if (GetValidationError(property) != null)

                        return false;
                }
                return true;
            }
        }

        string GetValidationError(string propertyName)
        {
            string error = null;

            switch (propertyName)
            {
                case "ProductName":
                    error = ValidateProductName();
                    break;
                case "ProductCode":
                    error = ValidateProductCode();
                    break;
                case "ProductDescription":
                    error = ValidateProductDescription();
                    break;
                case "ProductPrice":
                    error = ValidateProductPrice();
                    break;
                case "ProductUnit":
                    error = ValidateProductUnit();
                    break;
                default:
                    error = null;
                    throw new Exception("Unexpected property being validated on Service");
            }

            return error;
        }

      
        private string ValidateProductName()
        {
            if (String.IsNullOrWhiteSpace(ProductName))
            {
                
                return "Product name cannot be empty";
            }

            return null;
        }

        private string ValidateProductCode()
        {
            if (String.IsNullOrWhiteSpace(ProductCode))
            {
                return "Product code cannot be empty";
            }

            return null;
        }

        private string ValidateProductDescription()
        {
            if (String.IsNullOrWhiteSpace(ProductDescription))
            {
                return "Product description cannot be empty";
            }

            return null;
        }

        private string ValidateProductUnit()
        {
            if (String.IsNullOrWhiteSpace(ProductUnit))
            {
                return "Product unit cannot be empty";
            }

            return null;
        }

        private string ValidateProductPrice()
        {
            decimal d;

            if (String.IsNullOrWhiteSpace(ProductPrice))
            {
                return "Product price cannot be empty";
            }
            else if(!decimal.TryParse(ProductPrice, out d))
            {
                return "Valid price required";
            }

            return null;
        }

        protected bool CanSave
        {
            get
            {
                return IsValid;
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        #region Commands

        public ICommand AddProductCommand
        {
            get
            {
                if (_addProductCommand == null)
                    _addProductCommand = new A1QSystem.Commands.RelayCommand(param => this.AddProduct(), param => this.CanSave);

                return _addProductCommand;
            }
        }

        public ICommand ClearProductCommand
        {
            get
            {
                return _clearProductCommand ?? (_clearProductCommand = new LogOutCommandHandler(() => ClearProduct(), _canExecute));
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new LogOutCommandHandler(() => Switcher.Switch(new ProductsMenu(_userName, state, _privilages, metaData)), _canExecute));
            }
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => NavigateHome(), _canExecute));
            }
        }
        public ICommand NavProductsCommand
        {
            get
            {
                return navProductCommand ?? (navProductCommand = new LogOutCommandHandler(() => NavigateProducts(), _canExecute));
            }
        }

        #endregion
    }
}
