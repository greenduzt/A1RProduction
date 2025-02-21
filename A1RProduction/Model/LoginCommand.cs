using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.ViewModel.PageSwitcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace A1QSystem.Commands
{
    public class LoginCommand : ICommand
    {
        LoginViewModel loginViewModel = null;
        List<UserPrivilages> uPriv;
        Grid TopContent = null;
        //Pass an instance of the ViewModel into the constructor
        public LoginCommand(LoginViewModel loginViewModel, Grid topContent)
        {
            this.loginViewModel = loginViewModel;
            this.TopContent = topContent;
        }

        public LoginCommand(LoginViewModel loginViewModel)
        {
            this.loginViewModel = loginViewModel;
           
        }

        public bool CanExecute(object parameter)
        {
            //Execution should only be possible if both Username and Password have been supplied
            if (!string.IsNullOrWhiteSpace(this.loginViewModel.Username) && this.loginViewModel.PasswordSecureString != null && this.loginViewModel.PasswordSecureString.Length > 0)
                return true;
            else
                return false;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
       
        public void Execute(object parameter)
        {     
            User userLogin = DBAccess.Login(this.loginViewModel.Username);
          
            if (userLogin.Username == null)
            {
                this.loginViewModel.ErrorMessage = "User not found!";
                return;
            }
            else
            {
                byte[] enteredValueHash = PasswordHashing.CalculateHash(SecureStringManipulation.ConvertSecureStringToByteArray(this.loginViewModel.PasswordSecureString));
                
               
                if (!PasswordHashing.SequenceEquals(enteredValueHash, userLogin.Password))
                {
                    this.loginViewModel.ErrorMessage = "Incorrect Password entered!";
                    return;
                }
               
                this.loginViewModel.ErrorMessage = "Login Successful!";
               
                string userDetails = userLogin.FirstName + " " +userLogin.LastName;
                string userstate = userLogin.State;

                uPriv = new List<UserPrivilages>();
                uPriv = DBAccess.GetUserPrivilages(userLogin.ID);

                List<MetaData> metaData = new List<MetaData>();
                metaData = DBAccess.GetMetaData();


                userLogin.FullName = userLogin.FirstName.ToUpper() + " " + userLogin.LastName.ToUpper();

                bool adminEnabled = false;
                bool productionEnabled = false;
                bool maintenanceEnabled = false;
                bool stockEnabled = false;
                bool ordersEnabled = false;
                bool workStationsEnabled = false;

                bool menuProd = false;
                bool menuGrading = false;
                bool menuMixing = false;
                bool menuSlitting = false;
                bool menuPeeling = false;


                foreach (var item in uPriv)
                {
                    if (item.Area == "Admin")
                    {
                        adminEnabled = GetPrivBool(item.Visibility);   
                    }
                    else if (item.Area == "Production")
                    {
                        productionEnabled = GetPrivBool(item.Visibility);
                    }
                    else if (item.Area == "Maintenance")
                    {
                        maintenanceEnabled = GetPrivBool(item.Visibility);      
                    }
                    else if (item.Area == "Stock")
                    {
                        stockEnabled = GetPrivBool(item.Visibility);
                    }
                    else if (item.Area == "Orders")
                    {
                        ordersEnabled = GetPrivBool(item.Visibility);
                    }
                    else if (item.Area == "WorkStations")
                    {
                        workStationsEnabled = GetPrivBool(item.Visibility);
                    }

                }

                //foreach (var item in uPriv)
                //{
                //    menuProd = item.MenuProduction;
                //    menuGrading = item.MenuGrading;
                //    menuMixing = item.MenuMixing;
                //    menuSlitting = item.MenuSlitting;
                //    menuPeeling = item.MenuPeeling;
                //}

                TopContentViewModel tcvm = new TopContentViewModel(userDetails, userstate, uPriv, TopContent, metaData);

                tcvm.user = userLogin;
                tcvm.UserWrapPanel = "Visible";
                tcvm.AdminEnabled = adminEnabled;
                tcvm.ProductionEnabled = productionEnabled;
                tcvm.MaintenanceEnabled = maintenanceEnabled;               
                tcvm.StockEnabled = stockEnabled;
                tcvm.OrdersEnabled = ordersEnabled;
                tcvm.ProductionActive = menuProd;
                tcvm.GradingActive = menuGrading;
                tcvm.MixingActive = menuMixing;
                tcvm.SlittingActive = menuSlitting;
                tcvm.PeelingActive = menuPeeling;
                tcvm.WorkStationsEnabled = workStationsEnabled;
                //TopContent.Visibility = Visibility.Visible;
                TopContent.DataContext = tcvm;


                Switcher.Switch(new MainMenu(userDetails, userstate, uPriv, metaData));
            }           
        }       

        private bool GetPrivBool(string str)
        {
            bool r = false;
            if(str == "Visible")
            {
                r = true;
            }
            else if (str == "Collapsed")
            {
                r = false;
            }
            return r;
        }
    }
}
