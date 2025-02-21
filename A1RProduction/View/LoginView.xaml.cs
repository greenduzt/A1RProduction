using A1QSystem.Interfaces;
using A1QSystem.ViewModel.PageSwitcher;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace A1QSystem
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public TopContentViewModel Tcvm;

        public LoginView(Grid topContent)
        {
            InitializeComponent();

            this.DataContext = new LoginViewModel(topContent);

           

           
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            //Cast the 'sender' to a PasswordBox
            PasswordBox pwd = sender as PasswordBox;

            //Set this "EncryptedPassword" dependency property to the "SecurePassword"
            //of the PasswordBox.
            A1QSystem.PasswordBoxMVVMProperties.SetEncryptedPassword(pwd, pwd.SecurePassword);

        }
    }
}
