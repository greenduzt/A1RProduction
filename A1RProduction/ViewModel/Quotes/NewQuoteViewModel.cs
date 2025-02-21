using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using A1QSystem.View.Quoting;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MsgBox;
using A1QSystem.Converter;
using A1QSystem.Model.Meta;

namespace A1QSystem.ViewModel.Quotes
{
    public class NewQuoteViewModel : ViewModelBase,IDataErrorInfo
    {
        private BindingList<QuoteDetails> _quoteDetails;
        public ObservableCollection<Product> Products { get; set; }
        private ObservableCollection<Customer> _customerList;
        private ObservableCollection<Freight> _freightList;

        private string _quoteDate;
        private string _userName;
        private string state;
        private List<UserPrivilages> _privilages;

        private decimal _listPriceTotal;
        private decimal _discountedTotal;
        private decimal _gST;
        private decimal _freightTotal;
        private decimal _totalAmount;
        private string _freightName;
        private List<MetaData> metaData;
        private ICommand backCommand;
        private ICommand createCommand;
        private ICommand clearFields;
        private ICommand navHomeCommand;
        private ICommand navQuotingCommand;
        ICommand _command;
        private Microsoft.Practices.Prism.Commands.DelegateCommand showChildWindowCommand;
        private Microsoft.Practices.Prism.Commands.DelegateCommand _showAddFreightCommand;

        private bool canExecute;

        public NewQuoteViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {
            _userName = UserName;
            state = State;
            _privilages = Privilages;
            PriceReadOnly = true;
            quoteDetails = new BindingList<QuoteDetails>();
            metaData = md;
            LoadProducts();
            LoadFreights();
            LoadCustomers();

            showChildWindowCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(ShowChildWindow);
            _showAddFreightCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(ShowAddFreightWindow);
           
            canExecute = true;
            
        }
       

