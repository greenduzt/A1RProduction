using A1QSystem.Commands;
using A1QSystem.DB;
using A1QSystem.View.Quoting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace A1QSystem.ViewModel
{
    public class SearchProductByNameViewModel : IDataErrorInfo,INotifyPropertyChanged
    {

        private ObservableCollection<string> _productDescription;
        private string _selectedProduct;
        private string _productCode;

       
        private ICommand _addProductCode;
        private ICommand _clearProductCommand;
        bool _canExecute;

        public SearchProductByNameViewModel()
        {
            DataView pdv = new DataView();
            pdv = DBAccess.GetAllProducts().Tables["Products"].DefaultView;
            pdv.Sort = "ProductDescription ASC";

            var productDescription = new ObservableCollection<string>();
            
            for (int x = 0; x < pdv.Count; x++)
            {
                productDescription.Add(pdv[x]["ProductDescription"].ToString());
                           
            }

            ProductDescription = productDescription;
            _canExecute = true;
        }

        public ObservableCollection<string> ProductDescription
        {
            get { return _productDescription; }
            set 
            { 
                _productDescription = value;
                OnPropertyChanged("ProductDescription");               
            }
        }


        public string SelectedProduct 
        {
            get 
            {
                return _selectedProduct;
            }
            set
            {
                _selectedProduct = value;

                _productCode = DBAccess.GetProductionData(SelectedProduct);

                OnPropertyChanged("SelectedProduct");
                OnPropertyChanged("ProductCode");               
            }
        }

       
       

        public string ProductCode
        {
            get
            {
                return _productCode;
            }
            set
            {               
                _productCode = value; 
            }
        }


        public ICommand AddProductCode
        {
            get
            {
                if (_addProductCode == null)
                    _addProductCode = new A1QSystem.Commands.RelayCommand(param => this.AddToGrid(), param => this.CanSave);

                return _addProductCode;
            }
        }

        public ICommand ClearProductCommand
        {
            get
            {
                return _clearProductCommand ?? (_clearProductCommand = new LogOutCommandHandler(() => ClearData(), _canExecute));
            }
        }

        private void ClearData()
        {
            SelectedProduct = "";
            ProductCode = "";

        }

        private void AddToGrid()
        {
            Console.WriteLine("sdsd");
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
            "SelectedProduct"            
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
                case "SelectedProduct":
                    error = ValidateProductDescription();
                    break;
                default:
                    error = null;
                    throw new Exception("Unexpected property being validated on Service");
            }

            return error;
        }

               

        private string ValidateProductDescription()
        {
            if (String.IsNullOrWhiteSpace(SelectedProduct))
            {
                return "Type in product description to find Product Code";
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
       
    }
}
