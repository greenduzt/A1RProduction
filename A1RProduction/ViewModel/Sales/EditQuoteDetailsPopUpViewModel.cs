
using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.PDFGeneration;
using A1QSystem.View;
using A1QSystem.View.Quoting;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.ViewModel.Sales
{
    public class EditQuoteDetailsPopUpViewModel : ViewModelBase, IDataErrorInfo
    {
        private BindingList<QuoteDetails> _quoteDetails;
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Freight> FreightDetails { get; set; }
        private ObservableCollection<string> _quoteNos;
        private ObservableCollection<Customer> _customerList;

        private string _quoteNo;
        private string _quoteDate;
        private string _selectedQuoteNo;
        private string _userName;
        private string _state;
       

        private decimal _listPriceTotal;
        private decimal _discountedTotal;
        private decimal _gST;
        private decimal _freightTotal;
        private decimal _totalAmount;
        private string _freightName;
        
        public event Action<QuoteDetails> Closed;
        private ICommand _updateCommand;
        private ICommand _clearCommand;
        private ICommand _searchQuoteCommand;
        ICommand _command;
        private Microsoft.Practices.Prism.Commands.DelegateCommand showChildWindowCommand;
        private Microsoft.Practices.Prism.Commands.DelegateCommand _showAddFreightCommand;
        private Microsoft.Practices.Prism.Commands.DelegateCommand _closeCommand;

        private bool _canExecute;


        public EditQuoteDetailsPopUpViewModel(int QuoteNo, string UserName, string State)
        {
            _closeCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(CloseForm);

            _quoteDetails = new BindingList<QuoteDetails>();
            PriceReadOnly = true;
            _userName = UserName;
            _state = State;

            SelectedQuoteNo = State+QuoteNo.ToString();

            LoadQuoteNumbers();
            LoadProjectNames();
            LoadProducts();
            LoadFreights();
            LoadCustomers();

           _canExecute = true;

           showChildWindowCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(ShowChildWindow);
           _showAddFreightCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(ShowAddFreightWindow);
           _canExecute = true;
           BtnAddCustomerActive = false;
           BtnAddFreightActive = false;

    
            SearchQuoteByID(QuoteNo);

        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                var qd = new QuoteDetails();

                Closed(qd);
            }
        }

     

        #region PUBLIC PROPERTIES


        public ObservableCollection<string> QuoteNos
        {
            get { return _quoteNos; }
            set
            {
                _quoteNos = value;
                RaisePropertyChanged(() => this.QuoteNos);
            }
        }
        private ObservableCollection<string> _projectNames;
        public ObservableCollection<string> ProjectNames
        {
            get { return _projectNames; }
            set
            {
                _projectNames = value;
                RaisePropertyChanged(() => this.ProjectNames);
            }
        }
        private ObservableCollection<string> _cusProjectNames;
        public ObservableCollection<string> CusProjectNames
        {
            get { return _cusProjectNames; }
            set
            {
                _cusProjectNames = value;
                RaisePropertyChanged(() => this.CusProjectNames);
            }
        }

        public string QuoteNo
        {
            get { return _quoteNo; }
            set
            {
                _quoteNo = value;
                RaisePropertyChanged(() => this.QuoteNo);
            }
        }

        public string QuoteDate
        {
            get
            {
                _quoteDate = DateTime.Now.ToString("dd/MM/yyyy");
                return _quoteDate;
            }
            set
            {
                _quoteDate = value;
                RaisePropertyChanged(() => this.QuoteDate);
            }
        }

        public string SelectedQuoteNo
        {
            get
            {
                return _selectedQuoteNo;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(SelectedProjectName) && !string.IsNullOrWhiteSpace(SelectedSearchedCustomerName))
                {
                    SelectedProjectName = string.Empty;
                    SelectedSearchedCustomerName = string.Empty;
                    Clear();
                }

                _selectedQuoteNo = value;
                RaisePropertyChanged(() => this.SelectedProjectName);
                RaisePropertyChanged(() => this.SelectedQuoteNo);
                RaisePropertyChanged(() => this.SelectedSearchedCustomerName);
            }
        }
        private string _selectedProjectName;
        public string SelectedProjectName
        {
            get
            {
                return _selectedProjectName;
            }
            set
            {
                _selectedProjectName = value;

                if (!string.IsNullOrWhiteSpace(SelectedQuoteNo) && !string.IsNullOrWhiteSpace(SelectedSearchedCustomerName))
                {
                    SelectedQuoteNo = string.Empty;
                    SelectedSearchedCustomerName = string.Empty;
                    Clear();
                }
                RaisePropertyChanged(() => this.SelectedProjectName);
                RaisePropertyChanged(() => this.SelectedQuoteNo);
                RaisePropertyChanged(() => this.SelectedSearchedCustomerName);
            }
        }
        private string _selectedSearchedCustomerName;
        public string SelectedSearchedCustomerName
        {
            get
            {
                return _selectedSearchedCustomerName;
            }
            set
            {
                _selectedSearchedCustomerName = value;

                if (!string.IsNullOrWhiteSpace(SelectedQuoteNo) && !string.IsNullOrWhiteSpace(SelectedProjectName))
                {
                    SelectedQuoteNo = string.Empty;
                    SelectedProjectName = string.Empty;
                    Clear();
                }
                RaisePropertyChanged(() => this.SelectedProjectName);
                RaisePropertyChanged(() => this.SelectedQuoteNo);
                RaisePropertyChanged(() => this.SelectedSearchedCustomerName);

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

        public BindingList<QuoteDetails> QuoteDetails
        {
            get { return _quoteDetails; }
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
                }

                RaisePropertyChanged(() => this.QuoteDetails);
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
                _qError = QuoteDetails.Any(i => i.Quantity.Equals(0));
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

                UpdateFreightDetails();
                FreightDis = 0;
                Pallets = 0;

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
                if (!String.IsNullOrWhiteSpace(QuoteNo))
                {
                    int newQuoteNo = int.Parse(Regex.Replace(QuoteNo, "[^0-9.]", ""));
                    EnteredProjectName = DBAccess.CheckProjectNameWithoutCurrentQuote(ProjectName, newQuoteNo);

                }
            }
        }
        private bool _enteredProjectName;
        public bool EnteredProjectName
        {
            get
            {
                return _enteredProjectName;
            }
            set
            {
                _enteredProjectName = value;
                RaisePropertyChanged(() => this.EnteredProjectName);
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

        private string _selectedCustomer;
        public string SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                RaisePropertyChanged(() => this.SelectedCustomer);

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

        private bool _freightNameActive;
        public bool FreightNameActive
        {
            get { return _freightNameActive; }
            set
            {
                _freightNameActive = value;
                RaisePropertyChanged(() => this.FreightNameActive);
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
        private bool _instructionsActive;
        public bool InstructionsActive
        {
            get { return _instructionsActive; }
            set
            {
                _instructionsActive = value;
                RaisePropertyChanged(() => this.InstructionsActive);
            }
        }
        private bool _intCommActive;
        public bool IntCommActive
        {
            get { return _intCommActive; }
            set
            {
                _intCommActive = value;
                RaisePropertyChanged(() => this.IntCommActive);
            }
        }
        private bool _customerActive;
        public bool CustomerActive
        {
            get { return _customerActive; }
            set
            {
                _customerActive = value;
                RaisePropertyChanged(() => this.CustomerActive);
            }
        }

        private bool _shipToTickActive;
        public bool ShipToTickActive
        {
            get { return _shipToTickActive; }
            set
            {
                _shipToTickActive = value;
                RaisePropertyChanged(() => this.ShipToTickActive);
            }
        }

        private bool _shipToActive;
        public bool ShipToActive
        {
            get { return _shipToActive; }
            set
            {
                _shipToActive = value;
                RaisePropertyChanged(() => this.ShipToActive);
            }
        }
        private bool _quoteNoSearchedActive;
        public bool QuoteNoSearchedActive
        {
            get { return _quoteNoSearchedActive; }
            set
            {
                _quoteNoSearchedActive = value;
                RaisePropertyChanged(() => this.QuoteNoSearchedActive);
            }
        }

        private bool _salesPersonActive;
        public bool SalesPersonActive
        {
            get { return _salesPersonActive; }
            set
            {
                _salesPersonActive = value;
                RaisePropertyChanged(() => this.SalesPersonActive);
            }
        }

        private bool _proNameActive;
        public bool ProNameActive
        {
            get { return _proNameActive; }
            set
            {
                _proNameActive = value;
                RaisePropertyChanged(() => this.ProNameActive);
            }
        }
        private bool _dateActive;
        public bool DateActive
        {
            get { return _dateActive; }
            set
            {
                _dateActive = value;
                RaisePropertyChanged(() => this.DateActive);
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


        private bool _btnAddCustomerActive;
        public bool BtnAddCustomerActive
        {
            get
            {
                return _btnAddCustomerActive;
            }
            set
            {
                _btnAddCustomerActive = value;
                RaisePropertyChanged(() => this.BtnAddCustomerActive);
            }
        }

        private bool _btnAddFreightActive;
        public bool BtnAddFreightActive
        {
            get
            {
                return _btnAddFreightActive;
            }
            set
            {
                _btnAddFreightActive = value;
                RaisePropertyChanged(() => this.BtnAddFreightActive);
            }
        }

        private bool _updateQuoteActive;
        public bool UpdateQuoteActive
        {
            get
            {
                return _updateQuoteActive;
            }
            set
            {
                _updateQuoteActive = value;
                RaisePropertyChanged(() => this.UpdateQuoteActive);
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
       
        #endregion

        private void UpdateFreightDetails()
        {

           for (int x = 0; x < FreightDetails.Count; x++)
            {
                if (SelectedFreight == FreightDetails[x].FreightName)
                {
                    FreightDescription = FreightDetails[x].FreightDescription;
                    FreightUnit = FreightDetails[x].FreightUnit;
                    Pallets = Pallets;
                    FreightPrice = FreightDetails[x].FreightPrice;
                    FreightDis = FreightDis;
                    FreightTotal = FreightTotal;
                    break;
                }                
            }
           CalculateTotalCost();
          
        }

        private void SearchQuote()
        {
           
            bool executeConn = false;
            int newQuoteNo = 0;
            ClearFreightDetails();
            ProjectName = String.Empty;

            if (String.IsNullOrWhiteSpace(SelectedProjectName) && !String.IsNullOrWhiteSpace(SelectedQuoteNo) && String.IsNullOrWhiteSpace(SelectedSearchedCustomerName))
            {
                executeConn = true;
                newQuoteNo = int.Parse(Regex.Replace(SelectedQuoteNo, "[^0-9.]", ""));
            }
            else if (!String.IsNullOrWhiteSpace(SelectedProjectName) && String.IsNullOrWhiteSpace(SelectedQuoteNo) && String.IsNullOrWhiteSpace(SelectedSearchedCustomerName))
            {
                string NewProjectName = SelectedProjectName.Substring(0, SelectedProjectName.LastIndexOf(" [") + 1);

                newQuoteNo = DBAccess.GetQuoteNoByProName(NewProjectName);               
                executeConn = true;               
            }
            else if (!String.IsNullOrWhiteSpace(SelectedSearchedCustomerName) && String.IsNullOrWhiteSpace(SelectedQuoteNo) && String.IsNullOrWhiteSpace(SelectedProjectName))
            {
                string proName = Regex.Match(SelectedSearchedCustomerName, @"\[([^]]*)\]").Groups[1].Value;

                newQuoteNo = DBAccess.GetQuoteNoByProName(proName);
                executeConn = true;
            }
            else
            {
                executeConn = false;               
            }
            
            if (executeConn)
            {
                SearchQuoteByID(newQuoteNo);
            }
            else
            {
                Msg.Show("Quote not available!."+Environment.NewLine+"Please select the Quote No or Project Name or Customer Name from the drop down and click SEARCH.", "Quote Not Found", MsgBoxButtons.OK, MsgBoxImage.Alert, MsgBoxResult.Yes);
                Clear();
            }
        }

        private void SearchQuoteByID(int newQuoteNo)
        {
            QuoteDetails = new BindingList<QuoteDetails>();

            QuoteDetails = DBAccess.SearchQuote(newQuoteNo);
            if (QuoteDetails.Count > 0)
                {
        

                ActivateComponents();

                DataView fd = new DataView();
                fd = DBAccess.GetFreightDetailsByCode(newQuoteNo).Tables["FreightDetails"].DefaultView;

                if (fd.Count > 0)
                {
                    for (int x = 0; x < fd.Count; x++)
                    {
                        SelectedFreight = fd[x]["FreightName"].ToString();
                        FreightUnit = fd[x]["FreightUnit"].ToString();
                        Pallets = Convert.ToDecimal(fd[x]["FreightPallets"]);
                        FreightPrice = Convert.ToDecimal(fd[x]["FreightPrice"]);
                        FreightDis = Convert.ToDecimal(fd[x]["FreightDiscount"]);
                        FreightDescription = fd[x]["FreightDescription"].ToString();
                        FreightTotal = Convert.ToDecimal(fd[x]["FreightTotal"]);
                    }
                }
                else
                {
                    Msg.Show("Your searched quote does not contain any Freight details." + System.Environment.NewLine + "Please add Freight details to your quote.", "Freight Details Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }

                DataView q = new DataView();
                q = DBAccess.GetQuoteByCode(newQuoteNo).Tables["Quotes"].DefaultView;

                if (q.Count > 0)
                {
                    for (int x = 0; x < q.Count; x++)
                    {
                        QuoteNo = Regex.Replace(q[x]["Prefix"].ToString() + q[x]["ID"].ToString(), @"\s+", "");
                        QuoteDate = q[x]["QuoteDate"].ToString();
                        SelectedCustomer = q[x]["CustomerName"].ToString();
                        ProjectName = q[x]["ProName"].ToString();
                        Instructions = q[x]["Instructions"].ToString();
                        InternalComments = q[x]["InternalComments"].ToString();
                    }

                    if (!String.IsNullOrWhiteSpace(SelectedCustomer))
                    {
                        ISChecked = true;
                    }
                    else
                    {
                        ISChecked = false;
                    }
                }
                else
                {
                    Msg.Show("Quote details not available. Please try again later.", "Quote Details Not Found", MsgBoxButtons.OK, MsgBoxImage.Alert, MsgBoxResult.Yes);

                }
                CalculateTotalCost();
            }
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
            DataView dvF = new DataView();
            dvF = DBAccess.GetFreightNames().Tables["Freight"].DefaultView;
            dvF.Sort = "FreightName asc";
            FreightDetails = new ObservableCollection<Freight>();

            for (int x = 0; x < dvF.Count; x++)
            {
                FreightDetails.Add(new Freight() { FreightName = dvF[x]["FreightName"].ToString(), FreightUnit = dvF[x]["FreightUnit"].ToString(), FreightPrice = Convert.ToDecimal(dvF[x]["FreightPrice"]), FreightDescription = dvF[x]["FreightDescription"].ToString() });
            }
        }

        private void LoadQuoteNumbers()
        {
            QuoteNos = DBAccess.GetAllQuoteNos();
        }

        private void LoadProjectNames()
        {
            DataView pdv = new DataView();
            pdv = DBAccess.GetAllProjectNames().Tables["Quotes"].DefaultView;
            pdv.Sort = "ProName ASC";

            var proNames = new ObservableCollection<string>();
            var cusNames = new ObservableCollection<string>();

            for (int x = 0; x < pdv.Count; x++)
            {
                proNames.Add(pdv[x]["ProName"].ToString() + " [" + pdv[x]["CustomerName"].ToString() + "]");
            }
            ProjectNames = proNames;

            pdv.Sort = "CustomerName ASC";
            for (int x = 0; x < pdv.Count; x++)
            {
                cusNames.Add(pdv[x]["CustomerName"].ToString() + " [" + pdv[x]["ProName"].ToString() + "]");
            }
            CusProjectNames = cusNames;
        }

        private void LoadCustomers()
        {
            CustomerList = DBAccess.GetCustomerData();
        }

        private void UpdateQuote()
        {

            int newQuoteNo = int.Parse(Regex.Replace(QuoteNo, "[^0-9.]", ""));
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

            Quote updateQuote = new Quote();
            updateQuote.QuoteID = newQuoteNo;
            updateQuote.Prefix = _state;
            updateQuote.quoteDetails = QuoteDetails;
            updateQuote.QuoteDate = QuoteDate;
            updateQuote.FreightTotal = FreightTotal;
            updateQuote.ListPriceTot = ListPriceTotal;
            updateQuote.SubTotal = DiscountedTotal;
            updateQuote.Tax = GST;
            updateQuote.TotAmount = TotalAmount;
            updateQuote.SalesPerson = UserName;
            updateQuote.ProName = ProjectName;
            updateQuote.Instructions = Instructions;
            updateQuote.InternalComments = InternalComments;
            updateQuote.customer = new Customer { CustomerId = CustomerID, CompanyName = SelectedCustomer, FirstName = FirstName, LastName = LastName, Telephone = Telephone, Mobile = Mobile, Email = Email, Address = Address, City = City, State = State, PostCode = Postcode };
            updateQuote.freightDetails = new FreightDetails { FreightName = SelectedFreight, FreightDescription = FreightDescription, FreightPrice = FreightPrice, FreightUnit = FreightUnit, FreightPallets = Pallets, FreightTotal = FreightTotal, FreightDiscount = FreightDis, ShipToAddress = ShipTo };

            int res = DBAccess.UpdateQuote(updateQuote);

            if (res == 1)
            {
                UpdateQuotePDF uqpdf = new UpdateQuotePDF();

                bool result = uqpdf.CreateQuote(updateQuote);

                if (result)
                {
                    Clear();
                    LoadProjectNames();
                    CloseForm();
                }
            }
            else
            {
                Msg.Show("This quote cannot be updated at this moment" + System.Environment.NewLine + "Please try again later", "Quote Update Failed", MsgBoxButtons.OK, MsgBoxImage.Process_Stop, MsgBoxResult.Yes);
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

                FreightDetails.Add(new Freight() { FreightName = FreightName, FreightUnit = FreightUnit, FreightPrice = FreightPrice, FreightDescription = FreightDescription });
            });
            childWindow.ShowFreight(1);
        }
        private void ActivateComponents()
        {
            UnitActive = true;
            PalletsActive = true;
            FreightPriceActive = true;
            FreightPriceActive = true;
            DiscountActive = true;
            FreightDescActive = true;
            InstructionsActive = true;
            IntCommActive = true;
            CustomerActive = true;
            ShipToTickActive = true;
            ShipToActive = true;
            QuoteNoSearchedActive = true;
            SalesPersonActive = true;
            ProNameActive = true;
            DateActive = true;
            QuoteDetailsActive = true;
            BtnAddCustomerActive = true;
            BtnAddFreightActive = true;
            FreightNameActive = true;
        }

        private void ClearFreightDetails()
        {
            SelectedFreight = null;
            FreightUnit = null;
            Pallets = 0;
            FreightPrice = 0;
            FreightDis = 0;
            FreightDescription = null;
        }

        private void Clear()
        {
            QuoteDetails.Clear();
            SelectedCustomer = null;
            ProjectName = string.Empty;
            QuoteNo = null;
            SelectedFreight = null;
            FreightUnit = null;
            Pallets = 0;
            FreightPrice = 0;
            FreightDis = 0;
            FreightDescription = null;
            Instructions = null;
            InternalComments = null;
            FreightNameActive = false;
            UnitActive = false;
            PalletsActive = false;
            FreightPriceActive = false;
            DiscountActive = false;
            FreightDescActive = false;
            InstructionsActive = false;
            IntCommActive = false;
            CustomerActive = false;
            ShipToTickActive = false;
            ShipToActive = false;
            QuoteNoSearchedActive = false;
            SalesPersonActive = false;
            ProNameActive = false;
            DateActive = false;
            QuoteDetailsActive = false;
            BtnAddFreightActive = false;
            BtnAddCustomerActive = false;

            if (!string.IsNullOrWhiteSpace(SelectedQuoteNo))
            {
                SelectedQuoteNo = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(SelectedProjectName))
            {
                SelectedProjectName = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(SelectedSearchedCustomerName))
            {
                SelectedSearchedCustomerName = string.Empty;
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
            "SelectedQuoteNo",
            "SelectedFreight",
            "FreightPrice",
            "DiscountedTotal",
            "Pallets",
            "ShipTo",
            "SelectedCustomer",
            "SelectedProjectName",
            "ProjectName"
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
                case "SelectedQuoteNo":
                case "SelectedProjectName":
                case "SelectedSearchedCustomerName":
                    error = ValidateSearchFields();
                    break;
                case "SelectedFreight":
                    error = ValidateFreigthName();
                    break;
                case "DiscountedTotal":
                    error = ValidateQuote();
                    break;
                case "FreightPrice":
                    error = ValidateFreightPrice();
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
                case "ProjectName":
                    error = ValidateEntetedProjectName();
                    break;
                default:
                    error = null;
                    throw new Exception("Unexpected property being validated on Service");
            }

            UpdateQuoteActive = CheckProperties();

            return error;
        }


        private bool CheckProperties()
        {
            if ((EnteredProjectName == true) ||
                (String.IsNullOrWhiteSpace(SelectedQuoteNo) && String.IsNullOrWhiteSpace(SelectedProjectName) && String.IsNullOrWhiteSpace(SelectedSearchedCustomerName)) ||
                (String.IsNullOrWhiteSpace(SelectedFreight)) || (FreightPrice == 0 && SelectedFreight != "Pickup") ||
                (DiscountedTotal <= 0) || (String.IsNullOrWhiteSpace(SelectedCustomer)) || (String.IsNullOrWhiteSpace(ShipTo)) || (Convert.ToDouble(Pallets) == 0 || Pallets.ToString().Length == 0) || QError == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private string ValidateEntetedProjectName()
        {
            if (EnteredProjectName == true)
            {
                return "This project name is already taken!";
            }

            return null;
        }

        private string ValidateSearchFields()
        {

            if (String.IsNullOrWhiteSpace(SelectedQuoteNo) && String.IsNullOrWhiteSpace(SelectedProjectName) && String.IsNullOrWhiteSpace(SelectedSearchedCustomerName))
            {
                //Clear();
                return "Quote number/project name/customer name required!";
            }

            return null;
        }

        private string ValidateFreigthName()
        {
            if (String.IsNullOrWhiteSpace(SelectedFreight))
            {
                return "Freight name required!";
            }

            return null;
        }

        private string ValidateFreightPrice()
        {
            if (FreightPrice == 0 && SelectedFreight != "Pickup")
            {
                return "Price required!";
            }

            return null;
        }

        private string ValidateProjectName()
        {
            if (String.IsNullOrWhiteSpace(SelectedProjectName) && String.IsNullOrWhiteSpace(SelectedQuoteNo))
            {
                return "Project name required!";
            }
            return null;
        }

        private string ValidateQuote()
        {
            if (QError)
            {
                return "Quantity required!";
            }
            else if (DiscountedTotal <= 0)
            {
                return "Quote cannot be empty! please add items to the quote!";
            }

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


        private string ValidateQuoteNoSearchDB()
        {
            return "Invalid quote number!";
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


        #region COMMANDS

        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new LogOutCommandHandler(() => Clear(), _canExecute));
            }
        }

        public ICommand UpdateCommand
        {
            get
            {
                if (_updateCommand == null)
                    _updateCommand = new A1QSystem.Commands.RelayCommand(param => this.UpdateQuote(), param => this.CanSave);
                return _updateCommand;
            }
        }

        public Microsoft.Practices.Prism.Commands.DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }


        public ICommand SearchQuoteCommand
        {
            get
            {
                return _searchQuoteCommand ?? (_searchQuoteCommand = new LogOutCommandHandler(() => SearchQuote(), _canExecute));
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

        #endregion

        private void Execute(object parameter)
        {
            int index = QuoteDetails.IndexOf(parameter as QuoteDetails);
            if (index > -1 && index < QuoteDetails.Count)
            {
                QuoteDetails.RemoveAt(index);
            }
            if (QuoteDetails.Count == 0)
            {
                QuoteDetails = new BindingList<QuoteDetails>();
            }


        }

        private bool CanExecute(object parameter)
        {
            return true;
        }
        
    
    }
}