        #region Public Properties
               
      
        public string QuoteDate
        {
            get
            {
                _quoteDate = NTPServer.GetNetworkTime().ToString("dd/MM/yyyy");
                return _quoteDate;
            }
            set
            {
                _quoteDate = value;
                RaisePropertyChanged(() => this.QuoteDate);
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

        public BindingList<QuoteDetails> quoteDetails
        {
            get{return _quoteDetails; }
            set
            {
                _quoteDetails = value;

                if (_quoteDetails != null)
                {
                    
                    _quoteDetails.ListChanged += (o, e) => RaisePropertyChanged(() => this.DiscountedTotal);
                    _quoteDetails.ListChanged += (o, e) => RaisePropertyChanged(() => this.ListPriceTotal);
                    _quoteDetails.ListChanged += (o, e) => RaisePropertyChanged(() => this.GST);
                    _quoteDetails.ListChanged += (o, e) => RaisePropertyChanged(() => this.FreightTotal);
                    _quoteDetails.ListChanged += (o, e) => RaisePropertyChanged(() => this.TotalAmount);
                    _quoteDetails.ListChanged += (o, e) => RaisePropertyChanged(() => this.QError);
                    _quoteDetails.ListChanged += (o, e) => RaisePropertyChanged(() => this.SelectedProductExists);

                    Console.WriteLine(DiscountedTotal);
                }
                RaisePropertyChanged(() => this.quoteDetails);
            }                
        }

        public decimal ListPriceTotal
        {
            get
            {
                
                _listPriceTotal = _quoteDetails.Sum(x => x.ProductPrice * x.Quantity);
                return _listPriceTotal;
            }
            set
            {
                _listPriceTotal = value;
                RaisePropertyChanged(() => this.ListPriceTotal);
            }
        }

        public decimal DiscountedTotal
        {
            get
            {             

                _discountedTotal = _quoteDetails.Sum(x => x.Total);
                _qError = quoteDetails.Any(i => i.Quantity.Equals(0));
                _selectedProductExists = quoteDetails.Any(i => i.Total.Equals(0));

                return _discountedTotal;
            }
            set
            {
                _discountedTotal = value;
                RaisePropertyChanged(() => this.DiscountedTotal);
                RaisePropertyChanged(() => this.QError);
            }
        }

        public decimal GST
        {
            get
            {
                _gST = ((FreightTotal + DiscountedTotal) * 10) / 100;

                return _gST;
            }
            set
            {
                _gST = value;
                RaisePropertyChanged(() => this.GST);
            }
        }

        public decimal FreightTotal
        {
            get
            {
                return _freightTotal;
            }
            set
            {
                _freightTotal = value;
                RaisePropertyChanged(() => this.FreightTotal);
            }
        }

        public decimal TotalAmount
        {
            get
            {
                _totalAmount = DiscountedTotal + FreightTotal + GST;
                return _totalAmount;
            }
            set
            {
                _totalAmount = value;
                RaisePropertyChanged(() => this.TotalAmount);

                

            }
        }


        public string FreightName
        {
            get
            {
                return _freightName;
            }
            set
            {
                _freightName = value;
                RaisePropertyChanged(() => this.FreightName);

            }
        }

        private string _freightUnit;
        public string FreightUnit
        {
            get
            {
                return _freightUnit;
            }
            set
            {
                _freightUnit = value;
                RaisePropertyChanged(() => this.FreightUnit);
            }
        }
        private decimal _pallets;
        public decimal Pallets
        {
            get
            {
                return _pallets;
            }
            set
            {
                //if(String.IsNullOrWhiteSpace(_pallets))
                //{
                _pallets = value;
                //}
                RaisePropertyChanged(() => this.Pallets);

                CalculateTotalCost();
            }
        }

        private decimal _freightPrice;
        public decimal FreightPrice
        {
            get
            {
                return _freightPrice;
            }
            set
            {
                _freightPrice = value;
                RaisePropertyChanged(() => this.FreightPrice);
                CalculateTotalCost();
            }
        }

        private decimal _freightDis;
        public decimal FreightDis
        {
            get
            {
                return _freightDis;
            }
            set
            {
                _freightDis = value;
                RaisePropertyChanged(() => this.FreightDis);
                CalculateTotalCost();
            }
        }

        private string _freightDescription;
        public string FreightDescription
        {
            get
            {
                return _freightDescription;
            }
            set
            {
                _freightDescription = value;
                RaisePropertyChanged(() => this.FreightDescription);
            }
        }

       

        private string _instructions;
        public string Instructions
        {
            get
            {
                return _instructions;
            }
            set
            {
                _instructions = value;
                RaisePropertyChanged(() => this.Instructions);
            }
        }

        private string _internalComments;
        public string InternalComments
        {
            get
            {
                return _internalComments;
            }
            set
            {
                _internalComments = value;
                RaisePropertyChanged(() => this.InternalComments);
            }
        }

        private string _projectName;
        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                _projectName = value;
                RaisePropertyChanged(() => this.ProjectName);

                ProjectAvailable = DBAccess.CheckProjectName(ProjectName);                

            }
        }

        private bool _projectAvailable;
        public bool ProjectAvailable
        {
            get
            {
                return _projectAvailable;
            }
            set
            {
                _projectAvailable = value;
            }
        }

        private bool _priceReadOnly;
        public bool PriceReadOnly
        {
            get
            {
                return _priceReadOnly;
            }
            set
            {
                _priceReadOnly = value;
                RaisePropertyChanged(() => this.PriceReadOnly);
            }
        }


        public string _selectedFreight;
        public string SelectedFreight
        {
            get { return _selectedFreight; }
            set
            {
                _selectedFreight = value;

                RaisePropertyChanged(() => this.SelectedFreight);

                if (_selectedFreight == "Freight Out" || _selectedFreight == "Freight NT" || _selectedFreight == "Freight North QLD" || _selectedFreight == "Freight TAS")
                {
                    PriceReadOnly = false;
                }
                else
                {
                    PriceReadOnly = true;
                }
                EnableFreightComponents();
                UpdateFreightDetails();
                FreightDis = 0;
                Pallets = 0;
            }
        }

