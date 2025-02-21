using A1QSystem.DB;
using A1QSystem.Model;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBox;
using System.ComponentModel;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace A1QSystem.ViewModel
{
    public class AddCustomerViewModel : BaseViewModel, IDataErrorInfo
    {
        #region Events

        public event Action<Customer> Closed;
        private Regex emailRegex = new Regex(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$");
        private Regex numberRegex = new Regex(@"^[0-9]+$");  

        #endregion


        public AddCustomerViewModel(int personId)
        {
            persons = new List<Customer>()
            {
                new Customer(){}               
            };

           // okCommand = new DelegateCommand(SavePerson);
            _closeCommand = new DelegateCommand(CloseForm);
            _clearCommand = new DelegateCommand(ClearForm);

            this.PersonId = personId;

         //   Init();
        }


        #region Public Properties

        public int PersonId { get; set; }

        private string companyName;
        public string CompanyName
        {
            get { return companyName; }
            set
            {
                companyName = value;
                RaisePropertyChanged("CompanyName");
            }
        }

        private string firstName;
        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
                RaisePropertyChanged("FirstName");
            }
        }

        private string lastName;
        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;
                RaisePropertyChanged("LastName");
            }
        }

        private string telephone;
        public string Telephone
        {
            get { return telephone; }
            set
            {
                telephone = value;
                RaisePropertyChanged("Telephone");
            }
        }

        private string mobile;
        public string Mobile
        {
            get { return mobile; }
            set
            {
                mobile = value;
                RaisePropertyChanged("Mobile");
            }
        }
        
        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                RaisePropertyChanged("Email");
            }
        }
        
        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                RaisePropertyChanged("Address");
            }
        }

        private string city;
        public string City
        {
            get { return city; }
            set
            {
                city = value;
                RaisePropertyChanged("City");
            }
        }

        private string state;
        public string State
        {
            get { return state; }
            set
            {
                state = value;
                RaisePropertyChanged("State");
            }
        }

        private string postCode;
        public string PostCode
        {
            get { return postCode; }
            set
            {
                postCode = value;
                RaisePropertyChanged("PostCode");
            }
        }
       

        private List<Customer> persons;
        public List<Customer> Persons
        {
            get { return persons; }
            set { persons = value; }
        }

        private bool IsValidEmailAddress
        {
            get { return emailRegex.IsMatch(Email); }
        }

        private bool IsValidTelephone
        {            
            get 
            {
                return numberRegex.IsMatch(Telephone);
            }
        }
        private bool IsValidMobile
        {
            get
            {
                return numberRegex.IsMatch(Mobile);
            }
        }
        private bool IsValidPostCode
        {
            get
            {
                return numberRegex.IsMatch(PostCode);
            }
        }

        #endregion

        #region Command Properties

        private ICommand okCommand;
        public ICommand OkCommand
        {
            get
            {
                if (okCommand == null)
                    okCommand = new A1QSystem.Commands.RelayCommand(param => this.SavePerson(), param => this.CanSave);

                return okCommand;
            }
        }

     /*   private DelegateCommand okCommand;
        public DelegateCommand OkCommand
        {
            get { return okCommand; }
        }
        */
        private DelegateCommand _closeCommand;
        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        private DelegateCommand _clearCommand;
        public DelegateCommand ClearCommand
        {
            get { return _clearCommand; }
        }
        

        #endregion

        #region Private Methods
        private void SavePerson()
        {
            if (Closed != null)
            {
                

                int res = DBAccess.CheckCustomerAvailable(CompanyName);

                if (res < 1)
                {
                    Customer newCustomer = new Customer();
                    newCustomer.CompanyName = CompanyName;
                    newCustomer.FirstName = FirstName;
                    newCustomer.LastName = LastName;
                    newCustomer.Telephone = Telephone;
                    newCustomer.Mobile = Mobile;
                    newCustomer.Email = Email;
                    newCustomer.Address = Address;
                    newCustomer.City = City;
                    newCustomer.State = State;
                    newCustomer.PostCode = PostCode;

                    int result = DBAccess.InsertCustomerDetails(newCustomer);

                    if (result > 0)
                    {
                        Msg.Show("Customer added successfully!", "Customer Added", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);

                    }
                    else
                    {
                        Msg.Show("Something went wrong and details haven't added to the database! Please try again later", "Data cannot save", MsgBoxButtons.OK, MsgBoxImage.Alert, MsgBoxResult.Yes);
                    }

                    var customer = new Customer()
                    {
                        CustomerId = result,
                        CompanyName = companyName,
                        FirstName = firstName,
                        LastName = lastName,
                        Address = address,
                        Email = email,
                        City = city,
                        State = state,
                        PostCode = postCode,
                        Mobile = mobile,
                        Telephone = telephone                       
                    };

                    Closed(customer);

                }
                else
                {
                    Msg.Show("Customer already exists in the database!", "Customer Exists!", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                }
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                var customer = new Customer();
                

                Closed(customer);
            }
        }

        private void ClearForm()
        {
            CompanyName = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Telephone = string.Empty;
            Mobile = string.Empty;
            Email = string.Empty;
            Address = string.Empty;
            City = string.Empty;
            State = string.Empty;
            PostCode = string.Empty;

        }

        private void Init()
        {
            var customer = persons.FirstOrDefault(p => p.CustomerId == PersonId);
            FirstName = customer.FirstName;
            RaisePropertyChanged("FirstName");
            LastName = customer.LastName;
            RaisePropertyChanged("LastName");
            Email = customer.Email;
            RaisePropertyChanged("Email");
            Address = customer.Address;
            RaisePropertyChanged("Address");
        }

        #endregion



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
            "CompanyName",
            "FirstName",
            "LastName",
            "Telephone",
            "Mobile",
            "Email",
            "PostCode"
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
                case "CompanyName":
                    error = ValidateCompanyName();
                    break;
                case "FirstName":
                    error = ValidateFirstName();
                    break;
                case "LastName":
                    error = ValidateLastName();
                    break;
                case "Telephone":
                    error = ValidateTelephone();
                    break;
                case "Email":
                    error = ValidateEmail();
                    break;
                case "Mobile":
                    error = ValidateMobile();
                    break;
                case "PostCode":
                    error = ValidatePostCode();
                    break;
                default:
                    error = null;
                    throw new Exception("Unexpected property being validated on Service");
            }
            return error;
        }


        private string ValidateCompanyName()
        {
            if (String.IsNullOrWhiteSpace(CompanyName))
            {
                return "Company name required!";
            }

            return null;
        }

        private string ValidateFirstName()
        {
            if (String.IsNullOrWhiteSpace(FirstName))
            {
                return "First name required!";
            }
            return null;
        }

        private string ValidateLastName()
        {
            if (String.IsNullOrWhiteSpace(LastName))
            {
                return "Last name required!";
            }
            return null;
        }

        private string ValidateEmail()
        {
            if (String.IsNullOrEmpty(Email))
            {
                return "Email required!";
            }
            else if (!IsValidEmailAddress)
            {

                 return "A valid email required!";
            }
            return null;
        }

        private string ValidateTelephone()
        {
            if (!String.IsNullOrEmpty(Telephone))
            {
                if (!IsValidTelephone)
                {
                    return "Invalid telephone number!";
                }
            }            
            return null;
        }

        private string ValidateMobile()
        {
            if (!String.IsNullOrEmpty(Mobile))
            {
                if (!IsValidMobile)
                {
                    return "Invalid mobile number!";
                }
            }
            return null;
        }

        private string ValidatePostCode()
        {
            if (!String.IsNullOrEmpty(PostCode))
            {
                if (!IsValidPostCode)
                {
                    return "Invalid PostCode!";
                }
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

    }
}
