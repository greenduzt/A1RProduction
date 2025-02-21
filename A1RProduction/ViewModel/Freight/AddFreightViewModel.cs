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
    public class AddFreightViewModel : BaseViewModel, IDataErrorInfo
    {
        #region Events

        public event Action<Freight> Closed;
       
        #endregion


        public AddFreightViewModel(int personId)
        {
            _freight = new List<Freight>()
            {
                new Freight(){}               
            };

            _closeCommand = new DelegateCommand(CloseForm);
            _clearCommand = new DelegateCommand(ClearForm);

            this.PersonId = personId;

         //   Init();
        }


        #region Public Properties

        public int PersonId { get; set; }

        private string _freightName;
        public string FreightName
        {
            get { return _freightName; }
            set
            {
                _freightName = value;
                RaisePropertyChanged("FreightName");
            }
        }

        private decimal _freightPrice;
        public decimal FreightPrice
        {
            get { return _freightPrice; }
            set
            {
                _freightPrice = value;
                
                RaisePropertyChanged("FreightPrice");
            }
        }

        private string _freightUnit;
        public string FreightUnit
        {
            get { return _freightUnit; }
            set
            {
                _freightUnit = value;
                RaisePropertyChanged("FreightUnit");
            }
        }

        private string _freightDescription;
        public string FreightDescription
        {
            get { return _freightDescription; }
            set
            {
                _freightDescription = value;
                RaisePropertyChanged("FreightDescription");
            }
        }        

        private List<Freight> _freight;
        public List<Freight> Freight
        {
            get { return _freight; }
            set { _freight = value; }
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
               
                Freight newFreight = new Freight();
                newFreight.FreightName = FreightName;
                newFreight.FreightPrice = FreightPrice;
                newFreight.FreightUnit = FreightUnit;
                newFreight.FreightDescription = FreightDescription;
               

                int res = DBAccess.CheckFreightAvailable(FreightName);
                if (res < 1)
                {

                    int result = DBAccess.InsertFreightDetails(newFreight);

                    if (result > 0)
                    {
                        Msg.Show("Freight added successfully!", "Freight Added", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);

                    }
                    else
                    {
                        Msg.Show("An error has occured and details were not added to the database! Please try again later", "Cannot Save Data", MsgBoxButtons.OK, MsgBoxImage.Alert, MsgBoxResult.Yes);
                    }

                    var freight = new Freight()
                    {
                        FreightName = FreightName,
                        FreightPrice = FreightPrice,
                        FreightUnit = FreightUnit,
                        FreightDescription = FreightDescription,                       
                        Id = result
                    };
                    Closed(freight);
                }
                else
                {
                    Msg.Show("Freight Company already existing in the database!", "Freight Company Exists!", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
                }
                
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                var freight = new Freight();


                Closed(freight);
            }
        }

        private void ClearForm()
        {
            FreightName = string.Empty;
            FreightPrice = 0;
            FreightUnit = string.Empty;
            FreightDescription = string.Empty;
           
        }

        private void Init()
        {
            var freight = _freight.FirstOrDefault(p => p.Id == PersonId);
            FreightName = freight.FreightName;
            RaisePropertyChanged("FreightName");
            FreightPrice = freight.FreightPrice;
            RaisePropertyChanged("FreightPrice");
            FreightUnit = freight.FreightUnit;
            RaisePropertyChanged("FreightUnit");
            FreightDescription = freight.FreightDescription;
            RaisePropertyChanged("FreightDescription");
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
            "FreightName",
            "FreightPrice",
            "FreightUnit",
            "FreightDescription"
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
                case "FreightName":
                    error = ValidateName();
                    break;
                case "FreightPrice":
                    error = ValidatePrice();
                    break;
                case "FreightUnit":
                    error = ValidateUnit();
                    break;
                case "FreightDescription":
                    error = ValidateDescription();
                    break;
                default:
                    error = null;
                    throw new Exception("Unexpected property being validated on Service");
            }
            return error;
        }


        private string ValidateName()
        {
            if (String.IsNullOrWhiteSpace(FreightName))
            {
                return "Freight name required!";
            }

            return null;
        }

        private string ValidatePrice()
        {
          //  Regex rex = new Regex(@"^[A-Za-z0-9\s]{1,}$");
            if (FreightPrice.ToString().Contains(" "))
            {
                return "Freight price required!";
            }
            return null;
        }

        private string ValidateUnit()
        {
            if (String.IsNullOrWhiteSpace(FreightUnit))
            {
                return "Freight unit required!";
            }
            return null;
        }

        private string ValidateDescription()
        {
            if (String.IsNullOrEmpty(FreightDescription))
            {
                return "Freight description\nrequired!";
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