        private string _selectedCustomer;
        public string SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                RaisePropertyChanged(() => this.SelectedCustomer);
                UpdateCustomerDetails();

               

                
            }
        }

        private bool _iSChecked;
        public bool ISChecked
        {
            get { return _iSChecked; }
            set
            {
                _iSChecked = value;
                RaisePropertyChanged(() => this.ISChecked);

                if (_iSChecked)
                {
                    ShipTo = Address + Environment.NewLine + City + Environment.NewLine + CusState + Environment.NewLine + PostCode;
                }
                else
                {
                    ShipTo = "";
                }
            }
        }


        private string _shipTo;
        public string ShipTo
        {
            get
            {
                return _shipTo;
            }
            set
            {
                _shipTo = value;
                RaisePropertyChanged(() => this.ShipTo);
            }
        }

        private string _address;
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
                RaisePropertyChanged(() => this.Address);

            }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
                RaisePropertyChanged(() => this.City);

            }
        }


        private string _cusState;
        public string CusState
        {
            get { return _cusState; }
            set
            {
                _cusState = value;
                RaisePropertyChanged(() => this.CusState);

            }
        }

        private string _postCode;
        public string PostCode
        {
            get { return _postCode; }
            set
            {
                _postCode = value;
                RaisePropertyChanged(() => this.PostCode);

            }
        }     

        private bool _unitActive;
        public bool UnitActive
        {
            get { return _unitActive; }
            set
            {
                _unitActive = value;
                RaisePropertyChanged(() => this.UnitActive);
            }
        }
        private bool _palletsActive;
        public bool PalletsActive
        {
            get { return _palletsActive; }
            set
            {
                _palletsActive = value;
                RaisePropertyChanged(() => this.PalletsActive);
            }
        }
        private bool _freightPriceActive;
        public bool FreightPriceActive
        {
            get { return _freightPriceActive; }
            set
            {
                _freightPriceActive = value;
                RaisePropertyChanged(() => this.FreightPriceActive);
            }
        }
        private bool _discountActive;
        public bool DiscountActive
        {
            get { return _discountActive; }
            set
            {
                _discountActive = value;
                RaisePropertyChanged(() => this.DiscountActive);
            }
        }
        private bool _freightDescActive;
        public bool FreightDescActive
        {
            get { return _freightDescActive; }
            set
            {
                _freightDescActive = value;
                RaisePropertyChanged(() => this.FreightDescActive);
            }
        }           
        private bool _quoteDetailsActive;
        public bool QuoteDetailsActive
        {
            get { return _quoteDetailsActive; }
            set
            {
                _quoteDetailsActive = value;
                RaisePropertyChanged(() => this.QuoteDetailsActive);
            }
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

                RaisePropertyChanged(() => this.CustomerList);

               
            }
        }

        public ObservableCollection<Freight> FreightList
        {
            get
            {
                return _freightList;
            }
            set
            {
                _freightList = value;

                RaisePropertyChanged(() => this.FreightList);
            }
        }
        private bool _creteQuoteActive;
        public bool CreteQuoteActive
        {
             get
             {
               
                return _creteQuoteActive;
             }
             set
             {
                 _creteQuoteActive = value;
                 RaisePropertyChanged(() => this.CreteQuoteActive);
                 
             }
        }

        private bool _qError;
        public bool QError
          {
              get
              {
                  return _qError;
              }
              set
              {
                  _qError = value;
                 
                  RaisePropertyChanged(() => this.QError);
                 
              }
          }
        private bool _selectedProductExists;
        public bool SelectedProductExists
        {
            get
            {
                return _selectedProductExists;
            }
            set
            {
                _selectedProductExists = value;
               
                
                // ReInitializeBindingList();
            }
        }
        
        #endregion


       
        private void UpdateFreightDetails()
        {

            for (int x = 0; x < FreightList.Count; x++)
            {
                if (SelectedFreight == FreightList[x].FreightName)
                {
                    FreightDescription = FreightList[x].FreightDescription;
                    FreightUnit = FreightList[x].FreightUnit;
                    Pallets = Pallets;
                    FreightPrice = FreightList[x].FreightPrice;
                    FreightDis = FreightDis;
                    FreightTotal = FreightTotal;
                    break;                    
                }
            }
            CalculateTotalCost();
        }

        private void UpdateCustomerDetails()
        {
            for (int x = 0; x < CustomerList.Count; x++)
            {
                if (SelectedCustomer == CustomerList[x].CompanyName)
                {
                    Address = CustomerList[x].Address;
                    City = CustomerList[x].City;
                    CusState = CustomerList[x].State;
                    PostCode = CustomerList[x].PostCode;
                    break;
                }
            }
            ISChecked = false;
        }
        

        private void CalculateTotalCost()
        {

            decimal freightDiscount = 0;

            ListPriceTotal = _quoteDetails.Sum(x => x.ProductPrice * x.Quantity);
            DiscountedTotal = _quoteDetails.Sum(x => x.Total);

            FreightTotal = FreightPrice * Pallets;
            freightDiscount = (FreightTotal * FreightDis) / 100;
            FreightTotal = FreightTotal - freightDiscount;

            GST = ((FreightTotal + DiscountedTotal) * 10) / 100;

            TotalAmount = DiscountedTotal + FreightTotal + GST;
          
        }

        private void LoadProducts()
        {
            DataView pdv = new DataView();
            pdv = DBAccess.GetAllProducts().Tables["Products"].DefaultView;
            pdv.Sort = "ProductName ASC";
            Products = new ObservableCollection<Product>();

            for (int x = 0; x < pdv.Count; x++)
            {
                Products.Add(new Product() { ProductID = Convert.ToInt16(pdv[x]["ProductID"]), ProductCode = pdv[x]["ProductCode"].ToString(), ProductName = pdv[x]["ProductName"].ToString(), UnitPrice = Convert.ToDecimal(pdv[x]["ProductPrice"]), ProductDescription = pdv[x]["ProductDescription"].ToString(), ProductUnit = pdv[x]["ProductUnit"].ToString() });
            }
        }

        private void LoadFreights()
        {        
            FreightList = DBAccess.GetFreightData();
        }


        private void LoadCustomers()
        {
            CustomerList = DBAccess.GetCustomerData();
        }

        private void CreateQuote()
        {
            int CustomerID = 0;
            string FirstName = string.Empty;
            string LastName = string.Empty;
            string Telephone = string.Empty;
            string Mobile = string.Empty;
            string Email = string.Empty;
            string Address = string.Empty;
            string City = string.Empty;
            string State = string.Empty;
            string Postcode = string.Empty;

            foreach (var x in CustomerList)
            {
                if (x.CompanyName == SelectedCustomer)
                {
                    CustomerID = x.CustomerId;
                    FirstName = x.FirstName;
                    LastName = x.LastName;
                    Telephone = x.Telephone;
                    Mobile = x.Mobile;
                    Email = x.Email;
                    Address = x.Address;
                    City = x.City;
                    State = x.State;
                    Postcode = x.PostCode;
                }
            }

            Quote newQuote = new Quote();

            newQuote.Prefix = state;
            newQuote.quoteDetails = quoteDetails;
            newQuote.customer = new Customer { CustomerId = CustomerID, CompanyName = SelectedCustomer,FirstName = FirstName,LastName=LastName,Telephone= Telephone,Mobile=Mobile,Email=Email,Address=Address,City=City,State=State,PostCode=Postcode };
            newQuote.QuoteDate = QuoteDate;
            newQuote.ListPriceTot = ListPriceTotal;
            newQuote.SubTotal = DiscountedTotal;
            newQuote.Tax = GST;
            newQuote.TotAmount = TotalAmount;
            newQuote.SalesPerson = UserName;
            newQuote.ProName = ProjectName;
            newQuote.Instructions = Instructions;
            newQuote.InternalComments = InternalComments;
            newQuote.freightDetails = new FreightDetails { FreightName = SelectedFreight, FreightDescription = FreightDescription, FreightPrice = FreightPrice, FreightUnit = FreightUnit, FreightPallets = Pallets, FreightTotal = FreightTotal, FreightDiscount = FreightDis, ShipToAddress = ShipTo };
            
            int NewQuoteNo = DBAccess.InsertQuoteDetails(newQuote);

            newQuote.QuoteID = NewQuoteNo;

            if (NewQuoteNo == 1)
            {
                Msg.Show("An error has occured! Please try again later", "Quote Creation Failed", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);

            }
            else if (NewQuoteNo == -1)
            {
                Msg.Show("Cannot create quote number! Please try again later", "Quote Creation Failed", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else if (NewQuoteNo == -2)
            {
                Msg.Show("Server has encountered a problem! Please try again later", "Quote Creation Failed", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
            else
            {
                UpdateQuotePDF uqpdf = new UpdateQuotePDF();
                bool result = uqpdf.CreateQuote(newQuote);
                if (result)
                {
                    Clear();
                }
            }
        }

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

        private void ShowAddFreightWindow()
        {
            var childWindow = new ChildWindowView();
            childWindow.freight_Closed += (r =>
            {
                string FreightName = r.FreightName;
                string FreightUnit = r.FreightUnit;
                decimal FreightPrice = r.FreightPrice;
                string FreightDescription = r.FreightDescription;

                FreightList.Add(new Freight() { FreightName = FreightName, FreightUnit = FreightUnit, FreightPrice = FreightPrice, FreightDescription = FreightDescription });
               
            });
            childWindow.ShowFreight(1);
        }

        private void Clear()
        {
            quoteDetails.Clear();
            SelectedCustomer = null;
            ProjectName = string.Empty;
            SelectedFreight = null;
            FreightUnit = null;
            Pallets = 0;
            FreightPrice = 0;
            FreightDis = 0;
            FreightDescription = null;
            Instructions = null;
            InternalComments = null;            
            UnitActive = false;
            PalletsActive = false;
            FreightPriceActive = false;
            DiscountActive = false;
            FreightDescActive = false; 
            QuoteDetailsActive = false;
            quoteDetails = new BindingList<QuoteDetails>();     
        }

        private void EnableFreightComponents() 
        {
            if (String.IsNullOrWhiteSpace(SelectedFreight))
            {
                UnitActive = false;
                PalletsActive = false;
                FreightPriceActive = false;
                DiscountActive = false;
                FreightDescActive = false;
                FreightUnit = null;
                Pallets = 0;
                FreightPrice = 0;
                FreightDis = 0;
                FreightDescription = null;
            }
            else
            {
                UnitActive = true;
                PalletsActive = true;
                FreightPriceActive = true;
                DiscountActive = true;
                FreightDescActive = true;
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
            "FreightPrice",
            "DiscountedTotal",
            "ProjectName",
            "SelectedFreight",
            "Pallets",
            "ShipTo",
            "SelectedCustomer"         
           
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
                case "FreightPrice":
                    error = ValidateFreightPrice();
                    break;
                case "DiscountedTotal":
                    error = ValidateDiscountedTotal();                    
                    break;
                case "ProjectName":
                    error = ValidateProjectName();                   
                    break;
                case "SelectedFreight":
                    error = ValidateFreightName();                    
                    break;
                case "Pallets":
                    error = ValidatePallets();                    
                    break;
                case "ShipTo":
                    error = ValidateShipTo();
                    break;
                case "SelectedCustomer":
                    error = ValidateCustomer();
                    break;
            
                default:
                    error = null;
                    throw new Exception("Unexpected property being validated on Service");
            }

            CreteQuoteActive = CheckProperties();

            return error;
        }

        private bool CheckProperties()
        {
            if ((FreightPrice == 0 && SelectedFreight != "Pickup") || (DiscountedTotal <= 0) || ((String.IsNullOrWhiteSpace(ProjectName) || (ProjectAvailable == true))) ||
                (String.IsNullOrWhiteSpace(SelectedFreight)) || (String.IsNullOrWhiteSpace(SelectedCustomer)) || (String.IsNullOrWhiteSpace(ShipTo)) || (Convert.ToDouble(Pallets) == 0 || Pallets.ToString().Length == 0) || QError == true)
            {
            
                return false;
            }
            else 
            {
                return true;
            }
        }
       
      
        private string ValidateFreightPrice()
        {
            if (FreightPrice == 0 && SelectedFreight != "Pickup")
            {
                return "Price required!";
            }

            return null;
        }
   
        private string ValidateDiscountedTotal()
        {
                      
            if(QError){
                return "Quantity required!";
            }
            else if (DiscountedTotal <= 0)
            {
                return "Quote cannot be empty! please add items to the quote!";
            }           
            return null; 
        }

       
        private string ValidateProjectName()
        {
            if (String.IsNullOrWhiteSpace(ProjectName))
            {
                return "Project name required!";
            }
            else if (ProjectAvailable == true)
            {
                return "Project name already exists! Please enter a different name";    
            }
            return null;
        }

        private string ValidateFreightName()
        {
            if (String.IsNullOrWhiteSpace(SelectedFreight))
            {
                return "Freight name required!";
            }
            return null;
        }

        private string ValidateQuote()
        {
            

           
            return null;            
        }

        private string ValidateCustomer()
        {
            if (String.IsNullOrWhiteSpace(SelectedCustomer))
            {
                return "Customer name required!";
            }
            return null;
        }

        private string ValidateShipTo()
        {
            if (String.IsNullOrWhiteSpace(ShipTo))
            {
                if (ISChecked == true)
                {
                    ISChecked = false;
                }

                return "Shipping address required!";
            }
            return null;
        }
        private string ValidatePallets()
        {
            if (Convert.ToDouble(Pallets) == 0 || Pallets.ToString().Length == 0)
            {
                return "Required!";
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

        private void Execute(object parameter)
        {
            int index = quoteDetails.IndexOf(parameter as QuoteDetails);
            if (index > -1 && index < quoteDetails.Count)
            {
                quoteDetails.RemoveAt(index);
            }
            if (quoteDetails.Count == 0)
            {
                quoteDetails = new BindingList<QuoteDetails>(); 
            }

            if (index == quoteDetails.Count)
            {
                ReInitializeBindingList();
            }       
        }

        private void ReInitializeBindingList()
        {
            BindingList<QuoteDetails> qDetails = new BindingList<QuoteDetails>();

            for (int i = 0; i < quoteDetails.Count; i++)
            {
                qDetails.Add(new QuoteDetails() { Quantity = quoteDetails[i].Quantity, ProductID = quoteDetails[i].ProductID, ProductCode = quoteDetails[i].ProductCode, ProductDescription = quoteDetails[i].ProductDescription, ProductPrice = quoteDetails[i].ProductPrice, ProductUnit = quoteDetails[i].ProductUnit, Discount = quoteDetails[i].Discount, Total = quoteDetails[i].Total });
            }

            quoteDetails.Clear();
            quoteDetails = new BindingList<QuoteDetails>();
            for (int i = 0; i < qDetails.Count; i++)
            {
                quoteDetails.Add(new QuoteDetails() { Quantity = qDetails[i].Quantity, ProductID = qDetails[i].ProductID, ProductCode = qDetails[i].ProductCode, ProductDescription = qDetails[i].ProductDescription, ProductPrice = qDetails[i].ProductPrice, ProductUnit = qDetails[i].ProductUnit, Discount = qDetails[i].Discount, Total = qDetails[i].Total });
            }
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }
       

        #region Commands


        public ICommand RemoveCommand
        {
            get
            {
                if (_command == null)
                {
                    _command = new A1QSystem.Commands.DelegateCommand(CanExecute, Execute);
                }
                return _command;
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return backCommand ?? (backCommand = new LogOutCommandHandler(() => Switcher.Switch(new QuotingMainMenu(_userName, state, _privilages, metaData)), canExecute));
            }
        }
        public ICommand CreateCommand
        {
            get
            {
               if(createCommand == null)
                 createCommand = new A1QSystem.Commands.RelayCommand(param => this.CreateQuote(), param => this.CanSave);
              
               return createCommand;
            }
        }

        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => CheckQuoteForHomeName(), canExecute));
            }
        }
        public ICommand NavQuotingCommand
        {
            get
            {
                return navQuotingCommand ?? (navQuotingCommand = new LogOutCommandHandler(() => CheckQuoteForQuoting(), canExecute));
            }
        }

        public ICommand ClearFields
        {
            get
            {
                return clearFields ?? (clearFields = new LogOutCommandHandler(() => Clear(), canExecute));
            }
        }
        public Microsoft.Practices.Prism.Commands.DelegateCommand ShowChildWindowCommand
        {
            get { return showChildWindowCommand; }
        }
        public Microsoft.Practices.Prism.Commands.DelegateCommand ShowAddFreightCommand
        {
            get { return _showAddFreightCommand; }
        }

        #endregion

        private void CheckQuoteForHomeName()
        {
            if (!String.IsNullOrWhiteSpace(SelectedCustomer))
            {
                if (Msg.Show("Are you sure you want to navigate to HOME screen?" + System.Environment.NewLine + "You haven't finished updaing quote!", "Naivgation Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    Switcher.Switch(new MainMenu(_userName, state, _privilages, metaData));
                }
            }
            else
            {
                Switcher.Switch(new MainMenu(_userName, state, _privilages, metaData));
            }
        }
        private void CheckQuoteForQuoting()
        {
            if (!String.IsNullOrWhiteSpace(SelectedCustomer))
            {
                if (Msg.Show("Are you sure you want to navigate to QUOTING MENU screen?" + System.Environment.NewLine + "You haven't finished updaing quote!", "Naivgation Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    Switcher.Switch(new QuotingMainMenu(_userName, state, _privilages, metaData));
                }
            }
            else
            {
                Switcher.Switch(new QuotingMainMenu(_userName, state, _privilages, metaData));
            }
        }
    }
}
