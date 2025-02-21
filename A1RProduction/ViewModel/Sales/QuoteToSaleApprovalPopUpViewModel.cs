using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.View;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBox;
using System.Text.RegularExpressions;

namespace A1QSystem.ViewModel.Sales
{
    public class QuoteToSaleApprovalPopUpViewModel : ViewModelBase
    {
        public event Action Closed;

        private DelegateCommand _showQuoteToSaleApprovalCommand;
        private DelegateCommand _closeCommand;
        private DelegateCommand _addCommand;

        public int QuoteNo { get; set; }
        public string QuoteNoString { get; set; }
        public string State { get; set; }
        public DateTime OrderProDateStart { get; set; }       

        public QuoteToSaleApprovalPopUpViewModel(int quoteNo, string state)
        {
            _closeCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(CloseForm);
            _addCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(AddData);
            _showQuoteToSaleApprovalCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(ShowQuoteToSaleApprovalWindow);
            QuoteNo = quoteNo;
            State = state;
            QuoteNoString = Regex.Replace(State + QuoteNo, @"\s+", "");

            OrderProDateStart = Convert.ToDateTime(NTPServer.GetNetworkTime().ToString("dd/MM/yyyy"));
        }

        private void ShowQuoteToSaleApprovalWindow()
        {
            var childWindow = new ChildWindowView();
            childWindow.ShowQuoteToSaleApproval(QuoteNo, State);
        }

        private void AddData()
        {

            string approvedDate = string.Empty;
            approvedDate = NTPServer.GetNetworkTime().ToString("dd/MM/yyyy");


            int res = DBAccess.InsertTempOrder(QuoteNo, OrderProDateStart.Date.ToString("dd/MM/yyyy"), approvedDate, false);

            if (res > 0)
            {
                Msg.Show("Quote has been sent for varification", "Quote Convertion Successful", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.Yes);
                CloseForm();
            }
            else
            {
                Msg.Show("An error has occured! Please try again later", "Data Addition Failed", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.Yes);
            }            
           
        }
        private void CloseForm()
        {
            if (Closed != null)
            {
                Closed();
            }
        }
       
        

        #region COMMANDS

        public DelegateCommand AddCommand
        {
            get { return _addCommand; }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        #endregion
    }
}
