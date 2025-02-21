using A1QSystem.Commands;
using A1QSystem.DB;
using A1QSystem.Interfaces;
using A1QSystem.Model.Meta;
using A1QSystem.ViewModel.PageSwitcher;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;

namespace A1QSystem
{
    public class LoginViewModel : CommonBase
    {
        private LoginCommand loginCommand = null;
        private bool _loginFailed;
        private string _errorMessage;
        private string _username;
        private SecureString _password;
        private bool _loginActive;
        private bool _isUserNameFocused;
        private int _id;
        private string _firstName;
        private string _lastName;
        private string _version;
       
        public LoginViewModel(Grid TopContent)
        {
            this.LoginActive = false;
            loginCommand = new LoginCommand(this, TopContent);
            IsUserNameFocused = true;
           
            LoadMetaData();
        }      
       
        private void LoadMetaData()
        {
            List<MetaData> metaData = new List<MetaData>();
            metaData = DBAccess.GetMetaData();


            var data =metaData.SingleOrDefault(x=>x.KeyName=="version");
            Version = data.Description;
        }

        private void CheckUserNamePassLength()
        {
            if (!String.IsNullOrWhiteSpace(Username) && PasswordSecureString != null && PasswordSecureString.Length > 0)
            {
                LoginActive = true;
                ErrorMessage=string.Empty;
            }
            else
            {
                LoginActive = false;               
            }
        }

        private void CloseWindow(object o)
        {
            if (MessageBox.Show("Are you sure you want to exit from A1 Rubber Console?", "Exit Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                ((Window)o).Close();
            }
        }

        #region PUBLIC PROPERTIES

        public LoginCommand UserLoginCommand
        {
            get { return loginCommand; }

        }

        public bool LoginFailed
        {
            get { return _loginFailed; }
            set
            {
                _loginFailed = value;
                RaisePropertyChanged("FailedLogin");
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (value != _errorMessage)
                {
                    _errorMessage = value;
                    RaisePropertyChanged("ErrorMessage");
                }
            }
        }

        public string Version
        {
            get { return _version; }
            set
            {
                if (value != _version)
                {
                    _version = value;
                    RaisePropertyChanged("Version");
                }
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                if (!string.Equals(value.ToString(), _username, StringComparison.OrdinalIgnoreCase))
                {
                    _username = value;
                    RaisePropertyChanged("Username");
                   
                }

                CheckUserNamePassLength();
                if (String.IsNullOrWhiteSpace(Username))
                {
                    ErrorMessage = "User name required!";
                }
                else
                {
                    ErrorMessage = string.Empty;
                }
               
            }
        }
        public SecureString PasswordSecureString
        {
            get { return _password; }
            set
            {
                if (value != null)
                {
                    _password = value;
                    RaisePropertyChanged("Password");
                   
                }

                CheckUserNamePassLength();

                if (!String.IsNullOrWhiteSpace(Username) && PasswordSecureString != null && PasswordSecureString.Length > 0)
                {
                    ErrorMessage = string.Empty;
                }
                else
                {
                    ErrorMessage = "Password required!";

                }
                
            }
        }

        public bool LoginActive
        {
            get
            {
                return _loginActive;
            }
            set
            {
                _loginActive = value;
                RaisePropertyChanged("LoginActive");
            }
        }

        public int ID 
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
                RaisePropertyChanged("FirstName");
            }
        }
       
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
                RaisePropertyChanged("LastName");
            }
        }

        public bool IsUserNameFocused
        {
            get
            {
                return _isUserNameFocused;
            }
            set
            {
                _isUserNameFocused = value;
                RaisePropertyChanged("IsUserNameFocused");
            }
        }

#endregion

        #region COMMANDS

      
        public ICommand CloseCommand 
        {
            get { return new RelayCommand((o) => CloseWindow(o), (o) => true); }

        }

        #endregion

    }
}
