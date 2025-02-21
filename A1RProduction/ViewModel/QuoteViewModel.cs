using A1QSystem.Commands;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.View;
using A1QSystem.View.Quoting;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel
{
    public class QuoteViewModel : BaseViewModel
    {
        public ObservableCollection<ProductItem> ProductItems { get; set; }

        private String _CurrentProduct;
        private string _userName;
        private string _state;
        private List<UserPrivilages> _privilages;

        private Microsoft.Practices.Prism.Commands.DelegateCommand showChildWindowCommand;
        private ICommand _searchProductCommand;

        private ObservableCollection<Customer> _customerList;
        
        private bool _ValueAvalible;
        private bool _canExecute;

        private SearchProductByName window;

        public QuoteViewModel(string UserName,string State, List<UserPrivilages> Privilages)
        {
            _userName = UserName;
            _state = State;
            _privilages = Privilages;

            DataView pdv = new DataView();
            pdv = DBAccess.GetAllProducts().Tables["Products"].DefaultView;
            pdv.Sort = "ProductCode ASC";

            ProductItems = new ObservableCollection<ProductItem>();

            for (int x = 0; x < pdv.Count; x++)
            {

                ProductItems.Add(new ProductItem() { ProductID = Convert.ToInt16(pdv[x]["ProductID"]), ProductCode = pdv[x]["ProductCode"].ToString() });
            }

            
            CustomerList = GetCustomers();


            showChildWindowCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(ShowChildWindow);

            _canExecute = true;

           
        }

        
        public String CurrentProduct
        {
            get { return _CurrentProduct; }
            set
            {
                _CurrentProduct = value;

                RaisePropertyChanged("CurrentProduct");

               

                if (_CurrentProduct != null)
                {
                    ProductsInList = DBAccess.GetProductsInCategory(_CurrentProduct).Tables["Products"].DefaultView;

                    if (ProductsInList.Count > 0)
                    {
                        ProductName = (string)_ProductsInList[0]["ProductName"];
                        ProductPrice = (decimal)ProductsInList[0]["ProductPrice"];
                        ProductCode = (string)ProductsInList[0]["ProductCode"];
                        ProductDescription = (string)ProductsInList[0]["ProductDescription"];
                        ProductUnit = (string)ProductsInList[0]["ProductUnit"];
                    }
                }
                
            }
        }

        void AddProductName()
        {
            window = new SearchProductByName();
            if (window.ShowDialog() == true)
            {
                CurrentProduct = window.ProductCode;
                focus = true;
               
            }
        }

        #region PUBLIC MEMBERS

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public ObservableCollection<Customer> CustomerList
        {
            get
            {
                return _customerList;
            }
            set
            {
                _customerList = value;

                RaisePropertyChanged("CustomerList");
            }
        }


        public bool ValueAvalible
        {
            get
            {
                return _ValueAvalible;
            }
            set
            {
                _ValueAvalible = value;
            }
        }
        private bool _focus;
        public bool focus
        {
            get { return _focus; }
            set
            {
                _focus = value;
                RaisePropertyChanged("focus");
            }
        }

        private DataView _ProductsInList;
        public DataView ProductsInList
        {
            get { return _ProductsInList; }
            set
            {
                _ProductsInList = value;
                RaisePropertyChanged("ProductsInList");
            }
        }


        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set { _productName = value; }
        }

        private decimal _ProductPrice;
        public decimal ProductPrice
        {
            get { return _ProductPrice; }
            set
            {

                _ProductPrice = Math.Round(value, 2);

                if (_ProductPrice.ToString() == "0.00")
                {
                    ValueAvalible = true;
                }
                else
                {
                    ValueAvalible = false;
                }

                RaisePropertyChanged("ProductPrice");
                RaisePropertyChanged("Total");
                RaisePropertyChanged("ValueAvalible");
            }
        }


        private string _pCode;
        public string PCode
        {
            get
            {
                return _pCode;
            }
            set
            {
                _pCode = value;

                RaisePropertyChanged("PCode");
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

                RaisePropertyChanged("ProductCode");
                RaisePropertyChanged("Total");

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

                RaisePropertyChanged("ProductDescription");
                RaisePropertyChanged("Total");

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

                RaisePropertyChanged("Quantity");
                RaisePropertyChanged("Total");

               

            }
        }

        private int _discount;
        public int Discount
        {
            get
            {
                return _discount;
            }
            set
            {
                _discount = value;
                RaisePropertyChanged("Discount");
                RaisePropertyChanged("Total");

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
                RaisePropertyChanged("ProductUnit");
                RaisePropertyChanged("Total");

            }
        }


        private decimal _total;
        public decimal Total
        {
            get
            {
                decimal x = ProductPrice * Quantity;
                decimal y = (x * Discount) / 100;
                decimal tot = x - y;

                return tot;
            }
            set
            {

                _total = value;
            }
        }

        private string _selectedItem;
        public string SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged("SelectedItem");

               
            }
        }

        #endregion

        #region COMMANDS
        public Microsoft.Practices.Prism.Commands.DelegateCommand ShowChildWindowCommand
        {
            get { return showChildWindowCommand; }
        }

        public ICommand searchProductCommand
        {
            get
            {
                return _searchProductCommand ?? (_searchProductCommand = new LogOutCommandHandler(() => AddProductName(), _canExecute));
            }
        }

       

        

        #endregion

        #region Private Methods

        private void ShowChildWindow()
        {
            var childWindow = new ChildWindowView();
            childWindow.customer_Closed += (r =>
            {
                string CompName = r.CompanyName;

                string FirstName = r.FirstName;
                string LastName = r.LastName;
                string Telephone = r.Telephone;
                string Mobile = r.Mobile;
                string Email = r.Email;
                string Address = r.Address;
                string city = r.City;
                string state = r.State;
                string PostCode = r.PostCode;
                
                CustomerList.Add(new Customer() { CompanyName = CompName, FirstName = FirstName, LastName = LastName, Telephone = Telephone, Mobile = Mobile, Email = Email, Address = Address, City = city, State = state, PostCode = PostCode });

          
            });
            childWindow.Show(1);           
       }

        private ObservableCollection<Customer> GetCustomers()
        {

            ObservableCollection<Customer> cusList = new ObservableCollection<Customer>();

            cusList = DBAccess.GetCustomerData();

            return cusList;
        }

        #endregion
    }
}
