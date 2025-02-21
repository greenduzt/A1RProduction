using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Outlook = Microsoft.Office.Interop.Outlook;
namespace A1QSystem.View
{
    /// <summary>
    /// Interaction logic for SendEmailView.xaml
    /// </summary>
    public partial class SendEmailView : Window
    {

        public string FileName;
        public string Email;

        public SendEmailView(string fileName,string email,string cutomer)
        {
            InitializeComponent();
            FileName = fileName;
            Email = email;
            setValues(Email, cutomer, fileName);     
           
        }

        private void setValues(string email,string customer,string file){
            txtTo.Text = email;
            txtCusName.Text = customer;
            txtAttachment.Text = file;

            if (string.IsNullOrEmpty(txtTo.Text))
            {
                txtTo.Background = Brushes.Yellow;
                txtTo.BorderBrush = Brushes.Red;
                lblToError.Visibility = Visibility.Visible;
                lblToError.Content = "This customer does not have\nan email address in the profile.";
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(this.txtTo.Text))
            {
                txtTo.Background = Brushes.Yellow;
                txtTo.BorderBrush = Brushes.Red;
                lblToError.Visibility = Visibility.Visible;
                lblToError.Content = "Email address required!";

            }
            else if (string.IsNullOrWhiteSpace(this.txtSubject.Text))
            {
                txtSubject.Background = Brushes.Yellow;
                txtSubject.BorderBrush = Brushes.Red;
                lblSubjectError.Visibility = Visibility.Visible;
                lblSubjectError.Content = "Subject required!";
            }
            else if (string.IsNullOrWhiteSpace(this.txtMessage.Text))
            {
                txtMessage.Background = Brushes.Yellow;
                txtMessage.BorderBrush = Brushes.Red;
                lblMessageError.Visibility = Visibility.Visible;
                lblMessageError.Content = "Message required!";
            }
            else
            {
                try
                {                  
                    string sendTo = txtTo.Text;
                    string subject = txtSubject.Text;
                    string message = txtMessage.Text;                   
                   
                    Outlook.Application oApp = new Outlook.Application();                   
                    Outlook.MailItem oMsg = (Outlook.MailItem)oApp.CreateItem(Outlook.OlItemType.olMailItem);                    
                    oMsg.HTMLBody = message;
                    
                    String sDisplayName = "MyAttachment";
                    int iPosition = (int)oMsg.Body.Length + 1;
                    int iAttachType = (int)Outlook.OlAttachmentType.olByValue;
                    
                    Outlook.Attachment oAttach = oMsg.Attachments.Add(@"S:\SALES SUPPORT\CUSTOMERS\Customer Quotes\" + FileName, iAttachType, iPosition, sDisplayName);
                    
                    oMsg.Subject = subject;                   
                    Outlook.Recipients oRecips = (Outlook.Recipients)oMsg.Recipients;                   
                    Outlook.Recipient oRecip = (Outlook.Recipient)oRecips.Add(sendTo);
                    oRecip.Resolve();                    
                    oMsg.Send();                    
                    oRecip = null;
                    oRecips = null;
                    oMsg = null;
                    oApp = null;

                    MessageBoxImage icon = MessageBoxImage.Information;
                    MessageBox.Show("Email has been sent successfully", "Email Confirmation", MessageBoxButton.OK, icon);
                   
                    this.Close();                
                }
                catch (Exception ex)
                {
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show("Email cannot be sent at this time. please try again later\nError : " + ex, "Sending Error", MessageBoxButton.OK, icon);
                }//end of catch
            }
        }

        private void txtTo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtTo.Text))
            {
                txtTo.Background = Brushes.Yellow;
                txtTo.BorderBrush = Brushes.Red;
                lblToError.Visibility = Visibility.Visible;
                lblToError.Content = "Email address required!";
            }
            else
            {
                lblToError.Visibility = Visibility.Collapsed;
                txtTo.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                txtTo.BorderBrush = Brushes.LightGray;
                lblToError.Content = "";

            }
        }

        private void txtSubject_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtSubject.Text))
            {
                txtSubject.Background = Brushes.Yellow;
                txtSubject.BorderBrush = Brushes.Red;
                lblSubjectError.Visibility = Visibility.Visible;
                lblSubjectError.Content = "Subject required!";
            }
            else
            {
                lblSubjectError.Visibility = Visibility.Collapsed;
                txtSubject.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                txtSubject.BorderBrush = Brushes.LightGray;
                lblSubjectError.Content = "";

            }
        }

        private void txtMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.txtMessage.Text))
            {
                txtMessage.Background = Brushes.Yellow;
                txtMessage.BorderBrush = Brushes.Red;
                lblMessageError.Visibility = Visibility.Visible;
                lblMessageError.Content = "Message required!";
            }
            else
            {
                lblMessageError.Visibility = Visibility.Collapsed;
                txtMessage.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                txtMessage.BorderBrush = Brushes.LightGray;
                lblMessageError.Content = "";
            }
        }
    }
}
