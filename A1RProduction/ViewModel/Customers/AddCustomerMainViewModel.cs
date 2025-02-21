using A1QSystem.Commands;
using A1QSystem.DB;
using A1QSystem.View.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using MsgBox;
using Microsoft.Practices.Prism.Commands;
using A1QSystem.Model;
using A1QSystem.View;
using A1QSystem.Model.Meta;

namespace A1QSystem.ViewModel.Customers
{
    public class AddCustomerMainViewModel : BaseViewModel, IDataErrorInfo
    {
        private string _userName;
        private string _state;
        private List<UserPrivilages> _privilages;
        private List<MetaData> metaData;

        private Regex emailRegex = new Regex(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$");
        private Regex numberRegex = new Regex(@"^[0-9]+$");  

        private bool _canExecute;

        private ICommand _addCommand;
        private ICommand _clearCommand;
        private ICommand _backCommand;
        private ICommand navHomeCommand;
        private ICommand navCustomerCommand;

        public AddCustomerMainViewModel(string UserName, string State, List<UserPrivilages> Privilages, List<MetaData> md)
        {

            _userName = UserName;
            _state = State;
            _privilages = Privilages;
            metaData = md;
            _canExecute = true;

        }

        private void AddCustomer()
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
                    ClearData();

                }
                else
                {
                    Msg.Show("Something went wrong and details haven't added to the database! Please try again later", "Data cannot save", MsgBoxButtons.OK, MsgBoxImage.Alert, MsgBoxResult.Yes);
                }
            }
            else
            {
                Msg.Show("Customer already exists in the database!", "Customer did not add to the database", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }
        }
       
        private void ClearData()
        {
            CompanyName = "";
            FirstName = "";
            LastName = "";
            Telephone = "";
            Mobile = "";
            Email = "";
            Address = "";
            City = "";
            State = "";
            PostCode = "";
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
       
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                RaisePropertyChanged("UserName");
            }
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

        #endregion

        private void CheckCustomerForHomeName()
        {
            if (!String.IsNullOrWhiteSpace(CompanyName))
            {
                if (Msg.Show("Are you sure you want to navigate to HOME screen?" + System.Environment.NewLine + "You haven't finished filling all the fields!", "Naivgation Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    Switcher.Switch(new MainMenu(_userName, state, _privilages, metaData));
                }
            }
            else
            {
                Switcher.Switch(new MainMenu(_userName, state, _privilages, metaData));
            }
        }
        private void CheckCustomerForCustomerName()
        {
            if (!String.IsNullOrWhiteSpace(CompanyName))
            {
                if (Msg.Show("Are you sure you want to navigate to CUSTOMERS screen?" + System.Environment.NewLine + "You haven't finished filling all the fields!", "Naivgation Confirmation", MsgBoxButtons.YesNo, MsgBoxImage.Question, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                {
                    Switcher.Switch(new CustomersMenuView(_userName, state, _privilages, metaData));
                }
            }
            else
            {
                Switcher.Switch(new CustomersMenuView(_userName, state, _privilages, metaData));
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
            "CompanyName",
            "FirstName",
            "LastName",
            "Telephone",
            "Mobile",
            "Email",
            "PostCode"
        };

       

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

       
        #endregion


        #region COMMANDS
                 

        public ICommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                    _addCommand = new A1QSystem.Commands.RelayCommand(param => this.AddCustomer(), param => this.CanSave);

                return _addCommand;
            }
        }

        public ICommand ClearCommand
        {
            get
            {
                return _clearCommand ?? (_clearCommand = new LogOutCommandHandler(() => ClearData(), _canExecute));
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new LogOutCommandHandler(() => Switcher.Switch(new CustomersMenuView(_userName, _state, _privilages, metaData)), _canExecute));
            }
        }
       
        public ICommand NavHomeCommand
        {
            get
            {
                return navHomeCommand ?? (navHomeCommand = new LogOutCommandHandler(() => CheckCustomerForHomeName(), _canExecute));
            }
        }
        public ICommand NavCustomerCommand
        {
            get
            {
                return navCustomerCommand ?? (navCustomerCommand = new LogOutCommandHandler(() => CheckCustomerForCustomerName(), _canExecute));
            }
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
