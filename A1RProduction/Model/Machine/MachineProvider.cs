using A1QSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.Machine
{
    public class MachineProvider : ViewModelBase
    {


        private int _providerID;
        private string _providerName;
        private string _address;
        private string _suburb;
        private string _postCode;
        private string _state;
        private string _active;
        private string _contactName;
        private string _email;
        private string _phone;

        public string Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                _phone = value;
                RaisePropertyChanged(() => this.Phone);
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                RaisePropertyChanged(() => this.Email);
            }
        }

        public string ContactName
        {
            get
            {
                return _contactName;
            }
            set
            {
                _contactName = value;
                RaisePropertyChanged(() => this.ContactName);
            }
        }

        public int ProviderID
        {
            get
            {
                return _providerID;
            }
            set
            {
                _providerID = value;
                RaisePropertyChanged(() => this.ProviderID);
            }
        }

        public string ProviderName
        {
            get
            {
                return _providerName;
            }
            set
            {
                _providerName = value;
                RaisePropertyChanged(() => this.ProviderName);
            }
        }

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

        public string Suburb
        {
            get
            {
                return _suburb;
            }
            set
            {
                _suburb = value;
                RaisePropertyChanged(() => this.Suburb);
            }
        }

        public string PostCode
        {
            get
            {
                return _postCode;
            }
            set
            {
                _postCode = value;
                RaisePropertyChanged(() => this.PostCode);
            }
        }

        public string State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                RaisePropertyChanged(() => this.State);
            }
        }

        public string Active
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
                RaisePropertyChanged(() => this.Active);
            }
        }
    }
}
